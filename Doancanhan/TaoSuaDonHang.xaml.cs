using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Doancanhan
{
    public partial class TaoSuaDonHang : Window
    {
        private RestaurantDBEntities _context = new RestaurantDBEntities();
        private DonHang _donHang;
        private int _maDonHang;
        private List<ChiTietDonHang> _chiTietItems;

        public TaoSuaDonHang(int maDonHang)
        {
            InitializeComponent();
            _maDonHang = maDonHang;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _donHang = _context.DonHangs.Find(_maDonHang);
            if (_donHang == null)
            {
                MessageBox.Show("Không tìm thấy đơn hàng. Cửa sổ sẽ đóng.");
                this.Close();
                return;
            }

            dgMonAn.ItemsSource = _context.MonAns
                                          .Include(m => m.LoaiMonAn)
                                          .Where(m => m.DangBan == true)
                                          .ToList();

            LoadChiTietDonHang();
        }

        private void LoadChiTietDonHang()
        {
            _chiTietItems = _context.ChiTietDonHangs
                                    .Include(ct => ct.MonAn)
                                    .Where(ct => ct.MaDonHang == _donHang.MaDonHang)
                                    .ToList();

            dgChiTietDonHang.ItemsSource = _chiTietItems;
            lblThongTinDonHang.Text = $"Đơn hàng #{_donHang.MaDonHang} - Khách: {_donHang.TenKhachHang}";
            CapNhatTongTien();
        }

        private void CapNhatTongTien()
        {
            decimal tongTien = _chiTietItems.Sum(ct => ct.SoLuong * ct.Gia);
            _donHang.TongTien = tongTien;
            txtTongTien.Text = tongTien.ToString("N0") + " ₫";
        }

        private void DgMonAn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MonAn selectedMon = dgMonAn.SelectedItem as MonAn;
            if (selectedMon == null) return;

            ChiTietDonHang existingItem = _chiTietItems.FirstOrDefault(ct => ct.MaMonAn == selectedMon.MaMonAn);

            if (existingItem != null)
            {
                existingItem.SoLuong++;
            }
            else
            {
                ChiTietDonHang newItem = new ChiTietDonHang
                {
                    MaDonHang = _donHang.MaDonHang,
                    MaMonAn = selectedMon.MaMonAn,
                    SoLuong = 1,
                    Gia = selectedMon.Gia,
                    MonAn = selectedMon
                };
                _chiTietItems.Add(newItem);
                _context.ChiTietDonHangs.Add(newItem);
            }

            dgChiTietDonHang.ItemsSource = null;
            dgChiTietDonHang.ItemsSource = _chiTietItems;
            CapNhatTongTien();
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Đã lưu đơn hàng thành công!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu đơn hàng: " + ex.Message);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _context.Dispose();
        }
    }
}
