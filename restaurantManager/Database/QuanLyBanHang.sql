-- Tạo cơ sở dữ liệu nếu chưa tồn tại
CREATE DATABASE QuanLyNhaHang;
GO

USE QuanLyNhaHang;
GO

-- 1. Bảng NguoiDung (Users)
-- Quản lý thông tin người dùng, bao gồm Admin và Nhân viên.
CREATE TABLE NguoiDung (
    MaNhanVien NVARCHAR(50) NOT NULL PRIMARY KEY, -- Mã nhân viên
    MatKhau NVARCHAR(50) NOT NULL,
    VaiTro INT NOT NULL -- 0 = User, 1 = Admin
);


-- 2. Bảng NhanVien (Employee)
-- Thông tin chi tiết về nhân viên
CREATE TABLE NhanVien (
    MaNhanVien NVARCHAR(50) PRIMARY KEY,
    HoTen NVARCHAR(50) NOT NULL,
    NgaySinh DATE,
    SoDienThoai VARCHAR(15),
    DiaChi NVARCHAR(100),
	Luong DECIMAL(10, 2),
    ChucVu NVARCHAR(30),
    TrangThai NVARCHAR(20),
	FOREIGN KEY (MaNhanVien) REFERENCES NguoiDung(MaNhanVien) ON DELETE CASCADE
);


-- 3. Bảng MonAn (Dishes)
-- Thông tin món ăn, liên kết với danh mục.
CREATE TABLE MonAn (
    MaMonAn INT IDENTITY(1,1) PRIMARY KEY,
    TenMonAn NVARCHAR(255) NOT NULL,
    Gia DECIMAL(10, 2) NOT NULL CHECK (Gia >= 0),
    MoTa NVARCHAR(800),
    HinhAnhURL NVARCHAR(255),
    Loai INT NOT NULL -- 0 món ăn, 1 thức uống
);

-- 4. Bảng KhuyenMai (Promotions)
-- Quản lý chương trình khuyến mãi và mã giảm giá, bổ sung SoLuotSuDungToiDa và SoLuotDaSuDung
CREATE TABLE KhuyenMai (
    MaKhuyenMai INT IDENTITY(1,1) PRIMARY KEY,
    TenChuongTrinh NVARCHAR(255) NOT NULL,
    MaCode NVARCHAR(50) UNIQUE,
    LoaiGiamGia NVARCHAR(50) NOT NULL CHECK (LoaiGiamGia IN ('PhanTram', 'TienCoDinh')),
    GiaTriGiam DECIMAL(10, 2) NOT NULL CHECK (GiaTriGiam >= 0),
    NgayBatDau DATETIME NOT NULL,
    NgayKetThuc DATETIME NOT NULL,
    TrangThai NVARCHAR(50) NOT NULL CHECK (TrangThai IN ('HoatDong', 'HetHan')) DEFAULT 'HoatDong',
    SoLuotSuDungToiDa INT CHECK (SoLuotSuDungToiDa > 0), -- Số lượt sử dụng tối đa cho khuyến mãi
    SoLuotDaSuDung INT NOT NULL DEFAULT 0 CHECK (SoLuotDaSuDung >= 0), -- Số lượt đã sử dụng
    CONSTRAINT CHK_NgayKetThuc CHECK (NgayKetThuc > NgayBatDau),
    CONSTRAINT CHK_SoLuotSuDung CHECK (SoLuotDaSuDung <= SoLuotSuDungToiDa OR SoLuotSuDungToiDa IS NULL)
);

-- 5. Bảng MonAnApDungKM (PromotionApplicableDishes)
-- Liên kết khuyến mãi với các món ăn cụ thể.
CREATE TABLE MonAnApDungKM (
    MaApDung INT IDENTITY(1,1) PRIMARY KEY,
    MaKhuyenMai INT NOT NULL,
    MaMonAn INT NOT NULL,
    FOREIGN KEY (MaKhuyenMai) REFERENCES KhuyenMai(MaKhuyenMai) ON DELETE CASCADE,
    FOREIGN KEY (MaMonAn) REFERENCES MonAn(MaMonAn) ON DELETE CASCADE,
    UNIQUE (MaKhuyenMai, MaMonAn) -- Đảm bảo không áp dụng trùng
);

-- 6. Bảng BanAn
-- Thông tin về bàn
CREATE TABLE BanAn (
    MaBan NVARCHAR(10) PRIMARY KEY,
    SoGhe INT NOT NULL,
    TrangThai NVARCHAR(20)
);

-- 7. Bảng DonHang (Orders)
-- Lưu trữ đơn hàng, bao gồm tổng tiền, giảm giá, và liên kết với nhân viên tạo.
CREATE TABLE DonHang (
    MaDonHang INT IDENTITY(1,1) PRIMARY KEY,
    NgayDat DATETIME NOT NULL DEFAULT GETDATE(),
    TongTienGoc DECIMAL(10, 2) NOT NULL CHECK (TongTienGoc >= 0),
    ThanhToanCuoi DECIMAL(10, 2) NOT NULL CHECK (ThanhToanCuoi >= 0),
    MaNhanVien NVARCHAR(50) NOT NULL,
    TrangThai NVARCHAR(50) NOT NULL CHECK (TrangThai IN ('DangXuLy', 'DaHoanThanh')) DEFAULT 'DangXuLy',
    HinhThucThanhToan NVARCHAR(50) CHECK (HinhThucThanhToan IN ('TienMat', 'ChuyenKhoan')) DEFAULT 'TienMat',
    MaKhuyenMai INT, -- Liên kết khuyến mãi nếu áp dụng cho toàn đơn hàng (tùy chọn)
	MaBan NVARCHAR(10),
	GiaTriKhuyenMai DECIMAL(10,2) NULL, -- % hoặc số tiền đã áp dụng
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien),
    FOREIGN KEY (MaKhuyenMai) REFERENCES KhuyenMai(MaKhuyenMai),
	FOREIGN KEY (MaBan) REFERENCES BanAn(MaBan)
);

-- 8. Bảng ChiTietDonHang (OrderDetails)
-- Chi tiết món ăn trong đơn hàng, lưu giá tại thời điểm để tránh thay đổi.
CREATE TABLE ChiTietDonHang (
    MaDonHang INT NOT NULL,
    MaMonAn INT NOT NULL,
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    GiaTaiThoiDiem DECIMAL(10, 2) NOT NULL CHECK (GiaTaiThoiDiem >= 0),
	SoTienGiam DECIMAL(10, 2) NOT NULL CHECK (SoTienGiam >= 0) DEFAULT 0,
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang) ON DELETE CASCADE,
    FOREIGN KEY (MaMonAn) REFERENCES MonAn(MaMonAn),
	PRIMARY KEY (MaDonHang, MaMonAn)
);

-- Tạo chỉ mục để hỗ trợ tìm kiếm món ăn theo tên, giá, thành phần (MoTa)
CREATE INDEX IDX_MonAn_Ten ON MonAn(TenMonAn);
CREATE INDEX IDX_MonAn_Gia ON MonAn(Gia);
CREATE INDEX IDX_MonAn_MoTa ON MonAn(MoTa);

-- Tạo chỉ mục cho thống kê doanh thu
CREATE INDEX IDX_DonHang_NgayDat ON DonHang(NgayDat);
CREATE INDEX IDX_DonHang_TrangThai ON DonHang(TrangThai);


-- Bảng NguoiDung
INSERT INTO NguoiDung (MaNhanVien, MatKhau, VaiTro) VALUES
('admin', '1', 1), -- Admin
('NV001', '1', 0),-- Nhân viên
('NV002', '1', 0), 
('NV003', '1', 0),
('NV004', '1', 0),
('NV005', '1', 0);


-- Bảng NhanVien
INSERT INTO NhanVien (MaNhanVien, HoTen, NgaySinh, SoDienThoai, DiaChi, Luong, ChucVu, TrangThai) VALUES
('admin', N'Nguyễn Văn Admin', '1985-01-01', '0901234567', N'123 Đường Láng, Hà Nội', 15000000, N'Quản trị viên', N'Đang làm việc'),
('NV002', N'Trần Thị Bình', '1990-05-10', '0912345678', N'456 Lê Lợi, TP.HCM', 8000000, N'Nhân viên phục vụ', N'Đang làm việc'),
('NV003', N'Lê Văn Cường', '1995-07-15', '0923456789', N'789 Nguyễn Huệ, Đà Nẵng', 8500000, N'Nhân viên bếp', N'Đang làm việc'),
('NV004', N'Phạm Thị Dung', '1993-03-20', '0934567890', N'101 Trần Phú, Nha Trang', 8200000, N'Nhân viên phục vụ', N'Đang làm việc'),
('NV005', N'Hoàng Văn Em', '1997-11-30', '0945678901', N'202 Phạm Văn Đồng, Hà Nội', 8300000, N'Nhân viên bếp', N'Đang làm việc'),
('NV001', N'Ngô Thị Fương', '1992-09-25', '0956789012', N'303 Nguyễn Văn Cừ, TP.HCM', 8100000, N'Nhân viên thu ngân', N'Đang làm việc');


-- Bảng MonAn
INSERT INTO MonAn (TenMonAn, Gia, MoTa, HinhAnhURL, Loai) VALUES
-- Thức ăn 0
(N'Phở bò', 50000, N'Phở bò truyền thống với nước dùng thơm ngon', 'Resources/Image/dish/pho_bo.jpg', 0),
(N'Cơm tấm sườn nướng', 45000, N'Cơm tấm với sườn nướng mật ong', 'Resources/Image/dish/com_tam.jpg', 0),
(N'Bún chả', 40000, N'Bún chả Hà Nội với chả nướng thơm lừng', 'Resources/Image/dish/bun_cha.jpg', 0),
(N'Mì quảng', 42000, N'Mì quảng đặc sản miền Trung', 'Resources/Image/dish/mi_quang.jpg', 0),
(N'Gỏi cuốn', 30000, N'Gỏi cuốn tôm thịt tươi ngon', 'Resources/Image/dish/goi_cuon.jpg', 0),
(N'Bánh mì pate', 25000, N'Bánh mì pate truyền thống', 'Resources/Image/dish/banh_mi.jpg', 0),
(N'Cá kho tộ', 60000, N'Cá kho tộ đậm đà hương vị', 'Resources/Image/dish/ca_kho.jpg', 0),
(N'Lẩu thái', 150000, N'Lẩu thái chua cay cho 3-4 người', 'Resources/Image/dish/lau_thai.jpg', 0),
(N'Gà nướng muối ớt', 120000, N'Gà nướng muối ớt nguyên con', 'Resources/Image/dish/ga_nuong.jpg', 0),
(N'Bò kho', 55000, N'Bò kho ăn kèm bánh mì', 'Resources/Image/dish/bo_kho.jpg', 0),
-- Thức uống 1
-- Nước ngọt
(N'Nước cam', 25000, N'Nước cam ép tươi', 'Resources/Image/beverage/nuoc_cam.jpg', 1),
(N'Trà sữa trân châu', 35000, N'Trà sữa trân châu nhà làm', 'Resources/Image/beverage/tra_sua.jpg', 1),
(N'Cà phê sữa đá', 20000, N'Cà phê sữa đá đậm đà', 'Resources/Image/beverage/ca_phe.jpg', 1),
(N'Sinh tố xoài', 30000, N'Sinh tố xoài thơm ngon', 'Resources/Image/beverage/sinh_to_xoai.jpg', 1),
(N'Nước lọc', 10000, N'Nước lọc đóng chai', 'Resources/Image/beverage/nuoc_loc.jpg', 1),
-- Rượu
(N'Rượu vang đỏ Cabernet Sauvignon', 350000.00, N'Rượu vang đỏ từ Chile, hương vị đậm đà với mùi trái cây chín và gỗ sồi.', 'Resources/Image/beverage/vang_do.jpg', 1),
(N'Rượu Sake Gekkeikan', 280000.00, N'Rượu sake Nhật Bản truyền thống, nhẹ nhàng, phù hợp với các món sashimi.', 'Resources/Image/beverage/sake.jpg', 1),
(N'Whisky Chivas Regal 18', 450000.00, N'Whisky Scotland cao cấp, hương vị mượt mà với chút khói và trái cây khô.', 'Resources/Image/beverage/chivas_18.jpg', 1),
-- Bia
(N'Bia Heineken', 35000.00, N'Bia Hà Lan nổi tiếng, hương vị tươi mát, nhẹ nhàng, phù hợp mọi bữa tiệc.', 'Resources/Image/beverage/heineken.jpg', 1),
(N'Bia Tiger', 30000.00, N'Bia Việt Nam phổ biến, vị đậm đà, thích hợp khi dùng với món ăn Việt.', 'Resources/Image/beverage/tiger.jpg', 1),
(N'Bia thủ công IPA', 80000.00, N'Bia thủ công địa phương với hương hoa bia và vị đắng nhẹ, độc đáo.', 'Resources/Image/beverage/craft_ipa.jpg', 1);


-- Bảng KhuyenMai
INSERT INTO KhuyenMai (TenChuongTrinh, MaCode, LoaiGiamGia, GiaTriGiam, NgayBatDau, NgayKetThuc, TrangThai, SoLuotSuDungToiDa, SoLuotDaSuDung) VALUES
(N'Giảm giá món ăn', 'FOOD10', 'PhanTram', 10, '2025-09-01 00:00:00', '2025-09-30 23:59:59', 'HoatDong', 50, 10),
(N'Giảm giá đồ uống', 'DRINK20', 'PhanTram', 20, '2025-09-01 00:00:00', '2025-09-15 23:59:59', 'HoatDong', 30, 8),
(N'Giảm giá lẩu', 'LAU5000', 'TienCoDinh', 50000, '2025-09-10 00:00:00', '2025-09-20 23:59:59', 'HoatDong', 20, 4),
(N'Khuyến mãi cuối tuần', 'WEEKEND15', 'PhanTram', 15, '2025-09-05 00:00:00', '2025-09-12 23:59:59', 'HoatDong', 40, 6),
(N'Giảm giá combo', 'COMBO20', 'TienCoDinh', 20000, '2025-09-01 00:00:00', '2025-09-30 23:59:59', 'HoatDong', 60, 12);


-- Bảng MonAnApDungKM
INSERT INTO MonAnApDungKM (MaKhuyenMai, MaMonAn) VALUES
(1, 1), -- Giảm giá món ăn áp dụng cho Phở bò
(1, 2), -- Giảm giá món ăn áp dụng cho Cơm tấm
(2, 11), -- Giảm giá đồ uống áp dụng cho Nước cam
(2, 12), -- Giảm giá đồ uống áp dụng cho Trà sữa
(3, 8), -- Giảm giá lẩu áp dụng cho Lẩu thái
(4, 3), -- Khuyến mãi cuối tuần áp dụng cho Bún chả
(5, 6); -- Giảm giá combo áp dụng cho Bánh mì pate


-- Bảng BanAn
INSERT INTO BanAn (MaBan, SoGhe, TrangThai) VALUES
('B01', 4, N'Trống'),
('B02', 4, N'Đã đặt'),
('B03', 6, N'Trống'),
('B04', 6, N'Trống'),
('B05', 8, N'Đã đặt'),
('B06', 2, N'Trống');


-- Bảng DonHang
INSERT INTO DonHang (NgayDat, TongTienGoc, ThanhToanCuoi, MaNhanVien, TrangThai, HinhThucThanhToan, MaKhuyenMai, MaBan, GiaTriKhuyenMai) VALUES
('2025-09-07 10:00:00', 100000, 90000, 'NV002', 'DaHoanThanh', 'TienMat', 1, 'B01', 10000),
('2025-09-07 10:30:00', 150000, 120000, 'NV003', 'DangXuLy', 'ChuyenKhoan', 2, 'B02', 30000),
('2025-09-07 11:00:00', 200000, 150000, 'NV004', 'DaHoanThanh', 'TienMat', 3, 'B03', 50000),
('2025-09-07 11:30:00', 80000, 68000, 'NV005', 'DangXuLy', 'TienMat', 4, 'B04', 12000),
('2025-09-07 12:00:00', 300000, 280000, 'NV001', 'DaHoanThanh', 'ChuyenKhoan', 5, 'B05', 20000),
('2025-09-07 12:30:00', 120000, 108000, 'NV002', 'DaHoanThanh', 'TienMat', 1, 'B06', 12000),
('2025-09-07 13:00:00', 180000, 144000, 'NV003', 'DangXuLy', 'ChuyenKhoan', 2, 'B01', 36000),
('2025-09-07 13:30:00', 250000, 200000, 'NV004', 'DaHoanThanh', 'TienMat', 3, 'B02', 50000),
('2025-09-07 14:00:00', 90000, 81000, 'NV005', 'DangXuLy', 'TienMat', 1, 'B03', 9000),
('2025-09-07 14:30:00', 320000, 300000, 'NV001', 'DaHoanThanh', 'ChuyenKhoan', 5, 'B04', 20000),
('2025-09-07 15:00:00', 140000, 126000, 'NV002', 'DaHoanThanh', 'TienMat', 1, 'B05', 14000),
('2025-09-07 15:30:00', 200000, 170000, 'NV003', 'DangXuLy', 'ChuyenKhoan', 4, 'B06', 30000),
('2025-09-07 16:00:00', 160000, 128000, 'NV004', 'DaHoanThanh', 'TienMat', 2, 'B01', 32000),
('2025-09-07 16:30:00', 270000, 229500, 'NV005', 'DangXuLy', 'ChuyenKhoan', 4, 'B02', 40500),
('2025-09-07 17:00:00', 110000, 99000, 'NV001', 'DaHoanThanh', 'TienMat', 1, 'B03', 11000),
('2025-09-07 17:30:00', 300000, 280000, 'NV002', 'DaHoanThanh', 'ChuyenKhoan', 5, 'B04', 20000),
('2025-09-07 18:00:00', 130000, 117000, 'NV003', 'DangXuLy', 'TienMat', 1, 'B05', 13000),
('2025-09-07 18:30:00', 190000, 152000, 'NV004', 'DaHoanThanh', 'ChuyenKhoan', 2, 'B06', 38000),
('2025-09-07 19:00:00', 220000, 187000, 'NV005', 'DangXuLy', 'TienMat', 4, 'B01', 33000),
('2025-09-07 19:30:00', 280000, 230000, 'NV001', 'DaHoanThanh', 'TienMat', 3, 'B02', 50000),
('2025-09-07 20:00:00', 150000, 135000, 'NV002', 'DaHoanThanh', 'ChuyenKhoan', 1, 'B03', 15000),
('2025-09-07 20:30:00', 170000, 136000, 'NV003', 'DangXuLy', 'TienMat', 2, 'B04', 34000),
('2025-09-07 21:00:00', 240000, 220000, 'NV004', 'DaHoanThanh', 'ChuyenKhoan', 5, 'B05', 20000),
('2025-09-07 21:30:00', 100000, 90000, 'NV005', 'DangXuLy', 'TienMat', 1, 'B06', 10000),
('2025-09-07 22:00:00', 260000, 221000, 'NV001', 'DaHoanThanh', 'ChuyenKhoan', 4, 'B01', 39000),
('2025-09-08 09:00:00', 180000, 130000, 'NV002', 'DaHoanThanh', 'TienMat', 3, 'B02', 50000),
('2025-09-08 09:30:00', 140000, 126000, 'NV003', 'DangXuLy', 'ChuyenKhoan', 1, 'B03', 14000),
('2025-09-08 10:00:00', 200000, 180000, 'NV004', 'DaHoanThanh', 'TienMat', 1, 'B04', 20000),
('2025-09-08 10:30:00', 160000, 128000, 'NV005', 'DangXuLy', 'ChuyenKhoan', 2, 'B05', 32000),
('2025-09-08 11:00:00', 230000, 195500, 'NV001', 'DaHoanThanh', 'TienMat', 4, 'B06', 34500);


-- Bảng ChiTietDonHang
INSERT INTO ChiTietDonHang (MaDonHang, MaMonAn, SoLuong, GiaTaiThoiDiem, SoTienGiam) VALUES
(1, 1, 2, 50000, 10000), -- Phở bò, giảm 10%
(2, 11, 3, 25000, 15000), -- Nước cam, giảm 20%
(2, 12, 2, 35000, 14000), -- Trà sữa, giảm 20%
(3, 8, 1, 150000, 50000), -- Lẩu thái, giảm 50000
(4, 3, 2, 40000, 12000), -- Bún chả, giảm 15%
(5, 6, 4, 25000, 20000), -- Bánh mì pate, giảm 20000
(6, 2, 2, 45000, 9000), -- Cơm tấm, giảm 10%
(6, 11, 1, 25000, 2500), -- Nước cam, giảm 10%
(7, 12, 3, 35000, 21000), -- Trà sữa, giảm 20%
(8, 8, 1, 150000, 50000), -- Lẩu thái, giảm 50000
(8, 13, 2, 20000, 0), -- Cà phê sữa đá, không giảm
(9, 1, 2, 50000, 9000), -- Phở bò, giảm 10%
(10, 6, 4, 25000, 20000), -- Bánh mì pate, giảm 20000
(11, 1, 2, 50000, 10000), -- Phở bò, giảm 10%
(12, 3, 3, 40000, 18000), -- Bún chả, giảm 15%
(13, 11, 2, 25000, 10000), -- Nước cam, giảm 20%
(13, 12, 2, 35000, 14000), -- Trà sữa, giảm 20%
(14, 2, 3, 45000, 20250), -- Cơm tấm, giảm 15%
(15, 1, 2, 50000, 10000), -- Phở bò, giảm 10%
(16, 6, 4, 25000, 20000), -- Bánh mì pate, giảm 20000
(17, 4, 2, 42000, 8400), -- Mì quảng, giảm 10%
(18, 12, 3, 35000, 21000), -- Trà sữa, giảm 20%
(19, 3, 3, 40000, 18000), -- Bún chả, giảm 15%
(20, 8, 1, 150000, 50000), -- Lẩu thái, giảm 50000
(21, 1, 2, 50000, 10000), -- Phở bò, giảm 10%
(22, 11, 3, 25000, 15000), -- Nước cam, giảm 20%
(23, 6, 4, 25000, 20000), -- Bánh mì pate, giảm 20000
(24, 2, 2, 45000, 9000), -- Cơm tấm, giảm 10%
(25, 3, 3, 40000, 18000), -- Bún chả, giảm 15%
(26, 8, 1, 150000, 50000), -- Lẩu thái, giảm 50000
(27, 1, 2, 50000, 10000), -- Phở bò, giảm 10%
(28, 2, 2, 45000, 9000), -- Cơm tấm, giảm 10%
(29, 12, 2, 35000, 14000), -- Trà sữa, giảm 20%
(30, 3, 3, 40000, 18000), -- Bún chả, giảm 15%
(30, 11, 1, 25000, 3750); -- Nước cam, giảm 15%