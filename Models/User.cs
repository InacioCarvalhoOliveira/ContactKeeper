using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        public required string Username { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }

        [MinLength(8, ErrorMessage = "This field must have at least 8 characters")]
        [MaxLength(15, ErrorMessage = "This field must have at most 15 characters")]
        [Required(ErrorMessage = "This field is required")]
        public required string Password { get; set; }

        public string? Role { get; set;}

        //public virtual PhoneNumber PhoneNumber { get; set; }   
    }
}