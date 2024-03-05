namespace HGO.Hub.Test.Entities;

public class Product: Post
{
    public override string PostType => "Product";
    public int Price { get; set; }
    public string ImageUrl { get; set; }
}