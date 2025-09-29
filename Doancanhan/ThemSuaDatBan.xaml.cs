using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Doancanhan
{
    public partial class ThemSuaDatBan : Window
    {
        private RestaurantDBEntities _context = new RestaurantDBEntities();
        private DatBan _datBan;
        private bool _isEditing = false;

        // Thuộc tính để binding với DatePicker, ngăn chặn chọn ngày quá khứ
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Constructor cho chức năng THÊM MỚI.
        /// </summary>
        public ThemSuaDatBan()
        {
            InitializeComponent();
            _isEditing = false;
            Title = "Thêm Đặt Bàn Mới";

            // Chỉ cho phép chọn từ ngày hôm nay trở đi
            StartDate = DateTime.Now.Date;

            // Khởi tạo đối tượng mới với các giá trị nghiệp vụ mặc định
            _datBan = new DatBan
            {
                ThoiGianDat = DateTime.Now,
                SoKhach = 2,
                TrangThai = "Đã xác nhận"
            };

            // Gán DataContext cho cửa sổ và các control cần thiết
            this.DataContext = this;          // Dùng cho StartDate
            dpNgayDat.DataContext = _datBan;  // Dùng cho ThoiGianDat

            // Cập nhật các TextBox từ đối tượng _datBan
            UpdateTextBoxesFromObject();
        }

        /// <summary>
        /// Constructor cho chức năng SỬA.
        /// </summary>
        public ThemSuaDatBan(int maDatBan)
        {
            InitializeComponent();
            _isEditing = true;
            Title = "Sửa Thông Tin Đặt Bàn";

            // Chỉ cho phép chọn từ ngày hôm nay trở đi
            StartDate = DateTime.Now.Date;

            // Tải bản ghi cũ từ CSDL
            _datBan = _context.DatBans.Find(maDatBan);

            // Gán DataContext
            this.DataContext = this;
            dpNgayDat.DataContext = _datBan;

            // Cập nhật các TextBox từ đối tượng _datBan đã tải
            UpdateTextBoxesFromObject();
        }

        /// <summary>
        /// Đồng bộ dữ liệu từ đối tượng _datBan lên các control trên giao diện.
        /// </summary>
        private void UpdateTextBoxesFromObject()
        {
            if (_datBan != null)
            {
                txtGio.Text = _datBan.ThoiGianDat.ToString("HH");
                txtPhut.Text = _datBan.ThoiGianDat.ToString("mm");
                txtTenKhachHang.Text = _datBan.TenKhachHang;
                txtSoDienThoai.Text = _datBan.SoDienThoai;
                txtSoKhach.Text = _datBan.SoKhach.ToString();
                cboTrangThai.Text = _datBan.TrangThai;
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Lưu.
        /// </summary>
        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenKhachHang.Text) ||
                string.IsNullOrWhiteSpace(txtSoDienThoai.Text) ||
                string.IsNullOrWhiteSpace(txtSoKhach.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tất cả thông tin.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // === GHÉP NGÀY, GIỜ, PHÚT LẠI VỚI NHAU TRƯỚC KHI LƯU ===
                DateTime selectedDate = dpNgayDat.SelectedDate ?? DateTime.Now.Date;
                int hour, minute;

                if (!int.TryParse(txtGio.Text, out hour) || !int.TryParse(txtPhut.Text, out minute))
                {
                    MessageBox.Show("Giờ hoặc phút nhập vào không hợp lệ.", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Cập nhật lại toàn bộ thuộc tính của đối tượng _datBan từ giao diện
                _datBan.TenKhachHang = txtTenKhachHang.Text;
                _datBan.SoDienThoai = txtSoDienThoai.Text;
                _datBan.SoKhach = int.Parse(txtSoKhach.Text);
                _datBan.ThoiGianDat = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, hour, minute, 0);
                _datBan.TrangThai = cboTrangThai.Text;
                // ==========================================================

                if (_isEditing) { _context.Entry(_datBan).State = EntityState.Modified; }
                else { _context.DatBans.Add(_datBan); }

                _context.SaveChanges();

                if (_datBan.TrangThai == "Đã đến" && !_context.DonHangs.Any(dh => dh.MaDatBan == _datBan.MaDatBan))
                {
                    var newOrder = new DonHang { MaDatBan = _datBan.MaDatBan, TenKhachHang = _datBan.TenKhachHang, ThoiGianTao = DateTime.Now, TrangThai = "Đang chờ món", TongTien = 0 };
                    _context.DonHangs.Add(newOrder);
                    _context.SaveChanges();
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu đặt bàn: " + (ex.InnerException?.Message ?? ex.Message), "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e) { this.Close(); }

        #region --- Input Validation ---

        private void DpNgayDat_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpNgayDat.SelectedDate.HasValue && dpNgayDat.SelectedDate.Value < DateTime.Now.Date)
            {
                MessageBox.Show("Không thể chọn ngày trong quá khứ. Vui lòng chọn lại.", "Ngày không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpNgayDat.SelectedDate = DateTime.Now.Date;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TxtGio_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txtGio.Text, out int hourValue) && hourValue > 23)
            {
                txtGio.Text = "23";
                txtGio.CaretIndex = txtGio.Text.Length;
            }
        }

        private void TxtPhut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txtPhut.Text, out int minuteValue) && minuteValue > 59)
            {
                txtPhut.Text = "59";
                txtPhut.CaretIndex = txtPhut.Text.Length;
            }
        }

        #endregion
    }
}
