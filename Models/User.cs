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
        
        public List<UserContact> UserContacts { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address")]
        public required string Email { get; set; }

        [MinLength(8, ErrorMessage = "This field must have at least 8 characters")]
        [MaxLength(15, ErrorMessage = "This field must have at most 15 characters")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Please enter a valid Username")]
        [MinLength(5, ErrorMessage = "This field must have at least 5 characters")]
        [MaxLength(20, ErrorMessage = "This field must have at most 20 characters")]
        public required string Username { get; set; }
        public string? Role { get; set; }

        //public virtual PhoneNumber PhoneNumber { get; set; }   
    }

    // For user creation (registration)
    public class UserDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MinLength(5, ErrorMessage = "Username must be at least 5 characters long")]
        [MaxLength(20, ErrorMessage = "Username must be at most 20 characters long")]
        public string Username { get; set; }

        [MinLength(8, ErrorMessage = "This field must have at least 8 characters")]
        [MaxLength(15, ErrorMessage = "This field must have at most 15 characters")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}