using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LANTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }

        SimpleHTTPServer _server = null;
        #endregion

        #region Events
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_server == null)
            {
                _server = new SimpleHTTPServer(null, 80, RequestHandler);
                System.Diagnostics.Process.Start("http://localhost");
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_server != null)
                _server.Stop();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
        #endregion

        #region Private Routines
        private static ReturnMessage RequestHandler(string filename)
        {
            return new ReturnMessage()
            {
                Content = GetResource("Template.html").Replace("{{Address}}", $"http://{GetLocalIPAddress()}")
            };
        }
        /// <summary>
        /// Get local IP address
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        /// <param name="name">In format "Namespace.Folder.Filename.Extension" or just "Filename.Extension"</param>
        public static string GetResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = name;
            if (!name.StartsWith(nameof(LANTest)))
                resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }
        #endregion
    }
}
