using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using CarPilesSystem.Models;

namespace CarPilesSystem.Commons
{
    public class CpsController : Controller
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public virtual string DBName { get; set; } = $@"{AppDomain.CurrentDomain.BaseDirectory}Database\Test.db";
        /// <summary>
        /// 数据库密码
        /// </summary>
        public virtual string DBPassword { get; set; }
        /// <summary>
        /// 数据库版本
        /// </summary>
        public virtual string DBVersion { get; set; } = "3";
        /// <summary>
        /// 数据库连接
        /// </summary>
        public SQLiteConnection DBConnection { get; set; }

        /// <summary>
        /// 打开默认数据库连接。
        /// <list type="bullet">
        /// <item>如果使用此方法，则在查询完成后手动关闭数据库连接。</item>
        /// <item>如果未使用此方法，则查询方法自动关闭数据库连接。</item>
        /// </list>
        /// </summary>
        public void DBOpen() => DBOpen(DBName, DBPassword, DBVersion);
        /// <summary>
        /// 打开指定的数据库连接。
        /// <list type="bullet">
        /// <item>如果使用此方法，则在查询完成后手动关闭数据库连接。</item>
        /// <item>如果未使用此方法，则查询方法自动关闭数据库连接。</item>
        /// </list>
        /// </summary>
        /// <param name="dbName">数据库文件路径和名称</param>
        /// <param name="version">数据库版本</param>
        public void DBOpen(string dbName, string password, string version = "3")
        {
            string connectionString;
            if (string.IsNullOrEmpty(password))
            { connectionString = $@"Data Source={dbName};Version={version};"; }
            else
            { connectionString = $@"Data Source={dbName};Password={password};Version={version};"; }
            DBConnection = new SQLiteConnection(connectionString);
            DBConnection.Open();
        }
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void DBClose()
        {
            DBConnection.Close();
            DBConnection.Dispose();
            DBConnection = null;
        }

        /// <summary>
        /// 插入一个实体
        /// <para>适用于自增主键的模型</para>
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <returns></returns>
        public int DBInsert<T>(T model) where T : IModel, new() => DBInsert(model, true);
        /// <summary>
        /// 插入一个实体
        /// <para>若要设置主键的值，则设置 <c>autoKey = false</c></para>
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <param name="autoKey">是否忽略被标记为 <c>[Key]</c> 的属性。</param>
        /// <returns></returns>
        public int DBInsert<T>(T model, bool autoKey) where T : IModel, new()
        {
            if (model == null) return -1;
            bool close = false;
            if (DBConnection == null)
            {
                DBOpen();
                close = true;
            }

            PropertyInfo[] properties = model.GetType().GetProperties();
            List<string> _fieldNames = new List<string>();
            List<object> _fieldValues = new List<object>();
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "_TableName") continue;
                if (autoKey && property.IsDefined(typeof(KeyAttribute), false)) continue;
                _fieldNames.Add($"`{property.Name}`");
                object value = property.GetValue(model);
                if (value == null)
                { _fieldValues.Add("null"); }
                else if (value is string)
                { _fieldValues.Add($"'{value}'"); }
                else
                { _fieldValues.Add(value); }
            }
            string fields = string.Join(",", _fieldNames.ToArray());
            string values = string.Join(",", _fieldValues.ToArray());
            string sql = $"insert into {model._TableName} ({fields}) values ({values})";
            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
            int result = command.ExecuteNonQuery();

            if (close) { DBClose(); }
            return result;
        }

        /// <summary>
        /// 删除一个实体
        /// <para>当属性为空时，不作为判断条件</para>
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <returns></returns>
        public bool DBDelete<T>(T model) where T : IModel, new() => DBDelete(model, false);
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <param name="nullProperty">是否检查空属性</param>
        /// <returns></returns>
        public bool DBDelete<T>(T model, bool nullProperty) where T : IModel, new()
        {
            if (model == null) return false;
            bool close = false;
            if (DBConnection == null)
            {
                DBOpen();
                close = true;
            }

            PropertyInfo[] properties = model.GetType().GetProperties();
            List<string> _conditions = new List<string>();
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "_TableName") continue;
                object value = property.GetValue(model);
                if (!nullProperty && value == null) continue;
                if (nullProperty && value == null)
                { _conditions.Add($"`{property.Name}` isnull"); }
                else if (value is string)
                { _conditions.Add($"`{property.Name}` = '{value}'"); }
                else
                { _conditions.Add($"`{property.Name}` = {value}"); }
            }
            string condition = string.Join(" AND ", _conditions.ToArray());
            string sql = $"delete from {model._TableName} where {condition}";
            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
            int result = command.ExecuteNonQuery();

            if (close) { DBClose(); }
            return result > 0;
        }

        /// <summary>
        /// 查询实体
        /// </summary>
        /// <typeparam name="T">实体模型</typeparam>
        /// <returns></returns>
        public List<T> DBQuery<T>() where T : IModel, new() => DBQuery<T>(null, null);
        /// <summary>
        /// 查询实体
        /// </summary>
        /// <typeparam name="T">实体模型</typeparam>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public List<T> DBQuery<T>(string condition) where T : IModel, new() => DBQuery<T>(null, condition);
        /// <summary>
        /// 查询实体
        /// </summary>
        /// <typeparam name="T">实体模型</typeparam>
        /// <param name="fieldNames">字段名</param>
        /// <returns></returns>
        public List<T> DBQuery<T>(string[] fieldNames) where T : IModel, new() => DBQuery<T>(fieldNames, null);
        /// <summary>
        /// 查询实体
        /// </summary>
        /// <param name="fieldNames">字段名</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> DBQuery<T>(string[] fieldNames, string condition) where T : IModel, new()
        {
            bool close = false;
            if (DBConnection == null)
            {
                DBOpen();
                close = true;
            }

            string field = "*";
            if (fieldNames != null && fieldNames.Count() > 0)
            {
                List<string> _fieldNames = new List<string>();
                fieldNames.ToList().ForEach((s) =>
                {
                    _fieldNames.Add($"`{s}`");
                });
                field = string.Join(",", _fieldNames);
            }
            string tableName = new T()._TableName;
            string sql = $"select {field} from {tableName} {condition}";
            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<T> vals = new List<T>();
            while (reader.Read())
            {
                T val = new T();
                PropertyInfo[] properties = val.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name == "_TableName") continue;
                    if (fieldNames != null && fieldNames.Count() > 0)
                    {
                        string _field = fieldNames.ToList().Find(m => m == property.Name);
                        if (_field != null)
                        {
                            if (reader[property.Name].GetType() == typeof(DBNull)) continue;
                            property.SetValue(val, reader[property.Name]);
                        }
                    }
                    else
                    {
                        if (reader[property.Name].GetType() == typeof(DBNull)) continue;
                        property.SetValue(val, reader[property.Name]);
                    }
                }
                vals.Add(val);
            }

            if (close) { DBClose(); }
            return vals;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <returns></returns>
        public bool DBUpdate<T>(T model) where T : IModel, new()
        {
            if (model == null) return false;
            bool close = false;
            if (DBConnection == null)
            {
                DBOpen();
                close = true;
            }

            PropertyInfo[] properties = model.GetType().GetProperties();
            List<string> _setValues = new List<string>();
            string condition = string.Empty;
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "_TableName") continue;
                object value = property.GetValue(model);
                if (property.IsDefined(typeof(KeyAttribute), false))
                {
                    if (value is string)
                    { condition = $"`{property.Name}` = '{value}'"; }
                    else
                    { condition = $"`{property.Name}` = {value}"; }
                }
                else
                {
                    if (value == null) continue;
                    if (value is string)
                    { _setValues.Add($@"`{property.Name}` = '{value}'"); }
                    else
                    { _setValues.Add($@"`{property.Name}` = {value}"); }
                }
            }
            if (string.IsNullOrEmpty(condition)) { throw new ArgumentException("更新的实体必须具有主键"); }
            string setValue = string.Join(",", _setValues.ToArray());
            string sql = $"update {model._TableName} set {setValue} where {condition}";
            SQLiteCommand command = new SQLiteCommand(sql, DBConnection);
            int result = command.ExecuteNonQuery();

            if (close) { DBClose(); }
            return result > 0;
        }

    }
}