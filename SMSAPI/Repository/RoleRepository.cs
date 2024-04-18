using SMS_API.Entity;
using SMS_API.Repository;

namespace SMS_API
{
  public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDBContext db)
            :base(db)
        {
        }
    }
    public interface IRoleRepository : IRepositoryBase<Role>
    {
    }
}
