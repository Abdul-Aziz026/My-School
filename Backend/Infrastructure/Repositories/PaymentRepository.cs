using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class PaymentRepository : Repository, IPaymentRepository
{
    public PaymentRepository(IDatabaseContext dbContext) : base(dbContext)
    {
    }
}
