using MapControl;
using MapDownloader.Enums;
using MapDownloader.Models;
using MapDownloader.Services.Commands;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapDownloader.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        private ServerTileLayer _serverTileLayer = new ServerTileLayer();

        public ServerTileLayer ServerTileLayer { get { return _serverTileLayer; } }

        private readonly AppSettings _settings;

        private TileLayer _mapResource;
        public TileLayer MapResource
        {
            get
            {
                return _mapResource;// _serverTileLayer.TileLayers.FirstOrDefault(x => x.SourceName == _settings.MapProviderName);
            }
            set
            {
                SetProperty(ref _mapResource, value);
            }
        }

        private Double _zoom = 17.5;
        public Double Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                SetProperty(ref _zoom, value);
            }
        }

        private Location _mapCenter;
        public Location MapCenter
        {
            get
            {
                return _mapCenter;
            }
            set
            {
                SetProperty(ref _mapCenter, value);
            }
        }
        private Location _mouseMovePosition;
        public Location MouseMovePosition
        {
            get { return _mouseMovePosition; }
            set
            {
                SetProperty(ref _mouseMovePosition, value);
            }
        }

        private Rectangle _selectedRegion;
        public Rectangle SelectedRegion
        {
            get { return _selectedRegion; }
            set
            {
                SetProperty(ref _selectedRegion, value);
            }
        }

        private bool _isMouseMoveEnable = true;
        public bool IsMouseMoveEnable
        {
            get { return _isMouseMoveEnable; }
            set {
                SetProperty(ref _isMouseMoveEnable, value); 
            }
        }

        private string _mapCursor;
        public string MapCursor
        {
            get { return _mapCursor; }
            set
            {
                SetProperty(ref _mapCursor, value);
            }
        }

        private MapMode _mapMode;
        public MapMode MapMode
        {
            get { return _mapMode; }
            set
            {
                SetProperty(ref _mapMode, value);
                IsMouseMoveEnable = (_mapMode == MapMode.MoveMode) ? true : false;
                MapCursor = (_mapMode == MapMode.MoveMode) ? "Arrow" : "Cross";
            }
        }

        //polylines layer
        public ObservableCollection<Polyline> Polylines { get; set; }

        //rectangles layer
        public ObservableCollection<Rectangle> Rectangles { get; set; }


        public void UpdateSelectedPolygon(Location p1, Location p2)
        {
            int selectedRegionId = Rectangles.IndexOf(SelectedRegion);
            //if (p1.Longitude < p2.Longitude && p1.Latitude > p2.Latitude)
            {
                SelectedRegion = new Rectangle() { Name = "SelectedRegion",
                    X1 = Math.Min(p1.Longitude, p2.Longitude), 
                    Y1 = Math.Max(p1.Latitude, p2.Latitude),
                    X2 = Math.Max(p1.Longitude, p2.Longitude),
                    Y2 = Math.Min(p1.Latitude, p2.Latitude)
                };
            }
            Rectangles[selectedRegionId] = SelectedRegion;
            //Rectangles.Add(new Rectangle());
        }

        public MapViewModel(IOptions<AppSettings> options)
        {
            _settings = options.Value;
            MapCenter = new Location(21.0326731, 105.4937939);
            MapResource = _serverTileLayer.TileLayers.FirstOrDefault(x => x.SourceName == _settings.MapProviderName);
            //MapResource = _serverTileLayer.TileLayers[2];

            //add sample polyline
            List<Location> polyline = new List<Location>();
            polyline.Add(new Location(21, 105));
            polyline.Add(new Location(21.1, 105.1));
            polyline.Add(new Location(21.3, 105.6));
            Polylines = new ObservableCollection<Polyline>();
            Polylines.Add(new Polyline { Name = "polyline1", Locations = new LocationCollection(polyline)});

            //add sample rectangle
            SelectedRegion = new Rectangle() {Name="SelectedRegion", X1 = 105, Y1 = 21.4, X2 = 105.2, Y2 = 21.0 };
            Rectangles = new ObservableCollection<Rectangle>();
            Rectangles.Add(SelectedRegion);

        }

    }
}
