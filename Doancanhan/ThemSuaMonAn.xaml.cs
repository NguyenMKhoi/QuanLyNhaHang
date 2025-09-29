using System;
using System.Data.Entity.Migrations; // Thư viện cần thiết cho AddOrUpdate
using System.Linq;
using System.Windows;

namespace Doancanhan
{
    public partial class ThemSuaMonAn : Window
    {
        private RestaurantDBEntities _context = new RestaurantDBEntities();
        private MonAn _monAn;
        private bool _isEditing = false;

        public ThemSuaMonAn()
        {
            InitializeComponent();
            _isEditing = false;
            Title = "Thêm Món Ăn Mới";
            _monAn = new MonAn { DangBan = true };
            LoadComboBox();
            this.DataContext = _monAn;
        }



        public ThemSuaMonAn(int maMonAn)
        {
            InitializeComponent();
            _isEditing = true;
            Title = "Sửa Thông Tin Món Ăn";
            _monAn = _context.MonAns.Find(maMonAn);
            LoadComboBox();
            this.DataContext = _monAn;
        }

        private void LoadComboBox()
        {
            cboLoaiMonAn.ItemsSource = _context.LoaiMonAns.ToList();
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_monAn.TenMonAn) || _monAn.Gia <= 0 || cboLoaiMonAn.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin hợp lệ.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_isEditing)
                {
                    // Chế độ Sửa: Cập nhật đối tượng đã có
                    _context.MonAns.AddOrUpdate(_monAn);
                }
                else
                {
                    // Chế độ Thêm: Thêm đối tượng mới
                    _context.MonAns.Add(_monAn);
                }

                // LƯU THAY ĐỔI VÀO CSDL
                _context.SaveChanges();

                MessageBox.Show("Lưu món ăn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
