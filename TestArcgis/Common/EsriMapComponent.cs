using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TestArcgis.Common
{
    public class EsriMapComponent
    {
        private readonly int mReRoutedTimeLimit = 15;
        private readonly int mGPSEPSG = 4326;
        private readonly string mLocateURI = "https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer";
        private readonly string mVWorldURI = "http://api.vworld.kr/req/wmts/1.0.0/CCED653C-0097-3563-AF9D-38EF084E13D8/WMTSCapabilities.xml";
        private readonly string mBaroEMapURI = "http://210.117.198.32:6080/arcgis/rest/services/NGII_BaseMAP/MapServer/WMTS/1.0.0/WMTSCapabilities.xml";

        private Esri.ArcGISRuntime.Xamarin.Forms.MapView mEsriMapview;
        //Overlay
        private Esri.ArcGISRuntime.UI.GraphicsOverlayCollection mGraphicsOverlayCollection;
        //Grapic
        private Esri.ArcGISRuntime.UI.Graphic mRouteAheadGraphic;
        private Esri.ArcGISRuntime.UI.Graphic mRouteTraveledGraphic;
        private Esri.ArcGISRuntime.UI.Graphic mRoutedEndGraphic;
        //Routed
        private readonly Uri mRoutingURI = new Uri("https://route-api.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World");
        private Esri.ArcGISRuntime.Tasks.NetworkAnalysis.RouteTask mRouteTask;
        private Esri.ArcGISRuntime.Tasks.NetworkAnalysis.RouteParameters mRouteParams;
        private Esri.ArcGISRuntime.Tasks.NetworkAnalysis.RouteResult mRouteResult;
        private Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Route mRouted;
        private Esri.ArcGISRuntime.Navigation.RouteTracker mRouteTracker;
        private IReadOnlyList<Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuver> mDirectionsList;
        private Action<int> mReRoutedCallback = null;
        private bool mRoutedFlag = false;
        private bool mReRoutedSupported = false;
        private bool mIsOnRouted = true;
        private int mReRoutedCount = 0;


        private bool mAutoLocateFlag = false;
        private Esri.ArcGISRuntime.Geometry.MapPoint mLastMapPosition = null;
        private Esri.ArcGISRuntime.Geometry.MapPoint mLastDestinationMapPosition = null;

        public EsriMapComponent(bool pRouted = false)
        {
            mEsriMapview = new Esri.ArcGISRuntime.Xamarin.Forms.MapView();
            mGraphicsOverlayCollection = new Esri.ArcGISRuntime.UI.GraphicsOverlayCollection
            {
                new Esri.ArcGISRuntime.UI.GraphicsOverlay(),
            };
            mEsriMapview.GraphicsOverlays = mGraphicsOverlayCollection;
            mRoutedFlag = pRouted;
        }

        public Esri.ArcGISRuntime.Xamarin.Forms.MapView GetMapView()
        {
            return mEsriMapview;
        }

        public async void CreateMap(string pIdentifier = "Base")
        {
            var _Map = new Esri.ArcGISRuntime.Mapping.Map(Esri.ArcGISRuntime.Mapping.BasemapStyle.ArcGISNavigation);
            Esri.ArcGISRuntime.Mapping.WmtsLayer _WmtsLayer = new Esri.ArcGISRuntime.Mapping.WmtsLayer(new Uri(mVWorldURI), pIdentifier);
            await _WmtsLayer.LoadAsync();
            _Map.OperationalLayers.Add(_WmtsLayer);
            mEsriMapview.Map = _Map;

            if (mRoutedFlag)
            {
                InitRouted();
            }
            else
            {
                InitLocate();
            }
        }
        public async void SetMapViewpointCenterAsync(double pLatitude, double pLogitude)
        {
            Esri.ArcGISRuntime.Geometry.MapPoint mapCenterPoint = new Esri.ArcGISRuntime.Geometry.MapPoint(pLogitude, pLatitude, Esri.ArcGISRuntime.Geometry.SpatialReferences.Wgs84);
            await mEsriMapview.SetViewpointCenterAsync(pLatitude, pLogitude);
        }

        public async void SetMapViewpointCenterAsync(Esri.ArcGISRuntime.Geometry.MapPoint pMapPoint)
        {
            await mEsriMapview.SetViewpointCenterAsync(pMapPoint);
        }
        public void SetMapViewMapPosition(double pLatitude, double pLogitude, double pScale)
        {
            Esri.ArcGISRuntime.Geometry.MapPoint mapCenterPoint = new Esri.ArcGISRuntime.Geometry.MapPoint(pLogitude, pLatitude, Esri.ArcGISRuntime.Geometry.SpatialReferences.Wgs84);
            mEsriMapview.SetViewpoint(new Esri.ArcGISRuntime.Mapping.Viewpoint(mapCenterPoint, pScale));
        }
        public void SetMapViewMapPosition(Esri.ArcGISRuntime.Geometry.MapPoint pMapPoint, double pScale = 4000)
        {
            mEsriMapview.SetViewpoint(new Esri.ArcGISRuntime.Mapping.Viewpoint(pMapPoint, pScale));
        }
        public void SetMapViewMapPosition5179(double x, double y, double pScale = 4000)
        {
            var _ConvertMapPoint = ConvertCoordinateData(5179, mGPSEPSG, Convert.ToDouble(x), Convert.ToDouble(y));

            mEsriMapview.SetViewpoint(new Esri.ArcGISRuntime.Mapping.Viewpoint(_ConvertMapPoint, pScale));
        }
        public Esri.ArcGISRuntime.Geometry.MapPoint ConvertCoordinateData(int pSourceEPSG, int pDestinationEPSG, double pXvalue, double pYvalue)
        {
            Esri.ArcGISRuntime.Geometry.MapPoint _Source = new Esri.ArcGISRuntime.Geometry.MapPoint(pXvalue, pYvalue, new Esri.ArcGISRuntime.Geometry.SpatialReference(pSourceEPSG));
            Esri.ArcGISRuntime.Geometry.MapPoint _Destination = (Esri.ArcGISRuntime.Geometry.MapPoint)Esri.ArcGISRuntime.Geometry.GeometryEngine.Project(_Source, new Esri.ArcGISRuntime.Geometry.SpatialReference(pDestinationEPSG));

            return _Destination;
        }

        public Esri.ArcGISRuntime.Geometry.MapPoint GetWgs84MapPoint(double pLatitude, double pLogitude)
        {
            return new Esri.ArcGISRuntime.Geometry.MapPoint(pLogitude, pLatitude, Esri.ArcGISRuntime.Geometry.SpatialReferences.Wgs84);
        }

        private void InitLocate()
        {
            mEsriMapview.LocationDisplay.IsEnabled = true;
            mEsriMapview.LocationDisplay.AutoPanMode = Esri.ArcGISRuntime.UI.LocationDisplayAutoPanMode.Recenter;
            mEsriMapview.LocationDisplay.LocationChanged += LocationDisplay_LocationChanged;
            mEsriMapview.LocationDisplay.DataSource = Esri.ArcGISRuntime.Location.LocationDataSource.CreateDefault();
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                mEsriMapview.LocationDisplay.DataSource.StartAsync();
            });

            Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (mLastMapPosition != null)
                {
                    if (mAutoLocateFlag)
                    {
                        SetMapViewpointCenterAsync(mLastMapPosition);
                    }
                }
                return true;
            });
        }

        private async void InitRouted()
        {
            mRouteTask = await Esri.ArcGISRuntime.Tasks.NetworkAnalysis.RouteTask.CreateAsync(mRoutingURI);
            mRouteParams = await mRouteTask.CreateDefaultParametersAsync();

            mRouteParams.ReturnDirections = true;
            mRouteParams.ReturnStops = true;
            mRouteParams.ReturnRoutes = true;
            mRouteParams.OutputSpatialReference = Esri.ArcGISRuntime.Geometry.SpatialReferences.Wgs84;

            Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if(mIsOnRouted)
                {
                    mReRoutedCount = 0;
                }
                else
                {
                    mReRoutedCount++;
                    mReRoutedCallback(mReRoutedCount);
                }

                if(mReRoutedCount >= mReRoutedTimeLimit)
                {
                    mIsOnRouted = true;
                    mReRoutedCallback(100);
                }
                return true;
            });
        }

        public void SetAutoLocated(bool pAutoLocateFlag)
        {
            mAutoLocateFlag = pAutoLocateFlag;
        }

        private void LocationDisplay_LocationChanged(object sender, Esri.ArcGISRuntime.Location.Location e)
        {
            mLastMapPosition = e.Position;
        }

        public void GetReRoutedCallBack(Action<int> pReRoutedCallBack)
        {
            mReRoutedCallback = pReRoutedCallBack;
        }

        public void RoutedAddress(Esri.ArcGISRuntime.Geometry.MapPoint pDestination, Esri.ArcGISRuntime.Geometry.MapPoint pSource, Action<string> pCatchCallback)
        {
            mLastDestinationMapPosition = pDestination;

            DisableRoutedProcess();
            List<Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Stop> stopPoints = new List<Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Stop>();
            stopPoints.Add(new Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Stop(pSource));
            stopPoints.Add(new Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Stop(pDestination));
            RoutedProcess(stopPoints,pDestination, pCatchCallback);
        }

        private void DisableRoutedProcess()
        {
            try
            {
                mIsOnRouted = true;
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    Esri.ArcGISRuntime.UI.GraphicsOverlay _GrapicOverlay = this.mGraphicsOverlayCollection.FirstOrDefault();
                    if (mEsriMapview.LocationDisplay.IsEnabled)
                    {
                        mEsriMapview.LocationDisplay.DataSource.StopAsync();
                        mEsriMapview.LocationDisplay.IsEnabled = false;
                    }

                    if(mRouteAheadGraphic != null)
                    {
                        _GrapicOverlay.Graphics.Remove(mRouteAheadGraphic);
                    }
                    if (mRouteTraveledGraphic != null)
                    {
                        _GrapicOverlay.Graphics.Remove(mRouteTraveledGraphic);
                    }
                    if (mRoutedEndGraphic != null)
                    {
                        _GrapicOverlay.Graphics.Remove(mRoutedEndGraphic);
                    }
                });

                if (mRouteTracker != null)
                {
                    mRouteTracker.TrackingStatusChanged -= MRouteTracker_TrackingStatusChanged;
                    mRouteTracker.RerouteStarted -= MRouteTracker_RerouteStarted;
                    mRouteTracker.RerouteCompleted -= MRouteTracker_RerouteCompleted;
                    mRouteTracker = null;
                }
                mRouteParams.ClearStops();
            }
            catch
            {

            }
        }


        private async void RoutedProcess(List<Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Stop> pStopList, Esri.ArcGISRuntime.Geometry.MapPoint pDestination, Action<string> pCatchCallback)
        {
            try
            {
                if (mReRoutedCallback != null)
                {
                    mReRoutedCallback(-2);
                }
                Esri.ArcGISRuntime.UI.GraphicsOverlay _GrapicOverlay = this.mGraphicsOverlayCollection.FirstOrDefault();
                mRouteParams.ClearStops();
                mRouteParams.SetStops(pStopList);
                mRouteResult = await mRouteTask.SolveRouteAsync(mRouteParams);
                mRouted = mRouteResult.Routes[0];

                mRouteAheadGraphic = new Esri.ArcGISRuntime.UI.Graphic(mRouted.RouteGeometry)
                {
                    Symbol = new Esri.ArcGISRuntime.Symbology.SimpleLineSymbol(
                        Esri.ArcGISRuntime.Symbology.SimpleLineSymbolStyle.Dash,
                    Color.BlueViolet, 5)
                };

                mRouteTraveledGraphic = new Esri.ArcGISRuntime.UI.Graphic
                {
                    Symbol = new Esri.ArcGISRuntime.Symbology.SimpleLineSymbol(
                        Esri.ArcGISRuntime.Symbology.SimpleLineSymbolStyle.Solid,
                        Color.LightBlue, 3)
                };

                mRoutedEndGraphic = new Esri.ArcGISRuntime.UI.Graphic
                {
                    Geometry = pDestination,
                    Symbol = new Esri.ArcGISRuntime.Symbology.SimpleMarkerSymbol(Esri.ArcGISRuntime.Symbology.SimpleMarkerSymbolStyle.Square, Color.Red, 20),
                };

                _GrapicOverlay.Graphics.Add(mRouteAheadGraphic);
                _GrapicOverlay.Graphics.Add(mRouteTraveledGraphic);
                _GrapicOverlay.Graphics.Add(mRoutedEndGraphic);

                await mEsriMapview.SetViewpointGeometryAsync(mRouted.RouteGeometry, 100);

                mDirectionsList = mRouted.DirectionManeuvers;
                mRouteTracker = new Esri.ArcGISRuntime.Navigation.RouteTracker(mRouteResult, 0, true);
                mRouteTracker.TrackingStatusChanged += MRouteTracker_TrackingStatusChanged;

                if (mRouteTask.RouteTaskInfo.SupportsRerouting)
                {
                    mReRoutedSupported = true;
                    await mRouteTracker.EnableReroutingAsync(new Esri.ArcGISRuntime.Navigation.ReroutingParameters(mRouteTask, mRouteParams)
                    {
                        Strategy = Esri.ArcGISRuntime.Navigation.ReroutingStrategy.ToNextWaypoint,
                        VisitFirstStopOnStart = false
                    });

                    mRouteTracker.RerouteStarted += MRouteTracker_RerouteStarted;
                    mRouteTracker.RerouteCompleted += MRouteTracker_RerouteCompleted;
                }
                else
                {
                    pCatchCallback("Do not SupportsRerouting! Because ReRouted Service only works on local dataset.");
                }
                mEsriMapview.LocationDisplay.AutoPanMode = Esri.ArcGISRuntime.UI.LocationDisplayAutoPanMode.Navigation;
                mEsriMapview.LocationDisplay.DataSource = new RouteTrackerDisplayLocationDataSource(Esri.ArcGISRuntime.Location.LocationDataSource.CreateDefault(), mRouteTracker);
                mEsriMapview.LocationDisplay.IsEnabled = true;
            }
            catch(Exception ets)
            {
                pCatchCallback(ets.Message);
            }
            finally
            {
                if (mReRoutedCallback != null)
                {
                    mReRoutedCallback(-1);
                }
            }
        }

        private void MRouteTracker_RerouteStarted(object sender, EventArgs e)
        {
            if (mReRoutedCallback != null)
            {
                mReRoutedCallback(-2);
            }
            mRouteTracker.TrackingStatusChanged -= MRouteTracker_TrackingStatusChanged;
        }

        private void MRouteTracker_RerouteCompleted(object sender, Esri.ArcGISRuntime.Navigation.RouteTrackerRerouteCompletedEventArgs e)
        {
            if (mReRoutedCallback != null)
            {
                mReRoutedCallback(-1);
            }
            mDirectionsList = e.TrackingStatus.RouteResult.Routes[0].DirectionManeuvers;
            mRouteTracker.TrackingStatusChanged += MRouteTracker_TrackingStatusChanged;
        }

        private void MRouteTracker_TrackingStatusChanged(object sender, Esri.ArcGISRuntime.Navigation.RouteTrackerTrackingStatusChangedEventArgs e)
        {
            Esri.ArcGISRuntime.Navigation.TrackingStatus status = e.TrackingStatus;

            if(mReRoutedSupported)
            {
                if (status.IsOnRoute && !status.IsRouteCalculating)
                {
                    if (status.DestinationStatus == Esri.ArcGISRuntime.Navigation.DestinationStatus.NotReached || status.DestinationStatus == Esri.ArcGISRuntime.Navigation.DestinationStatus.Approaching)
                    {
                        mRouteAheadGraphic.Geometry = status.RouteProgress.RemainingGeometry;
                        mRouteTraveledGraphic.Geometry = status.RouteProgress.TraversedGeometry;
                    }
                    else if (status.DestinationStatus == Esri.ArcGISRuntime.Navigation.DestinationStatus.Reached)
                    {
                        mRouteAheadGraphic.Geometry = null;
                        mRoutedEndGraphic.Geometry = null;
                        mRouteTraveledGraphic.Geometry = status.RouteResult.Routes[0].RouteGeometry;

                        if (status.RemainingDestinationCount > 1)
                        {
                            mRouteTracker.SwitchToNextDestinationAsync();
                        }
                        else
                        {
                            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                            {
                                mEsriMapview.LocationDisplay.DataSource.StopAsync();
                            });
                        }

                    }
                }
            }
            else
            {
                if(status.IsOnRoute)
                {
                    mIsOnRouted = true;
                    if (status.DestinationStatus == Esri.ArcGISRuntime.Navigation.DestinationStatus.NotReached || status.DestinationStatus == Esri.ArcGISRuntime.Navigation.DestinationStatus.Approaching)
                    {
                        mRouteAheadGraphic.Geometry = status.RouteProgress.RemainingGeometry;
                        mRouteTraveledGraphic.Geometry = status.RouteProgress.TraversedGeometry;
                    }
                    else if (status.DestinationStatus == Esri.ArcGISRuntime.Navigation.DestinationStatus.Reached)
                    {
                        mRouteAheadGraphic.Geometry = null;
                        mRoutedEndGraphic.Geometry = null;
                        mRouteTraveledGraphic.Geometry = status.RouteResult.Routes[0].RouteGeometry;

                        if (status.RemainingDestinationCount > 1)
                        {
                            mRouteTracker.SwitchToNextDestinationAsync();
                        }
                        else
                        {
                            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                            {
                                mEsriMapview.LocationDisplay.DataSource.StopAsync();
                            });
                        }
                    }
                }
                else
                {
                    mIsOnRouted = false;
                }
            }
        }
    }
    public class RouteTrackerDisplayLocationDataSource : Esri.ArcGISRuntime.Location.LocationDataSource
    {
        private Esri.ArcGISRuntime.Location.LocationDataSource _inputDataSource;
        private Esri.ArcGISRuntime.Navigation.RouteTracker _routeTracker;

        public RouteTrackerDisplayLocationDataSource(Esri.ArcGISRuntime.Location.LocationDataSource dataSource, Esri.ArcGISRuntime.Navigation.RouteTracker routeTracker)
        {
            // Set the data source
            _inputDataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

            // Set the route tracker.
            _routeTracker = routeTracker ?? throw new ArgumentNullException(nameof(routeTracker));

            // Change the tracker location when the source location changes.
            _inputDataSource.LocationChanged += InputLocationChanged;

            // Update the location output when the tracker location updates.
            _routeTracker.TrackingStatusChanged += TrackingStatusChanged;
        }

        private void InputLocationChanged(object sender, Esri.ArcGISRuntime.Location.Location e)
        {
            // Update the tracker location with the new location from the source (simulation or GPS).
            _routeTracker.TrackLocationAsync(e);
        }

        private void TrackingStatusChanged(object sender, Esri.ArcGISRuntime.Navigation.RouteTrackerTrackingStatusChangedEventArgs e)
        {
            // Check if the tracking status has a location.
            if (e.TrackingStatus.DisplayLocation != null)
            {
                // Call the base method for LocationDataSource to update the location with the tracked (snapped to route) location.
                UpdateLocation(e.TrackingStatus.DisplayLocation);
            }
        }

        protected override System.Threading.Tasks.Task OnStartAsync() => _inputDataSource.StartAsync();

        protected override System.Threading.Tasks.Task OnStopAsync() => _inputDataSource.StartAsync();
    }
}
