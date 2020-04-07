using System.Web.Script.Serialization;
using CarPilesSystem.Commons;
using Newtonsoft.Json;

namespace CarPilesSystem.Models
{
    /// <summary>
    /// cps_user 表对应的实体
    /// </summary>
    public class Cps_User : IModel
    {
        [JsonIgnore]
        [ScriptIgnore]
        public string _TableName => "cps_user";
        [Key]
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Sex { get; set; }
        public long Age { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// 充电桩模型
    /// </summary>
    public class Cps_Pile : IModel
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
        /// X坐标 - 经度
        /// </summary>
        public string LocationX { get; set; }
        /// <summary>
        /// Y坐标 - 维度
        /// </summary>
        public string LocationY { get; set; }
        /// <summary>
        /// 充电桩标题
        /// </summary>
        public string Name { get; set; }
    }

    public class Cps_Admin : IModel
    {
        [JsonIgnore]
        [ScriptIgnore]
        public string _TableName => "cps_admin";
        [Key]
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}