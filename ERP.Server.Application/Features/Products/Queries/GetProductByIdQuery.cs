using AutoMapper;
using ERP.Server.Application.Common.Dtos;
using ERP.Server.Application.Common.Results;
using ERP.Server.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Server.Application.Features.Products.Queries;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.ProductQueryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
            {
                return Result<ProductDto>.Failure("Product not found");
            }

            var dto = _mapper.Map<ProductDto>(product);
            return Result<ProductDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with id {Id}", request.Id);
            return Result<ProductDto>.Failure("An error occurred while retrieving the product", new List<string> { ex.Message });
        }
    }
}
