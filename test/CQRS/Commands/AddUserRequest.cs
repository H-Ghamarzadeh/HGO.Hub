using HGO.Hub.Test.Entities;
using HGO.Hub.Test.DBContext;
using HGO.Hub.Interfaces.Requests;

namespace HGO.Hub.Test.CQRS.Commands
{
    public class AddUsersRequestHandler(TestDbContext dbContext) : IRequestHandler<AddUsersRequest, int>
    {
        public async Task<RequestHandlerResult<int>> Handle(AddUsersRequest request)
        {
            await dbContext.Users.AddAsync(request.User);
            await dbContext.SaveChangesAsync();
            return new RequestHandlerResult<int>(request.User.Id);
        }

        public int Priority => 0;
    }

    public class AddUsersRequest(User user): IRequest<int>
    {
        public User User { get; set; } = user;
    }
}
