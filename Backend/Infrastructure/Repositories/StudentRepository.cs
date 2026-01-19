
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class StudentRepository : Repository, IStudentRepository
{
    public StudentRepository(IDatabaseContext dbContext) : base(dbContext)
    {
    }
}
