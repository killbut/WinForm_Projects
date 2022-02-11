using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Microsoft.SqlServer.Types;
using System.Data.SqlClient;

namespace TZ_1
{
    public class MarkerClass : GMapMarker
    {
        private int _id;
        private GMapMarker _marker;
        public GMapMarker Marker { get { return _marker; } set { _marker = value; } }
        public int ID { get { return _id; } set { _id = value; } }
        public Dictionary<int, GMapMarker> IdCoor { get; set; } = new Dictionary<int, GMapMarker>();

        public MarkerClass(PointLatLng pos) : base(pos)
        {
        }
    }
}
