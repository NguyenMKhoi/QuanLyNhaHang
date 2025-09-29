using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;

namespace Doancanhan
{
    public partial class DangNhap : Window
    {
        private RestaurantDBEntities _context = new RestaurantDBEntities();

        public DangNhap()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnDangNhap_Click(object sender, RoutedEventArgs e)
        {
            string username = txtTenDangNhap.Text;
            string password = txtMatKhau.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.");
                return;
            }

            try
            {
                // Sử dụng .Include để tải thông tin LoaiNguoiDung kèm theo
                var user = _context.NguoiDungs
                                   .Include(u => u.LoaiNguoiDung)
                                   .FirstOrDefault(u => u.TenDangNhap == username && u.MatKhau == password);

                if (user == null)
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác.");
                    return;
                }

                if (user.HoatDong == false)
                {
                    MessageBox.Show("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
                    return;
                }

                MessageBox.Show($"Đăng nhập thành công! Chào mừng {user.TenDangNhap}.");

                // === THAY ĐỔI QUAN TRỌNG: TRUYỀN USER SANG TRANG CHỦ ===
                TrangChu mainWindow = new TrangChu(user);
                // ========================================================

                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
