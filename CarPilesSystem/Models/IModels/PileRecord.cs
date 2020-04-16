using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using View.SQLite.Handle;

namespace CarPilesSystem.Models.IModels
{
    public class PileRecord:IModel
    {
        [JsonIgnore]
        [ScriptIgnore]
        public string _TableName => "cps_pile_record";
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// 用户 Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 消费金额
        /// </summary>
        public string Money { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 支付状态
        /// <para>0：未支付，1：已支付</para>
        /// </summary>
        public long State { get; set; }
        /// <summary>
        /// 充电桩 Id
        /// </summary>
        public long PileId { get; set; }
    }
}