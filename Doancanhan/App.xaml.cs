using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Doancanhan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Lấy đường dẫn của thư mục chứa file .exe
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));

            // Đi ngược lên 2 cấp để đến thư mục gốc của project (từ bin\Debug -> project)
            // Lưu ý: Nếu cấu trúc project của bạn khác, có thể cần điều chỉnh số lần ".."
            path = System.IO.Path.GetFullPath(System.IO.Path.Combine(path, "..\\..\\"));

            // Thiết lập biến |DataDirectory| để trỏ đến thư mục gốc của project
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }
    }
}
