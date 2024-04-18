using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace SMS_API
{
  [AttributeUsage(AttributeTargets.Method)]
    public class RightId : TypeFilterAttribute
    {
        public string ControllerId { get; }
        public string Id { get; }

        public RightId(string controllerId, string id)
            : base(typeof(AuthorizeAttribute))
        {
            this.ControllerId = controllerId;
            this.Id = id;

            Arguments = new object[] { this.ControllerId, this.Id };
        }
    }
    public class AuthorizeAttribute : IAsyncAuthorizationFilter
    {
        private readonly string _controllerId;
        private readonly string _rightId;
        private readonly IRoleRepository _roleRepository;

        public AuthorizeAttribute(string controllerId, string rightId, IRoleRepository roleRepository)
        {
            _controllerId = controllerId;
            _rightId = rightId;
            _roleRepository = roleRepository;
        } 
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (int.TryParse(context.HttpContext.User.FindFirstValue("RoleId"), out var roleId))
            {
                var role = await _roleRepository.FindByCondition(o => o.Id == roleId).Include(o => o.RoleRights).FirstOrDefaultAsync();
                if (!role.RoleRights.Any(o => o.ControllerId == _controllerId && o.ControllerRightsId == _rightId))
                {
                    context.Result = new ForbidResult();
                }
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }
}

   
