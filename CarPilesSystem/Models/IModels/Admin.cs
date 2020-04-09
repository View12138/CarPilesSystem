using System.Web.Script.Serialization;
using Newtonsoft.Json;
using View.SQLite.Handle;

namespace CarPilesSystem.Models.Imodels
{
    public class Admin : IModel
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