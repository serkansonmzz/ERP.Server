using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Interfaces.Repositories;
using ERP.Server.Infrastructure.Data.Repositories.Base;

namespace ERP.Server.Infrastructure.Data.Repositories.OutBoxes;


public class OutBoxRepository : EfCommandRepository<OutBox>, IOutboxRepository
{

    // base EfCommandRepository<OutBox> üzerinden _context zaten geliyor, tekrar tanımlamaya gerek yok.

    public OutBoxRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<OutBox>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OutBoxes
            .Where(x => x.IsCompleted == false)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
