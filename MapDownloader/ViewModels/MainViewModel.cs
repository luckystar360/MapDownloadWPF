using MapControl;
using MapDownloader.Enums;
using MapDownloader.Helpers;
using MapDownloader.Models;
using MapDownloader.Services.Commands;
using MapDownloader.Services.MapCache;
using MapDownloader.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MapDownloader.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private MapViewModel _mapViewModel;
        public MapViewModel MapViewModel { get { return _mapViewModel; } }
        private readonly AppSettings _settings;

        private ITileDownload _tileDownload;

        private BackgroundWorker _bgwDownloadMap;

        public ObservableCollection<TileLayer> TileLayers { get; set; }
        public ObservableCollection<int> ListLayerIndex { get; set; }
        public Dictionary<string, UserControl> ListRightPanels = new Dictionary<string, UserControl>();


        private TileLayer _selectedTileLayer;
        public TileLayer SelectedTileLayer
        {
            get { return _selectedTileLayer; }
            set
            {
                SetProperty(ref _selectedTileLayer, value);
            }
        }

        private int _selectedTileLayerIndex;
        public int SelectedTileLayerIndex
        {
            get { return _selectedTileLayerIndex; }
            set
            {
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
                    default: break;
                }
            }
        }

        private UserControl _rightPanelItem;
        private UserControl _lastRightPanelItem;
        public UserControl RightPanelItem
        {
            get { return _rightPanelItem; }
            set { 
                SetProperty(ref _rightPanelItem, value);
                //LastRightPanelItem = value;
            }
        }
        public UserControl LastRightPanelItem
        {
            get { return _lastRightPanelItem; }
            set { SetProperty(ref _lastRightPanelItem, value); }
        }

        private string _mapName;
        public string MapName
        {
            get { return _mapName; }
            set { SetProperty(ref _mapName, value); }
        }

        private string _signatureFile;
        public string SignatureFile
        {
            get { return _signatureFile; }
            set { SetProperty(ref _signatureFile, value); }
        }

        private int _fromLayerIndex;
        public int FromLayerIndex
        {
            get { return _fromLayerIndex; }
            set { SetProperty(ref _fromLayerIndex, value); }
        }

        private int _toLayerIndex;
        public int ToLayerIndex
        {
            get { return _toLayerIndex; }
            set { SetProperty(ref _toLayerIndex, value); }
        }


        public RelayCommand AddNewRegionCommand { get; set; }
        public RelayCommand DownloadRegionCommand { get; set; }
        public RelayCommand DeleteRegionCommand { get; set; }
        public RelayCommand SaveRegionCommand { get; set; }

        public RelayCommand DownloadCommand { get; set; }
        public RelayCommand CancelDownloadCommand { get; set; }

        private DispatcherTimer _updateLayoutTimer;

        public MainViewModel(IOptions<AppSettings> options)
        {
            //load settings
            _settings = options.Value;
            MapName = _settings.MapCacheNameDefault;
            SignatureFile = _settings.MapCacheSignatureDefault;
            ListLayerIndex = new ObservableCollection<int> { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };
            FromLayerIndex = 1;
            ToLayerIndex = 2;
            RightPanelItem = null;
            LastRightPanelItem = null;
            ListRightPanels.Add("InfoPanel", new PolygonInfoPanel() as UserControl); //0
            ListRightPanels.Add("DownloadPanel", new DownloadPanel() as UserControl); //1

            _tileDownload = App.ServiceProvider.GetRequiredService<ITileDownload>();
            //RightPanelItem = ListRightPanels[0];
            _mapViewModel = App.ServiceProvider.GetRequiredService<MapViewModel>();
            TileLayers = new ObservableCollection<TileLayer>();
            foreach (TileLayer item in _mapViewModel.ServerTileLayer.TileLayers)
            {
                TileLayers.Add(item);
            }
            AppMode = AppMode.Normal;

            //command register
            AddNewRegionCommand = new RelayCommand(HandleAppModeChanged, (obj) => { return AppMode != AppMode.NewPolygon; });
            DeleteRegionCommand = new RelayCommand(HandleAppModeChanged);
            SaveRegionCommand = new RelayCommand(HandleAppModeChanged);
            DownloadRegionCommand = new RelayCommand(HandleAppModeChanged, (obj) => { return AppMode != AppMode.DownloadMap; });
            DownloadCommand = new RelayCommand(HandleDownloadCommand, (obj) => {return !_isDownloading;});
            CancelDownloadCommand = new RelayCommand(HandleCancelDownloadCommand, (obj) => { return _isDownloading; });

            //backgroundWorker downloadMap
            _bgwDownloadMap = new BackgroundWorker();
            _bgwDownloadMap.WorkerSupportsCancellation = true;
            _bgwDownloadMap.WorkerReportsProgress = true;
            _bgwDownloadMap.DoWork += _bgwDownloadMap_DoWork;
            _bgwDownloadMap.ProgressChanged += _bgwDownloadMap_ProgressChanged;
            _bgwDownloadMap.RunWorkerCompleted += _bgwDownloadMap_RunWorkerCompleted;

            _updateLayoutTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _updateLayoutTimer.Tick += (s, e) =>
            {

            };
            _updateLayoutTimer.Start();
        }

        private void _bgwDownloadMap_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsDownloading = false;
            CommandManager.InvalidateRequerySuggested();
        }


        //download backgroundworker
        private void _bgwDownloadMap_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
          
        }

        private long _totalFileDownload = 0;
        private long _fileDownloaded = 0;
        private List<KeyValuePair<string, Uri>> _queueImageLinks = new List<KeyValuePair<string, Uri>>();
        private void _bgwDownloadMap_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _fileDownloaded = 0;
                _totalFileDownload = 0;
                _queueImageLinks.Clear();
                string uriFormat = string.Empty;
                var dispatcher = _mapViewModel.MapResource.Dispatcher;

                string mapName = this.MapName;
                string signatureFile = this.SignatureFile;
                double fromLat = this._mapViewModel.SelectedRegion != null ? this._mapViewModel.SelectedRegion.Y1 : 0;
                double fromLon = this._mapViewModel.SelectedRegion != null ? this._mapViewModel.SelectedRegion.X1 : 0;
                double toLat = this._mapViewModel.SelectedRegion != null ? this._mapViewModel.SelectedRegion.Y2 : 0;
                double toLon = this._mapViewModel.SelectedRegion != null ? this._mapViewModel.SelectedRegion.X2 : 0;

                int fromLayer = this.ListLayerIndex[this.FromLayerIndex];
                int toLayer = this.ListLayerIndex[this.ToLayerIndex];

                IsDownloading = true;
                CommandManager.InvalidateRequerySuggested();
                for (int z = fromLayer; z <= toLayer; z++)
                {
                    double fromX, fromY, toX, toY;
                    LocationConvert.ConverLatLontoXY(z, fromLat, fromLon, out fromX, out fromY);
                    LocationConvert.ConverLatLontoXY(z, toLat, toLon, out toX, out toY);

                    double offsetFromX = Math.Min(fromX, toX) - (int)Math.Min(fromX, toX);
                    double offsetFromY = Math.Min(fromY, toY) - (int)Math.Min(fromY, toY);
                    double offsetToX = Math.Max(fromX, toX) - (int)Math.Max(fromX, toX);
                    double offsetToY = Math.Max(fromY, toY) - (int)Math.Max(fromY, toY);
                    int fromXInt = (int)Math.Min(fromX, toX);
                    int fromYInt = (int)Math.Min(fromY, toY);
                    int toXInt = offsetToX > 0 ? (int)Math.Max(fromX, toX) + 1 : (int)Math.Max(fromX, toX);
                    int toYInt = offsetToY > 0 ? (int)Math.Max(fromY, toY) + 1 : (int)Math.Max(fromY, toY);
                    _totalFileDownload += ((Int64)(toXInt - fromXInt + 1)) * ((Int64)(toYInt - fromYInt + 1));
                }
                int j = 0;
                for (int z = fromLayer; z <= toLayer; z++)
                {
                    double fromX, fromY, toX, toY;
                    LocationConvert.ConverLatLontoXY(z, fromLat, fromLon, out fromX, out fromY);
                    LocationConvert.ConverLatLontoXY(z, toLat, toLon, out toX, out toY);

                    double offsetFromX = Math.Min(fromX, toX) - (int)Math.Min(fromX, toX);
                    double offsetFromY = Math.Min(fromY, toY) - (int)Math.Min(fromY, toY);
                    double offsetToX = Math.Max(fromX, toX) - (int)Math.Max(fromX, toX);
                    double offsetToY = Math.Max(fromY, toY) - (int)Math.Max(fromY, toY);
                    int fromXInt = (int)Math.Min(fromX, toX);
                    int fromYInt = (int)Math.Min(fromY, toY);
                    int toXInt = offsetToX > 0 ? (int)Math.Max(fromX, toX) + 1 : (int)Math.Max(fromX, toX);
                    int toYInt = offsetToY > 0 ? (int)Math.Max(fromY, toY) + 1 : (int)Math.Max(fromY, toY);

                    for (int x = fromXInt; x <= toXInt; x++)
                        for (int y = fromYInt; y <= toYInt; y++)
                        {
                            if (!_bgwDownloadMap.CancellationPending)
                            {
                                Uri url = null;
                                dispatcher.Invoke(new Action(() =>
                                {
                                    url = _mapViewModel.MapResource.TileSource.GetUri(x, y, z);
                                }));

                                string imgName = string.Format("{0}-{1}-{2}.png", x, y, z);
                                _queueImageLinks.Add(new KeyValuePair<string, Uri>(imgName, url));
                                if (_queueImageLinks.Count > 50 || _totalFileDownload - _fileDownloaded < 50)
                                {
                                    downloadListImage(_queueImageLinks);
                                    _queueImageLinks.Clear();
                                }
                                //ImageSource imageSource = _tileDownload.Download(url);
                                //var bitmap = imageSource as BitmapSource;
                                //_fileDownloaded++;
                                //int percentDownloaded = _totalFileDownload > 0 ? (int)(_fileDownloaded * 100 / _totalFileDownload) : 0;
                                //PercentDownloading = percentDownloaded;
                            }
                        }
                }
                while (_percentDownloading < 100 && !_bgwDownloadMap.CancellationPending) Thread.Sleep(100);
            }
            finally
            {
            
            }
        }

        public void downloadListImage(List<KeyValuePair<string, Uri>> listLinks)
        {
            try
            {
                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = 50;
                //po.CancellationToken = cts.Token;
                int i = 0;
                Parallel.ForEach<KeyValuePair<string, Uri>>(listLinks, po, (site, state, index) =>
                {
                    if (!_bgwDownloadMap.CancellationPending)
                    {
                        KeyValuePair<string, Uri> item = site;
                        //lock (new object())
                        //{
                        string name = item.Key;
                        Uri url = item.Value;
                        ImageSource imageSource = _tileDownload.Download(url);
                        _fileDownloaded++;
                        int percentDownloaded = _totalFileDownload > 0 ? (int)(_fileDownloaded * 100 / _totalFileDownload) : 0;
                        PercentDownloading = percentDownloaded;
                    }
                });

            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
              
                //cts.Dispose();
            }

        }
        /// <summary>
        /// Handle add new polygon
        /// </summary>
        /// <param name="obj"></param>
        private void HandleAppModeChanged(object obj)
        {
            if (obj is string)
            {
                switch (obj as string)
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
            this._bgwDownloadMap.RunWorkerAsync();
        }

        /// <summary>
        /// Handle button CancelDownload
        /// </summary>
        private void HandleCancelDownloadCommand(object obj)
        {
            this._bgwDownloadMap.CancelAsync();
           
        }

        public void UpdateSelectedMapSource()
        {
            _mapViewModel.MapResource = SelectedTileLayer;
        }
    }
}
