using System.Web.Script.Serialization;
using Newtonsoft.Json;
using View.SQLite.Handle;

namespace CarPilesSystem.Models.Imodels
{
    /// <summary>
    /// cps_user 表对应的实体
    /// </summary>
    public class User : IModel
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
}