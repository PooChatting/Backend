using System.ComponentModel.DataAnnotations;

namespace Poochatting.Entities
{
    public class CreateMessageDto
    {
        [Required]
        public int ChannelId { get; set; }
        
        [Required]
        [MaxLength(2048)]
        public string MessageText { get; set; }
    }
}
