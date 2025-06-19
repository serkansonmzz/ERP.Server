using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ERP.Server.Domain.Entities;

namespace ERP.Server.Domain.Interfaces.Repositories;

public interface IOutboxRepository : ICommandRepository<OutBox>
{
    Task<IReadOnlyList<OutBox>> GetAllAsync(CancellationToken cancellationToken = default);
}
