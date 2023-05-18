DROP DATABASE IF EXISTS FinalProject_SOA
GO

CREATE DATABASE FinalProject_SOA
GO
USE FinalProject_SOA
GO

DROP TABLE IF EXISTS Users, ViTriCamTrai, GiaCa, DatCho, HoaDon, LoaiViTri

CREATE TABLE Users
(
  user_id INT IDENTITY(1,1) NOT NULL,
  hoten NVARCHAR(50) DEFAULT N'user',
  email NVARCHAR(30) DEFAULT N'user@test.com',
  sdt VARCHAR(11) NULL,
  phanquyen NVARCHAR(20) DEFAULT N'user',
  diachi NVARCHAR(40) DEFAULT N'TP.HCM',
  password NVARCHAR(25) DEFAULT N'user1234',
  Confirmed BIT DEFAULT 0,
  ConfirmCode NVARCHAR(50) NULL,
  PRIMARY KEY (user_id)
);

CREATE TABLE LoaiViTri
(
  loaivitri_id INT IDENTITY(1,1) NOT NULL,
  tenloaivitri NVARCHAR(50) NULL,
  mota NVARCHAR(max) NULL,
  meta NVARCHAR(max) NULL,
  PRIMARY KEY (loaivitri_id)
);

CREATE TABLE ViTriCamTrai
(
  vitri_id INT IDENTITY(1,1) NOT NULL,
  tenvitri NVARCHAR(50) NULL,
  ngaythem SMALLDATETIME NULL,
  mota NVARCHAR(max) NULL,
  motachitiet NVARCHAR(max) NULL,
  diemdanhgia Decimal(10, 1),
  hinhanh NVARCHAR(max) NULL,
  soluongnguoi int null,
  trangthai NVARCHAR(40) NULL,
  meta NVARCHAR(max) NULL,
  loaivitri_id int null,
  FOREIGN KEY (loaivitri_id) REFERENCES LoaiViTri(loaivitri_id),
  PRIMARY KEY (vitri_id)
);

CREATE TABLE DichVuKemTheo (
  dichvu_id INT IDENTITY(1, 1) NOT NULL,
  ten_dichvu NVARCHAR(50) NULL,
  mo_ta NVARCHAR(MAX) NULL,
  PRIMARY KEY (dichvu_id)
);

CREATE TABLE ViTriDichVu (
  vitri_id INT NOT NULL,
  dichvu_id INT NOT NULL,
  FOREIGN KEY (vitri_id) REFERENCES ViTriCamTrai (vitri_id),
  FOREIGN KEY (dichvu_id) REFERENCES DichVuKemTheo (dichvu_id),
  PRIMARY KEY (vitri_id, dichvu_id)
);

CREATE TABLE GiaCa
(
  gia_id INT IDENTITY(1,1) NOT NULL,
  ngaybatdau SMALLDATETIME NULL,
  ngayketthuc SMALLDATETIME NULL,
  giatien Decimal(19,4) NULL,
  ti_le_thue DECIMAL(10, 2) NULL,
  price_stripe_id NVARCHAR(max) NULL,
  vitri_id INT NULL,
  PRIMARY KEY (gia_id),
  FOREIGN KEY (vitri_id) REFERENCES ViTriCamTrai(vitri_id)
);

CREATE TABLE DatCho
(
  datcho_id INT IDENTITY(1,1) NOT NULL,
  ngaydatcho SMALLDATETIME NULL,
  ngaybatdau SMALLDATETIME NULL,
  ngayketthuc SMALLDATETIME NULL,
  thoigianhuy SMALLDATETIME NULL,
  soluongnguoi INT NULL,
  trangthaidatcho NVARCHAR(30) NULL,
  tongtien Decimal(19,4) NULL,
  maxacnhan NVARCHAR(MAX) NULL,
  user_id INT NULL,
  vitri_id INT NULL,
  PRIMARY KEY (datcho_id),
  FOREIGN KEY (user_id) REFERENCES Users(user_id),
  FOREIGN KEY (vitri_id) REFERENCES ViTriCamTrai(vitri_id)
);

CREATE TABLE HoaDon
(
  hoadon_id INT IDENTITY(1,1) NOT NULL,
  phuongthuc NVARCHAR(30) NULL,
  thoigiantao SMALLDATETIME NULL,
  thoigiancapnhat SMALLDATETIME NULL,
  sotienthanhtoan Decimal(19,4) NULL,
  loaitiente nvarchar(10),
  giaodich_id nvarchar(100) NULL,
  trangthai nvarchar(50),
  datcho_id INT NULL,
  user_id int null,
  PRIMARY KEY (hoadon_id),
  FOREIGN KEY (user_id) REFERENCES Users(user_id),
  FOREIGN KEY (datcho_id) REFERENCES DatCho(datcho_id)
);

INSERT INTO Users VALUES (N'Nguyễn Văn A', N'cowandshark0701@gmail.com', N'0931276768', N'user', N'Quận 7', N'user1234', 'True', NULL)

INSERT INTO LoaiViTri (tenloaivitri, mota, meta) VALUES (N'Vị trí gần hồ', N'Mô tả loại vị trí 1', N'loai-vi-tri-1');
INSERT INTO LoaiViTri (tenloaivitri, mota, meta) VALUES (N'Vị trí gần rừng cây', N'Mô tả loại vị trí 2', N'loai-vi-tri-2');
INSERT INTO LoaiViTri (tenloaivitri, mota, meta) VALUES (N'Ngắm hoàng hôn', N'Mô tả loại vị trí 1', N'loai-vi-tri-3');
INSERT INTO LoaiViTri (tenloaivitri, mota, meta) VALUES (N'Ngắm bình minh', N'Mô tả loại vị trí 2', N'loai-vi-tri-4');

INSERT INTO DichVuKemTheo Values (N'Có ghế sẵn', N'Dịch vụ có ghế sẵn');
INSERT INTO DichVuKemTheo Values (N'Có bàn sẵn', N'Dịch vụ 2');
INSERT INTO DichVuKemTheo Values (N'Than bếp sẵn', N'Dịch vụ 3');
INSERT INTO DichVuKemTheo Values (N'Có súp chèo', N'Dịch vụ 4');
INSERT INTO DichVuKemTheo Values (N'Tắm miễn phí', N'Dịch vụ 5');


INSERT INTO ViTriCamTrai (tenvitri, ngaythem, mota, motachitiet, diemdanhgia, hinhanh, soluongnguoi,trangthai, meta, loaivitri_id) VALUES (N'Bãi trống gần hồ 1', '2023-04-21', N'Bãi trống có súp chèo gần hồ', N'<h2>Khu cắm trại</h2>
            <p>
                Khu cắm trại là nơi tuyệt vời để trải nghiệm thiên nhiên và tận hưởng không khí trong lành. Tại đây, bạn có thể cắm trại và nghỉ ngơi trên đất hoặc trên giường nằm. Khu cắm trại của chúng tôi có đầy đủ các tiện nghi cần thiết để bạn có một kỳ nghỉ tuyệt vời như nhà vệ sinh, vòi hoa sen, bếp nướng, đồ nấu ăn, bàn ghế ngoài trời, lều trại và bình gas.
            </p>
            <p>
                Nếu bạn muốn trải nghiệm một cuộc sống cắm trại thực sự, bạn có thể tự mang theo lều trại và túi ngủ của riêng mình hoặc thuê từ chúng tôi.
            </p>
            <p>
                Đêm tối, bạn có thể ngắm sao và nghe tiếng động của động vật hoang dã trong rừng. Vào buổi sáng, bạn có thể tự nấu ăn bữa sáng và thưởng thức cà phê tươi để bắt đầu một ngày mới đầy năng lượng.
            </p>
            <p>
                Hãy đến với khu cắm trại của chúng tôi để tận hưởng kỳ nghỉ trong lành và trở thành một phần của thiên nhiên.
            </p>', 4.5,'package-1.jpg', 3,N'Có sẵn', N'vi-tri-1',1);
INSERT INTO ViTriCamTrai (tenvitri, ngaythem, mota, motachitiet, diemdanhgia, hinhanh, soluongnguoi,trangthai, meta, loaivitri_id) VALUES (N'Bãi trống gần hồ 2', '2023-04-21', N'Bãi gần hồ thoáng mát, có thể câu cá', N'<h2>Khu cắm trại</h2>
            <p>
                Khu cắm trại là nơi tuyệt vời để trải nghiệm thiên nhiên và tận hưởng không khí trong lành. Tại đây, bạn có thể cắm trại và nghỉ ngơi trên đất hoặc trên giường nằm. Khu cắm trại của chúng tôi có đầy đủ các tiện nghi cần thiết để bạn có một kỳ nghỉ tuyệt vời như nhà vệ sinh, vòi hoa sen, bếp nướng, đồ nấu ăn, bàn ghế ngoài trời, lều trại và bình gas.
            </p>
            <p>
                Nếu bạn muốn trải nghiệm một cuộc sống cắm trại thực sự, bạn có thể tự mang theo lều trại và túi ngủ của riêng mình hoặc thuê từ chúng tôi.
            </p>
            <p>
                Đêm tối, bạn có thể ngắm sao và nghe tiếng động của động vật hoang dã trong rừng. Vào buổi sáng, bạn có thể tự nấu ăn bữa sáng và thưởng thức cà phê tươi để bắt đầu một ngày mới đầy năng lượng.
            </p>
            <p>
                Hãy đến với khu cắm trại của chúng tôi để tận hưởng kỳ nghỉ trong lành và trở thành một phần của thiên nhiên.
            </p>', 4.1,'package-1.jpg', 4,N'Có sẵn', N'vi-tri-2',1);
INSERT INTO ViTriCamTrai (tenvitri, ngaythem, mota, motachitiet, diemdanhgia, hinhanh, soluongnguoi,trangthai, meta, loaivitri_id) VALUES (N'Bãi cắm ở rừng 1', '2023-04-21', N'Cho những người thích cây cối', N'<h2>Khu cắm trại</h2>
            <p>
                Khu cắm trại là nơi tuyệt vời để trải nghiệm thiên nhiên và tận hưởng không khí trong lành. Tại đây, bạn có thể cắm trại và nghỉ ngơi trên đất hoặc trên giường nằm. Khu cắm trại của chúng tôi có đầy đủ các tiện nghi cần thiết để bạn có một kỳ nghỉ tuyệt vời như nhà vệ sinh, vòi hoa sen, bếp nướng, đồ nấu ăn, bàn ghế ngoài trời, lều trại và bình gas.
            </p>
            <p>
                Nếu bạn muốn trải nghiệm một cuộc sống cắm trại thực sự, bạn có thể tự mang theo lều trại và túi ngủ của riêng mình hoặc thuê từ chúng tôi.
            </p>
            <p>
                Đêm tối, bạn có thể ngắm sao và nghe tiếng động của động vật hoang dã trong rừng. Vào buổi sáng, bạn có thể tự nấu ăn bữa sáng và thưởng thức cà phê tươi để bắt đầu một ngày mới đầy năng lượng.
            </p>
            <p>
                Hãy đến với khu cắm trại của chúng tôi để tận hưởng kỳ nghỉ trong lành và trở thành một phần của thiên nhiên.
            </p>', 4.1,'Rung-cay-se-9244-1656845719.jpg', 4,N'Có sẵn', N'vi-tri-3',2);
INSERT INTO ViTriCamTrai (tenvitri, ngaythem, mota, motachitiet, diemdanhgia, hinhanh, soluongnguoi,trangthai, meta, loaivitri_id) VALUES (N'Bãi cắm ở rừng 2', '2023-04-21', N'Mát mẻ, thoáng mát', N'<h2>Khu cắm trại</h2>
            <p>
                Khu cắm trại là nơi tuyệt vời để trải nghiệm thiên nhiên và tận hưởng không khí trong lành. Tại đây, bạn có thể cắm trại và nghỉ ngơi trên đất hoặc trên giường nằm. Khu cắm trại của chúng tôi có đầy đủ các tiện nghi cần thiết để bạn có một kỳ nghỉ tuyệt vời như nhà vệ sinh, vòi hoa sen, bếp nướng, đồ nấu ăn, bàn ghế ngoài trời, lều trại và bình gas.
            </p>
            <p>
                Nếu bạn muốn trải nghiệm một cuộc sống cắm trại thực sự, bạn có thể tự mang theo lều trại và túi ngủ của riêng mình hoặc thuê từ chúng tôi.
            </p>
            <p>
                Đêm tối, bạn có thể ngắm sao và nghe tiếng động của động vật hoang dã trong rừng. Vào buổi sáng, bạn có thể tự nấu ăn bữa sáng và thưởng thức cà phê tươi để bắt đầu một ngày mới đầy năng lượng.
            </p>
            <p>
                Hãy đến với khu cắm trại của chúng tôi để tận hưởng kỳ nghỉ trong lành và trở thành một phần của thiên nhiên.
            </p>', 4.1,'Rung-cay-se-9244-1656845719.jpg', 4,N'Có sẵn', N'vi-tri-4',2);

INSERT INTO GiaCa (ngaybatdau, ngayketthuc, giatien, vitri_id, price_stripe_id, ti_le_thue) VALUES ('2023-04-21', '2023-04-30', 100000, 1, N'price_1N02kCGU9Onc63X1avO1RvtP', 0.1);
INSERT INTO GiaCa (ngaybatdau, ngayketthuc, giatien, vitri_id, price_stripe_id, ti_le_thue) VALUES ('2023-04-21', '2023-04-30', 120000, 2, N'price_1N02n7GU9Onc63X1vlnXlsiZ', 0,1);
INSERT INTO GiaCa (ngaybatdau, ngayketthuc, giatien, vitri_id, price_stripe_id, ti_le_thue) VALUES ('2023-04-21', '2023-04-30', 110000, 3, N'price_1N02pFGU9Onc63X1li6689ib', 0,1);
INSERT INTO GiaCa (ngaybatdau, ngayketthuc, giatien, vitri_id, price_stripe_id, ti_le_thue) VALUES ('2023-04-21', '2023-04-30', 130000, 4, N'price_1N02rAGU9Onc63X1NFeD2q6m', 0,1);

INSERT INTO ViTriDichVu Values (1, 1);
INSERT INTO ViTriDichVu Values (1, 2);
INSERT INTO ViTriDichVu Values (1, 3);
INSERT INTO ViTriDichVu Values (2, 1);
INSERT INTO ViTriDichVu Values (2, 5);
INSERT INTO ViTriDichVu Values (2, 4);