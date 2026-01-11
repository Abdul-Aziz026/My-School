using Application.Common.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class ClassRepository : Repository, IClassRepository
{
    public ClassRepository(DatabaseContext dbContext) : base(dbContext)
    {
    }
}
