# 🍽️ Ứng dụng Quản Lý Nhà Hàng (WPF - C#)

## 📌 Giới thiệu
Ứng dụng **Quản Lý Nhà Hàng** được phát triển bằng **WPF - C# (.NET)** nhằm hỗ trợ quản lý toàn diện cho nhà hàng:

- Quản lý món ăn & thực đơn  
- Quản lý bàn ăn & đặt bàn  
- Quản lý hóa đơn & thanh toán  
- Quản lý nhân viên & ca làm  

Hệ thống sử dụng **SQL Server** để lưu trữ dữ liệu, theo kiến trúc **MVVM (Model – View – ViewModel)**.

---

## 🏗️ Cấu trúc thư mục
```plaintext
RestaurantManager/
├── Models/                 # Chứa class dữ liệu (Food, Table, Bill, Employee, ...)
│
├── Services/               # Xử lý logic & kết nối Database
│   ├── DatabaseService.cs
│   └── AuthService.cs
│
├── View/                   # Giao diện XAML
│   ├── Features/
│   │   ├── Admin/
│   │   │   ├── MenuManager.xaml
│   │   │   ├── RevenueManager.xaml
│   │   │   ├── SaleManager.xaml
│   │   │   └── StaffManager.xaml
│   │   └── Staff/
│   │       ├── ConfirmPayFood.xaml
│   │       └── OrderFood.xaml
│   │
│   ├── Admin.xaml
│   ├── Home.xaml
│   └── Staff.xaml
│
├── ViewModels/             # Xử lý logic UI (MVVM)
│   ├── Admin/
│   │   ├── MenuManagerViewModel.cs
│   │   ├── RevenueManagerViewModel.cs
│   │   ├── SaleManagerViewModel.cs
│   │   └── StaffManagerViewModel.cs
│   └── Staff/
│       ├── ConfirmPayFoodViewModel.cs
│       └── OrderFoodViewModel.cs
│
├── App.xaml
└── MainWindow.xaml
```

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
4. Cập nhật `Connection String` trong `App.config`  
5. Build & Run  

---

## 🌱 Hướng dẫn Git cho Team

### 1. Cấu trúc nhánh
- `main` → nhánh chính (code ổn định)  
- `test` → nhánh kiểm thử trước khi đưa vào main  
- Các nhánh module (đã tạo sẵn, mỗi chức năng 1 nhánh):  
  - `menuManager`  
  - `revenueManager`  
  - `saleManager`  
  - `staffManager`  
  - `confirmPayFood`  
  - `orderFood`  

---

### 2. Quy trình làm việc cho thành viên

#### 🔹 Bước 1: Clone repo
```bash
git clone https://github.com/cuongle4399/quanlynhahang.git
cd quanlynhahang
```

#### 🔹 Bước 2: Cập nhật code mới nhất từ `main`
```bash
git checkout main
git pull origin main
```

#### 🔹 Bước 3: Checkout nhánh được phân công
Ví dụ bạn phụ trách `orderFood`:
```bash
git checkout orderFood
git pull origin orderFood
```

#### 🔹 Bước 4: Code + Commit
```bash
git add .
git commit -m "Hoàn thiện chức năng OrderFood"
```

#### 🔹 Bước 5: Push code lên GitHub
```bash
git push origin orderFood
```

#### 🔹 Bước 6: Tạo Pull Request (PR)
- Lên GitHub → chọn nhánh `orderFood`  
- Tạo PR merge vào `test`  
- Sau khi test OK → Leader sẽ merge `test` → `main`  

---

### 3. Merge code (chỉ Leader thực hiện)

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

⚠️ **Lưu ý:** Chỉ Leader mới được merge vào nhánh `main`.

---

## 👨‍💻 Tác giả
- **Lê Quốc Cường** – Developer  (Nhóm trưởng)  
- **Huỳnh Ngọc Khang** – Developer  
- **Nguyễn Nhất Nguyên** – Developer  
- **Nguyễn Võ Thành Tín** – Developer  

📅 Phiên bản: 1.0.0  
📌 Công nghệ: WPF, .NET, SQL Server  
