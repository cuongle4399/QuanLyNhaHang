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

---

## 🌱 Hướng dẫn Git cho Team

### 1. Cấu trúc nhánh
- `main` → nhánh chính (code ổn định)  
- `test` → nhánh kiểm thử (được đổi từ `master`)  
- Các nhánh module (mỗi chức năng 1 nhánh riêng):  
  - `menuManager`  
  - `revenueManager`  
  - `saleManager`  
  - `staffManager`  
  - `confirmPayFood`  
  - `orderFood`  

---

### 2. Đổi `master` → `test`
Thực hiện 1 lần để chuẩn hóa repo:
```bash
git branch -m master test
git push origin test
git push origin --delete master
```

---

### 3. Hướng dẫn quy trình làm việc cho thành viên

#### 🔹 Bước 1: Clone repo
```bash
git clone https://github.com/cuongle4399/quanlynhahang.git
cd quanlynhahang
```

#### 🔹 Bước 2: Cập nhật code mới nhất
```bash
git checkout main
git pull origin main
```

#### 🔹 Bước 3: Checkout sang nhánh đã được phân công
Ví dụ phụ trách `OrderFood`:
```bash
git checkout orderFood
git pull origin orderFood
```

#### 🔹 Bước 4: Code + Commit
```bash
git add .
git commit -m "Thêm chức năng OrderFood"
```

#### 🔹 Bước 5: Push nhánh lên GitHub
```bash
git push origin orderFood
```

#### 🔹 Bước 6: Tạo Pull Request (PR)
- Lên GitHub → chọn nhánh `orderFood`  
- Tạo PR merge vào `test`  
- Sau khi test OK → Leader sẽ merge `test` → `main`  

---

### 4. Merge code (chỉ Leader thực hiện)

#### Merge vào `test` để kiểm thử
```bash
git checkout test
git pull origin test
git merge orderFood
git push origin test
```

#### Merge code ổn định vào `main`
```bash
git checkout main
git pull origin main
git merge test
git push origin main
```

---

## 👨‍💻 Tác giả
- **Lê Cường** – Team Leader & Developer
- **Nguyễn Võ Thành Tín** – Developer
- **Huỳnh Ngọc Khang** – Developer  
- **Nguyễn Nhất Nguyên** – Developer  

📅 Phiên bản: 1.0.0  
📌 Công nghệ: WPF, .NET, SQL Server
