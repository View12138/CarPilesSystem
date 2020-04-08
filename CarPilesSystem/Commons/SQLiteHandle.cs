using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CarPilesSystem.Commons
{
    /// <summary>
    /// SQLite 操作
    /// </summary>
    public class SQLiteHandle : IDisposable
    {
        /// <summary>
        /// 初始化一个数据库连接
        /// </summary>
        /// <param name="name">数据库名称</param>
        public SQLiteHandle(string name) : this(name, null, 3) { }
        /// <summary>
        /// 初始化一个数据库连接
        /// </summary>
        /// <param name="name">数据库名称</param>
        /// <param name="version">数据库版本</param>
        public SQLiteHandle(string name, int version) : this(name, null, version) { }
        /// <summary>
        /// 初始化一个数据库连接
        /// </summary>
        /// <param name="name">数据库名称</param>
        /// <param name="password">数据库密码</param>
        public SQLiteHandle(string name, string password) : this(name, password, 3) { }
        /// <summary>
        /// 初始化一个数据库连接
        /// </summary>
        /// <param name="name">数据库名称</param>
        /// <param name="password">数据库密码</param>
        /// <param name="version">数据库版本</param>
        public SQLiteHandle(string name, string password, int version)
        {
            string connectionString;
            if (string.IsNullOrEmpty(password))
            { connectionString = $@"Data Source={name};Version={version};"; }
            else
            { connectionString = $@"Data Source={name};Password={password};Version={version};"; }
            Connection = new SQLiteConnection(connectionString);
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public SQLiteConnection Connection { get; }
        /// <summary>
        /// 数据库连接的状态
        /// </summary>
        public ConnectionState State { get => Connection.State; }

        /// <summary>
        /// 打开默认数据库连接。
        /// <list type="bullet">
        /// <item>如果使用此方法，则在查询完成后手动关闭数据库连接。</item>
        /// <item>如果未使用此方法，则查询方法自动关闭数据库连接。</item>
        /// </list>
        /// </summary>
        public void Open()
        {
            if (State != ConnectionState.Open)
                Connection.Open();
        }
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void Close()
        {
            if (State != ConnectionState.Closed)
                Connection.Close();
        }

        /// <summary>
        /// 更改密码。
        /// </summary>
        /// <param name="newPassword">新密码</param>
        public void ChangePassword(string newPassword)
        {
            RunAutoClose(() =>
            {
                Connection.ChangePassword(newPassword);
            });
        }

        /// <summary>
        /// 实现自动关闭
        /// </summary>
        /// <param name="action">要执行的操作</param>
        private void RunAutoClose(Action action) => RunAutoClose(() => { action(); return 0; });
        /// <summary>
        /// 实现自动关闭
        /// </summary>
        /// <typeparam name="TResult">指定操作的返回类型</typeparam>
        /// <param name="func">执行的操作</param>
        /// <returns></returns>
        private TResult RunAutoClose<TResult>(Func<TResult> func)
        {
            bool autoClosed = State != ConnectionState.Open;
            if (autoClosed) { Open(); }
            TResult result = func();
            if (autoClosed) { Close(); }
            return result;
        }

        /// <summary>
        /// 提供事务支持
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <returns>事务执行是否成功</returns>
        public bool RunTransaction(Action<SQLiteTransaction> action)
        {
            bool autoClosed = State != ConnectionState.Open;
            if (autoClosed) { Open(); }
            var trans = Connection.BeginTransaction();
            bool result;
            try
            {
                action(trans);
                trans.Commit();
                result = true;
            }
            catch (Exception)
            {
                trans.Rollback();
                result = false;
            }
            if (autoClosed) { Close(); }
            return result;
        }

        /// <summary>
        /// 插入一个实体
        /// <para>适用于自增主键的模型</para>
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <returns></returns>
        public bool Insert<T>(T model) where T : IModel, new() => Insert(model, true, null, out _);
        /// <summary>
        /// 插入一个实体
        /// <para>适用于自增主键的模型</para>
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <param name="id">成功插入后的Id</param>
        /// <returns></returns>
        public bool Insert<T>(T model, out long id) where T : IModel, new() => Insert(model, true, null, out id);
        /// <summary>
        /// 插入一个实体
        /// <para>适用于自增主键的模型</para>
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <param name="trans">事务支持</param>
        /// <returns></returns>
        public bool Insert<T>(T model, SQLiteTransaction trans) where T : IModel, new() => Insert(model, true, trans, out _);
        /// <summary>
        /// 插入一个实体
        /// <para>适用于自增主键的模型</para>
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <param name="trans">事务支持</param>
        /// <param name="id">成功插入后的Id</param>
        /// <returns></returns>
        public bool Insert<T>(T model, SQLiteTransaction trans, out long id) where T : IModel, new() => Insert(model, true, trans, out id);
        /// <summary>
        /// 插入一个实体
        /// <para>若要设置主键的值，则设置 <c>autoKey = false</c></para>
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <param name="autoKey">是否忽略被标记为 <c>[Key]</c> 的属性。</param>
        /// <param name="trans">事务支持</param>
        /// <returns></returns>
        public bool Insert<T>(T model, bool autoKey, SQLiteTransaction trans, out long id) where T : IModel, new()
        {
            if (model == null)
            { throw new ArgumentNullException("model", "The arameter cannot be null."); }

            bool result = RunAutoClose(() =>
            {
                PropertyInfo[] properties = model.GetType().GetProperties();
                List<string> _fieldNames = new List<string>();
                List<object> _fieldValues = new List<object>();
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name == "_TableName") continue;
                    if (autoKey && property.IsDefined(typeof(Key), false)) continue;
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
                using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
                {
                    if (trans != null)
                    { command.Transaction = trans; }
                    return command.ExecuteNonQuery() > 0;
                }
            });
            if (result)
            { id = Connection.LastInsertRowId; }
            else
            { id = -1; }
            return result;
        }

        /// <summary>
        /// 删除一个实体
        /// <para>当属性为空时，不作为判断条件</para>
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <returns></returns>
        public bool Delete<T>(T model) where T : IModel, new() => Delete(model, false, null);
        /// <summary>
        /// 删除一个实体
        /// <para>当属性为空时，不作为判断条件</para>
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <param name="trans">事务支持</param>
        /// <returns></returns>
        public bool Delete<T>(T model, SQLiteTransaction trans) where T : IModel, new() => Delete(model, false, trans);
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <param name="nullProperty">是否检查空属性</param>
        /// <param name="trans">事务支持</param>
        /// <returns></returns>
        public bool Delete<T>(T model, bool nullProperty, SQLiteTransaction trans) where T : IModel, new()
        {
            if (model == null)
            { throw new ArgumentNullException("model", "The arameter cannot be null."); }

            return RunAutoClose(() =>
            {
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
                using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
                {
                    if (trans != null)
                    { command.Transaction = trans; }
                    return command.ExecuteNonQuery() > 0;
                }
            });
        }

        /// <summary>
        /// 查询实体
        /// </summary>
        /// <typeparam name="T">实体模型</typeparam>
        /// <returns></returns>
        public List<T> Query<T>() where T : IModel, new() => Query<T>(null, null);
        /// <summary>
        /// 查询实体
        /// </summary>
        /// <typeparam name="T">实体模型</typeparam>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public List<T> Query<T>(string condition) where T : IModel, new() => Query<T>(null, condition);
        /// <summary>
        /// 查询实体
        /// </summary>
        /// <typeparam name="T">实体模型</typeparam>
        /// <param name="fieldNames">字段名</param>
        /// <returns></returns>
        public List<T> Query<T>(string[] fieldNames) where T : IModel, new() => Query<T>(fieldNames, null);
        /// <summary>
        /// 查询实体
        /// </summary>
        /// <param name="fieldNames">字段名</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> Query<T>(string[] fieldNames, string condition) where T : IModel, new()
        {
            return RunAutoClose(() =>
            {
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
                SQLiteCommand command = new SQLiteCommand(sql, Connection);
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
                return vals;
            });
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <returns></returns>
        public bool Update<T>(T model) where T : IModel, new() => Update(model, null);
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">实体模型</param>
        /// <param name="trans">事务支持</param>
        /// <returns></returns>
        public bool Update<T>(T model, SQLiteTransaction trans) where T : IModel, new()
        {
            if (model == null)
            { throw new ArgumentNullException("model", "The arameter cannot be null."); }

            return RunAutoClose(() =>
            {
                PropertyInfo[] properties = model.GetType().GetProperties();
                List<string> _setValues = new List<string>();
                string condition = string.Empty;
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name == "_TableName") continue;
                    object value = property.GetValue(model);
                    if (property.IsDefined(typeof(Key), false))
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
                using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
                {
                    if (trans != null)
                    { command.Transaction = trans; }
                    return command.ExecuteNonQuery() > 0;
                }
            });
        }

        /// <summary>
        /// 释放数据库连接
        /// </summary>
        public void Dispose()
        {
            if (State != ConnectionState.Closed)
            { Close(); }
            Connection.Dispose();
        }

        ~SQLiteHandle() => Dispose();
    }
}