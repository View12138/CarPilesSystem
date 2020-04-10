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
    }
}