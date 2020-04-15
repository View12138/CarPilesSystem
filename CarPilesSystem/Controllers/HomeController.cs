using System.Web.Mvc;

namespace CarPilesSystem.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 主页-视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Index() => View();
        /// <summary>
        /// 充电桩管理 - 视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Piles() => View();
        /// <summary>
        /// 支付页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Pay(long userId, long recordId, string money)
        {
            ViewBag.Money = money.Substring(0, 6);
            ViewBag.UserId = userId;
            ViewBag.RecordId = recordId;
            return View();
        }
        /// <summary>
        /// 个人中心
        /// </summary>
        /// <returns></returns>
        public ActionResult Personal() => View();
    }
}