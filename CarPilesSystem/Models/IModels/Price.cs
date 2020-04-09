using System.Web.Script.Serialization;
using Newtonsoft.Json;
using View.SQLite.Handle;

namespace CarPilesSystem.Models.Imodels
{
    /// <summary>
    /// 定价表
    /// </summary>
    public class Price : IModel
    {
        [JsonIgnore]
        [ScriptIgnore]
        public string _TableName => "cps_pricing";
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// 时间段开始
        /// </summary>
        public long Min { get; set; }
        /// <summary>
        /// 时间段结束
        /// </summary>
        public long Max { get; set; }
        /// <summary>
        /// 收费
        /// </summary>
        public double Money { get; set; }
    }
}