using HGO.Hub.Test.Entities;

namespace HGO.Hub.Test.DBContext;

public static class DbInitializer
{
    public static void InitializeData(TestDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.Set<User>().AddRange(
            new User()
            {
                Id = 1,
                FirstName = "William",
                LastName = "Shakespeare",
                EmailAddress = "W.Shakespeare@abc.com"
            },
            new User()
            {
                Id = 2,
                FirstName = "Catherine",
                LastName = "Torphy",
                EmailAddress = "C.Torphy@abc.com"
            },
            new User()
            {
                Id = 3,
                FirstName = "Gerhard",
                LastName = "Larson",
                EmailAddress = "G.Larson@abc.com"
            },
            new User()
            {
                Id = 4,
                FirstName = "Amiya",
                LastName = "Barrows",
                EmailAddress = "A.Barrows@abc.com"
            },
            new User()
            {
                Id = 5,
                FirstName = "Katrine",
                LastName = "Homenick",
                EmailAddress = "K.Homenick@abc.com"
            }
        );

        context.Set<Post>().AddRange(
            new Post()
            {
                Id = 1,
                Title = "Post 1",
                Body = "Post 1 Description",
                AuthorId = 1
            },
            new Post()
            {
                Id = 2,
                Title = "Post 2",
                Body = "Post 2 Description",
                AuthorId = 1
            },
            new Post()
            {
                Id = 3,
                Title = "Post 3",
                Body = "Post 3 Description",
                AuthorId = 4
            },
            new Post()
            {
                Id = 4,
                Title = "Post 4",
                Body = "Post 4 Description",
                AuthorId = 3
            },
            new Post()
            {
                Id = 5,
                Title = "Post 5",
                Body = "Post 5 Description",
                AuthorId = 2
            },
            new Post()
            {
                Id = 6,
                Title = "Post 6",
                Body = "Post 6 Description",
                AuthorId = 2
            },
            new Post()
            {
                Id = 7,
                Title = "Post 7",
                Body = "Post 7 Description",
                AuthorId = 2
            },
            new Post()
            {
                Id = 8,
                Title = "Post 8",
                Body = "Post 8 Description",
                AuthorId = 5
            },
            new Post()
            {
                Id = 9,
                Title = "Post 9",
                Body = "Post 9 Description",
                AuthorId = 1
            },
            new Post()
            {
                Id = 10,
                Title = "Post 10",
                Body = "Post 10 Description",
                AuthorId = 5
            }
        );

        context.Set<Product>().AddRange(
            new Product()
            {
                Id = 11,
                Title = "Product 1",
                Body = "Product 1 Description",
                AuthorId = 1,
                Price = 10,
                ImageUrl = "http://sample.com/img1.png"
            },
            new Product()
            {
                Id = 12,
                Title = "Product 2",
                Body = "Product 2 Description",
                AuthorId = 1,
                Price = 15,
                ImageUrl = "http://sample.com/img2.png"
            }, 
            new Product()
            {
                Id = 13,
                Title = "Product 3",
                Body = "Product 3 Description",
                AuthorId = 4,
                Price = 8,
                ImageUrl = "http://sample.com/img3.png"
            }, 
            new Product()
            {
                Id = 14,
                Title = "Product 4",
                Body = "Product 4 Description",
                AuthorId = 3,
                Price = 19,
                ImageUrl = "http://sample.com/img4.png"
            }, 
            new Product()
            {
                Id = 15,
                Title = "Product 5",
                Body = "Product 5 Description",
                AuthorId = 2,
                Price = 7,
                ImageUrl = "http://sample.com/img5.png"
            }
        );

        context.SaveChanges();
    }
}