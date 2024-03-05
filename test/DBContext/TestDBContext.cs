using HGO.Hub.Test.Entities;
using Microsoft.EntityFrameworkCore;

namespace HGO.Hub.Test.DBContext
{
    public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<Post>? Posts { get; set; }
        public virtual DbSet<Product>? Products { get; set; }
    }
}
