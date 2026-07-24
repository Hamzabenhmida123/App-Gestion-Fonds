using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.Data;

namespace FondsSocial.Infrastructure.Repositories
{
    public class TypeDePretRepository : Repository<TypeDePret>, ITypeDePretRepository
    {
        public TypeDePretRepository(FondsSocialDbContext context) : base(context)
        {
        }
    }
}
