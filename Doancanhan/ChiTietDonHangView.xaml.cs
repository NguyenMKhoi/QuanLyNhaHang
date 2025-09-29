using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Doancanhan
{
    public partial class ChiTietDonHangView : Window
    {
        private int _maDonHang;
        private RestaurantDBEntities _context = new RestaurantDBEntities();

        public ChiTietDonHangView(int maDonHang)
        {
            InitializeComponent();
            _maDonHang = maDonHang;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadChiTietDonHang();
        }

        private void LoadChiTietDonHang()
        {
            try
            {
                var donHang = _context.DonHangs
                                      .Include(dh => dh.ChiTietDonHangs.Select(ct => ct.MonAn))
                                      .FirstOrDefault(dh => dh.MaDonHang == _maDonHang);

                if (donHang != null)
                {
                    // Hiển thị thông tin chung
                    lblTieuDe.Text += donHang.MaDonHang.ToString();
                    lblTenKhachHang.Text = donHang.TenKhachHang;
                    lblThoiGianTao.Text = donHang.ThoiGianTao.ToString("HH:mm dd/MM/yyyy");
                    lblTrangThai.Text = donHang.TrangThai;

                    // Tính toán và hiển thị thành tiền cho mỗi món
                    var chiTietList = donHang.ChiTietDonHangs.Select(ct => new
                    {
                        MonAn = ct.MonAn,
                        SoLuong = ct.SoLuong,
                        ThanhTien = ct.SoLuong * ct.MonAn.Gia
                    }).ToList();

                    dgChiTietMonAn.ItemsSource = chiTietList;

                    // Hiển thị tổng cộng
                    lblTongCong.Text = donHang.TongTien.ToString("N0", new CultureInfo("vi-VN")) + " VNĐ";
                }
                else
                {
                    MessageBox.Show("Không tìm thấy đơn hàng.", "Lỗi");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải chi tiết đơn hàng: " + ex.Message, "Lỗi");
            }
        }
    }
}
