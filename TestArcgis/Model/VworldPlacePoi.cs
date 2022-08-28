using System.Collections.Generic;

namespace TestArcgis.Model
{
    public class VworldPlacePoi
    {
        public Response response { get; set; }

        public class Address
        {
            public string road { get; set; }
            public string parcel { get; set; }
        }

        public class PoiItem
        {
            public string id { get; set; }
            public string title { get; set; }
            public string category { get; set; }
            public Address address { get; set; }
            public Point point { get; set; }
        }

        public class Page
        {
            public string total { get; set; }
            public string current { get; set; }
            public string size { get; set; }
        }

        public class Point
        {
            public string x { get; set; }
            public string y { get; set; }
        }

        public class Record
        {
            public string total { get; set; }
            public string current { get; set; }
        }

        public class Response
        {
            public Service service { get; set; }
            public string status { get; set; }
            public Record record { get; set; }
            public Page page { get; set; }
            public Result result { get; set; }
        }

        public class Result
        {
            public string crs { get; set; }
            public string type { get; set; }
            public List<PoiItem> items { get; set; }
        }

        public class Root
        {
            public Response response { get; set; }
        }

        public class Service
        {
            public string name { get; set; }
            public string version { get; set; }
            public string operation { get; set; }
            public string time { get; set; }
        }
    }
}
