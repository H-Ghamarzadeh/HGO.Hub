using HGO.Hub.Interfaces;
using HGO.Hub.Interfaces.Actions;
using HGO.Hub.Interfaces.Events;
using HGO.Hub.Interfaces.Filters;
using HGO.Hub.Interfaces.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace HGO.Hub
{
    /// <inheritdoc />
    public class Hub: IHub
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the Hub class
        /// </summary>
        /// <param name="serviceProvider">Service provider</param>
        public Hub(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public async Task PublishEventAsync<T>(T @event) where T : IEvent
        {
            var services = _serviceProvider.GetServices<IEventHandler<T>>().ToList();

            if (services.Any())
            {
                await Task.WhenAll(services.Select(p => p.Handle(TryClone(@event))).ToArray());
                
                //foreach (var service in services)
                //{
                    //service.Handle(TryClone(@event)).ConfigureAwait(false);
                //}
            }
        }

        /// <inheritdoc />
        public async Task DoActionAsync<T>(T action) where T : IAction
        {
            var services = _serviceProvider.GetServices<IActionHandler<T>>().ToList();

            if (services.Any())
            {
                foreach (var service in services.OrderBy(p => p.Order))
                {
                    await service.Handle(TryClone(action));
                    if (service.Stop)
                    {
                        break;
                    }
                }
            }
        }

        /// <inheritdoc />
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
        public async Task<T> ApplyFiltersAsync<T>(T data)
        {
            var services = _serviceProvider.GetServices<IFilterHandler<T>>().ToList();
            if (services.Any())
            {
                foreach (var service in services.OrderBy(p => p.Order))
                {
                    data = await service.Handle(TryClone(data));
                    if (service.Stop)
                    {
                        break;
                    }
                }
            }

            return data;
        }

        /// <inheritdoc />
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
        public async Task<TRes?> RequestAsync<TRes>(IRequest<TRes> request, bool autoApplyFiltersOnResponse = true)
        {
            var serviceType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TRes));
            var services = _serviceProvider.GetServices(serviceType).ToList();
            if (services.Any())
            {
                var service = services.OrderByDescending(p=> (int)serviceType.GetProperty("Priority").GetValue(p)).First();
                var resultObject = serviceType.GetMethod("Handle").Invoke(service, new[] { request });

                var result = await (Task<TRes>)resultObject;

                if (autoApplyFiltersOnResponse)
                {
                    return await ApplyFiltersAsync(result);
                }

                return result;
            }

            return default(TRes);
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
