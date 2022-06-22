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

namespace MapDownloader.Controls
{
    /// <summary>
    /// Interaction logic for MessageBoxWindow.xaml
    /// </summary>
    /// <summary>
    /// Interaction logic for MessageBoxWindow.xaml
    /// </summary>
    public partial class MessageBoxWindow : IDisposable
    {
        public MessageBoxResult Result { get; set; }

        public MessageBoxWindow()
        {
            InitializeComponent();
            Result = MessageBoxResult.Cancel;
        }
        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }
        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        private void BtnCopyMessage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(TxtMessage.Text);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
        }

        private void TitleBackgroundPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
