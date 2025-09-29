using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Doancanhan
{
    /// <summary>
    /// Interaction logic for QuanLyNguyenLieu.xaml
    /// </summary>
    public partial class QuanLyNguyenLieu : Window
    {
        public QuanLyNguyenLieu()
        {
            InitializeComponent();
            DgInventory.ItemsSource = GetSampleInventory();

            // Load dữ liệu mẫu cho Orders
            DgOrders.ItemsSource = GetSampleOrders();
        }
        private void BtnInventory_Click(object sender, RoutedEventArgs e)
        {
            InventoryView.Visibility = Visibility.Visible;
            OrdersView.Visibility = Visibility.Collapsed;
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            InventoryView.Visibility = Visibility.Collapsed;
            OrdersView.Visibility = Visibility.Visible;
        }

        // ==== Quick actions ====
        private void BtnAddSample_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã thêm dữ liệu mẫu");
            DgInventory.ItemsSource = GetSampleInventory();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Xuất dữ liệu ra CSV (chưa code)");
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // ==== Search ====
        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearch_Click(sender, e);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = TxtSearch.Text.Trim();
            MessageBox.Show($"Đang tìm: {keyword}");
        }

        // ==== Inventory buttons ====
        private void BtnNewProduct_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Thêm nguyên liệu mới");
        }

        private void BtnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (DgInventory.SelectedItem != null)
            {
                MessageBox.Show("Sửa nguyên liệu đã chọn");
            }
            else
            {
                MessageBox.Show("Chọn nguyên liệu để sửa");
            }
        }

        private void BtnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (DgInventory.SelectedItem != null)
            {
                MessageBox.Show("Xóa nguyên liệu đã chọn");
            }
            else
            {
                MessageBox.Show("Chọn nguyên liệu để xóa");
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Nhập dữ liệu từ file (chưa code)");
        }

        // ==== Orders ====
        private void CboOrderStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboOrderStatus.SelectedItem is ComboBoxItem item)
            {
                string status = item.Content.ToString();
                MessageBox.Show("Lọc đơn theo trạng thái: " + status);
            }
        }

        // ==== Sample data ====
        private List<Ingredient> GetSampleInventory()
        {
            return new List<Ingredient>
            {
                new Ingredient{ Id="NL01", Name="Đường", Quantity=10, Unit="Kg", Note="Đường trắng" },
                new Ingredient{ Id="NL02", Name="Sữa", Quantity=20, Unit="Hộp", Note="Sữa tươi" },
                new Ingredient{ Id="NL03", Name="Bột mì", Quantity=15, Unit="Kg", Note="Bột mì số 11" }
            };
        }

        private List<Order> GetSampleOrders()
        {
            return new List<Order>
            {
                new Order{ Id="DH01", Customer="Nguyễn Văn A", Total=150000, Status="Chờ", Date="2025-09-01"},
                new Order{ Id="DH02", Customer="Trần Thị B", Total=300000, Status="Đang xử lý", Date="2025-09-02"},
                new Order{ Id="DH03", Customer="Lê Văn C", Total=500000, Status="Đã giao", Date="2025-09-03"}
            };
        }
    }

    // ==== Models ====
    public class Ingredient
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Note { get; set; }
    }

    public class Order
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public double Total { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
    }
}
