using Application.Interfaces.Merchant;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.Merchant
{
    public class MerchantRepository : RepositoryAsync<Domain.Entities.Merchant.Merchant, long>, IMerchantRepository
    {
        public MerchantRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
