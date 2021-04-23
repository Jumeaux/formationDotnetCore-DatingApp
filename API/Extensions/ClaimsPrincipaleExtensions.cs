using System.Security.Claims;
using System;
namespace API.Extensions
{
    public static class ClaimsPrincipaleExtensions
    {
        public static string GetUsername( this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
        
        public static int GetUserId( this ClaimsPrincipal user)
        { 
            var res=int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);

           // return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value); 
            return res;

        }
    }
}