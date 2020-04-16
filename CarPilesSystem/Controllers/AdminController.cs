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
        #region 管理员登录
        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="uname"></param>
        /// <param name="upswd"></param>
        /// <returns></returns>
        public ActionResult Login(string uname, string upswd)
        {
            using (var db = this.BuildDB())
            {
                var admin = db.Query<Admin>($"where `UserName` = '{uname}' and `Password` = '{upswd}'").FirstOrDefault();
                if (admin != null)
                { return Success(admin, "登录成功"); }
                else
                { return Error("你不具有管理员权限"); }
            }
        }
        #endregion

        #region 充电桩管理
        /// <summary>
        /// 创建一个充电桩
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="money">充电桩一小时的价格</param>
        /// <param name="name">充电桩名称</param>
        /// <returns></returns>
        public ActionResult CreatePile(string longitude, string latitude, string price, string name = "cps 充电桩")
        {
            using (var db = this.BuildDB())
            {
                var pile = new Pile() { Longitude = longitude, Latitude = latitude, Name = name, Price = price, EndTime = "", StartTime = "", State = 0, UserId = 0 };
                if (db.Insert(pile))
                {
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
                    return Success("数据修改成功");
                }
                else
                {
                    return Error("修改失败");
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
    }
}
