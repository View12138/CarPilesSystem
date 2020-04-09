using System.Web.Script.Serialization;
using Newtonsoft.Json;
using View.SQLite.Handle;

namespace CarPilesSystem.Models.Imodels
{
    /// <summary>
    /// 充电桩模型
    /// </summary>
    public class Pile : IModel
    {
        [JsonIgnore]
        [ScriptIgnore]
        public string _TableName => "cps_pile";
        /// <summary>
        /// Id 主键
        /// </summary>
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public string Longitude { get; set; }
        /// <summary>
        /// 维度
        /// </summary>
        public string Latitude { get; set; }
        /// <summary>
        /// 充电桩标题
        /// </summary>
        public string Name { get; set; }
    }
}