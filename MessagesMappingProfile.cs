using AutoMapper;
using Poochatting.Entities;
using Poochatting.Models;

namespace Poochatting
{
    public class MessagesMappingProfile : Profile
    {
        public MessagesMappingProfile()
        {
            CreateMap<Message, MessageDto>();
            CreateMap<MessageDto, Message>();
            CreateMap<CreateMessageDto, Message>();
        }
    }
}
