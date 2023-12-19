using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList;

public class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrderVm>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetOrdersListQueryHandler(
        IOrderRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<OrderVm>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Order> orders = await _repository.GetOrdersByUserName(request.UserName);
        return _mapper.Map<List<OrderVm>>(orders);
    }
}