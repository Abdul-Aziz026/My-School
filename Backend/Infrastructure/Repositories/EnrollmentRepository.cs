using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class EnrollmentRepository : Repository, IEnrollmentRepository
{
    public EnrollmentRepository(IDatabaseContext dbContext) : base(dbContext)
    {
    }
}
