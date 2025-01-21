using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ContactKeeper.Models
{
    [Table("PhoneNumber")]
    public class PhoneNumber
    {
        [Key]
        public int Id { get; set; }        

        // EXAMPLE: 55-11-5555-5555 (ddi-ddd-localnumber)
        [Required(ErrorMessage = "Please enter a valid International DDI")]
        [MinLength(2, ErrorMessage = "This field must have at least 2 characters")]
        [MaxLength(3, ErrorMessage = "This field must have at most 3 characters")]
        public string DDI { get; set; }

        [Required(ErrorMessage = "Please enter a valid DDD")]
        [RegularExpression(@"^[0-9]{2}$", ErrorMessage = "DDD must be a 2-digit number")]
        public int DDD { get; set; }

        [Required(ErrorMessage = "Please enter a valid Local Number")]
        [MinLength(8, ErrorMessage = "This field must have at least 8 characters")]
        [MaxLength(10, ErrorMessage = "This field must have at most 10 characters")]

        public string LocalNumber { get; set; }         
    }
}