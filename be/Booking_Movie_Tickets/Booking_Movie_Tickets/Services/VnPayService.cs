using Booking_Movie_Tickets.DTOs.Payment.VNPAY;
using Booking_Movie_Tickets.Helper;
using Booking_Movie_Tickets.Interfaces;
using static Booking_Movie_Tickets.Helper.VnPayLibrary;

namespace Booking_Movie_Tickets.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;
        public VnPayService(IConfiguration configuration)
        {
            _config = configuration;
        }
        public string CreatePaymentURL(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]); 
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đon hàng:" + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:ReturnURL"]);
            //vnpay.AddRequestData("vnp_IpnUrl", _config["VnPay:IpnURL"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseURL"], _config["VnPay:HashSecret"]);

            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExcute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();

            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            //var vnp_orderId = vnpay.GetResponseData("vnp_TxnRef");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var extractedOrderId = ExtractOrderId(vnp_OrderInfo);
            var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            //var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);

            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false,
                    Message = "Chữ ký không hợp lệ"
                };
            }

            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00") 
            {
                return new VnPaymentResponseModel
                {
                    Success = true,
                    PaymentMethod = "VnPay",
                    OrderDescription = vnp_OrderInfo,
                    OrderId = extractedOrderId,
                    TransactionId = vnp_TransactionId,
                    Token = vnp_SecureHash,
                    VnPayResponseCode = vnp_ResponseCode,
                    Message = "Thanh toán thành công"
                };
            }

            return new VnPaymentResponseModel
            {
                Success = false,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = extractedOrderId,
                TransactionId = vnp_TransactionId,
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,
                Message = GetErrorMessage(vnp_ResponseCode, vnp_TransactionStatus)
            };
        }
        private string ExtractOrderId(string orderInfo)
        {
            if (string.IsNullOrEmpty(orderInfo)) return "";

            var parts = orderInfo.Split(':');
            if (parts.Length > 1)
            {
                return parts[1].Trim(); 
            }

            return "";
        }

        private string GetErrorMessage(string responseCode, string transactionStatus)
        {
            var errorMessages = new Dictionary<string, string>
    {
        { "02", "Mã định danh kết nối không hợp lệ (kiểm tra lại TmnCode)" },
        { "03", "Dữ liệu gửi sang không đúng định dạng" },
        { "91", "Không tìm thấy giao dịch yêu cầu" },
        { "94", "Yêu cầu trùng lặp, duplicate request trong thời gian giới hạn của API" },
        { "97", "Checksum không hợp lệ" },
        { "99", "Lỗi không xác định, vui lòng thử lại" }
    };

            var transactionStatusMessages = new Dictionary<string, string>
    {
        { "01", "Giao dịch chưa hoàn tất" },
        { "02", "Giao dịch bị lỗi" },
        { "04", "Giao dịch đảo (Khách hàng đã bị trừ tiền tại Ngân hàng nhưng GD chưa thành công ở VNPAY)" },
        { "05", "VNPAY đang xử lý giao dịch này (GD hoàn tiền)" },
        { "06", "VNPAY đã gửi yêu cầu hoàn tiền sang Ngân hàng (GD hoàn tiền)" },
        { "07", "Giao dịch bị nghi ngờ gian lận" },
        { "09", "GD Hoàn trả bị từ chối" }
    };

            if (errorMessages.ContainsKey(responseCode))
            {
                return errorMessages[responseCode];
            }

            if (transactionStatusMessages.ContainsKey(transactionStatus))
            {
                return transactionStatusMessages[transactionStatus];
            }

            return "Lỗi không xác định, vui lòng liên hệ hỗ trợ.";
        }

    }
}
