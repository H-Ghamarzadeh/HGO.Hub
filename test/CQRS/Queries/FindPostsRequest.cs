using HGO.Hub.Interfaces.Requests;
using HGO.Hub.Test.DBContext;
using HGO.Hub.Test.Entities;

namespace HGO.Hub.Test.CQRS.Queries;

public class FindPostsRequestHandler(TestDbContext dbContext) : IRequestHandler<FindPostsRequest, List<Post>>
{
    public async Task<RequestHandlerResult<List<Post>>> Handle(FindPostsRequest request)
    {
        var result = new RequestHandlerResult<List<Post>>(null);
        result.Result = request.PostType == "Product" ? 
            dbContext.Products.Where(request.Term).ToList() : 
            dbContext.Posts.Where(request.Term).ToList();
        return result;
    }

    public int Priority => 0;
}

public class FindPostsRequest(Func<Post, bool> term, string postType) : IRequest<List<Post>>
{
    public Func<Post, bool> Term = term;

    public string PostType { get; set; } = postType;
}