using System.ComponentModel.DataAnnotations;

namespace HGO.Hub.Test.Entities
{
    public abstract class BaseEntity : ICloneable
    {
        [Key]
        public int Id { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
