using CarPilesSystem.Commons;
using CarPilesSystem.Models;
using CarPilesSystem.Models.IModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace CarPilesSystem.Controllers
{
    /// <summary>
    /// 用户交互 API
    /// </summary>
    public class UserController : CpsController
    {
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
        #endregion

        #region 充电桩相关
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
        /// <summary>
        /// 获取正在使用的充电桩
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public ActionResult GetMyUsePile(long userId)
        {
            using (var db = this.BuildDB())
            {
                var pile = db.Query<Pile>($"where `UserId` = {userId} and `State` != 0 ").FirstOrDefault();
                if (pile == null)
                {
                    return Error("没有使用或预约的充电桩");
                }
                else
                {
                    return Success(pile, "您有一个正在进行的订单");
                }
            }
        }
        /// <summary>
        /// 开始使用充电桩
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pileId"></param>
        /// <returns></returns>
        public ActionResult StartUsePile(long userId, long pileId)
        {
            using (var db = BuildDB())
            {
                var pile = db.Query<Pile>($"where `Id` = {pileId} and `State` = 0 or `State` = 2").FirstOrDefault();
                if (pile != null)
                {
                    pile.UserId = userId;
                    pile.State = 1;
                    pile.StartTime = DateTime.Now.ToString();
                    if (db.Update(pile)) { return Success(pile.StartTime, "开始充电"); }
                    else
                    { return Error("充电桩启动失败"); }
                }
                else
                { return Error("充电桩启动失败"); }
            }
        }
        /// <summary>
        /// 结束使用充电桩
        /// </summary>
        /// <param name="userId">用户 id</param>
        /// <param name="pileId">充电桩 id</param>
        /// <returns></returns>
        public ActionResult EndUsePile(long userId, long pileId)
        {
            using (var db = BuildDB())
            {
                var pile = db.Query<Pile>($"where `Id` = {pileId} and `State` = 1 ").FirstOrDefault();
                if (pile != null)
                {
                    var record = new PileRecord()
                    {
                        StartTime = pile.StartTime,
                        EndTime = DateTime.Now.ToString(),
                        UserId = userId,
                    };
                    record.Money = ((DateTime.Parse(record.EndTime) - DateTime.Parse(record.StartTime)).TotalHours * double.Parse(pile.Price)).ToString();
                    pile.UserId = 0;
                    pile.State = 0;
                    pile.StartTime = "";
                    pile.EndTime = "";
                    long id = 0;
                    if (db.RunTransaction((trans) =>
                    {
                        var result = db.Insert(record, trans, out id);
                        if (!result) return false;
                        result = db.Update(pile, trans);
                        if (!result) return false;
                        return true;
                    }))
                    {
                        record.Id = id;
                        return Success(record, "结束充电");
                    }
                    else
                    { return Error("充电桩关闭失败"); }
                }
                else
                { return Error("充电桩关闭失败"); }
            }
        }
        /// <summary>
        /// 预约使用充电桩
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pileId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult PlanUsePile(long userId, long pileId, DateTime startTime, DateTime endTime)
        {
            using (var db = BuildDB())
            {
                var pile = db.Query<Pile>($"where `Id` = {pileId} and `State` = 0 ").FirstOrDefault();
                if (pile == null)
                {
                    return Error("预约失败，请稍后再试。");
                }
                else
                {
                    pile.StartTime = startTime.ToString();
                    pile.EndTime = endTime.ToString();
                    pile.State = 2;
                    pile.UserId = userId;
                    if (db.Update(pile))
                    {
                        // 预约计时，超时10分钟后任然没有开始充电则取消预约；
                        Task.Run(async () =>
                        {
                            bool isUsed = false;
                            bool isTimeout = false;
                            while (!(isUsed || isTimeout))
                            {
                                await Task.Delay(TimeSpan.FromSeconds(30));
                                using (var _db = BuildDB())
                                {
                                    var _pile = _db.Query<Pile>($"where `UserId` = {userId}").FirstOrDefault();
                                    if (_pile == null)
                                    {
                                        continue;
                                    }
                                    if (_pile.State == 2)
                                    {// 预约中
                                        DateTime end = DateTime.Parse(_pile.StartTime);
                                        if (DateTime.Now > end.AddMinutes(10))
                                        {
                                            _pile.State = 0;
                                            _pile.UserId = 0;
                                            _pile.StartTime = "";
                                            _pile.EndTime = "";
                                            _db.Update(_pile);
                                            isTimeout = true;
                                        }
                                    }
                                    else
                                    {
                                        isUsed = true;
                                    }
                                }
                            }
                        });
                        return Success("预约成功，请在预定时间使用，逾期自动取消。");
                    }
                    else { return Error("预约失败，请稍后再试。"); }
                }
            }
        }
        /// <summary>
        /// 取消预约
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pileId"></param>
        /// <returns></returns>
        public ActionResult PlanCanelPile(long userId, long pileId)
        {
            using (var db = BuildDB())
            {
                var pile = db.Query<Pile>($"where `Id` = {pileId} and `UserId` = '{userId}' ").FirstOrDefault();
                if (pile != null)
                {
                    pile.UserId = 0;
                    pile.State = 0;
                    pile.StartTime = "";
                    pile.EndTime = "";
                    if (db.Update(pile))
                    { return Success("取消成功"); }
                    else
                    { return Error("取消失败，请稍后重试"); }
                }
                else
                { return Error("取消失败，请稍后重试"); }
            }
        }
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public ActionResult Pay(long userId, long recordId)
        {
            using (var db = this.BuildDB())
            {
                var record = db.Query<PileRecord>($"where `UserId` = '{userId}' and `Id` = {recordId}").FirstOrDefault();
                if (record != null)
                {
                    record.State = 1;
                    if (db.Update(record))
                    {
                        return Success("支付成功");
                    }
                    else
                    { return Error("支付失败"); }
                }
                else
                { return Error("支付失败"); }
            }
        }
        #endregion

        #region 系统信息相关
        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAbout()
        {
            About about = new About()
            {
                Name = "新能源汽车充电导航系统",
                Info = "基于互联网+智能充电引导系统，用户可以通过手机实现充电装置状态查询、定位导航、充电预约及智能充电与充电装置锁定等功能。后台通过内置在汽车中已经连接的车载数据采集系统采集相关数据，并发回给服务器端，服务器端就可从远端实时监控汽车的电力状态。",
                Merit = "基于互联网云平台的智能充电导航系统能让车主及时了解全城各充电桩状态、自动寻找空闲充电桩、并能实时预约空闲充电桩，不仅将大大减少车主在寻找充电桩.上浪费的时间与进行充电前的排队时间，而且能有效提高充电桩利用率，对解决充电站占地过大的城市规划问题起到积极作用。",
                Adrees = "昆明&nbsp;•&nbsp;云南&nbsp;•&nbsp;中国<br />云南财经大学,龙泉路",
                Email = "1907087110@qq.com",
                Phone = "",
            };
            return Success(about, "获取数据成功");
        }
        #endregion
    }
}
