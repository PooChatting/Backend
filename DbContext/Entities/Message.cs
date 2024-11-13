using Poochatting.Models.Enums;

namespace Poochatting.DbContext.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int ChannelId { get; set; }
        public string MessageText { get; set; }
        public DateTime Publication { get; set; }
        public bool WasEdited { get; set; }
        public MessageTypeEnum MessageTypeEnum { get; set; }
        public int? ReplyToId { get; set; }
        public bool HadBeenRead { get; set; }
    }
}
