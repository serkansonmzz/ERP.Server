// ERP.Server.Application/Features/Products/Queries/GetAllProductsQuery.cs
using AutoMapper;
using ERP.Server.Application.Common.Dtos;
using ERP.Server.Application.Common.Results;
using ERP.Server.Domain.Interfaces;
using MediatR;

namespace ERP.Server.Application.Features.Products.Queries;

public sealed record GetAllProductsQuery() : IRequest<Result<IReadOnlyList<ProductDto>>>;

public sealed class GetAllProductsQueryHandler 
    : IRequestHandler<GetAllProductsQuery, Result<IReadOnlyList<ProductDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;

    public GetAllProductsQueryHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ILogger<GetAllProductsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<ProductDto>>> Handle(
        GetAllProductsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(cancellationToken);
            var result = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            
            return Result<IReadOnlyList<ProductDto>>.Success(
                result,
                "Products retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving products");
            return Result<IReadOnlyList<ProductDto>>.Failure(
                "An error occurred while retrieving products",
                new List<string> { ex.Message });
        }
    }
}