using FitFanShop.Application.Abstractions;
using FitFanShop.Application.Common.Exceptions;
using FitFanShop.Domain.Entities.Sales;
using FitFanShop.Domain.Entities.Tickets;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFanShop.Application.Modules.Sales.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly IAppDbContext _ctx;
    private readonly IAppCurrentUser _currentUser;

    public CreateOrderCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException();

        await using var tx = await _ctx.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var pendingStatus = await _ctx.OrderStatuses.FirstOrDefaultAsync(x => x.Name == "Pending", cancellationToken);
            if (pendingStatus == null)
                throw new FitFanShopBusinessRuleException("OrderStatusNotFound", "Order status 'Pending' not found in the database.");

            var order = new OrderEntity
            {
                UserId = userId.Value,
                StatusId = pendingStatus.Id,
                TotalAmount = 0m
            };

            if ((request.Products == null || request.Products.Count == 0) && (request.Tickets == null || request.Tickets.Count == 0))
                throw new FitFanShopBusinessRuleException("EmptyOrder", "Order must contain at least one product or ticket.");

            foreach (var p in request.Products ?? Enumerable.Empty<CreateOrderProductItemDto>())
            {
                if (p.Quantity <= 0)
                    throw new FitFanShopBusinessRuleException("InvalidQuantity", $"Quantity must be greater than 0 for product variant {p.ProductVariantId}.");

                var variant = await _ctx.ProductVariants
                    .Include(x => x.Product)
                    .ThenInclude(x => x.DiscountProducts)
                    .ThenInclude(dp => dp.Discount)
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.Id == p.ProductVariantId, cancellationToken);
                
                if (variant == null)
                    throw new FitFanShopNotFoundException($"Product variant {p.ProductVariantId} not found");
                
                if (variant.IsDeleted)
                    throw new FitFanShopBusinessRuleException("ProductDeleted", $"Product variant {p.ProductVariantId} is no longer available (deleted)");
                
                if (variant.Product == null)
                    throw new FitFanShopNotFoundException($"Product for variant {p.ProductVariantId} not found");
                
                if (variant.Product.IsDeleted)
                    throw new FitFanShopBusinessRuleException("ProductDeleted", $"Product for variant {p.ProductVariantId} is no longer available (deleted)");
                
                if (variant.StockQuantity < p.Quantity)
                    throw new FitFanShopBusinessRuleException("InsufficientStock", $"Not enough stock for product variant {p.ProductVariantId}. Requested: {p.Quantity}, Available: {variant.StockQuantity}");

                decimal price = variant.Product.Price;
                var discount = variant.Product.DiscountProducts
                    .Select(dp => dp.Discount)
                    .FirstOrDefault(d => d.IsActive && d.StartDate <= DateTime.UtcNow && d.EndDate >= DateTime.UtcNow);
                if (discount != null)
                    price = price * (1 - discount.Percentage / 100m);

                var item = new OrderItemEntity
                {
                    ProductVariantId = p.ProductVariantId,
                    Quantity = p.Quantity,
                    UnitPrice = price,
                };
                order.Items.Add(item);
                order.TotalAmount += price * p.Quantity;

                variant.StockQuantity -= p.Quantity;
            }

            foreach (var t in request.Tickets ?? Enumerable.Empty<CreateOrderTicketItemDto>())
            {
                if (t.Quantity <= 0)
                    throw new FitFanShopBusinessRuleException("InvalidQuantity", $"Quantity must be greater than 0 for ticket type {t.TicketTypeId}.");

                var ticketType = await _ctx.TicketTypes.FirstOrDefaultAsync(x => x.Id == t.TicketTypeId, cancellationToken);
                if (ticketType == null)
                    throw new FitFanShopNotFoundException($"Ticket type {t.TicketTypeId} not found");
                
                if (ticketType.TotalAvailable < t.Quantity)
                    throw new FitFanShopBusinessRuleException("InsufficientTickets", $"Not enough tickets available for ticket type {t.TicketTypeId}. Requested: {t.Quantity}, Available: {ticketType.TotalAvailable}");

                int soldCount = await _ctx.Tickets.CountAsync(x => x.TicketTypeId == t.TicketTypeId, cancellationToken);

                string sectionPrefix;
                if (!string.IsNullOrEmpty(ticketType.Name) && ticketType.Name.Trim().ToUpper().StartsWith("VIP"))
                    sectionPrefix = "VIP";
                else
                    sectionPrefix = !string.IsNullOrEmpty(ticketType.Name) ? ticketType.Name.Trim().ToUpper()[0].ToString() : "X";

                for (int i = 0; i < t.Quantity; i++)
                {
                    var seatNumber = $"{sectionPrefix}-{soldCount + i + 1}";
                    var ticket = new TicketEntity
                    {
                        TicketTypeId = t.TicketTypeId,
                        EventId = ticketType.EventId,
                        UserId = userId.Value,
                        PricePaid = ticketType.Price,
                        QRCode = $"QR-{Guid.NewGuid():N}",
                        SeatNumber = seatNumber
                    };
                    order.Tickets.Add(ticket);
                    order.TotalAmount += ticketType.Price;
                }

                ticketType.TotalAvailable -= t.Quantity;
            }

            var member = await _ctx.Members.FirstOrDefaultAsync(m =>
                m.UserId == userId &&
                !m.IsDeleted &&
                (m.EndDate == null || m.EndDate >= DateTime.UtcNow), cancellationToken);
            if (member != null)
                order.TotalAmount *= 0.9m;

            _ctx.Orders.Add(order);
            await _ctx.SaveChangesAsync(cancellationToken);
            
            foreach (var ticket in order.Tickets)
            {
                ticket.OrderId = order.Id;
            }
            
            await _ctx.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return new CreateOrderResponse
            {
                OrderId = order.Id,
                OrderNumber = $"ORD-{order.Id:D6}",
                Total = order.TotalAmount,
                Status = "Pending"
            };
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
