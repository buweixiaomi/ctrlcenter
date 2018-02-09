using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Controllers
{
    public class WatchLogController : ManageBaseController
    {
        //
        // GET: /WatchLog/


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TimeWatch(DateTime? date, int? hour, DateTime? begintime, DateTime? endtime, string projectName, string title, string addition, long? groupID, long? innerGroupID, int pno = 1, int ordertype = 0, int? usetimemin = null, int? usetimemax = null)
        {
            ViewBag.Title = "耗时日志";
            if (date == null)
                date = DateTime.Now;
            if (hour == null)
                hour = DateTime.Now.Hour;
            ViewBag.hour = hour;
            ViewBag.ordertype = ordertype;
            ViewBag.date = date.Value.ToString("yyyy-MM-dd");
            ViewBag.usetimemin = usetimemin;
            ViewBag.usetimemax = usetimemax;
            if (begintime != null)
                ViewBag.begintime = begintime.Value.ToString("HH:mm:ss");
            if (endtime != null)
                ViewBag.endtime = endtime.Value.ToString("HH:mm:ss");
            ViewBag.projectName = projectName;
            ViewBag.stitle = title;
            ViewBag.addition = addition;
            ViewBag.groupid = groupID;
            ViewBag.innergroupid = innerGroupID;
            int pagesize = 20;
            pno = Math.Max(pno, 1);
            var data = new ManageDomain.BLL.WatchLogBllNew().GetListLogs(date.Value, hour.Value, projectName ?? "", 1, begintime, endtime, title ?? "", addition ?? "", groupID, innerGroupID, ordertype, pno, pagesize, usetimemin ?? 0, usetimemax ?? 0);
            return View("List", data);
        }

        public ActionResult CommLog(DateTime? date, int? hour, DateTime? begintime, DateTime? endtime, string projectName, string title, string addition, long? groupID, long? innerGroupID, int pno = 1)
        {
            ViewBag.Title = "普通日志";
            if (date == null)
                date = DateTime.Now;
            // ViewBag.ordertype = ordertype;
            ViewBag.date = date.Value.ToString("yyyy-MM-dd");
            if (hour == null)
                hour = DateTime.Now.Hour;
            ViewBag.hour = hour;
            if (begintime != null)
                ViewBag.begintime = begintime.Value.ToString("HH:mm:ss");
            if (endtime != null)
                ViewBag.endtime = endtime.Value.ToString("HH:mm:ss");
            ViewBag.projectName = projectName;
            ViewBag.stitle = title;
            ViewBag.addition = addition;
            ViewBag.groupid = groupID;
            ViewBag.innergroupid = innerGroupID;
            int pagesize = 20;
            pno = Math.Max(pno, 1);
            var data = new ManageDomain.BLL.WatchLogBllNew().GetListLogs(date.Value, hour.Value, projectName ?? "", 0, begintime, endtime, title ?? "", addition ?? "", groupID, innerGroupID, 0, pno, pagesize);
            return View("List", data);
        }

        public ActionResult ErrorLog(DateTime? date, int? hour, DateTime? begintime, DateTime? endtime, string projectName, string title, string addition, long? groupID, long? innerGroupID, int pno = 1)
        {
            ViewBag.Title = "错误日志";
            if (date == null)
                date = DateTime.Now;
            //ViewBag.ordertype = ordertype;
            ViewBag.date = date.Value.ToString("yyyy-MM-dd");
            if (hour == null)
                hour = DateTime.Now.Hour;
            ViewBag.hour = hour;
            if (begintime != null)
                ViewBag.begintime = begintime.Value.ToString("HH:mm:ss");
            if (endtime != null)
                ViewBag.endtime = endtime.Value.ToString("HH:mm:ss");
            ViewBag.projectName = projectName;
            ViewBag.stitle = title;
            ViewBag.addition = addition;
            ViewBag.groupid = groupID;
            ViewBag.innergroupid = innerGroupID;
            int pagesize = 20;
            pno = Math.Max(pno, 1);
            var data = new ManageDomain.BLL.WatchLogBllNew().GetListLogs(date.Value, hour.Value, projectName ?? "", 2, begintime, endtime, title ?? "", addition ?? "", groupID, innerGroupID, 0, pno, pagesize);
            return View("List", data);
        }

        public ActionResult Detail(int id, DateTime date, int typelimeorder = 0)
        {
            var data = new ManageDomain.BLL.WatchLogBllNew().GetDetail(date, date.Hour, id);
            var timeline = data.Item2;
            if (timeline != null)
            {
                if (typelimeorder == 1)
                {
                    timeline = timeline.OrderByDescending(x => x.Elapsed).ToList();
                }
                else
                {
                    timeline = timeline.OrderBy(x => x.CreateTime).ToList();
                }
            }
            ViewBag.typelimeorder = typelimeorder;
            ViewBag.sublogs = timeline;
            return View(data.Item1);
        }

        public ActionResult BuildTimeWatchAna(DateTime date, int hour)
        {
            var bll = new ManageDomain.BLL.WatchLogBllNew();
            bll.BuildAnaData(date, hour);
            return JsonE("OK");
        }

        public ActionResult TimeWatchAna(DateTime? date, int? hour, int? groupId, int mincount = 0, int maxcount = 0, string dbname = "", int ordertype1 = 11, int ordertype2 = 11, int pagesize = 20, int pno = 1)
        {
            if (date == null)
                date = DateTime.Now;
            if (hour == null)
                hour = DateTime.Now.Hour;
            ViewBag.hour = hour;
            ViewBag.ordertype1 = ordertype1;
            ViewBag.ordertype2 = ordertype2;
            ViewBag.mincount = mincount;
            ViewBag.maxcount = maxcount;
            ViewBag.date = date.Value.ToString("yyyy-MM-dd");
            var bll = new ManageDomain.BLL.WatchLogBllNew();
            if (pagesize <= 0 || pagesize > 500)
                pagesize = 30;
            ViewBag.pagesize = pagesize;
            ViewBag.dbname = dbname;
            ViewBag.groupId = groupId;
            var models = bll.GetAna(pno, pagesize, date.Value, hour.Value, groupId, mincount, maxcount, dbname, ordertype1, ordertype2);
            return View(models);
        }

    }
}
