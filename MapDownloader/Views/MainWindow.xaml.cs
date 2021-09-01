using MapDownloader.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel _mainViewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
        //MapViewModel _mapViewModel = App.ServiceProvider.GetRequiredService<MapViewModel>();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _mainViewModel;
        }

        private void MenuToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            DemoItemsSearchBox.Focus();
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;

            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void PopupBox_OnClosed(object sender, RoutedEventArgs e)
        {

        }

        private void PopupBox_OnOpened(object sender, RoutedEventArgs e)
        {

        }

        private void DemoItemsListBox_Selected(object sender, RoutedEventArgs e)
        {
            //_mapViewModel.MapResource = _mainViewModel.SelectedTileLayer;
        }
        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mainViewModel.UpdateSelectedMapSource();
        }
    }
}
