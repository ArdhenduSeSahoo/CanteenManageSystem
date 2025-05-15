using Microsoft.EntityFrameworkCore;

namespace CanteenManage.CanteenRepository.Contexts
{
    public class CanteenManageContextFactory : IDbContextFactory<CanteenManageDBContext>
    {
        private readonly IDbContextFactory<CanteenManageDBContext> _pooledFactory;
        public CanteenManageContextFactory(IDbContextFactory<CanteenManageDBContext> pooledFactory)
        {
            _pooledFactory = pooledFactory;
        }
        public CanteenManageDBContext CreateDbContext()
        {
            var context = _pooledFactory.CreateDbContext();
            //context.TenantId = _tenantId;
            return context;
        }
    }
}
