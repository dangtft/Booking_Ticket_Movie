using Booking_Movie_Tickets.Data;
using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.DTOs.Tickets.Request;
using Booking_Movie_Tickets.Interfaces;
using Booking_Movie_Tickets.Models.Cinemas;
using Booking_Movie_Tickets.Models.Movies;
using Booking_Movie_Tickets.Models.Tickets;
using Microsoft.EntityFrameworkCore;
using ZXing;

namespace Booking_Movie_Tickets.Services
{
    public class TicketService : ITicketService
    {
        private readonly BookingDbContext _context;
        public TicketService(BookingDbContext context)
        {
            _context = context;
        }
        public Ticket CreateTicket(TicketRequest request)
        {
            if (request.OrderDetailId == Guid.Empty)
            {
                throw new InvalidOperationException("OrderDetailId không hợp lệ.");
            }

            var pendingStatus = _context.TicketStatuses
                .FirstOrDefault(ts => ts.StatusName == "Pending");

            if (pendingStatus == null)
            {
                throw new InvalidOperationException("Pending status not found.");
            }

            var showtime = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .AsNoTracking()
                .FirstOrDefault(s => s.Id == request.ShowTimeId);

            if (showtime == null)
            {
                throw new InvalidOperationException("Showtime không tồn tại.");
            }

            if (showtime.Movie == null || string.IsNullOrEmpty(showtime.Movie.Title))
            {
                throw new InvalidOperationException("Thông tin phim không hợp lệ hoặc bị thiếu.");
            }

            if (showtime.Room == null || string.IsNullOrEmpty(showtime.Room.Name))
            {
                throw new InvalidOperationException("Thông tin phòng chiếu không hợp lệ hoặc bị thiếu.");
            }

            var seat = _context.Seats
                .FirstOrDefault(s => s.Id == request.SeatId);

            if (seat == null)
            {
                throw new InvalidOperationException("Ghế không tồn tại.");
            }

            if (string.IsNullOrEmpty(seat.Row) || seat.SeatNumber <= 0)
            {
                throw new InvalidOperationException("Thông tin ghế không hợp lệ.");
            }

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                ShowTimeId = request.ShowTimeId,
                SeatId = request.SeatId,
                TicketStatusId = pendingStatus.Id,
                TicketPrice = request.TicketPrice,
                OrderDetailId = request.OrderDetailId
            };

            try
            {
                ticket.QRCode = GenerateQRCode(ticket, showtime, seat);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi tạo QRCode: {ex.Message}");
            }

            var seatStatus = _context.SeatStatusTracking
                .FirstOrDefault(s => s.SeatId == request.SeatId && s.ShowTimeId == request.ShowTimeId);

            if (seatStatus != null)
            {
                seatStatus.Status = "Pending";
            }

            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            return ticket;
        }

        public async Task<bool> ConfirmPayment(Guid ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.TicketStatus)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null || ticket.TicketStatus.StatusName != "Pending")
                return false;

            var paidStatus = await _context.TicketStatuses.FirstOrDefaultAsync(ts => ts.StatusName == "Paid");
            if (paidStatus == null)
                return false;

            ticket.TicketStatusId = paidStatus.Id;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelTicket(Guid ticketId) 
        {
            var ticket = await _context.Tickets
                .Include(t => t.TicketStatus)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null || ticket.TicketStatus.StatusName == "Paid")
                return false;

            _context.Tickets.Remove(ticket);

            var seatStatus = await _context.SeatStatusTracking
                .FirstOrDefaultAsync(s => s.SeatId == ticket.SeatId && s.ShowTimeId == ticket.ShowTimeId);

            if (seatStatus != null)
            {
                _context.SeatStatusTracking.Remove(seatStatus);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<string?> GetQRCodeAsync(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                throw new ArgumentException("Ticket ID is invalid", nameof(ticketId));
            }

            return await _context.Tickets
                .Where(t => t.Id == ticketId)
                .Select(t => t.QRCode) 
                .FirstOrDefaultAsync();
        }
        private string GenerateBarcode(string text)
        {
            var writer = new BarcodeWriterSvg
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 100,
                    Margin = 10
                }
            };

            var barcodeSvg = writer.Write(text);
            return barcodeSvg.Content;
        }

        private string GenerateQRCode(Ticket ticket, Showtime showtime, Seat seat)
        {
            string qrData = $"Movie: {showtime.Movie.Title}, " +
                            $"Date: {showtime.Date:yyyy-MM-dd}, " +
                            $"Time: {showtime.Time}, " +
                            $"Hall: {showtime.Room.Name}, " +
                            $"Row: {seat.Row}, " +
                            $"Seat: {seat.SeatNumber}";

            var qrCodeWriter = new BarcodeWriterSvg
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 300,
                    Margin = 10
                }
            };

            var svg = qrCodeWriter.Write(qrData);
            return svg.Content;
        }

    }
}
