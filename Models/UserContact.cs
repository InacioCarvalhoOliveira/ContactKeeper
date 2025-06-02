using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ContactKeeper.Models
{
    [Table("UserContact")]
    public class UserContact
    {
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        

        // EXAMPLE: 55-11-5555-5555 (ddi-ddd-localnumber)
        [Required(ErrorMessage = "Please enter a valid International DDI")]
        [MinLength(2, ErrorMessage = "This field must have at least 2 characters")]
        [MaxLength(3, ErrorMessage = "This field must have at most 3 characters")]
        public string DDI { get; set; }

        [Required(ErrorMessage = "Please enter a valid DDD")]
        [System.ComponentModel.DataAnnotations.Range(10, 99, ErrorMessage = "DDD must be between 10 and 99")]
        [RegularExpression(@"^[0-9]{2}$", ErrorMessage = "DDD must be a 2-digit number")]
        public int DDD { get; set; }

        [Required(ErrorMessage = "Please enter a valid Local Number")]
        [MinLength(8, ErrorMessage = "This field must have at least 8 characters")]
        [MaxLength(10, ErrorMessage = "This field must have at most 10 characters")]
        
        [RegularExpression(@"^[0-9]{8,10}$", ErrorMessage = "Local Number must be a number with 8 to 10 digits")]
        public string LocalNumber { get; set; }    

    }
}
