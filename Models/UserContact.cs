using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactKeeper.Models
{
    [Table("UserContact")]
    public class UserContact
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; } // Foreign key
       // public User User { get; set; }  // Navigation property
        public int DDD { get; set; }
        public int DDI { get; set; }
        public string PhoneNumber { get; set; }
        //public virtual User User { get; set; }
        
    }
}
