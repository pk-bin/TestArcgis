using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace TestArcgis.Common
{
    public class LocationRestAPI
    {
        private static readonly string mVworldURL = "http://api.vworld.kr/req/search?service=search&request=search&version=2.0&crs=EPSG:4326";
        //3개월마다 갱신됨. 추후 회사명 구독하고 영구적으로 받도록 해야함.

        private static readonly string mVworldAPIKey = "CCED653C-0097-3563-AF9D-38EF084E13D8";

        private static readonly string mBaroEMapURL = "https://map.ngii.go.kr/openapi/search.json?";
        //제한 없음. 다만 상용되는지는 확인 필요.(개인용은 무제한)
        private static readonly string mBaroEMapAPIKey = "A6482EDB327B8D118018BDC7C0933EC4";
        private static readonly string mBaroEMapReferneceURL = "http://localhost:7070";

        private static readonly string mJusoURL = "https://www.juso.go.kr/addrlink/addrEngApi.do?resultType=json";
        //기간 90일 (2022.04.16 ~ 2022.07.15). 추후 회사명 구독하고 영구적으로 받도록 해야함.
        private static readonly string mJusoAPIKey = "devU01TX0FVVEgyMDIyMDQxNjExMzI0MTExMjQ2OTY=";

        /// <summary>
        /// Vworld에서 장소 요청시 들어오는 각종 데이터들
        /// </summary>
        /// <returns>Json 파싱된 장소값들</returns>
        public static Model.VworldPlacePoi GeVWorldAddressList(string pAddress, Xamarin.Essentials.Location pLocation, int pPageIndex = 1, int pPageSize = 10)
        {
            StringBuilder mHttpURLBuilder = new StringBuilder();
            mHttpURLBuilder.Clear();
            mHttpURLBuilder.AppendFormat("{0}&key={1}&size={2}&page={3}&query={4}&type=place&format=json&bbox={5}"
                , mVworldURL
                , mVworldAPIKey
                , pPageSize
                , pPageIndex
                , pAddress
                , string.Format("{0},{1},{2},{3}", pLocation.Longitude - 1.0, pLocation.Latitude - 1.0, pLocation.Longitude + 1.0, pLocation.Latitude + 1.0));
            HttpClient _Client = new HttpClient();
            string _JsonString = _Client.GetStringAsync(mHttpURLBuilder.ToString()).GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<Model.VworldPlacePoi>(_JsonString);
        }

        /// <summary>
        /// Vworld에서 장소 요청시 들어오는 각종 데이터들
        /// </summary>
        /// <returns>Json 파싱된 장소값들</returns>
        public static Model.VworldPlacePoi GeVWorldAddressList(string pAddress, int pPageIndex = 1, int pPageSize = 10)
        {
            StringBuilder mHttpURLBuilder = new StringBuilder();
            mHttpURLBuilder.Clear();
            mHttpURLBuilder.AppendFormat("{0}&key={1}&size={2}&page={3}&query={4}&type=place&format=json"
                , mVworldURL
                , mVworldAPIKey
                , pPageSize
                , pPageIndex
                , pAddress);
            HttpClient _Client = new HttpClient();
            string _JsonString = _Client.GetStringAsync(mHttpURLBuilder.ToString()).GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<Model.VworldPlacePoi>(_JsonString);
        }
    }
}
