using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Microsoft.SqlServer.Types;
using System.Data.SqlClient;

namespace TZ_1
{
    public partial class Form1 : Form
    {

        private GMapOverlay databaseMarkers = new GMapOverlay(nameof(databaseMarkers));
        private bool isLeftButtonDown = false;

        MarkerClass Markers = new MarkerClass(new PointLatLng(0, 0));

        private string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}\Database.mdf;Integrated Security=True";
        public Form1()
        {
            InitializeComponent();
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                Connection.Open();
                this.toolStrip1.Items[0].Text = "Connected";
                SqlCommand command = new SqlCommand();
                command.CommandText = "SELECT ID,Coordinates FROM Locations";
                command.Connection = Connection;
                SqlDataReader b = command.ExecuteReader();
                if (b.HasRows)
                {
                    while (b.Read())
                    {
                        Markers.ID = (int)b.GetInt32(0);
                        AddMarkers((SqlGeography)b.GetValue(1));
                    }
                    this.gMapControl1.Update();
                }
            }
        }
        //Загрузка карты
        private void gMapControl1_Load(object sender, EventArgs e)
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache; //выбор подгрузки карты – онлайн или из ресурсов
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance; //какой провайдер карт используется (в нашем случае гугл) 
            gMapControl1.Dock = DockStyle.Fill;
            gMapControl1.MinZoom = 2; //минимальный зум
            gMapControl1.MaxZoom = 20; //максимальный зум
            gMapControl1.Zoom = 15; // какой используется зум при открытии
            gMapControl1.Position = new GMap.NET.PointLatLng(54.98617071740424, 82.89079118980577);// точка в центре карты при открытии (центр России)
            gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter; // как приближает (просто в центр карты или по положению мыши)
            gMapControl1.CanDragMap = true; // перетаскивание карты мышью
            gMapControl1.DragButton = MouseButtons.Left; // какой кнопкой осуществляется перетаскивание
            gMapControl1.ShowCenter = false; //показывать или скрывать красный крестик в центре
            gMapControl1.ShowTileGridLines = false; //показывать или скрывать тайлы

            gMapControl1.MouseMove += GMapControl1_MouseMove;
            gMapControl1.MouseDown += GMapControl1_MouseDown;
            gMapControl1.MouseUp += GMapControl1_MouseUp;
        }
        // добавление маркера
        private void AddMarkers(SqlGeography asd)
        {
            PointLatLng p = new PointLatLng();
            p.Lat = (double)asd.Lat;
            p.Lng = (double)asd.Long;
            Markers.Marker = new GMarkerGoogle(p, GMarkerGoogleType.blue_pushpin);
            databaseMarkers.Markers.Add(Markers.Marker);
            Markers.IdCoor.Add(Markers.ID, Markers.Marker);
            this.gMapControl1.Overlays.Add(databaseMarkers);
        }


        // Drag&Drop
        private void GMapControl1_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isLeftButtonDown = false;
        }
        private void GMapControl1_MouseDown(object? sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
                isLeftButtonDown = true;
        }
        private void GMapControl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isLeftButtonDown)
            {
                if (Markers.Marker != null)
                {
                    PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                    Markers.Marker.Position = point;
                    Markers.Marker.ToolTipText = String.Format("{0} {1}", point.Lat.ToString("0.000"), point.Lng.ToString("0.000"));

                    string sqlExpression = "UPDATE Locations SET Coordinates=@coordinates WHERE ID=@id";
                    var coord = SqlGeography.Point(Markers.Marker.Position.Lat, Markers.Marker.Position.Lng, 4326);
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand();
                        var sqlTypeGeography = new SqlParameter("@coordinates", coord) { UdtTypeName = "Geography" };
                        var sqlTypeId = new SqlParameter("@id", Markers.ID);
                        command.Parameters.Add(sqlTypeGeography);
                        command.Parameters.Add(sqlTypeId);
                        command.Connection = connection;
                        command.CommandText = sqlExpression;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        private void gMapControl1_OnMarkerEnter(GMapMarker item)
        {
            Markers.Marker = item;
            Markers.ID = Markers.IdCoor.FirstOrDefault(x => x.Value == item).Key;
        }
        private void gMapControl1_OnMarkerLeave(GMapMarker item)
        {
            Markers.Marker = null;
        }

    }
}