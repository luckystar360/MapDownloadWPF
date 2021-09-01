using MapControl;
using MapDownloader.Enums;
using MapDownloader.Models;
using MapDownloader.Services.Commands;
using MapDownloader.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MapDownloader.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private MapViewModel _mapViewModel;
        public MapViewModel MapViewModel { get { return _mapViewModel; } }
        private readonly AppSettings _settings;
        public ObservableCollection<TileLayer> TileLayers { get; set; }
        public ObservableCollection<int> ListLayerIndex { get; set; }
        public Dictionary<string, UserControl> ListRightPanels = new Dictionary<string, UserControl>();


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

        private bool _isDownloading = false;
        public bool IsDownloading
        {
            get { return _isDownloading; }
            set
            {
                SetProperty(ref _isDownloading, value);
            }
        }

        private int _percentDownloading = 0;
        public int PercentDownloading
        {
            get { return _percentDownloading; }
            set
            {
                SetProperty(ref _percentDownloading, value);
            }
        }

        private AppMode _appMode;
        public AppMode AppMode
        {
            get { return _appMode; }
            set
            {
                SetProperty(ref _appMode, value);
                switch (_appMode)
                {
                    case AppMode.Normal:
                        _mapViewModel.MapMode = MapMode.MoveMode;
                        break;
                    case AppMode.NewPolygon:
                        _mapViewModel.MapMode = MapMode.SelectMode;
                        RightPanelItem = ListRightPanels["InfoPanel"];
                        break;
                    case AppMode.DownloadMap:
                        _mapViewModel.MapMode = MapMode.MoveMode;
                        RightPanelItem = ListRightPanels["DownloadPanel"];
                        break;
                    default:break;
                }
            }
        }

        private UserControl _rightPanelItem;
        public UserControl RightPanelItem
        {
            get { return _rightPanelItem; }
            set { SetProperty(ref _rightPanelItem, value); }
        }


        public RelayCommand AddNewRegionCommand { get; set; }
        public RelayCommand DownloadRegionCommand { get; set; }
        public RelayCommand DeleteRegionCommand { get; set; }
        public RelayCommand SaveRegionCommand { get; set; }

        public RelayCommand DownloadCommand { get; set; }
        public RelayCommand CancelDownloadCommand { get; set; }

        public MainViewModel(IOptions<AppSettings> options)
        {
            _settings = options.Value;
            ListRightPanels.Add("InfoPanel", new PolygonInfoPanel() as UserControl); //0
            ListRightPanels.Add("DownloadPanel", new DownloadPanel() as UserControl); //1

            //RightPanelItem = ListRightPanels[0];
            _mapViewModel = App.ServiceProvider.GetRequiredService<MapViewModel>();
            TileLayers = new ObservableCollection<TileLayer>();
            foreach (TileLayer item in _mapViewModel.ServerTileLayer.TileLayers)
            {
                TileLayers.Add(item);
            }
            AppMode = AppMode.Normal;

            ListLayerIndex = new ObservableCollection<int> { 7,8,9,10,11,12,13,14,15,16,17};
            //command register
            AddNewRegionCommand = new RelayCommand(HandleAppModeChanged, (obj)=>{ return AppMode != AppMode.NewPolygon; });
            DeleteRegionCommand = new RelayCommand(HandleAppModeChanged);
            SaveRegionCommand = new RelayCommand(HandleAppModeChanged);
            DownloadRegionCommand = new RelayCommand(HandleAppModeChanged, (obj) => { return AppMode != AppMode.DownloadMap; });
            DownloadCommand = new RelayCommand(HandleDownloadCommand, (obj) => { return !_isDownloading; });
            CancelDownloadCommand = new RelayCommand(HandleCancelDownloadCommand, (obj) => { return _isDownloading; });

        }

        /// <summary>
        /// Handle add new polygon
        /// </summary>
        /// <param name="obj"></param>
        private void HandleAppModeChanged(object obj)
        {
            if(obj is string)
            {
                switch(obj as string)
                {
                    case "AddNew":
                        AppMode = AppMode.NewPolygon;
                        break;
                    case "Delete":
                        _mapViewModel.ClearSelectedPolygon();
                        break;
                    case "Save":
                        break;
                    case "Download":
                        AppMode = AppMode.DownloadMap;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Handle button Download Click
        /// </summary>
        /// <param name="obj"></param>
        private void HandleDownloadCommand(object obj)
        {
            IsDownloading = true;
        }

        /// <summary>
        /// Handle button CancelDownload
        /// </summary>
        private void HandleCancelDownloadCommand(object obj)
        {
            IsDownloading = false;
        }

        public void UpdateSelectedMapSource()
        {
            _mapViewModel.MapResource = SelectedTileLayer;
        }
    }
}
