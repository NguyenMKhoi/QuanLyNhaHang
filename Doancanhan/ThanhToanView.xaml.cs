using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Doancanhan
{
    public partial class ThanhToanView : Window
    {
        private int _maDonHang;
        private RestaurantDBEntities _context = new RestaurantDBEntities();
        private DonHang _donHang;

        public ThanhToanView(int maDonHang)
        {
            InitializeComponent();
            _maDonHang = maDonHang;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            _donHang = _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs.Select(ct => ct.MonAn))
                .FirstOrDefault(dh => dh.MaDonHang == _maDonHang);

            if (_donHang != null)
            {
                lblMaDonHang.Text = "HÓA ĐƠN #" + _donHang.MaDonHang;
                dgChiTietDonHang.ItemsSource = _donHang.ChiTietDonHangs.ToList();
                UpdateAllCalculations();
            }
        }

        private void UpdateAllCalculations()
        {
            if (_donHang == null) return;

            // === THAY THẾ BẰNG TÊN THUỘC TÍNH ĐÚNG: 'Gia' ===
            decimal tongTien = _donHang.ChiTietDonHangs.Sum(ct => (ct.SoLuong) * (ct.MonAn.Gia));
            // ===============================================

            decimal giamGia = 0;
            decimal khachCanTra = tongTien - giamGia;

            decimal.TryParse(txtKhachDua.Text, out decimal khachDua);
            decimal tienThoi = khachDua - khachCanTra;
            if (tienThoi < 0) tienThoi = 0;

            lblTongTien.Text = tongTien.ToString("N0", new CultureInfo("vi-VN")) + " VNĐ";
            lblTienGiamGia.Text = giamGia.ToString("N0", new CultureInfo("vi-VN")) + " VNĐ";
            lblTongTienTrungGian.Text = khachCanTra.ToString("N0", new CultureInfo("vi-VN")) + " VNĐ";
            lblTienThoi.Text = tienThoi.ToString("N0", new CultureInfo("vi-VN")) + " VNĐ";
        }

        private void TxtKhachDua_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAllCalculations();
        }

        private void BtnXacNhanThanhToan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var orderInDb = _context.DonHangs.Find(_maDonHang);
                if (orderInDb != null)
                {
                    orderInDb.TrangThai = "Đã hoàn thành";
                    // === THAY THẾ BẰNG TÊN THUỘC TÍNH ĐÚNG: 'Gia' ===
                    orderInDb.TongTien = _donHang.ChiTietDonHangs.Sum(ct => ct.SoLuong * ct.MonAn.Gia);
                    // ===============================================
                    _context.SaveChanges();

                    MessageBox.Show("Thanh toán thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hoàn tất thanh toán: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnInHoaDon_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng 'In Hóa Đơn' đang được phát triển.");
        }
    }
}
