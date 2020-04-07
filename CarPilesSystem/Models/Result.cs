using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarPilesSystem.Models
{
    /// <summary>
    /// 返回结果模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        /// <summary>
        /// 状态码
        /// <para>0 代表正确</para>
        /// </summary>
        public int State { get; set; } = 0;
        /// <summary>
        /// 返回的数据
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
    }
}