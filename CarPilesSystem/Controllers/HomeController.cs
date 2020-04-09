using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using CarPilesSystem.Commons;
using CarPilesSystem.Models;
using View.SQLite.Handle;
using CarPilesSystem.Models.Imodels;

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
            ViewBag.XuQingDian = "123";
            return View();
        }
        /// <summary>
        /// 联系-视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact() => View();
        /// <summary>
        /// 充电桩管理 - 视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Piles() => View();
        #endregion

        #region API
        #region 用户登录/注册相关
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
                var user = db.Query<User>($"where `UserName` = '{username}'").FirstOrDefault();
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
        public ActionResult SignUp(User user)
        {
            using (var db = this.BuildDB())
            {
                if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
                {
                    return Error(-1, "请核对必填字段。");
                }
                var _user = db.Query<User>($"where `UserName` = '{user.UserName}'").FirstOrDefault();
                if (_user == null)
                {
                    if (db.Insert(user))
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
        /// <summary>
        /// 获取指定点附近的充电桩
        /// </summary>
        /// <param name="longitude">指定点的纬度</param>
        /// <param name="latitude">指定点的经度</param>
        /// <param name="distance">与指定点的距离 :m</param>
        /// <returns></returns>
        public ActionResult GetNearbyPiles(string longitude, string latitude, double distance = 5000)
        {
            LngLat lngLat = new LngLat(longitude, latitude);
            DMS top = new DMS(lngLat.Latitude.ToDouble() + LngLat.MeterToLat(distance));
            DMS bottom = new DMS(lngLat.Latitude.ToDouble() - LngLat.MeterToLat(distance));
            DMS left = new DMS(lngLat.Longitude.ToDouble() - LngLat.MeterToLng(distance, lngLat.Latitude.ToDouble()));
            DMS right = new DMS(lngLat.Longitude.ToDouble() + LngLat.MeterToLng(distance, lngLat.Latitude.ToDouble()));

            using (var db = this.BuildDB())
            {
                string condition = $"where Longitude > {left} and Longitude < {right} and Latitude > {bottom} and Latitude < {top}";
                List<Pile> piles = db.Query<Pile>(condition);
                if (piles != null && piles.Count > 0)
                { return Success(piles, "获取充电桩成功"); }
                else
                { return Error("附近没有充电桩"); }
            }
        }
        #endregion

        #region 后台管理相关
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
        /// 获取计价表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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
        #endregion
    }
}