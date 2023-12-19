using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder;

public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ILogger<CheckoutOrderCommandHandler> _logger;

    public CheckoutOrderCommandHandler(
        IOrderRepository repository,
        IMapper mapper,
        IEmailService emailService,
        ILogger<CheckoutOrderCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var order = _mapper.Map<Order>(request);

        Order newOrder = await _repository.AddAsync(order);

        _logger.LogInformation($"Order {newOrder.Id} is successfully created.");

        await SendEmail(newOrder);

        return newOrder.Id;
    }

    private async Task SendEmail(Order order)
    {
        var email = new Email()
        {
            To = "exe.pine@gmail.com",
            Body = $"Order was created.",
            Subject = $"Order {order.Id} created!"
        };

        try
        {
            await _emailService.SendEmail(email);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Order {order.Id} failed due to an error with the email service: {ex.Message}");
        }
    }
}
