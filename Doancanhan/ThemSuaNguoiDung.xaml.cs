using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Doancanhan
{
    public partial class ThemSuaNguoiDung : Window
    {
        private RestaurantDBEntities _context = new RestaurantDBEntities();
        private NguoiDung _nguoiDung;
        private bool _isEditing = false;

        public ThemSuaNguoiDung()
        {
            InitializeComponent();
            _isEditing = false;
            Title = "Thêm Người Dùng Mới";

            _nguoiDung = new NguoiDung
            {
                HoatDong = true,
                MaLoaiNguoiDung = 2
            };

            LoadLoaiNguoiDungComboBox();
            this.DataContext = _nguoiDung;
        }

        public ThemSuaNguoiDung(int maNguoiDung)
        {
            InitializeComponent();
            _isEditing = true;
            Title = "Sửa Thông Tin Người Dùng";

            _nguoiDung = _context.NguoiDungs.Find(maNguoiDung);

            LoadLoaiNguoiDungComboBox();
            this.DataContext = _nguoiDung;

            txtMatKhau.Password = _nguoiDung.MatKhau; // Hiển thị mật khẩu cũ khi sửa
        }

        private void LoadLoaiNguoiDungComboBox()
        {
            try
            {
                cboLoaiNguoiDung.ItemsSource = _context.LoaiNguoiDungs.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách loại người dùng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_nguoiDung.TenDangNhap) || string.IsNullOrWhiteSpace(txtMatKhau.Password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Gán mật khẩu trực tiếp từ PasswordBox
                _nguoiDung.MatKhau = txtMatKhau.Password;

                if (_isEditing)
                {
                    _context.Entry(_nguoiDung).State = EntityState.Modified;
                }
                else
                {
                    _context.NguoiDungs.Add(_nguoiDung);
                }

                _context.SaveChanges();
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu người dùng: " + (ex.InnerException?.Message ?? ex.Message), "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e) { this.Close(); }
    }
}
