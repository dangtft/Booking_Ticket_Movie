using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Extras.Request;
using Booking_Movie_Tickets.DTOs.Orders.Request;
using Booking_Movie_Tickets.DTOs.Tickets.Request;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Orders;
using Booking_Movie_Tickets.Models.Tickets;
using Microsoft.EntityFrameworkCore;

namespace Booking_Movie_Tickets.Services
{
    public class OrderService : IOrderService
    {
        private readonly BookingDbContext _context;
        private readonly ITicketService _ticketService;

        public OrderService(BookingDbContext context, ITicketService ticketService)
        {
            _context = context;
            _ticketService = ticketService;
        }
        public async Task<bool> CreateOrderAsync(OrderRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || request.OrderDetails == null || !request.OrderDetails.Any())
            {
                Console.WriteLine("OrderRequest is null or missing required fields.");
                return false;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    IsDeleted = false,
                    TotalAmount = 0,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                foreach (var detailRequest in request.OrderDetails)
                {
                    if (detailRequest == null)
                    {
                        Console.WriteLine("OrderDetailRequest is null.");
                        return false;
                    }
                    var orderDetail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        Subtotal = 0
                    };

                    Console.WriteLine($"Created OrderDetail: {orderDetail.Id}");

                    await _context.OrderDetails.AddAsync(orderDetail);
                    await _context.SaveChangesAsync(); 

                    if (detailRequest.TicketRequests != null)
                    {
                        foreach (var ticketRequest in detailRequest.TicketRequests)
                        {
                            if (ticketRequest == null || ticketRequest.ShowTimeId == Guid.Empty || ticketRequest.SeatId == Guid.Empty || ticketRequest.TicketTypeId == Guid.Empty)
                            {
                                Console.WriteLine("TicketRequest contains null or empty values.");
                                await transaction.RollbackAsync();
                                return false;
                            }

                            Console.WriteLine($"Processing Ticket for ShowTime: {ticketRequest.ShowTimeId}");

                            var newTicketRequest = new TicketRequest
                            {
                                ShowTimeId = ticketRequest.ShowTimeId,
                                SeatId = ticketRequest.SeatId,
                                TicketTypeId = ticketRequest.TicketTypeId,
                                TicketPrice = ticketRequest.TicketPrice,
                                OrderDetailId = orderDetail.Id 
                            };

                            var ticket = _ticketService.CreateTicket(newTicketRequest);

                            if (ticket == null)
                            {
                                Console.WriteLine("Failed to create Ticket.");
                                await transaction.RollbackAsync();
                                return false;
                            }

                            Console.WriteLine($"Created Ticket: {ticket.Id}");

                            orderDetail.Subtotal += ticket.TicketPrice;
                            order.TotalAmount += ticket.TicketPrice;
                        }
                    }

                    // Xử lý Extra
                    if (detailRequest.Extras != null && detailRequest.Extras.Any())
                    {
                        foreach (var extraRequest in detailRequest.Extras)
                        {
                            if (extraRequest == null || extraRequest.Id == Guid.Empty)
                            {
                                Console.WriteLine("ExtraRequest is null or has an empty ID.");
                                continue;
                            }

                            var extra = await _context.Extras.FindAsync(extraRequest.Id);

                            if (extra == null)
                            {
                                Console.WriteLine($"Extra with ID {extraRequest.Id} not found.");
                                continue;
                            }

                            Console.WriteLine($"Adding Extra: {extra.Id} with Quantity: {extraRequest.Quantity}");

                            if (extra != null)
                            {
                                var orderDetailExtra = new OrderDetailExtras
                                {
                                    OrderDetailId = orderDetail.Id,
                                    ExtraId = extra.Id,
                                    Quantity = extraRequest.Quantity
                                };

                                _context.OrderDetailExtras.Add(orderDetailExtra);
                                orderDetail.Subtotal += extra.Price * extraRequest.Quantity; 
                                order.TotalAmount += extra.Price * extraRequest.Quantity;
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Lỗi khi tạo Order: {ex.Message}");
                return false;
            }
        }

        public async Task CleanupUnpaidOrdersAsync()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var expirationTime = DateTime.UtcNow.AddMinutes(-5);

                var unpaidOrders = await _context.Orders
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Tickets)
                    .Where(o => !_context.Payments.Any(p => p.OrderId == o.Id) && o.OrderDetails.Any())
                    .ToListAsync();

                if (!unpaidOrders.Any())
                {
                    await transaction.CommitAsync();
                    return;
                }

                var ticketIds = unpaidOrders
                    .SelectMany(o => o.OrderDetails)
                    .SelectMany(od => od.Tickets)
                    .Select(t => t.Id)
                    .Distinct()
                    .ToList();

                if (ticketIds.Any())
                {
                    var seatStatuses = await _context.SeatStatuses
                        .Where(s => ticketIds.Contains(s.Seat_Id))
                        .ToListAsync();

                    _context.SeatStatuses.RemoveRange(seatStatuses);
                }

                foreach (var order in unpaidOrders)
                {
                    order.IsDeleted = true;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"🔥 Error in CleanupUnpaidOrdersAsync: {ex.Message}");
            }
        }
    }
}
