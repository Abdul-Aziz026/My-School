
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class TimeTableRepository : Repository, ITimeTableRepository
{
    public TimeTableRepository(IDatabaseContext dbContext) : base(dbContext)
    {
    }
}
