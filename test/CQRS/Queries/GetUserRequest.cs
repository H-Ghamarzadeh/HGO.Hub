using HGO.Hub.Interfaces.Requests;
using HGO.Hub.Test.DBContext;
using HGO.Hub.Test.Entities;
using Microsoft.EntityFrameworkCore;

namespace HGO.Hub.Test.CQRS.Queries
{
    public class GetUserRequestHandler : IRequestHandler<GetUserRequest, User>
    {
        private readonly TestDbContext _dbContext;

        public GetUserRequestHandler(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User> Handle(GetUserRequest request)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(p => p.Id == request.Id);
        }

        public int Priority => 0;
    }

    public class GetUserRequest(int id) : IRequest<User>
    {
        public int Id { get; set; } = id;
    }
}
