using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Poochatting.Entities;
using System.Security.Claims;

namespace Poochatting.Services
{
    public interface IAccountContextService
    {
        int GetAccountId();
    }
    public class AccountContextService(IHttpContextAccessor httpContextAccessor) : IAccountContextService
    {
        public ClaimsPrincipal User => httpContextAccessor.HttpContext!.User;

        public int GetAccountId()
        {
            return int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
