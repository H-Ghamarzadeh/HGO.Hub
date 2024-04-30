using HGO.Hub.Interfaces.Requests;
using HGO.Hub.Test.DBContext;
using HGO.Hub.Test.Entities;
using Microsoft.EntityFrameworkCore;

namespace HGO.Hub.Test.CQRS.Queries
{
    public class GetPostRequestHandler(TestDbContext dbContext) : IRequestHandler<GetPostRequest, Post>
    {
        public async Task<RequestHandlerResult<Post>> Handle(GetPostRequest request)
        {
            var result = new RequestHandlerResult<Post>(null);

            result.Result = request.PostType == "Product"
                ? await dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.Id)
                : await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == request.Id);

            return result;
        }

        public int Priority => 0;
    }

    public class GetPostRequest(int id, string postType) : IRequest<Post>
    {
        public int Id { get; set; } = id;
        public string PostType { get; set; } = postType;
    }
}
