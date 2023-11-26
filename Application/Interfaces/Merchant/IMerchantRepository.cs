using Application.Interfaces.Repositories;

namespace Application.Interfaces.Merchant
{
    public interface IMerchantRepository : IRepositoryAsync<Domain.Entities.Merchant.Merchant, long>
    {
    }
}
