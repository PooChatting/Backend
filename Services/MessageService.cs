using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Poochatting.Authorization;
using Poochatting.DbContext;
using Poochatting.DbContext.Entities;
using Poochatting.Entities;
using Poochatting.Exceptions;
using Poochatting.Mappers;
using Poochatting.Models;
using Poochatting.Models.Enums;
using Poochatting.Models.Queries;
using System.Collections.Generic;
using System.Security.Claims;

namespace Poochatting.Services
{
    public interface IMessageService
    {
        Task<PagedResult<MessageModel>> GetAll(int channelId, MessageQueryParams paginationParameters);
        MessageModel GetById(int id);
        public Task<int> PostMessageAsync(CreateMessageDto dto);
        void Delete(int id, ClaimsPrincipal user);
        void PutMessage(EditMessageDto dto, ClaimsPrincipal user);
    }
    public class MessageService : IMessageService
    {
        private readonly MessageDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAccountContextService _accountContext;
        private readonly IHubContext<ChatHub.ChatHub> _hubContext;
        private readonly IAuthorizationService _authorizationService;
        public MessageService(MessageDbContext dbContext, IMapper mapper, IAccountContextService accountContext, IHubContext<ChatHub.ChatHub> hubContext, IAuthorizationService authorizationService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _accountContext = accountContext;
            _hubContext = hubContext;
            _authorizationService = authorizationService;
        }
        public async Task<PagedResult<MessageModel>> GetAll(int channelId, MessageQueryParams paginationParameters)
        {
            var usersInChannel = _dbContext.Users.Where(x => x.ChannelsIds.Contains(channelId)).ToList();
            var curUserId = _accountContext.GetAccountId();

            if (!usersInChannel.Any(x => x.Id == curUserId)) throw new UnauthorizedException("You don't have access to this");

            var messages = await _dbContext.Messages
                .Where(x => x.ChannelId == channelId)
                .OrderByDescending(x => x.Id)
                .ProjectToDto()
                .Paginate(paginationParameters);

            foreach (var message in messages.Items)
            {
                var dbMessage = _dbContext.Messages.FirstOrDefault(x => x.Id == message.Id);
                if(dbMessage.AuthorId != curUserId)
                {
                    dbMessage.HadBeenRead = true;
                }
            }

            _dbContext.SaveChanges();
            messages.Items = messages.Items.Reverse();

            foreach (var item in messages.Items)
            {
                var user = usersInChannel.FirstOrDefault(u => u.Id == item.AuthorId)?.Username;
                if (user is null)
                {
                    user = "Deleted User";
                }
                item.AuthorName = user;
            }


            return messages;
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
            if (message.MessageTypeEnum == MessageTypeEnum.Screenshot)
            {
                message.MessageText = "Has took a screenshot of this part of conversation";
            }
            else if (message.MessageTypeEnum == MessageTypeEnum.Share)
            {
                message.MessageText = "Has shared this message";
            }

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
        public async void PutMessage(EditMessageDto dto, ClaimsPrincipal user)
        {
            var message = _dbContext.Messages.FirstOrDefault(r => r.Id == dto.Id);
            if (message is null) throw new NotFoundException("Message not found");

            var authorization = _authorizationService.AuthorizeAsync(user, message, new ResourceOperationRequirement(ResourceOperation.Update)).Result;
            if (!authorization.Succeeded) throw new UnauthorizedException("You don't have access to this");

            message.MessageText = dto.UpdatedMessage;
            message.WasEdited = true;

            _dbContext.SaveChanges();

            var curUserId = _accountContext.GetAccountId();
            var otherUserInChat = _dbContext.Users.FirstOrDefault(x => x.ChannelsIds.Contains(message.ChannelId) && x.Id != curUserId);

            var curUser = _dbContext.Users.FirstOrDefault(x => x.Id == curUserId);
            var messageDto = _mapper.Map<MessageModel>(message);
            messageDto.AuthorName = curUser.Username;
            var messageJsonString = JsonConvert.SerializeObject(messageDto);

            await _hubContext.Clients.User(otherUserInChat!.Id.ToString()).SendAsync("ReceiveEditedMessage", messageJsonString);
            await _hubContext.Clients.User(curUserId.ToString()).SendAsync("ReceiveEditedMessage", messageJsonString);
        }
        public async void Delete(int id, ClaimsPrincipal user)
        {
            var message = _dbContext.Messages.FirstOrDefault(r => r.Id == id);
            if (message is null) throw new NotFoundException("Message not found");

            var authorization = _authorizationService.AuthorizeAsync(user, message, new ResourceOperationRequirement(ResourceOperation.Update)).Result;
            if (!authorization.Succeeded) throw new UnauthorizedException("You don't have access to this");

            message.MessageTypeEnum = MessageTypeEnum.Deleted;
            message.MessageText = "Has deleted message";

            _dbContext.SaveChanges();

            var curUserId = _accountContext.GetAccountId();
            var otherUserInChat = _dbContext.Users.FirstOrDefault(x => x.ChannelsIds.Contains(message.ChannelId) && x.Id != curUserId);

            var curUser = _dbContext.Users.FirstOrDefault(x => x.Id == curUserId);
            var messageDto = _mapper.Map<MessageModel>(message);
            messageDto.AuthorName = curUser.Username;
            var messageJsonString = JsonConvert.SerializeObject(messageDto);

            await _hubContext.Clients.User(otherUserInChat!.Id.ToString()).SendAsync("DeleteMessage", messageJsonString);
            await _hubContext.Clients.User(curUserId.ToString()).SendAsync("DeleteMessage", messageJsonString);
        }
    }
}
