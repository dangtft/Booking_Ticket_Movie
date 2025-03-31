using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Extras.Request;
using Booking_Movie_Tickets.DTOs.Orders.Request;
using Booking_Movie_Tickets.DTOs.Orders.Response;
using Booking_Movie_Tickets.DTOs.Tickets.Request;
using Booking_Movie_Tickets.DTOs.Tickets.Response;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Orders;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Text.Json;

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
        //public async Task<OrderResponse> CreateOrderAsync(OrderRequest request)
        //{
        //    if (request == null || string.IsNullOrEmpty(request.UserId) || request.OrderDetails == null || !request.OrderDetails.Any())
        //    {
        //        Console.WriteLine("OrderRequest is null or missing required fields.");
        //        return null;
        //    }

        //    using var transaction = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var order = new Order
        //        {
        //            Id = Guid.NewGuid(),
        //            UserId = request.UserId,
        //            IsDeleted = false,
        //            TotalAmount = 0,
        //            CreatedAt = DateTime.UtcNow,
        //            Status = "PENDING"
        //        };

        //        await _context.Orders.AddAsync(order);
        //        await _context.SaveChangesAsync();

        //        foreach (var detailRequest in request.OrderDetails)
        //        {
        //            if (detailRequest == null) continue;

        //            var orderDetail = new OrderDetail
        //            {
        //                Id = Guid.NewGuid(),
        //                OrderId = order.Id,
        //                Subtotal = 0
        //            };

        //            await _context.OrderDetails.AddAsync(orderDetail);
        //            await _context.SaveChangesAsync();

        //            if (detailRequest.TicketRequests != null)
        //            {
        //                foreach (var ticketRequest in detailRequest.TicketRequests)
        //                {
        //                    if (ticketRequest == null || ticketRequest.ShowTimeId == Guid.Empty || ticketRequest.SeatId == Guid.Empty)
        //                    {
        //                        Console.WriteLine("Invalid TicketRequest.");
        //                        await transaction.RollbackAsync();
        //                        return null;
        //                    }

        //                    var existingTicket = await _context.Tickets
        //                        .Include(t => t.OrderDetail)
        //                        .ThenInclude(od => od.Order)
        //                        .FirstOrDefaultAsync(t => t.ShowTimeId == ticketRequest.ShowTimeId &&
        //                                                  t.SeatId == ticketRequest.SeatId &&
        //                                                  !t.OrderDetail.Order.IsDeleted);

        //                    if (existingTicket != null)
        //                    {
        //                        Console.WriteLine($"Seat {ticketRequest.SeatId} is already booked.");
        //                        await transaction.RollbackAsync();
        //                        return null;
        //                    }

        //                    var newTicketRequest = new TicketRequest
        //                    {
        //                        ShowTimeId = ticketRequest.ShowTimeId,
        //                        SeatId = ticketRequest.SeatId,
        //                        TicketPrice = ticketRequest.TicketPrice,
        //                        OrderDetailId = orderDetail.Id
        //                    };

        //                    var ticket = _ticketService.CreateTicket(newTicketRequest);
        //                    if (ticket == null)
        //                    {
        //                        await transaction.RollbackAsync();
        //                        return null;
        //                    }

        //                    orderDetail.Subtotal += ticket.TicketPrice;
        //                    order.TotalAmount += ticket.TicketPrice;
        //                }
        //            }

        //            if (detailRequest.Extras != null)
        //            {
        //                foreach (var extraRequest in detailRequest.Extras)
        //                {
        //                    var extra = await _context.Extras.FindAsync(extraRequest.Id);
        //                    if (extra != null)
        //                    {
        //                        var orderDetailExtra = new OrderDetailExtras
        //                        {
        //                            OrderDetailId = orderDetail.Id,
        //                            ExtraId = extra.Id,
        //                            Quantity = extraRequest.Quantity
        //                        };

        //                        _context.OrderDetailExtras.Add(orderDetailExtra);
        //                        orderDetail.Subtotal += extra.Price * extraRequest.Quantity;
        //                        order.TotalAmount += extra.Price * extraRequest.Quantity;
        //                    }
        //                }
        //            }

        //            await _context.SaveChangesAsync();
        //        }

        //        await _context.SaveChangesAsync();
        //        await transaction.CommitAsync();

        //        return new OrderResponse
        //        {
        //            OrderId = order.Id,
        //            TotalAmount = order.TotalAmount,
        //            Status = order.Status,
        //            CreatedAt = order.CreatedAt
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        Console.WriteLine($"Error creating order: {ex.Message}");
        //        return null;
        //    }
        //}


        public async Task<OrderResponse> CreateOrderAsync(OrderRequest request)
        {
            if (!ValidateOrderRequest(request)) return null;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await CreateOrderAsync(request.UserId);

                foreach (var detailRequest in request.OrderDetails)
                {
                    if (detailRequest == null) continue;
                    var orderDetail = await CreateOrderDetailAsync(order.Id);

                    if (!await ProcessTicketsAsync(detailRequest, orderDetail, order, transaction)) return null;
                    if (!await ProcessExtrasAsync(detailRequest, orderDetail)) return null;

                    await _context.SaveChangesAsync();

                    order.TotalAmount += orderDetail.Subtotal;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OrderResponse
                {
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating order: {ex.Message}");
                return null;
            }
        }

        private bool ValidateOrderRequest(OrderRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || request.OrderDetails == null || !request.OrderDetails.Any())
            {
                Console.WriteLine("OrderRequest is null or missing required fields.");
                return false;
            }
            return true;
        }

        private async Task<Order> CreateOrderAsync(string userId)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IsDeleted = false,
                TotalAmount = 0,
                CreatedAt = DateTime.UtcNow,
                Status = "PENDING"
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        private async Task<OrderDetail> CreateOrderDetailAsync(Guid orderId)
        {
            var orderDetail = new OrderDetail
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Subtotal = 0
            };

            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();
            return orderDetail;
        }

        private async Task<bool> ProcessTicketsAsync(OrderDetailRequest detailRequest, OrderDetail orderDetail, Order order, IDbContextTransaction transaction)
        {
            if (detailRequest.TicketRequests == null) return true;

            foreach (var ticketRequest in detailRequest.TicketRequests)
            {
                if (ticketRequest == null || ticketRequest.ShowTimeId == Guid.Empty || ticketRequest.SeatId == Guid.Empty)
                {
                    Console.WriteLine("Invalid TicketRequest.");
                    await transaction.RollbackAsync();
                    return false;
                }

                if (await IsSeatAlreadyBooked(ticketRequest.ShowTimeId, ticketRequest.SeatId))
                {
                    Console.WriteLine($"Seat {ticketRequest.SeatId} is already booked.");
                    await transaction.RollbackAsync();
                    return false;
                }

                var ticket = _ticketService.CreateTicket(new TicketRequest
                {
                    ShowTimeId = ticketRequest.ShowTimeId,
                    SeatId = ticketRequest.SeatId,
                    TicketPrice = ticketRequest.TicketPrice,
                    OrderDetailId = orderDetail.Id
                });

                if (ticket == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                orderDetail.Subtotal += ticket.TicketPrice;
                await _context.SaveChangesAsync();
            }
            return true;
        }

        private async Task<bool> IsSeatAlreadyBooked(Guid showTimeId, Guid seatId)
        {
            return await _context.Tickets
                .Include(t => t.OrderDetail)
                .ThenInclude(od => od.Order)
                .AnyAsync(t => t.ShowTimeId == showTimeId && t.SeatId == seatId && !t.OrderDetail.Order.IsDeleted);
        }

        private async Task<bool> ProcessExtrasAsync(OrderDetailRequest detailRequest, OrderDetail orderDetail)
        {
            if (detailRequest.Extras == null) return true;

            foreach (var extraRequest in detailRequest.Extras)
            {
                var extra = await _context.Extras.FindAsync(extraRequest.Id);
                if (extra != null)
                {
                    _context.OrderDetailExtras.Add(new OrderDetailExtras
                    {
                        OrderDetailId = orderDetail.Id,
                        ExtraId = extra.Id,
                        Quantity = extraRequest.Quantity
                    });

                    decimal extraCost = extra.Price * extraRequest.Quantity;
                    orderDetail.Subtotal += extraCost;
                    await _context.SaveChangesAsync();
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> DeleteOrderById(Guid orderId)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("DeleteOrderById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.UniqueIdentifier) { Value = orderId });

                    var result = await command.ExecuteNonQueryAsync();

                    return result > 0; 
                }
            }
        }

        public async Task<bool> UpdateOrderStatus(Guid orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return false;
            }

            order.Status = status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<OrderResponse>> GetOrdersByUserIdAsync(string userId)
            {
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("UserId is null or empty.");
                    return new List<OrderResponse>();
                }

                try
                {
                    var orders = await _context.Orders
                        .Where(o => o.UserId == userId && !o.IsDeleted)
                        .OrderByDescending(o => o.CreatedAt)
                        .Select(o => new OrderResponse
                        {
                            OrderId = o.Id,
                            TotalAmount = o.TotalAmount,
                            Status = o.Status,
                            CreatedAt = o.CreatedAt
                        })
                        .ToListAsync();

                    return orders;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving orders for UserId {userId}: {ex.Message}");
                    return new List<OrderResponse>();
                }
            }

        public async Task<List<TicketResponse>> GetTicketsByOrderIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                Console.WriteLine("OrderId is empty.");
                return new List<TicketResponse>();
            }

            try
            {
                var tickets = await _context.Tickets
                    .Where(t => t.OrderDetail.OrderId == orderId && !t.OrderDetail.Order.IsDeleted)
                    .Select(t => new TicketResponse
                    {
                        Id = t.Id,
                        ShowTimeDate = t.Showtime.Date,  
                        ShowTimeStart = t.Showtime.Time, 
                        SeatRow = t.Seat.Row, 
                        SeatNumber = t.Seat.SeatNumber,  
                        TicketPrice = t.TicketPrice,
                        QRCode = t.QRCode
                    })
                    .ToListAsync();

                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving tickets for OrderId {orderId}: {ex.Message}");
                return new List<TicketResponse>();
            }
        }

        public async Task<string> GetOrderStatus(Guid orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            return order?.Status;
        }
    }
}
