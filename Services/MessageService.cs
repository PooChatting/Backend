using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Poochatting.ChatHub;
using Poochatting.Entities;
using Poochatting.Exceptions;
using Poochatting.Models;

namespace Poochatting.Services
{
    public interface IMessageService
    {
        IEnumerable<MessageModel> GetAll(int channelId);
        MessageModel GetById(int id);
        public Task<int> PostMessageAsync(CreateMessageDto dto);
        void Delete(int id);
        void PutMessage(EditMessageDto dto);
    }
    public class MessageService : IMessageService
    {
        private readonly MessageDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAccountContextService _accountContext;
        private readonly IHubContext<ChatHub.ChatHub> _hubContext;
        public MessageService(MessageDbContext dbContext, IMapper mapper, IAccountContextService accountContext, IHubContext<ChatHub.ChatHub> hubContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _accountContext = accountContext;
            _hubContext = hubContext;
        }
        public IEnumerable<MessageModel> GetAll(int channelId)
        {
            var usersInChannel = _dbContext.Users.Where(x => x.ChannelsIds.Contains(channelId)).ToList();
            var curUserId = _accountContext.GetAccountId();

            if (!usersInChannel.Any(x => x.Id == curUserId)) throw new UnauthorizedException("You don't have access to this");

            var messages = _dbContext
                .Messages.Where(x => x.ChannelId == channelId).ToList();

            if (messages is null) return [];

            var messagesDtos = _mapper.Map<List<MessageModel>>(messages);

            messagesDtos.ForEach(m =>
            {
                var user = usersInChannel.FirstOrDefault(u => u.Id == m.AuthorId);
                m.AuthorName = user?.Username ?? "Deleted User";
            });
            
            return messagesDtos;
        }
        public MessageModel GetById(int id)
        {
            var message = _dbContext
                .Messages
                .FirstOrDefault(r => r.Id == id);

            if (message is null) throw new NotFoundException("Message not found");

            var messageDto = _mapper.Map<MessageModel>(message);
            return messageDto;
        }
        public async Task<int> PostMessageAsync(CreateMessageDto dto)
        {
            var curUserId = _accountContext.GetAccountId();
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == curUserId);

            if (!user.ChannelsIds.Contains(dto.ChannelId)) throw new UnauthorizedException("You don't have access to this");

            var message = _mapper.Map<Message>(dto);
            message.Publication = DateTime.Now;
            message.AuthorId = _accountContext.GetAccountId();

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            var messageDto = _mapper.Map<MessageModel>(message);
            messageDto.AuthorName = user.Username;

            var otherUserInChat = _dbContext.Users.FirstOrDefault(x => x.ChannelsIds.Contains(dto.ChannelId) && x.Id != curUserId);
            var messageJsonString = JsonConvert.SerializeObject(messageDto);

            await _hubContext.Clients.User(otherUserInChat.Id.ToString()).SendAsync("ReceiveMessage", messageJsonString);

            // Remove this, and add pushing posted messege to frontend and add little "oczekuje", and "fail" icon to indicate state of message,
            // this also gets rid of need to check if messege was not posted by user in toastr service
            await _hubContext.Clients.User(curUserId.ToString()).SendAsync("ReceiveMessage", messageJsonString);

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
