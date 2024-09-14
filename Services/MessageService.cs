using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Poochatting.Entities;
using Poochatting.Exceptions;
using Poochatting.Models;

namespace Poochatting.Services
{
    public interface IMessageService
    {
        IEnumerable<MessageDto> GetAll();
        MessageDto GetById(int id);
        int PostMessage(CreateMessageDto dto);
        void Delete(int id);
        void PutMessage(EditMessageDto dto);
    }
    public class MessageService : IMessageService
    {
        private readonly MessageDbContext _dbContext;
        private readonly IMapper _mapper;
        public MessageService(MessageDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public IEnumerable<MessageDto> GetAll()
        {
            var messages = _dbContext
                .Messages
                .ToList();

            var messagesDtos = _mapper.Map<List<MessageDto>>(messages);
            
            return messagesDtos;
        }
        public MessageDto GetById(int id)
        {
            var message = _dbContext
                .Messages
                .FirstOrDefault(r => r.Id == id);

            if (message is null) throw new NotFoundException("Message not found");

            var messageDto = _mapper.Map<MessageDto>(message);
            return messageDto;
        }
        public int PostMessage(CreateMessageDto dto)
        {
            var message = _mapper.Map<Message>(dto);
            message.Publication = DateTime.Now;
            message.AuthorId = 696969;

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            return message.Id;
        }
        public void PutMessage(EditMessageDto dto)
        {
            var message = _dbContext.Messages.FirstOrDefault(r => r.Id == dto.Id);
            if (message is null) throw new NotFoundException("Message not found");

            message.MessageText = dto.UpdatedMessage;
            message.WasEdited = true;

            _dbContext.SaveChanges();
        }
        public void Delete(int id)
        {
            var message = _dbContext.Messages.FirstOrDefault(r => r.Id == id);
            if (message is null) throw new NotFoundException("Message not found");

            _dbContext.Messages.Remove(message);
            _dbContext.SaveChanges() ;
        }
    }
}
