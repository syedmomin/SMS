namespace SMS_API.Entity
{
  public class User : BaseEntity
  {
    public string UserName { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
    public Role? Role { get; set; }
  }
}
