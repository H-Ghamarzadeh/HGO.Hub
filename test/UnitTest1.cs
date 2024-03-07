using System.Reflection;
using HGO.Hub.Interfaces;
using HGO.Hub.Test.CQRS.Commands;
using HGO.Hub.Test.CQRS.Queries;
using HGO.Hub.Test.DBContext;
using HGO.Hub.Test.Entities;
using HGO.Hub.Test.Events;
using HGO.Hub.Test.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace HGO.Hub.Test
{
    public class UnitTest1
    {
        private readonly IHub _hub;
        public UnitTest1()
        {
            var services = new ServiceCollection();
            services.AddDbContext<TestDbContext>(options => options.UseInMemoryDatabase("TestDB"));
            services.AddHgoHub(configuration =>
            {
                configuration.HandlersDefaultLifetime = ServiceLifetime.Transient;
                configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            services.AddSingleton <IEmailService, EmailService>();
            services.AddSingleton <ISmsService, SmsService>();
            var provider = services.BuildServiceProvider();

            _hub = provider.GetService<IHub>() ?? throw new InvalidOperationException();

            DbInitializer.InitializeData(provider.GetRequiredService<TestDbContext>());
        }

        [Fact]
        public async Task RegisterUser()
        {
            var user = new User()
            {
                FirstName = "John",
                LastName = "Smith",
                EmailAddress = "J.Smith@abc.com",
                PhoneNumber = "+11234567894"
            };

            var userId = await _hub.RequestAsync(new AddUsersRequest(user));

            userId.ShouldBeGreaterThan(0);

            await _hub.PublishEventAsync(new OnUserRegistered() { RegisteredUserId = userId });
        }

        [Fact]
        public async Task GetUser()
        {
            var user = await _hub.RequestAsync(new GetUserRequest(1));

            user.ShouldNotBeNull().EmailAddress.ShouldBe("W.Shakespeare@abc.com");
        }

        [Fact]
        public async Task FindUsers()
        {
            var users = await _hub.RequestAsync(new FindUsersRequest(p => p.EmailAddress.Contains("abc")));

            users.ShouldNotBeNull().Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task GetPost()
        {
            var post = await _hub.RequestAsync(new GetPostRequest(1, "BlogPost"));

            post.ShouldNotBeNull().Title.ShouldBe("Post 1");
        }

        [Fact]
        public async Task FindPosts()
        {
            var posts = await _hub.RequestAsync(new FindPostsRequest(p => p.Title.Contains("Post 2"), "BlogPost"));

            posts.ShouldNotBeNull().Count().ShouldBe(1);
            posts.ShouldBeOfType(typeof(List<Post>));
        }

        [Fact]
        public async Task GetProduct()
        {
            var product = await _hub.RequestAsync<Post>(new GetPostRequest(11, "Product"));

            product.ShouldNotBeNull().ShouldBeOfType(typeof(Product));
        }

        [Fact]
        public async Task FindProducts()
        {
            var products = await _hub.RequestAsync(new FindPostsRequest(_=> true, "Product"));

            products.ShouldNotBeNull().Count().ShouldBeGreaterThan(1);
            products.Select(p=> p.ShouldBeOfType<Product>());
        }

        [Fact]
        public async Task AddProduct()
        {
            var product = new Product()
            {
                Body = "New product description",
                ImageUrl = "http://sample.com/new-product-img.png",
                Title = "New Product",
                Price = -10,
                Author = await _hub.RequestAsync(new GetUserRequest(1))
            };

            var newProductId = await _hub.RequestAsync(new AddPostRequest(product));

            var newAddedProduct =
                await _hub.RequestAsync(new GetPostRequest(newProductId, "Product"), false);

            newAddedProduct.ShouldBeOfType<Product>();
            var newProduct = (Product)newAddedProduct;
            newProduct.ShouldNotBeNull();
            newProduct.Price.ShouldBe(0);
            newProduct.CreateDate.ShouldNotBeNull();
        }
    }
}