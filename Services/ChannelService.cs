using AutoMapper;
using Poochatting.Entities;
using Poochatting.Exceptions;

namespace Poochatting.Services
{
    public interface IChannelService
    {
        void addToChannel(int channelId);
    }
    public class ChannelService : IChannelService
    {
        private readonly MessageDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAccountContextService _accountContext;
        public ChannelService(MessageDbContext dbContext, IMapper mapper, IAccountContextService accountContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _accountContext = accountContext;
        }

        public void addToChannel(int channelId)
        {
            var userId = _accountContext.GetAccountId();
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
            if (user.ChannelsIds.Contains(channelId)) throw new BadRequestException("User already in channel");

            user.ChannelsIds.Add(channelId);

            _dbContext.SaveChanges();

        }

    }
}
