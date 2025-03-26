using Booking_Movie_Tickets.Configs;
using Booking_Movie_Tickets.DTOs.Payment.VNPAY;
using Booking_Movie_Tickets.Helper;
using Booking_Movie_Tickets.Helper.ZaloPay;
using Booking_Movie_Tickets.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IConfiguration _configuration;
        public PaymentController(IVnPayService vpnPayService,IConfiguration configuration)
        {
            _vnPayService = vpnPayService;
            _configuration = configuration;
        }

        [HttpPost("url")]
        public IActionResult CreatePaymentURL([FromBody] VnPaymentRequestModel model)
        {
            try
            {
                var paymentUrl = _vnPayService.CreatePaymentURL(HttpContext, model);
                return Ok(new { url = paymentUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo URL thanh toán", detail = ex.Message });
            }
        }

        [HttpGet("payment-execute")]
        public IActionResult PaymentExecute()
        {
            try
            {
                var response = _vnPayService.PaymentExcute(Request.Query);
                return Ok(new
                {
                    success = response.Success,
                    message = response.Message,
                    data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi xử lý thanh toán", detail = ex.Message });
            }
        }


    }
}
