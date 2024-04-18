using System.ComponentModel.DataAnnotations.Schema;

namespace SMS_API.Entity
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? CreatedUserId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? EditUserId { get; set; }
        public DateTime? EditTime { get; set; }
    }
}
