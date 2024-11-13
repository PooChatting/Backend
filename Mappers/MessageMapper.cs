using Poochatting.DbContext.Entities;
using Poochatting.Models;

namespace Poochatting.Mappers
{
    public static class MessageMapper
    {
        public static IQueryable<MessageModel> ProjectToDto(this IQueryable<Message> queryable)
        {
            return queryable
               .Select(p => new MessageModel
                {
                    Id = p.Id,
                    AuthorId = p.AuthorId,
                    ChannelId = p.ChannelId,
                    MessageText = p.MessageText,
                    MessageTypeEnum = p.MessageTypeEnum,
                    Publication = p.Publication,
                    ReplyToId = p.ReplyToId,
                    WasEdited = p.WasEdited,
                    HadBeenRead = p.HadBeenRead,
                    
                }
            );
        }
    }
}
