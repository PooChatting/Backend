using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Poochatting.ChatHub
{
    [Authorize]
    public class ChatHub : Hub
    {
    }
}
