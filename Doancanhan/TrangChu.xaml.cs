using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using System.Globalization;

namespace Doancanhan
{
    public partial class TrangChu : Window
    {
        private NguoiDung _currentUser;
        private RestaurantDBEntities _context = new RestaurantDBEntities();

        public TrangChu()
        {
            InitializeComponent();
        }

        public TrangChu(NguoiDung user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyPermissions();
            LoadDashboardData();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                var homNay = DateTime.Now.Date;

                var donHangHomNay = _context.DonHangs
                                            .Where(dh => DbFunctions.TruncateTime(dh.ThoiGianTao) == homNay)
                                            .ToList();

                // === SỬA LỖI Ở ĐÂY ===
                // Do TongTien là kiểu 'decimal' (không null), hàm Sum sẽ không bao giờ trả về null.
                // Vì vậy, ta loại bỏ toán tử '??' không cần thiết.
                decimal doanhThuHomNay = donHangHomNay.Sum(dh => dh.TongTien);

                // Giữ nguyên logic này vì nó an toàn cho trường hợp SoKhach có thể null.
                int tongSoKhachHomNay = _context.DatBans
                                               .Where(db => DbFunctions.TruncateTime(db.ThoiGianDat) == homNay && db.TrangThai != "Đã hủy")
                                               .Sum(db => (int?)db.SoKhach) ?? 0;
                // ======================

                int tongDonHangHomNay = donHangHomNay.Count();

                txtDoanhThuHomNay.Text = doanhThuHomNay.ToString("N0", new CultureInfo("vi-VN")) + " ₫";
                txtTongDonHang.Text = tongDonHangHomNay.ToString();
                txtSoKhachHomNay.Text = tongSoKhachHomNay.ToString();

                dgDonHangMoi.ItemsSource = donHangHomNay.OrderByDescending(dh => dh.ThoiGianTao).Take(10).ToList();
                lvDatBanSapToi.ItemsSource = _context.DatBans
                                                     .Where(db => DbFunctions.TruncateTime(db.ThoiGianDat) >= homNay && db.TrangThai == "Đã đặt")
                                                     .OrderBy(db => db.ThoiGianDat)
                                                     .Take(10)
                                                     .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu tổng quan: " + ex.Message);
            }
        }

        private void ApplyPermissions()
        {
            if (_currentUser != null && _currentUser.LoaiNguoiDung != null && _currentUser.LoaiNguoiDung.TenLoai == "Nhân viên")
            {
                btnQuanLyNguoiDung.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnQuanLyNguoiDung.Visibility = Visibility.Visible;
            }
        }

        #region Navigation Button Clicks
        private void BtnTrangChu_Click(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
            MessageBox.Show("Đã làm mới dữ liệu trang chủ.");
        }

        private void BtnQuanLyDatBan_Click(object sender, RoutedEventArgs e)
        {
            QuanLyDatBan quanLyDatBanWindow = new QuanLyDatBan();
            quanLyDatBanWindow.Show();
        }

        private void BtnQuanLyMonAn_Click(object sender, RoutedEventArgs e)
        {
            QuanLyMonAn quanLyMonAnWindow = new QuanLyMonAn();
            quanLyMonAnWindow.Show();
        }

        private void BtnQuanLyDonHang_Click(object sender, RoutedEventArgs e)
        {
            QuanLyDonHang quanLyDonHangWindow = new QuanLyDonHang();
            quanLyDonHangWindow.Show();
        }

        private void BtnQuanLyNguoiDung_Click(object sender, RoutedEventArgs e)
        {
            QuanLyNguoiDung quanLyNguoiDungWindow = new QuanLyNguoiDung();
            quanLyNguoiDungWindow.Show();
        }

        private void BtnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            DangNhap loginWindow = new DangNhap();
            loginWindow.Show();
            this.Close();
        }
        #endregion
    }
}
