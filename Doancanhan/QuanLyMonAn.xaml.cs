using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace Doancanhan
{
    public partial class QuanLyMonAn : Window
    {
        private RestaurantDBEntities _context = new RestaurantDBEntities();

        public QuanLyMonAn()
        {
            InitializeComponent();
        }

        // Sự kiện được gọi khi cửa sổ được tải xong
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMonAn();
        }

        // Hàm để tải hoặc tải lại danh sách món ăn
        private void LoadMonAn()
        {
            try
            {
                // Luôn tạo mới context để đảm bảo dữ liệu là mới nhất
                _context = new RestaurantDBEntities();

                // Sử dụng .Include() để tải kèm thông tin của bảng LoaiMonAn
                // Điều này rất quan trọng để hiển thị được tên loại món
                dgMonAn.ItemsSource = _context.MonAns.Include(m => m.LoaiMonAn).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách món ăn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xử lý nút Thêm Mới
        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            ThemSuaMonAn themSuaWindow = new ThemSuaMonAn();
            if (themSuaWindow.ShowDialog() == true)
            {
                LoadMonAn(); // Tải lại danh sách sau khi thêm thành công
            }
        }

        // Xử lý nút Sửa
        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgMonAn.SelectedItem is MonAn selectedMonAn)
            {
                ThemSuaMonAn themSuaWindow = new ThemSuaMonAn(selectedMonAn.MaMonAn);
                if (themSuaWindow.ShowDialog() == true)
                {
                    LoadMonAn(); // Tải lại danh sách sau khi sửa
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một món ăn để sửa.");
            }
        }

        // Xử lý nút Xóa
        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgMonAn.SelectedItem is MonAn selectedMonAn)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa món '{selectedMonAn.TenMonAn}' không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    // Kiểm tra ràng buộc khóa ngoại trước khi xóa
                    bool isInOrder = _context.ChiTietDonHangs.Any(ct => ct.MaMonAn == selectedMonAn.MaMonAn);
                    if (isInOrder)
                    {
                        MessageBox.Show("Không thể xóa món ăn này vì đã được sử dụng trong đơn hàng.", "Thao tác bị chặn", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }

                    _context.MonAns.Remove(selectedMonAn);
                    _context.SaveChanges();
                    LoadMonAn(); // Tải lại danh sách
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một món ăn để xóa.");
            }
        }
    }
}
