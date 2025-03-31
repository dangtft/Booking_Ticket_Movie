using Microsoft.AspNetCore.SignalR;

namespace Booking_Movie_Tickets.Helper
{
    public class PaymentHub : Hub
    {
        public async Task SendPaymentStatus(string status, string message)
        {
            await Clients.All.SendAsync("ReceivePaymentStatus", status, message);
        }
    }
}
