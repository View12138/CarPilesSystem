using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarPilesSystem.Commons;
using CarPilesSystem.Models;

namespace CarPilesSystem.Controllers
{
    public class HomeController : CpsController
    {
        #region View
        /// <summary>
        /// 主页-视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Index() => View();
        /// <summary>
        /// 关于-视图
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            ViewBag.About = "基于互联网+智能充电引导系统，用户可以通过手机实现充电装置状态查询、定位导航、充电预约及智能充电与充电装置锁定等功能。后台通过内置在汽车中已经连接的车载数据采集系统采集相关数据，并发回给服务器端，服务器端就可从远端实时监控汽车的电力状态。";
            ViewBag.Merit = "基于互联网云平台的智能充电导航系统能让车主及时了解全城各充电桩状态、自动寻找空闲充电桩、并能实时预约空闲充电桩，不仅将大大减少车主在寻找充电桩.上浪费的时间与进行充电前的排队时间，而且能有效提高充电桩利用率，对解决充电站占地过大的城市规划问题起到积极作用。";

            return View();
        }
        /// <summary>
        /// 联系-视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact() => View();
        #endregion

        #region API
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ActionResult SignIn(string username, string password)
        {
            using (var db = this.BuildDB())
            {
                var user = db.Query<Cps_User>($"where `UserName` = '{username}'").FirstOrDefault();
                if (user == null)
                {
                    return Error(-1, "用户名未注册。");
                }
                else if (user.Password != password)
                {
                    return Error(-1, "密码错误");
                }
                else
                {
                    return Success(user, "登录成功");
                }
            }
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns></returns>
        public ActionResult SignUp(Cps_User user)
        {
            using (var db = this.BuildDB())
            {
                if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
                {
                    return Error(-1, "请核对必填字段。");
                }
                var _user = db.Query<Cps_User>($"where `UserName` = '{user.UserName}'").FirstOrDefault();
                if (_user == null)
                {
                    if (db.Insert(user) > 0)
                    {
                        return Success(user, "注册成功，可以登录。");
                    }
                    else
                    {
                        return Error(-1, "注册失败，请重试。");
                    }
                }
                else
                {
                    return Error(-1, "用户名已存在。");
                }
            }
        }
        #endregion
    }
}