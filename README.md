# 🍽️ Ứng dụng Quản Lý Nhà Hàng (WPF - C#)

## 📌 Giới thiệu
Ứng dụng **Quản Lý Nhà Hàng** được phát triển bằng **WPF - C# (.NET)** nhằm hỗ trợ quản lý toàn diện cho nhà hàng:

- Quản lý món ăn & thực đơn  
- Quản lý bàn ăn & đặt bàn  
- Quản lý hóa đơn & thanh toán  
- Quản lý nhân viên & ca làm  

Hệ thống sử dụng **SQL Server** để lưu trữ dữ liệu, theo kiến trúc **MVVM (Model – View – ViewModel)**.

---

## 🗄️ Cơ sở dữ liệu
- Ảnh ERD Database:  
  ![Database ERD](https://github.com/cuongle4399/cuongle4399/blob/main/img/quanlynhahang.png)  

- Link trực tiếp ERD (DB Designer):  
  👉 [Xem sơ đồ Database tại đây](https://erd.dbdesigner.net/designer/schema/1757519693-quanlynhahang)

---

## ⚙️ Cài đặt
1. Clone project:
   ```bash
   git clone https://github.com/cuongle4399/quanlynhahang.git
   cd quanlynhahang
   ```
2. Mở project bằng **Visual Studio 2022**  
3. Import database vào SQL Server:
   - Tạo database mới `QuanLyNhaHang`  
   - Chạy script SQL trong thư mục `Database/`  
4. Cập nhật `Connection String` trong `Services/DatabaseConnect.cs`  
5. Build & Run  
Hoặc tải bản đã được build ở Releases
Sau đó import database là chạy dc


---

## 🌱 Hướng dẫn Git cho Team

### 1. Cấu trúc nhánh
- `main` → nhánh chính (code ổn định)  
- `test` → nhánh kiểm thử trước khi đưa vào main 
- Các nhánh module (mỗi chức năng 1 nhánh riêng):  
  - `menuManager`  
  - `revenueManager`  
  - `saleManager`  
  - `staffManager`  
  - `confirmPayFood`  
  - `orderFood`  

---
## 👨‍💻 Tác giả
- **Cường Lê** – Team Leader & Developer
- **Nguyễn Võ Thành Tín** – Developer
- **Huỳnh Ngọc Khang** – Developer  
- **Nguyễn Nhất Nguyên** – Developer  

📅 Phiên bản: 1.0.0  
📌 Công nghệ: WPF, .NET, SQL Server