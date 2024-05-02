using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using HGO.Hub.Interfaces;
using HGO.Hub.Interfaces.Actions;
using HGO.Hub.Interfaces.Events;
using HGO.Hub.Interfaces.Filters;
using HGO.Hub.Interfaces.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HGO.Hub
{
    /// <inheritdoc />
    public class Hub : IHub
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IHub>? _logger;

        /// <summary>
        /// Initializes a new instance of the Hub class
        /// </summary>
        /// <param name="serviceProvider">Service provider</param>
        /// <param name="logger"></param>
        public Hub(IServiceProvider serviceProvider, ILogger<IHub>? logger = null)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        private string ObjectToJson<T>(T obj)
        {
            if (obj == null)
            {
                return "{}";
            }

            try
            {
                return JsonSerializer.Serialize(obj, new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
            }
            catch (Exception e)
            {
                Log($"An exception occurred when try to serialize object ({obj}): {e.Message}");
            }

            return string.Empty;
        }

        private void Log(string message = "", Exception? ex = null)
        {
            if (ServiceCollectionExtensions.HgoHubServiceConfiguration.LogEvents)
            {
                if (ex != null)
                {
                    _logger?.Log(LogLevel.Error, ex, message);
                }
                else
                {
                    _logger?.Log(LogLevel.Information, message);
                }
            }
        }

        /// <inheritdoc />
        public async Task PublishEventAsync<T>(T @event, bool handleExceptions = false) where T : IEvent
        {
            Log($"Publishing '{@event.GetType().FullName}' Event{Environment.NewLine}{ObjectToJson(@event)}");

            var services = _serviceProvider.GetServices<IEventHandler<T>>().ToList();

            if (services.Any())
            {
                await Task.WhenAll(services.Select(p =>
                {
                    //Run Event Handler
                    Log($"Executing '{p.GetType().FullName}' Event Handler.");

                    return p.Handle(TryClone(@event));

                }).Select(p => p.ContinueWith(c =>
                {
                    //On exception
                    Log("Error", c.Exception);
                    if (!handleExceptions && c.Exception != null)
                        throw c.Exception;

                }, TaskContinuationOptions.OnlyOnFaulted).ContinueWith(t => t)).ToArray());
            }
        }

        /// <inheritdoc />
        public async Task DoActionAsync<T>(T action, bool handleExceptions = false) where T : IAction
        {
            Log($"Publishing '{action.GetType().FullName}' Action{Environment.NewLine}{ObjectToJson(action)}");

            var services = _serviceProvider.GetServices<IActionHandler<T>>().ToList();

            if (services.Any())
            {
                foreach (var service in services.OrderBy(p => p.Order))
                {
                    Log($"Executing '{service.GetType().FullName}' Action Handler.");

                    try
                    {
                        await service.Handle(TryClone(action));
                    }
                    catch (Exception e)
                    {
                        //On exception
                        Log($"An exception occurred in the '{service.GetType().FullName}' action handler", e);
                        if (!handleExceptions)
                            throw e;
                    }

                    if (service.Stop)
                    {
                        Log(
                            $"Execution of the remaining actions in the pipeline was stopped by '{service.GetType().FullName}' action handler.");

                        break;
                    }
                }
            }
        }

        /// <inheritdoc />
        [Obsolete]
        public async Task<List<HandlerException<T>>> DoActionAndHandleExceptionsAsync<T>(T action) where T : IAction
        {
            var services = _serviceProvider.GetServices<IActionHandler<T>>().ToList();
            var result = new List<HandlerException<T>>();

            if (services.Any())
            {
                int count = 0;
                foreach (var service in services.OrderBy(p => p.Order))
                {
                    try
                    {
                        await service.Handle(TryClone(action));
                        if (service.Stop)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        result.Add(new HandlerException<T>()
                        {
                            InputData = TryClone(action),
                            HandlerType = service.GetType(),
                            Exception = e,
                            HandlerDefaultOrder = service.Order,
                            HandlerOrderInPipeline = count
                        });
                    }

                    count++;
                }
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<T> ApplyFiltersAsync<T>(T data, bool handleExceptions = false)
        {
            if (data == null)
            {
                return data;
            }

            Log($"Applying filters on '{data?.GetType().FullName}' {Environment.NewLine}{ObjectToJson(data)}");

            var services = _serviceProvider.GetServices<IFilterHandler<T>>().ToList();
            if (services.Any())
            {
                foreach (var service in services.OrderBy(p => p.Order))
                {
                    Log($"Executing '{service.GetType().FullName}' Filter.");

                    try
                    {
                        data = await service.Handle(TryClone(data));

                        Log($"Result '{data?.GetType().FullName}' {Environment.NewLine}{ObjectToJson(data)}");
                    }
                    catch (Exception e)
                    {
                        //On exception
                        Log($"An exception occurred in the '{service.GetType().FullName}' filter handler", e);
                        if (!handleExceptions)
                            throw e;
                    }

                    if (service.Stop)
                    {
                        Log($"Execution of the remaining filters in the pipeline was stopped by '{service.GetType().FullName}' filter handler.");

                        break;
                    }
                }
            }

            return data;
        }

        /// <inheritdoc />
        [Obsolete]
        public async Task<FilterHandlerResult<T>> ApplyFiltersAndHandleExceptionsAsync<T>(T data)
        {
            var services = _serviceProvider.GetServices<IFilterHandler<T>>().ToList();
            var result = new FilterHandlerResult<T>();

            if (services.Any())
            {
                var count = 0;
                foreach (var service in services.OrderBy(p => p.Order))
                {
                    try
                    {
                        data = await service.Handle(TryClone(data));
                        if (service.Stop)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        result.Exceptions.Add(new HandlerException<T>()
                        {
                            InputData = TryClone(data),
                            HandlerType = service.GetType(),
                            Exception = e,
                            HandlerDefaultOrder = service.Order,
                            HandlerOrderInPipeline = count
                        });
                    }

                    count++;
                }
            }

            result.Result = data;
            return result;
        }

        /// <inheritdoc />
        public async Task<TRes> RequestAsync<TRes>(IRequest<TRes> request, bool autoApplyFiltersOnResponse = true, bool catchExceptionsAndRunNextHandler = false)
        {
            Log($"Publishing request for '{request.GetType().FullName}' {Environment.NewLine}{ObjectToJson(request)}");

            var serviceType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TRes));
            var services = _serviceProvider.GetServices(serviceType)
                                                       .OrderByDescending(p => (int)serviceType.GetProperty("Priority").GetValue(p))
                                                       .Where(p => p != null)
                                                       .ToList();
            if (services.Any())
            {
                foreach (var service in services)
                {
                    Log($"Executing '{service.GetType().FullName}' Request Handler.");

                    try
                    {
                        var resultObject = serviceType.GetMethod("Handle").Invoke(service, new[] { request });

                        var result = await (Task<RequestHandlerResult<TRes>>)resultObject;

                        Log($"'{service.GetType().FullName}' Result:{Environment.NewLine}Type: {result.Result?.GetType().FullName}{Environment.NewLine}Value:{ObjectToJson(result.Result)}{Environment.NewLine}Handled:{result.Handled}");

                        if (result.Handled)
                        {
                            if (autoApplyFiltersOnResponse)
                            {
                                return await ApplyFiltersAsync(result.Result);
                            }

                            return result.Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        //On exception
                        Log($"An exception occurred in the '{service?.GetType().FullName}' request handler", ex);
                        if (!catchExceptionsAndRunNextHandler)
                            throw ex;
                    }
                }
            }

            return default;
        }

        private static T TryClone<T>(T data)
        {
            if (data != null)
            {
                if (data is ICloneable cloneableData)
                {
                    return (T)cloneableData.Clone();
                }
            }
            return data;
        }
    }
}
