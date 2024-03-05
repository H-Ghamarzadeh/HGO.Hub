using HGO.Hub.Interfaces.Filters;
using HGO.Hub.Test.Entities;

namespace HGO.Hub.Test.Filters;

public class PostDataFilterHandler: IFilterHandler<Post>, IFilterHandler<Product>
{
    public async Task<Post> Handle(Post data)
    {
        if (data == null)
        {
            return data;
        }

        data.CreateDate ??= DateTime.Now;
        return data;
    }

    public async Task<Product> Handle(Product data)
    {
        if (data == null)
        {
            return data;
        }

        data.CreateDate ??= DateTime.Now;
        if (data.Price < 0)
        {
            data.Price = 0;
        }

        return data;
    }

    public int Order => 1;
    public bool Stop => false;
}

