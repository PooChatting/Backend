namespace Poochatting.Models
{
    public class MessageModel
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorProfile { get; set; }
        public int ChannelId { get; set; }
        public string MessageText { get; set; }
        public DateTime Publication { get; set; }
        public bool WasEdited { get; set; }
    }
}
