using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactKeeper.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a valid Username")]
        [MinLength(5, ErrorMessage = "This field must have at least 2 characters")]
        [MaxLength(20, ErrorMessage = "This field must have at most 20 characters")]
        public string Username { get; set; }
        public string? Email { get; set; }
        public PhoneNumber phoneNumber { get; set; }   
    }
}