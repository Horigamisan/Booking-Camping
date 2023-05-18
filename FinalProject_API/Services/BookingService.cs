using System;
using System.Collections.Generic;
using FinalProject_API.Libs;
using FinalProject_API.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_API.Services
{
    public interface IBookingService
    {
        public List<ViTriCamTrai> GetAllPostion();
        public ViTriCamTrai GetDetailPosition(int id);
        public List<ViTriCamTrai> GetPositionsByType(int typeId);
        public List<ViTriCamTrai> GetAvailablePositions(DateTime checkin, DateTime checkout);
        public List<LoaiViTri> GetAllTypePosition();
        public DatCho BookingCamp(DatCho booking);
        public List<DatCho> GetExpiredBookings();
        public Task CancelBooking(int bookingId);
        public List<ViTriCamTrai> FilterSearch(FilterSearchModel filterSearchModel);
        public List<ViTriCamTrai> Search(SearchModel searchModel);

        public List<DatCho> GetBookingByUser(int userId);

        public List<DichVuKemTheo> GetAllServices();
        public List<DatCho> GetAllBooking();

        public int GetNextBookingCode();


    }
    public class BookingService : IBookingService
    {
        private readonly FinalProject_SOAContext _context;
        public BookingService(FinalProject_SOAContext context)
        {
            _context = context;
        }
        public List<ViTriCamTrai> GetAllPostion()
        {
            return _context.ViTriCamTrais.ToList();
        }

        public ViTriCamTrai GetDetailPosition(int id)
        {
            return _context.ViTriCamTrais.Find(id);
        }

        public List<ViTriCamTrai> GetAvailablePositions(DateTime checkin, DateTime checkout)
        {
            var positions = _context.ViTriCamTrais.Include(p => p.GiaCas);
            var bookings = _context.DatChos
                .Where(b => (b.Trangthaidatcho == "Đã đặt" || b.Trangthaidatcho == "Đã thanh toán") &&
                    (b.Ngaybatdau <= checkin && b.Ngayketthuc > checkin ||
                     b.Ngaybatdau < checkout && b.Ngayketthuc >= checkout ||
                     b.Ngaybatdau >= checkin && b.Ngayketthuc <= checkout))
                .Select(b => b.VitriId).Distinct().ToList();

            var result = positions.Where(p => !bookings.Contains(p.VitriId)).Select(p => new ViTriCamTrai
            {
                VitriId = p.VitriId,
                Tenvitri = p.Tenvitri,
                LoaivitriId = p.LoaivitriId,
                Ngaythem = p.Ngaythem,
                Mota = p.Mota,
                Motachitiet = p.Motachitiet,
                Diemdanhgia = p.Diemdanhgia,
                Hinhanh = p.Hinhanh,
                Trangthai = p.Trangthai,
                Soluongnguoi = p.Soluongnguoi,
                Meta = p.Meta,
                Dichvus = p.Dichvus,
                GiaCas = p.GiaCas
                    .Select(g => new GiaCa { Giatien = g.Giatien }).ToList()
            }).ToList();

            return result;
        }

        public DatCho BookingCamp(DatCho booking)
        {
            booking.Ngaydatcho = DateTime.Now;
            //format dd-MM-yy HH:mm:ss
            //Thời gian huỷ của đơn hàng sẽ là 2 phút sau khi đặt
            booking.Thoigianhuy = DateTime.Now.AddMinutes(2);
            booking.Trangthaidatcho = "Đã đặt";

            var today = DateTime.Now.DayOfWeek;
            var datetostay = ((TimeSpan)(booking.Ngayketthuc - booking.Ngaybatdau)).Days;
            var vitri = _context.ViTriCamTrais.Find(booking.VitriId);

            booking.Tongtien = vitri.GiaCas.FirstOrDefault()?.Giatien * datetostay + vitri.GiaCas.FirstOrDefault()?.TiLeThue * vitri.GiaCas.FirstOrDefault()?.Giatien * datetostay;

            _context.DatChos.Add(booking);
            _context.SaveChanges();

            return booking;
        }

        public List<DatCho> GetExpiredBookings()
        {
            return _context.DatChos.Where(b => b.Trangthaidatcho == "Đã đặt" && b.Thoigianhuy < DateTime.Now).ToList();
        }

        public async Task CancelBooking(int bookingId)
        {
            var booking = _context.DatChos.Find(bookingId);
            booking.Trangthaidatcho = "Đã huỷ";

            await _context.SaveChangesAsync();
        }

        public List<ViTriCamTrai> GetPositionsByType(int typeId)
        {
            return _context.ViTriCamTrais.Where(p => p.LoaivitriId == typeId).ToList();
        }

        public List<ViTriCamTrai> Search(SearchModel searchModel)
        {
            var positions = GetAvailablePositions(searchModel.Ngaybatdau.Value, searchModel.Ngayketthuc.Value);


            if (searchModel.Soluongnguoi != null)
            {
                positions = positions.Where(p => p.Soluongnguoi >= searchModel.Soluongnguoi).ToList();
            }

            return positions.ToList();
        }

        public List<ViTriCamTrai> FilterSearch(FilterSearchModel filterSearchModel)
        {
            var positions = GetAllPostion();

            if (filterSearchModel.Ngayketthuc != null || filterSearchModel.Ngaybatdau != null)
            {
                positions = GetAvailablePositions(filterSearchModel.Ngaybatdau.Value, filterSearchModel.Ngayketthuc.Value);
            }

            if (filterSearchModel.Soluongnguoi != null)
            {
                positions = positions.Where(p => p.Soluongnguoi >= filterSearchModel.Soluongnguoi).ToList();
            }

            if (filterSearchModel.Loaidichvu != null)
            {
                //dich vu from other table and filterSearchModel.Loaidichvu is id of dich vu, filterSearchModel.Loaidichvu is array of id, check if a position has loai dich vu in filterSearchModel.Loaidichvu
                positions = positions.Where(p => filterSearchModel.Loaidichvu.All(id => p.Dichvus.Any(d => d.DichvuId == id))).ToList();

            }

            if (filterSearchModel.Loaivitri != null)
            {
                positions = positions.Where(p => p.LoaivitriId == filterSearchModel.Loaivitri).ToList();
            }

            //filter filterSearchModel.Giatien have 4 option, 1: < 100.000, 2: 100.000 - 200.000, 3: 200.000 - 500.000, 4: > 500.000

            if (filterSearchModel.Giatien != null)
            {
                switch (filterSearchModel.Giatien)
                {
                    case "< 100.000":
                        positions = positions.Where(p => p.GiaCas.FirstOrDefault().Giatien < 100000).ToList();
                        break;
                    case "100.000 - 200.000":
                        positions = positions.Where(p => p.GiaCas.FirstOrDefault().Giatien >= 100000 && p.GiaCas.FirstOrDefault().Giatien < 200000).ToList();
                        break;
                    case "200.000 - 500.000":
                        positions = positions.Where(p => p.GiaCas.FirstOrDefault().Giatien >= 200000 && p.GiaCas.FirstOrDefault().Giatien < 500000).ToList();
                        break;
                    case "> 500.000":
                        positions = positions.Where(p => p.GiaCas.FirstOrDefault().Giatien >= 500000).ToList();
                        break;
                }
            }

            //filter filterSearchModel.Diemdanhgia have 5 option, 1: 1 sao, 2: 2 sao, 3: 3 sao, 4: 4 sao, 5: 5 sao

            if (filterSearchModel.Diemdanhgia != null)
            {
                switch (filterSearchModel.Diemdanhgia)
                {
                    case "1 sao":
                        positions = positions.Where(p => p.Diemdanhgia >= 1 && p.Diemdanhgia < 2).ToList();
                        break;
                    case "2 sao":
                        positions = positions.Where(p => p.Diemdanhgia >= 2 && p.Diemdanhgia < 3).ToList();
                        break;
                    case "3 sao":
                        positions = positions.Where(p => p.Diemdanhgia >= 3 && p.Diemdanhgia < 4).ToList();
                        break;
                    case "4 sao":
                        positions = positions.Where(p => p.Diemdanhgia >= 4 && p.Diemdanhgia < 5).ToList();
                        break;
                    case "5 sao":
                        positions = positions.Where(p => p.Diemdanhgia == 5).ToList();
                        break;
                }
            }

            return positions.ToList();
        }

        public List<LoaiViTri> GetAllTypePosition()
        {
            return _context.LoaiViTris.ToList();
        }

        public List<DatCho> GetBookingByUser(int userId)
        {
            return _context.DatChos.Where(b => b.UserId == userId).ToList();
        }

        public List<DichVuKemTheo> GetAllServices()
        {
            return _context.DichVuKemTheos.ToList();
        }

        public List<DatCho> GetAllBooking()
        {
            return _context.DatChos.ToList();
        }

        public int GetNextBookingCode()
        {
            var lastBooking = _context.DatChos.OrderByDescending(b => b.DatchoId).FirstOrDefault();

            if (lastBooking == null)
            {
                return 1;
            }
            else
            {
                return lastBooking.DatchoId + 1;
            }
        }
    }
}
