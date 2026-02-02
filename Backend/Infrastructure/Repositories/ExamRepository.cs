using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class ExamRepository : Repository, IExamRepository
{
    public ExamRepository(IDatabaseContext dbContext) : base(dbContext)
    {
    }
}
