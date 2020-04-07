using System;

namespace CarPilesSystem.Models
{
    /// <summary>
    /// 标记此属性为主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute { }
    /// <summary>
    /// 数据库实体接口
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// 实体对应的表名
        /// </summary>
        string _TableName { get; }
    }
    /// <summary>
    /// cps_user 表对应的实体
    /// </summary>
    public class Cps_User : IModel
    {
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

    public class Cps_Pile : IModel
    {
        public string _TableName => "cps_pile";
        [Key]
        public long Id { get; set; }
        public string LocationX { get; set; }
        public string LocationY { get; set; }
        public string Name { get; set; }
    }

    public class Cps_Admin : IModel
    {
        public string _TableName => "cps_admin";
        [Key]
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}