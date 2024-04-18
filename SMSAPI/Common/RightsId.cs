using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using School.Repository;
using System.Security.Claims;


namespace SMS_API
{
  [AttributeUsage(AttributeTargets.Method)]
    public class RightId : TypeFilterAttribute
    {
        public string ControllerId { get; }
        public string Id { get; }
        public string Caption { get; }

        public RightId(string controllerId, string id,string caption)
            : base(typeof(AuthorizeAttribute))
        {
            this.ControllerId = controllerId;
            this.Id = id;
            this.Caption = caption;

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
                var role = await _roleRepository.FindAsync(roleId);
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

   
