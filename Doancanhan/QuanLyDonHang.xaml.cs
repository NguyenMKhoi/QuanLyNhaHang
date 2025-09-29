using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Doancanhan
{
    public partial class QuanLyDonHang : Window
    {
        private RestaurantDBEntities _context = new RestaurantDBEntities();

        public QuanLyDonHang()
        {
            InitializeComponent();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            LoadDonHangs();
        }

        private void LoadDonHangs()
        {
            try
            {
                _context = new RestaurantDBEntities();
                IQueryable<DonHang> query = _context.DonHangs.Include(dh => dh.ChiTietDonHangs);

                if (chkHienThiDaHoanThanh != null && chkHienThiDaHoanThanh.IsChecked == false)
                {
                    query = query.Where(dh => dh.TrangThai != "Đã hoàn thành");
                }

                dgDonHang.ItemsSource = query.OrderByDescending(dh => dh.ThoiGianTao).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách đơn hàng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChkHienThiDaHoanThanh_Changed(object sender, RoutedEventArgs e)
        {
            LoadDonHangs();
        }

        /// <summary>
        /// Xử lý nút Xem Chi Tiết: Mở cửa sổ ThanhToanView để xem thông tin.
        /// </summary>
        private void BtnXemChiTiet_Click(object sender, RoutedEventArgs e)
        {
            if (dgDonHang.SelectedItem is DonHang selectedOrder)
            {
                // Mở cửa sổ chi tiết mới, thay vì cửa sổ thanh toán
                ChiTietDonHangView chiTietWindow = new ChiTietDonHangView(selectedOrder.MaDonHang);
                chiTietWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng để xem chi tiết.", "Chưa chọn đơn hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnChuyenTrangThai_Click(object sender, RoutedEventArgs e)
        {
            if (dgDonHang.SelectedItem is DonHang selectedOrder)
            {
                try
                {
                    var orderInDb = _context.DonHangs.Find(selectedOrder.MaDonHang);
                    if (orderInDb != null)
                    {
                        switch (orderInDb.TrangThai)
                        {
                            case "Đang chờ món":
                                orderInDb.TrangThai = "Đã xong món";
                                break;
                            case "Đã xong món":
                                orderInDb.TrangThai = "Đang phục vụ";
                                break;
                            case "Đang phục vụ":
                                MessageBox.Show("Đơn hàng đang được phục vụ, không thể chuyển trạng thái tiếp.", "Thông báo");
                                return;
                            case "Đã hoàn thành":
                                MessageBox.Show("Đơn hàng đã hoàn thành, không thể thay đổi trạng thái.", "Thông báo");
                                return;
                            default:
                                orderInDb.TrangThai = "Đang chờ món";
                                break;
                        }

                        _context.SaveChanges();
                        LoadDonHangs();
                        MessageBox.Show($"Đã cập nhật trạng thái thành công: {orderInDb.TrangThai}", "Thành công");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi cập nhật trạng thái: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng để chuyển trạng thái.", "Chưa chọn đơn hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnThanhToan_Click(object sender, RoutedEventArgs e)
        {
            if (dgDonHang.SelectedItem is DonHang selectedOrder)
            {
                if (selectedOrder.TrangThai == "Đã hoàn thành")
                {
                    MessageBox.Show("Đơn hàng này đã được thanh toán trước đó.", "Thông báo");
                    return;
                }

                ThanhToanView thanhToanWindow = new ThanhToanView(selectedOrder.MaDonHang);
                thanhToanWindow.ShowDialog();
                LoadDonHangs();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng để thanh toán.", "Chưa chọn đơn hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnXoaDonHang_Click(object sender, RoutedEventArgs e)
        {
            if (dgDonHang.SelectedItem is DonHang selectedOrder)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa đơn hàng này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var deleteContext = new RestaurantDBEntities())
                        {
                            var orderToDelete = deleteContext.DonHangs.Find(selectedOrder.MaDonHang);
                            if (orderToDelete != null)
                            {
                                var chiTiet = deleteContext.ChiTietDonHangs.Where(ct => ct.MaDonHang == orderToDelete.MaDonHang).ToList();
                                deleteContext.ChiTietDonHangs.RemoveRange(chiTiet);
                                deleteContext.DonHangs.Remove(orderToDelete);
                                deleteContext.SaveChanges();
                                LoadDonHangs();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa đơn hàng: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng để xóa.");
            }
        }
    }
}
