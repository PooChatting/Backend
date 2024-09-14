using System.ComponentModel.DataAnnotations;

namespace Poochatting.Models
{
    public class EditMessageDto
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        [MaxLength(2048)]
        public string UpdatedMessage { get; set; }
    }
}
