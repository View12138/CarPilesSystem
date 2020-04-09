using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarPilesSystem.Commons
{
    /// <summary>
    /// 表示经纬度
    /// </summary>
    public class LngLat
    {
        public LngLat() : this(0, 0) { }
        public LngLat(double longitude, double latitude)
        {
            Longitude = new DMS(longitude);
            Latitude = new DMS(latitude);
        }
        public LngLat(string longitude, string latitude) : this(double.Parse(longitude), double.Parse(latitude)) { }
        /// <summary>
        /// 经度
        /// </summary>
        public DMS Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public DMS Latitude { get; set; }

        /// <summary>
        /// X坐标，
        /// </summary>
        [Obsolete]
        public double X
        {
            get;set;
        }
        /// <summary>
        /// Y 坐标，
        /// </summary>
        [Obsolete]
        public double Y
        {
            get;set;
        }

        // static
        /// <summary>
        /// 纬度距转米
        /// </summary>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static double LatToMeter(double lat)
        {
            return lat * 111000;
        }
        /// <summary>
        /// 米转纬度距
        /// </summary>
        /// <param name="meter"></param>
        /// <returns></returns>
        public static double MeterToLat(double meter)
        {
            return meter / 111000d;
        }
        /// <summary>
        /// 经度距转米
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static double LngToMeter(double lng, double lat)
        {
            return lng * 111000 * Math.Cos(Math.Abs(lat));
        }
        /// <summary>
        /// 米转纬度距
        /// </summary>
        /// <param name="meter"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static double MeterToLng(double meter, double lat)
        {
            return meter / (111000d * Math.Cos(Math.Abs(lat)));
        }
    }
}