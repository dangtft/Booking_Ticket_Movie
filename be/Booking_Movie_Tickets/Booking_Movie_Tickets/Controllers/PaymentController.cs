using Booking_Movie_Tickets.DTOs.Others;
using Booking_Movie_Tickets.DTOs.Payment.VNPAY;
using Booking_Movie_Tickets.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Movie_Tickets.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IOrderService _orderService;
        public PaymentController(IVnPayService vpnPayService, IOrderService orderService)
        {
            _vnPayService = vpnPayService;
            _orderService = orderService;
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
                return StatusCode(500, ApiMessages.EXTERNAL_SERVICE_ERROR);
            }
        }

        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallBack()
        {
            try
            {
                var response = _vnPayService.PaymentExcute(Request.Query);

                if (!Guid.TryParse(response.OrderId, out Guid orderGuid))
                {
                    return BadRequest(ApiMessages.ERROR);
                }

                if (response.Success)
                {
                    await _orderService.UpdateOrderStatus(orderGuid, "Paid");
                    return Ok(new { success = true, message = response.Message });
                }
                else
                {
                   // await _orderService.UpdateOrderStatus(orderGuid, "Pending");
                    await _orderService.DeleteOrderById(orderGuid);

                    return Ok(new { success = false, message = response.Message});
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiMessages.EXTERNAL_SERVICE_ERROR);
            }
        }

        [HttpGet("{orderId}/status")]
        public async Task<IActionResult> GetOrderStatus(Guid orderId)
        {
            try
            {
                var status = await _orderService.GetOrderStatus(orderId);
                if (status == null)
                {
                    return NotFound(ApiMessages.NOT_FOUND);
                }

                return Ok(new { success = true, status });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiMessages.EXTERNAL_SERVICE_ERROR);
            }
        }

    }
}
