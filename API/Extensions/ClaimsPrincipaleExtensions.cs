using System.Security.Claims;
using System;
namespace API.Extensions
{
    public static class ClaimsPrincipaleExtensions
    {
        public static string GetUserConnect( this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        }
    }
}