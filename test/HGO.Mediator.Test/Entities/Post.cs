namespace HGO.Hub.Test.Entities
{
    public class Post: BaseEntity
    {
        public virtual string PostType => "BlogPost";
        public string Title { get; set; }
        public string Body { get; set; }    
        public int AuthorId { get; set; }
        public DateTime? CreateDate { get; set; }
        public virtual User Author { get; set; }
    }
}
