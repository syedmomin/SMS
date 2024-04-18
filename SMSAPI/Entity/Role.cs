namespace SMS_API.Entity
{
  public class Role : BaseEntity
  {
    public string Name { get; set; }
    public RoleType RoleTypes { get; set; }

    private ICollection<RoleRights>? _roleRights;
    public ICollection<RoleRights>? RoleRights
    {
      get => _roleRights ?? (_roleRights = new List<RoleRights>());
    }
  }
  public enum RoleType
  {
    Admin = 1,
    User
  }
}
