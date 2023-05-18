using FinalProject_API.Libs;
using FinalProject_API.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_API.Services
{
    public class BookingCancellationService : BackgroundService
    {
        private readonly ILogger<BookingCancellationService> _logger;
        private readonly IBookingService _bookingService;
        private readonly FinalProject_SOAContext _context;

        public BookingCancellationService(ILogger<BookingCancellationService> logger, IBookingService bookingService, FinalProject_SOAContext context)
        {
            _logger = logger;
            _bookingService = bookingService;
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Lấy danh sách các đặt chỗ chưa thanh toán hết hạn
                    var expiredBookings = _bookingService.GetExpiredBookings();

                    foreach (var booking in expiredBookings)
                    {
                        // Huỷ đặt chỗ
                        await _bookingService.CancelBooking(booking.DatchoId);

                        // Gửi email thông báo huỷ đặt chỗ
                        var user = _context.Users.Find(booking.UserId);
                        var position = _context.ViTriCamTrais.Find(booking.VitriId);

                        Mailer.SendEmail(user.Email, "Hủy đặt chỗ do bạn chưa thanh toán", $@"
                            <p>Chào {user.Hoten},</p>
                            <p>Đặt chỗ của bạn đã bị huỷ do chưa thanh toán trong thời gian chờ.</p>
                            <p>Thông tin đặt chỗ:</p>
                            <ul>
                                <li>Mã đặt chỗ: {booking.DatchoId}</li>
                                <li>Ngày đặt chỗ: {booking.Ngaydatcho}</li>
                                <li>Ngày đến: {booking.Ngaybatdau}</li>
                                <li>Ngày đi: {booking.Ngayketthuc}</li>
                                <li>Vị trí: {position.Tenvitri}</li>
                                <li>Giá: {booking.Tongtien} VNĐ</li>
                            </ul>
                            <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>");
                    }
                    // Chờ 1 phút trước khi kiểm tra lại
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing unpaid bookings");
                }
            }
        }
    }
}
