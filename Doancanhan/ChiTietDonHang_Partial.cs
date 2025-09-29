namespace Doancanhan
{
    // Sử dụng "partial" để mở rộng lớp ChiTietDonHang đã được EF tạo ra
    public partial class ChiTietDonHang
    {
        // Thêm một thuộc tính mới không có trong CSDL
        public decimal ThanhTien
        {
            get
            {
                // Thuộc tính này sẽ tự động tính toán giá trị
                return this.SoLuong * this.Gia;
            }
        }
    }
}
