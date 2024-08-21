using Core.Entities;

namespace Core.Repositories;

public interface IOwnerRepository : IBaseRepository<Owner>
{
    Owner GetByPropertyId(int propertyId);
}
