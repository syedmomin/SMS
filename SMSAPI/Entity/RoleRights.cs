namespace SMS_API.Entity
{
  public class RoleRights : BaseEntity
  {
    public int RoleId { get; set; }
    public string ControllerId { get; set; }
    public string ControllerRightsId { get; set; }
    public Role? Role { get; set; }
  }
}
