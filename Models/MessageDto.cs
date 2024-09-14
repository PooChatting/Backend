using System.ComponentModel.DataAnnotations;

namespace Poochatting.Models
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int ChannelId { get; set; }
        public string MessageText { get; set; }
        public DateTime Publication { get; set; }
        public bool WasEdited { get; set; }
    }
}
