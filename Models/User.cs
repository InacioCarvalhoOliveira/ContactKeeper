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
        public string Username { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }
        public virtual PhoneNumber PhoneNumber { get; set; }   
    }
}