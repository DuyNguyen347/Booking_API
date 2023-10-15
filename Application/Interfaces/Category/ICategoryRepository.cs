using Application.Interfaces.Repositories;

namespace Application.Interfaces.Category
{
    public interface ICategoryRepository : IRepositoryAsync<Domain.Entities.Category.Category, long>
    {
    }   
}
