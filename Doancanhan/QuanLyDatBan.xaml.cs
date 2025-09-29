using System;
using System.Linq;
using System.Windows;

namespace Doancanhan
{
    public partial class QuanLyDatBan : Window
    {
        private RestaurantDBEntities _context = new RestaurantDBEntities();

        public QuanLyDatBan()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDatBan();
        }

        // Hàm tải/tải lại danh sách đặt bàn
        private void LoadDatBan()
        {
            // Luôn tạo mới context để lấy dữ liệu mới nhất
            _context = new RestaurantDBEntities();
            dgDatBan.ItemsSource = _context.DatBans.OrderByDescending(db => db.ThoiGianDat).ToList();
        }

        // Xử lý nút Thêm
        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            // Gọi constructor không tham số để Thêm mới
            ThemSuaDatBan themSuaWindow = new ThemSuaDatBan();
            if (themSuaWindow.ShowDialog() == true)
            {
                LoadDatBan(); // Tải lại danh sách sau khi thêm
            }
        }

        // Xử lý nút Sửa
        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            DatBan selectedDatBan = dgDatBan.SelectedItem as DatBan;
            if (selectedDatBan == null)
            {
                MessageBox.Show("Vui lòng chọn một lượt đặt bàn để sửa.");
                return;
            }

            // Chỉ truyền ID, không truyền cả đối tượng
            ThemSuaDatBan themSuaWindow = new ThemSuaDatBan(selectedDatBan.MaDatBan);
            if (themSuaWindow.ShowDialog() == true)
            {
                LoadDatBan(); // Tải lại danh sách sau khi sửa
            }
        }

        // Xử lý nút Xóa
        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            DatBan selectedDatBan = dgDatBan.SelectedItem as DatBan;
            if (selectedDatBan == null)
            {
                MessageBox.Show("Vui lòng chọn một lượt đặt bàn để xóa.");
                return;
            }

            var relatedOrders = _context.DonHangs.Where(dh => dh.MaDatBan == selectedDatBan.MaDatBan);
            if (relatedOrders.Any())
            {
                MessageBox.Show("Không thể xóa lượt đặt bàn này vì đã có đơn hàng liên quan. Hãy xóa đơn hàng trước.", "Thao tác bị chặn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa lượt đặt bàn của khách '" + selectedDatBan.TenKhachHang + "' không?",
                                                       "Xác nhận xóa",
                                                       MessageBoxButton.YesNo,
                                                       MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _context.DatBans.Remove(selectedDatBan);
                _context.SaveChanges();
                LoadDatBan();
            }
        }

        // Xử lý nút Tạo Đơn Hàng
        private void BtnTaoDonHang_Click(object sender, RoutedEventArgs e)
        {
            DatBan selectedDatBan = dgDatBan.SelectedItem as DatBan;
            if (selectedDatBan == null)
            {
                MessageBox.Show("Vui lòng chọn một lượt đặt bàn để tạo đơn hàng.");
                return;
            }

            DonHang donHang = _context.DonHangs.FirstOrDefault(dh => dh.MaDatBan == selectedDatBan.MaDatBan);

            if (donHang == null)
            {
                donHang = new DonHang
                {
                    MaDatBan = selectedDatBan.MaDatBan,
                    TenKhachHang = selectedDatBan.TenKhachHang,
                    ThoiGianTao = DateTime.Now,
                    TrangThai = "Đang chờ món",
                    TongTien = 0
                };
                _context.DonHangs.Add(donHang);
                _context.SaveChanges();
            }

            TaoSuaDonHang orderWindow = new TaoSuaDonHang(donHang.MaDonHang);
            orderWindow.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _context.Dispose();
        }
    }
}
