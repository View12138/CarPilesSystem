namespace CarPilesSystem.Commons
{
    /// <summary>
    /// 表示度分秒
    /// </summary>
    public class DMS
    {
        /// <summary>
        /// 初始化一个经纬度对象
        /// </summary>
        public DMS() : this(0) { }
        /// <summary>
        /// 使用指定的十进制数的字符串初始化一个经纬度对象
        /// </summary>
        /// <param name="lngLat"></param>
        public DMS(string lngLat) : this(double.Parse(lngLat)) { }
        /// <summary>
        /// 使用指定的十进制数初始化一个经纬度对象
        /// </summary>
        /// <param name="lngLat"></param>
        public DMS(double lngLat)
        {
            Degree = (int)lngLat;
            double _minute = (lngLat - Degree) * 60;
            Minute = (int)_minute;
            Second = (_minute - Minute) * 60;
        }
        /// <summary>
        /// 度
        /// </summary>
        public int Degree { get; set; }
        /// <summary>
        /// 分
        /// </summary>
        public int Minute { get; set; }
        /// <summary>
        /// 秒
        /// </summary>
        public double Second { get; set; }

        /// <summary>
        /// 转换为十进制
        /// </summary>
        /// <returns></returns>
        public double ToDouble()
        {
            return Degree + Minute / 60d + Second / 3600;
        }

        /// <summary>
        /// 转换为十进制的字符串表达形式
        /// <para>默认保留6为小数</para>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToDouble().ToString("#.######");
        /// <summary>
        /// 转换为十进制的字符串表达形式
        /// <para>自定义格式化字符串</para>
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <returns></returns>
        public string ToString(string format) => ToDouble().ToString(format);
    }
}