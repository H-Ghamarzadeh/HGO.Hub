using HGO.Hub.Interfaces;
using HGO.Hub.Test.Entities;
using HGO.Hub.Test.DBContext;
using HGO.Hub.Interfaces.Requests;

namespace HGO.Hub.Test.CQRS.Commands
{
    public class AddPostRequestHandler(TestDbContext dbContext, IHub hub) : IRequestHandler<AddPostRequest, int>
    {
        IHub _hub { get; } = hub;

        public async Task<int> Handle(AddPostRequest request)
        {
            if (request.Post.GetType() == typeof(Product))
            {
                var product = await _hub.ApplyFiltersAsync((Product)request.Post);
                await dbContext.Products.AddAsync(product);
                await dbContext.SaveChangesAsync();
                return product.Id;
            }

            var post = await _hub.ApplyFiltersAsync(request.Post);
            await dbContext.Posts.AddAsync(post);
            await dbContext.SaveChangesAsync();
            return post.Id;
        }

        public int Priority => 0;
    }

    public class AddPostRequest(Post post) : IRequest<int>
    {
        public Post Post { get; set; } = post;
    }
}
