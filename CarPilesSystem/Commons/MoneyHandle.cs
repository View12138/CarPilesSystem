using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarPilesSystem.Commons
{
    public class MoneyHandle
    {
        /// <summary>
        /// 返回一个两位小数精度的十进制数
        /// </summary>
        /// <param name="d"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static decimal GetPrice(decimal d, decimal unit)
        {
            decimal rm = d % unit;
            decimal result = d - rm;
            if (rm > 0)
                result += unit;
            return result;
        }

        /// <summary>
        /// 返回一个两位小数精度的浮点数
        /// </summary>
        /// <param name="d"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double GetPrice(double price, double unit) => (double)GetPrice(price == 0 ? 0.01m : (decimal)price, (decimal)unit);
    }
}