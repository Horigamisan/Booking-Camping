using FinalProject_API.Libs;
using FinalProject_API.Models;
using FinalProject_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace FinalProject_API.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly FinalProject_SOAContext _context;

        public BookingController(IBookingService bookingService, FinalProject_SOAContext context)
        {
            _context = context;
            _bookingService = bookingService;
        }

        [HttpGet("get-all-position")]
        public IActionResult GetAllPosition()
        {
            var positions = _bookingService.GetAllPostion();

            var result = positions.Select(p => new
            {
                p.VitriId,
                p.Tenvitri,
                p.Ngaythem,
                p.Mota,
                p.Motachitiet,
                p.Hinhanh,
                p.Diemdanhgia,
                p.Soluongnguoi,
                p.Trangthai,
                giatien = p.GiaCas.FirstOrDefault()?.Giatien
            });

            return Ok(result);
        }

        [HttpGet("get-position-by-type/{typeId}")]
        public IActionResult GetPositionByType(int typeId)
        {
            var type = _context.LoaiViTris.Find(typeId);

            if (type == null)
            {
                return NotFound(new
                {
                    code = StatusCodes.Status404NotFound,
                    message = "Không tìm thấy loại sản phẩm"
                });
            }    

            var positions = _bookingService.GetPositionsByType(typeId);

            var today = DateTime.Now.DayOfWeek;

            var result = positions.Select(p => new
            {
                p.VitriId,
                p.Tenvitri,
                p.Ngaythem,
                p.Mota,
                p.Motachitiet,
                p.Hinhanh,
                p.Diemdanhgia,
                p.Soluongnguoi,
                p.Trangthai,
                giatien = p.GiaCas.FirstOrDefault()?.Giatien
            });

            return Ok(result);
        }


        [HttpGet("get-detail-position/{id}")]
        public IActionResult GetDetailPosition(int id)
        {
            var position = _bookingService.GetDetailPosition(id);
            
            if(position == null)
            {
                return NotFound(new
                {
                    code = StatusCodes.Status404NotFound,
                    message = "Không tìm thấy vị trí cắm trại"
                });
            }

            var result = new
            {
                position.VitriId,
                position.Tenvitri,
                position.Ngaythem,
                position.Mota,
                position.Motachitiet,
                position.Hinhanh,
                position.Diemdanhgia,
                position.Soluongnguoi,
                position.Meta,
                position.Trangthai,
                position.Loaivitri,
                position.Dichvus,
                thue = position.GiaCas.FirstOrDefault()?.TiLeThue,
                giatrongtuan = position.GiaCas.Select(g => new
                {
                    g.Giatien
                }),
                giatien = position.GiaCas.FirstOrDefault()?.Giatien
            };

            return Ok(result);
        }

        [HttpGet("available-positions")]
        public IActionResult GetAvailablePositions([FromQuery] string checkin, [FromQuery] string checkout)
        {
            if (!DateTime.TryParse(checkin, out DateTime checkinDate) || !DateTime.TryParse(checkout, out DateTime checkoutDate))
            {
                return BadRequest("Invalid date format. Please use yyyy-MM-dd.");
            }
            
            var positions = _bookingService.GetAvailablePositions(checkinDate, checkoutDate);

            if (positions == null)
            {
                return NotFound(new
                {
                    code = StatusCodes.Status404NotFound,
                    message = "Không tìm thấy các vị trí cắm trại"
                });
            }

            var result = positions.Select(p => new
            {
                p.VitriId,
                p.Tenvitri,
                p.Ngaythem,
                p.Mota,
                p.Motachitiet,
                p.Hinhanh,
                p.Diemdanhgia,
                p.Soluongnguoi,
                p.Trangthai,
                thue = p.GiaCas.FirstOrDefault()?.TiLeThue,
                giatien = p.GiaCas.FirstOrDefault()?.Giatien
            });

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-booking-by-user")]
        public IActionResult GetBookingByUser()
        {
            var userId = User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var bookings = _bookingService.GetBookingByUser(Int32.Parse(userId));

            if (bookings == null)
            {
                return NotFound(new
                {
                    code = StatusCodes.Status404NotFound,
                    message = "Không tìm thấy các đặt chỗ"
                });
            }

            var result = bookings.Select(b => new
            {
                datchoId = b.DatchoId,
                ngaybatdau = b.Ngaybatdau,
                ngayketthuc = b.Ngayketthuc,
                ngaydatcho = b.Ngaydatcho,
                trangthaidatcho = b.Trangthaidatcho,
                soluongnguoi = b.Soluongnguoi,
                vitri = b.Vitri.Tenvitri,
                tongtien = b.Tongtien,
                loaivitri = b.Vitri.Loaivitri.Tenloaivitri,
                hinhanh = b.Vitri.Hinhanh,
                maxacnhan = b.Maxacnhan,
            });

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-all-booking")]
        public IActionResult GetAllBookings()
        {
            var bookings = _bookingService.GetAllBooking();

            return Ok(bookings);
        }

        [Authorize]
        [HttpGet("get-next-booking-code")]
        public IActionResult GetNextBookingCode()
        {
            var nextBookingCode = _bookingService.GetNextBookingCode();

            return Ok(nextBookingCode);
        }


        [Authorize]
        [HttpPost("booking-camp")]
        public IActionResult BookingCamp([FromBody] DatCho datCho)
        {
            //get user by claim
            var userId = User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value;
            datCho.UserId = Int32.Parse(userId);

            // Kiểm tra ngày checkin và checkout
            if (datCho.Ngayketthuc <= datCho.Ngaybatdau)
            {
                return BadRequest("Ngày checkin hoặc checkout không hợp lệ.");
            }

            //Kiểm tra vị trí cắm trại có sẵn hoặc không
            var vitri = _context.ViTriCamTrais.Find(datCho.VitriId);

            if(vitri == null)
            {
                return BadRequest("Vị trí cắm trại không tồn tại.");

            }

            if (vitri.Trangthai == "Không có sẵn")
            {
                return BadRequest("Vị trí cắm trại hiện không có.");
            }

            if (datCho.Soluongnguoi > vitri.Soluongnguoi)
            {
                return BadRequest("Số lượng người tối đa của vị trí cắm trại này là " + vitri.Soluongnguoi);
            }

            // Kiểm tra vị trí cắm trại có sẵn trong khoảng thời gian
            var availablePositions = _bookingService.GetAvailablePositions((DateTime)datCho.Ngaybatdau, (DateTime)datCho.Ngayketthuc);
            if (!availablePositions.Any(p => p.VitriId == datCho.VitriId))
            {
                return BadRequest("Vị trí cắm trại không còn trống trong khoảng thời gian này.");
            }

          
            var result = _bookingService.BookingCamp(datCho);
            result.Maxacnhan = Guid.NewGuid().ToString();

            //xác nhận link confirm đặt chổ qua email

            var user = _context.Users.Find(datCho.UserId);

            var position = _context.ViTriCamTrais.Find(datCho.VitriId);

            var callbackUrl = Url.Action("CheckOut", "Booking", new { datchoId = result.DatchoId, maxacnhan = result.Maxacnhan}, protocol: HttpContext.Request.Scheme);

            Mailer.SendEmail(user.Email, "Giữ chỗ thành công, vui lòng xác nhận thanh toán",
                $"<p>Xin chào {user.Hoten},</p>" +
                $"<p>Vui lòng xác nhận thanh toán của bạn tại <a href={callbackUrl}>đây</a>.</p>" +
                $"<p>Thời gian hết hạn thanh toán đặt chỗ: {result.Thoigianhuy}</p>" +
                $"<p>Các thông tin chi tiết về đặt chỗ của bạn như sau:</p>" +
                $"<ul>" +
                $"<li>Mã đặt chỗ: {result.DatchoId}</li>" +
                $"<li>Tên người đặt: {user.Hoten}</li>" +
                $"<li>Tên vị trí: {position.Tenvitri}</li>" +
                $"<li>Ngày bắt đầu: {result.Ngaybatdau}</li>" +
                $"<li>Ngày kết thúc: {result.Ngayketthuc}</li>" +
                $"<li>Số lượng người: {result.Soluongnguoi}</li>" +
                $"<li>Số tiền cần thanh toán: {result.Tongtien} VNĐ</li>" +
                $"</ul>"+
                $"<p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>"
            );

            var today = DateTime.Now.DayOfWeek;


            return Ok(new
            {
                result.DatchoId,
                result.VitriId,
                result.UserId,
                result.Ngaydatcho,
                result.Ngaybatdau,
                result.Ngayketthuc,
                result.Soluongnguoi,
                result.Trangthaidatcho,
                result.Tongtien,
                result.Thoigianhuy,
                result.Maxacnhan,
                vitri = new
                {
                    result.Vitri.VitriId,
                    result.Vitri.Tenvitri,
                    result.Vitri.Ngaythem,
                    result.Vitri.Mota,
                    result.Vitri.Motachitiet,
                    result.Vitri.Hinhanh,
                    result.Vitri.Diemdanhgia,
                    result.Vitri.Soluongnguoi,
                    result.Vitri.Trangthai,
                    giatien = result.Vitri.GiaCas.FirstOrDefault()?.Giatien
                },
            });
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody] SearchModel searchModel)
        {
            
            var positions = _bookingService.Search(searchModel);

            var today = DateTime.Now.DayOfWeek;

            var result = positions.Select(p => new
            {
                p.VitriId,
                p.Tenvitri,
                p.Ngaythem,
                p.Mota,
                p.Motachitiet,
                p.Hinhanh,
                p.Diemdanhgia,
                p.Meta,
                p.Soluongnguoi,
                p.Trangthai,
                giatien = p.GiaCas.FirstOrDefault()?.Giatien
            });

            return Ok(result);
        }

        [HttpGet("get-all-type")]
        public IActionResult GetAllType()
        {
            var types = _bookingService.GetAllTypePosition();

            return Ok(types);
        }

        [HttpGet("get-all-services")]
        public IActionResult GetAllServices()
        {
            var services = _bookingService.GetAllServices();

            return Ok(services);
        }


        [HttpPost("filter-search")]
        public IActionResult FilterSearch([FromBody] FilterSearchModel filterSearchModel)
        {
            var positions = _bookingService.FilterSearch(filterSearchModel);

            var today = DateTime.Now.DayOfWeek;

            var result = positions.Select(p => new
            {
                p.VitriId,
                p.Tenvitri,
                p.Ngaythem,
                p.Mota,
                p.Motachitiet,
                p.Hinhanh,
                p.Diemdanhgia,
                p.Soluongnguoi,
                p.Meta,
                p.Trangthai,
                giatien = p.GiaCas.FirstOrDefault()?.Giatien
            });

            return Ok(result);
        }

        [HttpGet("check-out/{datchoId}/{maxacnhan}")]
        public IActionResult CheckOut(int datchoId, string maxacnhan)
        {
            var result = _context.DatChos.FirstOrDefault(d => d.DatchoId == datchoId);
            if (result.Maxacnhan != maxacnhan)
            {
                return BadRequest("Mã xác nhận không đúng");
            }

            if (result == null)
            {
                return BadRequest("Không tìm thấy đặt chỗ");
            }

            if (result.Trangthaidatcho == "Đã thanh toán")
            {
                return BadRequest("Đặt chỗ đã được thanh toán");
            }

            if (result.Trangthaidatcho == "Đã huỷ")
            {
                return BadRequest("Đặt chỗ bị huỷ không thể thanh toán");
            }

            var songaythue = ((TimeSpan)(result.Ngayketthuc - result.Ngaybatdau)).Days;

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                    Price = result.Vitri.GiaCas.FirstOrDefault()?.PriceStripeId,
                    Quantity = songaythue,
                    TaxRates = new List<string>
                    {
                        "txr_1N8q9gGU9Onc63X1To8kz7Ij" // ID của thuế trong Stripe
                    }
                  },
                },
                Mode = "payment",
                SuccessUrl = Url.Action("CheckOutSuccess", "Booking", new { datchoId = datchoId, maxacnhan = result.Maxacnhan }, Request.Scheme),
                CancelUrl = Url.Action("CheckOutCancel", "Booking", new { datchoId = datchoId, maxacnhan = result.Maxacnhan }, Request.Scheme),
            };
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [HttpGet("check-out-success/{datchoId}/{maxacnhan}")]
        public IActionResult CheckOutSuccess(int datchoId, string maxacnhan)
        {
            var datCho = _context.DatChos.Find(datchoId);
            if (maxacnhan != datCho.Maxacnhan)
            {
                return BadRequest("Mã xác nhận không đúng");
            }

            if (datCho.Trangthaidatcho == "Đã thanh toán")
            {
                return BadRequest("Đặt chỗ đã được thanh toán");
            }

            if (datCho.Trangthaidatcho == "Đã huỷ")
            {
                return BadRequest("Đặt chỗ bị huỷ không thể thanh toán");
            }

            datCho.Trangthaidatcho = "Đã thanh toán";

            var hoadon = new HoaDon
            {
                DatchoId = datchoId,
                Thoigiantao = DateTime.Now,
                Thoigiancapnhat = DateTime.Now,
                Phuongthuc = "Thanh toán online",
                Loaitiente = "VND",
                Sotienthanhtoan = datCho.Tongtien,
                Trangthai = "Đã thanh toán",
                UserId = datCho.UserId,
                Datcho = datCho,
                User = datCho.User
            };
            _context.HoaDons.Add(hoadon);
            _context.SaveChanges();

            Mailer.SendEmail(datCho.User.Email, "Hoá đơn đặt chỗ thành công",
                    $"<p>Xin chào {datCho.User.Hoten},</p>" +
                    $"<p>Bạn đã thanh toán hoá đơn đặt chỗ với mã số {datCho.DatchoId} thành công. Dưới đây là thông tin hoá đơn của bạn:</p>" +
                    $"<ul>" +
                    $"<li>Mã hoá đơn: {hoadon.HoadonId}</li>" +
                    $"<li>Mã đặt chỗ: {datCho.DatchoId}</li>" +
                    $"<li>Tên người đặt: {datCho.User.Hoten}</li>" +
                    $"<li>Thời gian tạo: {hoadon.Thoigiantao}</li>" +
                    $"<li>Tên vị trí: {datCho.Vitri.Tenvitri}</li>" +
                    $"<li>Ngày bắt đầu: {datCho.Ngaybatdau}</li>" +
                    $"<li>Ngày kết thúc: {datCho.Ngayketthuc}</li>" +
                    $"<li>Số lượng người: {datCho.Soluongnguoi}</li>" +
                    $"<li>Phương thức thanh toán: {hoadon.Phuongthuc}</li>" +
                    $"<li>Số tiền cần thanh toán: {datCho.Tongtien} VNĐ</li>" +
                    $"</ul>" +
                    $"<p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi</p>" +
                    $"<br/>Chúc bạn một ngày tốt lành!</p>");
            //redirect to url 
            return Redirect("https://localhost:7268/my-booking");
        }
    }
}
