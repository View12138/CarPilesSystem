using CarPilesSystem.Commons;
using CarPilesSystem.Models.IModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace CarPilesSystem.Controllers
{
    /// <summary>
    /// 后台管理 API
    /// </summary>
    public class AdminController : CpsController
    {
        #region 充电桩管理
        /// <summary>
        /// 创建一个充电桩
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="name">充电桩名称</param>
        /// <returns></returns>
        public ActionResult CreatePile(string longitude, string latitude, string name = "cps 充电桩")
        {
            using (var db = this.BuildDB())
            {
                var pile = new Pile() { Longitude = longitude, Latitude = latitude, Name = name };
                if (db.Insert(pile, out long id))
                {
                    pile.Id = id;
                    return Success(pile, "创建充电桩成功");
                }
                else
                {
                    return Error("创建充电桩失败");
                }
            }
        }
        /// <summary>
        /// 删除充电桩
        /// </summary>
        /// <param name="piles"></param>
        /// <returns></returns>
        public ActionResult DeletePiles(Pile[] piles)
        {
            using (var db = BuildDB())
            {
                var result = db.RunTransaction((trans) =>
                {
                    foreach (var pile in piles.ToList())
                    {
                        if (!db.Delete(pile, trans))
                        { return false; }
                    }
                    return true;
                });
                if (result)
                {
                    return Success("删除成功");
                }
                else
                {
                    return Error("删除失败");
                }
            }
        }
        /// <summary>
        /// 更新充电桩信息
        /// </summary>
        /// <param name="pile"></param>
        /// <returns></returns>
        public ActionResult UpdatePiles(Pile pile)
        {
            using (var db = BuildDB())
            {
                if (db.Update(pile))
                {
                    return Success("获取充电桩成功");
                }
                else
                {
                    return Error("没有充电桩");
                }
            }
        }
        /// <summary>
        /// 获取充电桩
        /// <para>分页方式</para>
        /// </summary>
        /// <param name="page">当前页</param>
        /// <param name="row">每页行数</param>
        /// <returns></returns>
        public ActionResult GetPiles(int page, int row)
        {
            using (var db = BuildDB())
            {
                var piles = db.Query<Pile>($"limit {page},{row}");
                if (piles.Count > 0)
                {
                    return Success(piles, "获取充电桩成功");
                }
                else
                {
                    return Error("没有充电桩");
                }
            }
        }
        #endregion

        #region 计价表管理
        /// <summary>
        /// 获取计价表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPrices()
        {
            using (var db = this.BuildDB())
            {
                var prices = db.Query<Price>();
                if (prices != null && prices.Count > 0)
                { return Success(prices, "获取计价表成功"); }
                else
                { return Error("获取计价表失败"); }
            }
        }
        #endregion
    }
}
