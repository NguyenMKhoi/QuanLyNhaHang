using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace Doancanhan
{
    public partial class QuanLyNguoiDung : Window
    {
        private RestaurantDBEntities _context = new RestaurantDBEntities();

        public QuanLyNguoiDung()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNguoiDung();
        }

        private void LoadNguoiDung()
        {
            _context = new RestaurantDBEntities();
            dgNguoiDung.ItemsSource = _context.NguoiDungs.Include(u => u.LoaiNguoiDung).ToList();
        }

        private void BtnKhoaMo_Click(object sender, RoutedEventArgs e)
        {
            if (dgNguoiDung.SelectedItem is NguoiDung selected)
            {
                selected.HoatDong = !selected.HoatDong;

                // LƯU THAY ĐỔI VÀO CSDL
                _context.SaveChanges();

                LoadNguoiDung();
                string statusMessage = selected.HoatDong ? "mở khóa" : "khóa";
                MessageBox.Show($"Đã {statusMessage} tài khoản '{selected.TenDangNhap}'.");
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgNguoiDung.SelectedItem is NguoiDung selected)
            {
                if (selected.TenDangNhap.ToLower() == "admin")
                {
                    MessageBox.Show("Không thể xóa tài khoản admin gốc.", "Thao tác bị chặn", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Bạn có chắc muốn xóa tài khoản '{selected.TenDangNhap}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _context.NguoiDungs.Remove(selected);

                    // LƯU THAY ĐỔI VÀO CSDL
                    _context.SaveChanges();

                    LoadNguoiDung();
                }
            }
        }

        // Hàm Thêm và Sửa giữ nguyên
        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            ThemSuaNguoiDung themSuaWindow = new ThemSuaNguoiDung();
            if (themSuaWindow.ShowDialog() == true) LoadNguoiDung();
        }

        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgNguoiDung.SelectedItem is NguoiDung selected)
            {
                ThemSuaNguoiDung themSuaWindow = new ThemSuaNguoiDung(selected.MaNguoiDung);
                if (themSuaWindow.ShowDialog() == true) LoadNguoiDung();
            }
        }
    }
}
