using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using CarPilesSystem.Models;
using View.SQLite.Handle;

namespace CarPilesSystem.Commons
{
    public class CpsController : Controller
    {
        /// <summary>
        /// 构建数据库
        /// </summary>
        /// <returns></returns>
        public SQLiteHandle BuildDB()
        {
            string DBName = $@"{AppDomain.CurrentDomain.BaseDirectory}Database\CarSystemDB.db";
            string DBPassword = "gb123456";
            return new SQLiteHandle(DBName, DBPassword);
        }

        /// <summary>
        /// MD5字符串加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns>加密后字符串</returns>
        public string GenerateMD5(string txt)
        {
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(txt);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        public JsonResult Success() => Success(string.Empty);
        /// <summary>
        /// 成功
        /// </summary>
        /// <typeparam name="T">返回的数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public JsonResult Success<T>(T data) => Success(data, string.Empty);
        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="message">发送得消息</param>
        /// <returns></returns>
        public JsonResult Success(string message) => Success(true, message);
        /// <summary>
        /// 成功
        /// </summary>
        /// <typeparam name="T">返回的数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public JsonResult Success<T>(T data, string message)
        {
            return Json(new Result<T>()
            {
                State = 0,
                Data = data,
                Message = message,
            });
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <returns></returns>
        public JsonResult Error() => Error(string.Empty);
        /// <summary>
        /// 失败
        /// <para>状态码为 <c><see langword="-1"/></c></para>
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public JsonResult Error(string message) => Error(-1, message);
        /// <summary>
        /// 失败
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <param name="state">状态<para>状态码不能为 <c><see langword="0"/></c></para></param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public JsonResult Error(int state, string message) => Error(state, false, message);
        /// <summary>
        /// 失败
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <typeparam name="T">返回的数据类型</typeparam>
        /// <param name="state">状态<para>状态码不能为 <c><see langword="0"/></c></para></param>
        /// <param name="data">数据</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public JsonResult Error<T>(int state, T data, string message)
        {
            if (state == 0)
            { throw new ArgumentException("0 是标识成功的状态。"); }
            return Json(new Result<object>()
            {
                State = state,
                Data = data,
                Message = message,
            });
        }
    }
}