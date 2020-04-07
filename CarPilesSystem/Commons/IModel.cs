using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace CarPilesSystem.Commons
{
    /// <summary>
    /// SQLite 数据库实体模型
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// 实体对应的表名
        /// </summary>
        [JsonIgnore]
        [ScriptIgnore]
        string _TableName { get; }
    }
}