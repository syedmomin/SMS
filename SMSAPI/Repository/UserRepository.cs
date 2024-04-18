using Microsoft.IdentityModel.Tokens;
using SMS_API.Common;
using SMS_API.Entity;
using SMS_API.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SMS_API
{
  public class UserRepository : RepositoryBase<User>, IUserRepository
  {
    private readonly IConfiguration _configuration;
    public UserRepository(ApplicationDBContext db, IConfiguration configuration)
            : base(db)
    {
      _configuration = configuration;
    }
    protected override Task WhileInserting(User entity)
    {
      if (string.IsNullOrEmpty(entity.UserName) || string.IsNullOrEmpty(entity.Password))
      {
        throw new ServiceException("Username or password missing");
      }

      entity.Password = Helper.EnccyptText(entity.Password);

      return base.WhileInserting(entity);
    }
    public async Task<LoginInfo> LogIn(Login login)
    {
      var password = Helper.EnccyptText(login.Password);

      var User = this.FindByCondition(o => o.UserName == login.UserName && o.Password == password).FirstOrDefault();

      if (User == null)
      {
        throw new ServiceException($"Invalid username or password");
      }
      else
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["ApplicationSettings:JwtKey"]);

        var tokenDiscription = new SecurityTokenDescriptor
        {
          Subject = new ClaimsIdentity(new Claim[]
            {
                        new Claim(ClaimTypes.Name, User.UserName),
                        new Claim(ClaimKey.UserId.ToString(), User.Id.ToString()),
                        new Claim(ClaimKey.RoleId.ToString(), User.RoleId.ToString())

            }),
          Expires = DateTime.Now.AddDays(3),
          Issuer = _configuration["ApplicationSettings:Issuer"],
          Audience = _configuration["ApplicationSettings:Audience"],
          SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDiscription);
        User.Password = null;


        return new LoginInfo { User = User, Token = tokenHandler.WriteToken(token) };
      }

    }
  }
  public interface ILoginService
  {
    Task<LoginInfo> LogIn(Login login);
  }
  public enum ClaimKey
  {
    UserId = 1,
    RoleId,
    TeacherId
  }
  public class Login
  {
    public string UserName { get; set; }
    public string Password { get; set; }
  }
  public class LoginInfo
  {
    public string Token { get; set; }
    public User User { get; set; }
  }

  public interface IUserRepository : IRepositoryBase<User>
  {
  }
}
