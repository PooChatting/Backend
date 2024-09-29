using AutoMapper;
using Poochatting.Entities;
using Poochatting.Models;

namespace Poochatting
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
