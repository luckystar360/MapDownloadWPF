using MapControl;
using MapDownloader.Enums;
using MapDownloader.Models;
using MapDownloader.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapDownloader.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private MapViewModel _mapViewModel;
        public MapViewModel MapViewModel { get { return _mapViewModel; } }
        private readonly AppSettings _settings;
        public ObservableCollection<TileLayer> TileLayers { get; set; }

        private TileLayer _selectedTileLayer;
        public TileLayer SelectedTileLayer
        {
            get { return _selectedTileLayer; }
            set {
                SetProperty(ref _selectedTileLayer, value);
            }
        }

        private int _selectedTileLayerIndex;
        public int SelectedTileLayerIndex
        {
            get { return _selectedTileLayerIndex; }
            set { 
                SetProperty(ref _selectedTileLayerIndex, value);
            }
        }

        private Visibility _isShowNewPolygonPanel;
        public Visibility IsShowNewPolygonPanel
        {
            get { return _isShowNewPolygonPanel; }
            set
            {
                SetProperty(ref _isShowNewPolygonPanel, value);
            }
        }

        private AppMode _appMode;
        public AppMode AppMode
        {
            get { return _appMode; }
            set
            {
                SetProperty(ref _appMode, value);
                if (_appMode == AppMode.Normal)
                {
                    IsShowNewPolygonPanel = Visibility.Hidden;
                    _mapViewModel.MapMode = MapMode.MoveMode;
                }
                else
                {
                    IsShowNewPolygonPanel = Visibility.Visible;
                    _mapViewModel.MapMode = MapMode.SelectMode;
                }
            }
        }

        public MainViewModel(IOptions<AppSettings> options)
        {
            _settings = options.Value;
            _mapViewModel = App.ServiceProvider.GetRequiredService<MapViewModel>();
            TileLayers = new ObservableCollection<TileLayer>();
            foreach (TileLayer item in _mapViewModel.ServerTileLayer.TileLayers)
            {
                TileLayers.Add(item);
            }
            AppMode = AppMode.NewPolygon;

        }

        public void UpdateSelectedMapSource()
        {
            _mapViewModel.MapResource = SelectedTileLayer;
        }
    }
}
