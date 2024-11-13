using AutoMapper;
using Poochatting.DbContext.Entities;
using Poochatting.Entities;
using Poochatting.Models;

namespace Poochatting.Mappers
{
    public class MessagesMappingProfile : Profile
    {
        public MessagesMappingProfile()
        {
            CreateMap<Message, MessageModel>();
            CreateMap<MessageModel, Message>();
            CreateMap<CreateMessageDto, Message>();
        }
    }
}
