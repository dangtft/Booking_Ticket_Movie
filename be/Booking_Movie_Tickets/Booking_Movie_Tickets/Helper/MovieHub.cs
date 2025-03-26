using StackExchange.Redis;
using Microsoft.AspNetCore.SignalR;

namespace Booking_Movie_Tickets.Helper
{
    public class MovieHub : Hub
    {
        private readonly IConnectionMultiplexer _redis;

        public MovieHub(IConnectionMultiplexer redis)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));

            // Kiểm tra kết nối Redis
            var db = _redis.GetDatabase();
            if (!db.Multiplexer.IsConnected)
            {
                throw new Exception("Không thể kết nối đến Redis");
            }
        }

        public async Task SelectSeat(string showtimeId, string seatId, string userId)
        {
            var db = _redis.GetDatabase();
            string key = $"seat:{showtimeId}:{seatId}";

            var existingUser = await db.StringGetAsync(key);

            if (!existingUser.IsNullOrEmpty && existingUser.ToString() != userId)
            {
                await Clients.Caller.SendAsync("SeatTaken", seatId);
                return;
            }

            await db.StringSetAsync(key, userId, TimeSpan.FromMinutes(5));
            await Clients.All.SendAsync("SeatSelected", seatId, userId);
        }


        public async Task ReleaseSeat(string showtimeId, string seatId)
        {
            var db = _redis.GetDatabase();
            string key = $"seat:{showtimeId}:{seatId}";

            // Xóa ghế khỏi Redis
            await db.KeyDeleteAsync(key);

            // Gửi thông báo tới client để mở ghế lại
            await Clients.All.SendAsync("SeatReleased", seatId);
        }
    }
}
