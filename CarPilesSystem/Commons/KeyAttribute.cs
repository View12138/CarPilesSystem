using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarPilesSystem.Commons
{
    /// <summary>
    /// 标记此属性为主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Key : Attribute { }
}