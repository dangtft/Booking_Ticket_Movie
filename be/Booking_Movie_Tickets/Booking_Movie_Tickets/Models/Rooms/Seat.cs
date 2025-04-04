﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Booking_Movie_Tickets.Models.Rooms
{
    public class Seat
    {
        [Key]
        public Guid Id { get; set; } 

        public Guid RoomId { get; set; }

        public string Row { get; set; }

        public int SeatNumber { get; set; }

        public Guid SeatTypeId { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }

        [ForeignKey("SeatTypeId")]
        public virtual SeatType SeatType { get; set; }
    }

}
