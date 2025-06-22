using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERP.Server.Domain.Entities;

namespace ERP.Server.Domain.Interfaces.Repositories;

public interface IOutboxRepository : ICommandRepository<OutBox>
{
    Task<IReadOnlyList<OutBox>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutBox>> GetByConditionAsync(Expression<Func<OutBox, bool>> filter, CancellationToken cancellationToken = default);
}
