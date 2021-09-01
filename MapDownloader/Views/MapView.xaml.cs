using MapControl;
using MapDownloader.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapDownloader.Views
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        private MapViewModel _mapViewModel = App.ServiceProvider.GetRequiredService<MapViewModel>();
        public MapView()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetRequiredService<MapViewModel>();
        }

        private void MapManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 0.001;
        }

        private Location _mouseButtonDownLocation = null;
        private void map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_mapViewModel.MapMode != Enums.MapMode.SelectMode)
                return;
            var map = sender as Map;
            Point mouseButtonDown = e.GetPosition(map);
            _mouseButtonDownLocation = map.ViewportPointToLocation(mouseButtonDown);
        }

        private void map_MouseMove(object sender, MouseEventArgs e)
        {
            var map = sender as Map;
            Point mouseMove = e.GetPosition(map);
            Location mouseMoveLocation = map.ViewportPointToLocation(mouseMove);
            _mapViewModel.MouseMovePosition = mouseMoveLocation;
            if(_mouseButtonDownLocation != null && mouseMoveLocation != null)
            {
                _mapViewModel.UpdateSelectedPolygon(_mouseButtonDownLocation, mouseMoveLocation);
            }
        }

        private void map_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _mouseButtonDownLocation = null;
        }
    }
}
