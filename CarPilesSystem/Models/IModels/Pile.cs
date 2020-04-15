using System.Web.Script.Serialization;
using Newtonsoft.Json;
using View.SQLite.Handle;

namespace CarPilesSystem.Models.IModels
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
        /// <summary>
        /// 充电桩状态
        /// <para>0:空闲可用；1:已被使用；2:已被预约</para>
        /// </summary>
        public long State { get; set; }
        /// <summary>
        /// 预约开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 预约结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 正在使用或预约充电桩的用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public string Price { get; set; }
    }
}