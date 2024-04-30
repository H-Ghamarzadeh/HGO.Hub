using HGO.Hub.Interfaces.Requests;
using HGO.Hub.Test.DBContext;
using HGO.Hub.Test.Entities;

namespace HGO.Hub.Test.CQRS.Queries;

public class FindUsersRequestHandler(TestDbContext dbContext) : IRequestHandler<FindUsersRequest, List<User>>
{
    public async Task<RequestHandlerResult<List<User>>> Handle(FindUsersRequest request)
    {
        return new RequestHandlerResult<List<User>>(dbContext.Users.Where(request.Term).ToList());
    }

    public int Priority => 0;
}

public class FindUsersRequest(Func<User, bool> term): IRequest<List<User>>
{
    public Func<User, bool> Term = term;
}