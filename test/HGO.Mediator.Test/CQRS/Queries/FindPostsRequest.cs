using HGO.Hub.Interfaces.Requests;
using HGO.Hub.Test.DBContext;
using HGO.Hub.Test.Entities;

namespace HGO.Hub.Test.CQRS.Queries;

public class FindPostsRequestHandler(TestDbContext dbContext) : IRequestHandler<FindPostsRequest, List<Post>>
{
    public async Task<List<Post>> Handle(FindPostsRequest request)
    {
        if (request.PostType == "Product")
        {
            return dbContext.Products.Where(request.Term).ToList();
        }
        return dbContext.Posts.Where(request.Term).ToList();
    }

    public int Priority => 0;
}

public class FindPostsRequest(Func<Post, bool> term, string postType) : IRequest<List<Post>>
{
    public Func<Post, bool> Term = term;

    public string PostType { get; set; } = postType;
}