using System.Web;
using System.Web.Mvc;
using MyStoreMVC5.Models;

namespace MyStoreMVC5.Security
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string requiredRole;
        public RoleAuthorizeAttribute(string role) { requiredRole = role; }

        protected override bool AuthorizeCore(HttpContextBase context)
        {
            if (!base.AuthorizeCore(context) || context.User == null || !context.User.Identity.IsAuthenticated)
                return false;
            using (var db = new StoreDbContext())
            {
                var user = db.Users.Find(context.User.Identity.Name);
                return user != null && (user.UserRole ?? "").Trim() == requiredRole;
            }
        }
    }
}
