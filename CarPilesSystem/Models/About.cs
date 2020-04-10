using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarPilesSystem.Models
{
    /// <summary>
    /// 关于信息
    /// </summary>
    public class About
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 关于信息
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// 优势
        /// </summary>
        public string Merit { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Adrees { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
    }
}