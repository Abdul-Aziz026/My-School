
using Application.Common.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class SubjectRepository : Repository, ISubjectRepository
{
    public SubjectRepository(DatabaseContext dbContext) : base(dbContext)
    {
    }
}
