using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMS_API.Entity;
using System.Data;

namespace SMS_API
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }
        public ApplicationDbContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
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
                            baseEntity.EditUserId = user;
                            break;

                        case EntityState.Added:
                            baseEntity.CreateTime = now;
                            baseEntity.CreatedUserId = user;
                            baseEntity.EditTime = now;
                            baseEntity.EditUserId = user;
                            break;
                    }
                }
            }
        }
        private string GetCurrentUser() => $"{GetContextClaims("ClaimKey") ?? string.Empty} : {GetContextClaims("ClaimKey") ?? "<unknown>"}";

        private string GetContextClaims(string typeName) => _httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(c => c?.Type == typeName)?.Value;
    }

}
