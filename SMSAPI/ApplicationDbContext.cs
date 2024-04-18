using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMS_API.Entity;
using System.Data;

namespace SMS_API
{
  public class ApplicationDBContext : DbContext
  {
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IConfiguration configuration;
    public ApplicationDBContext(
            DbContextOptions<ApplicationDBContext> options,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
    {
      this.configuration = configuration;
      this.httpContextAccessor = httpContextAccessor;
    }
    public virtual DbSet<RoleRights> RoleRights { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
      OnBeforeSaving();
      return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
    {
      OnBeforeSaving();
      return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    public Task<int> SaveChangesWithoutStamp()
    {
      return base.SaveChangesAsync(true, default);
    }
    protected virtual void OnBeforeSaving()
    {
      var entries = ChangeTracker.Entries();
      var now = DateTime.UtcNow;
      var user = GetCurrentUser();

      foreach (var entry in entries)
      {
        if (entry.Entity is BaseEntity baseEntity)
        {
          switch (entry.State)
          {
            case EntityState.Modified:
              entry.Property("CreatedAt").IsModified = false;
              entry.Property("CreatedBy").IsModified = false;
              baseEntity.EditTime = now;
              baseEntity.EditUserId = "";
              break;

            case EntityState.Added:
              baseEntity.CreateTime = now;
              baseEntity.CreatedUserId = "";
              baseEntity.EditTime = now;
              baseEntity.EditUserId = "";
              break;
          }
        }
      }
    }
    private string GetCurrentUser() => $"{GetContextClaims("ClaimKey") ?? string.Empty} : {GetContextClaims("ClaimKey") ?? "<unknown>"}";

    private string GetContextClaims(string typeName) => this.httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(c => c?.Type == typeName)?.Value;
  }

}
