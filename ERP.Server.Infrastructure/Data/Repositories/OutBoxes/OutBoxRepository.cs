using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Interfaces.Repositories;
using ERP.Server.Infrastructure.Data.Repositories.Base;

namespace ERP.Server.Infrastructure.Data.Repositories.OutBoxes;

public class OutBoxRepository : EfCommandRepository<OutBox>, IOutboxRepository
{
    public OutBoxRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<OutBox>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OutBoxes
            .Where(x => !x.IsCompleted)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OutBox>> GetByConditionAsync(Expression<Func<OutBox, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _context.Set<OutBox>().Where(filter).ToListAsync(cancellationToken);
    }
}
