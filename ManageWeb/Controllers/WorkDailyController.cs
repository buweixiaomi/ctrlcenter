using ManageDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Controllers
{
    public class WorkDailyController : ManageBaseController
    {
        //
        // GET: /WorkDaily/
        ManageDomain.BLL.WorkDailyBll workdailybll = new ManageDomain.BLL.WorkDailyBll();
        public ActionResult Index(int? search_managerid, DateTime? search_begintime, DateTime? search_endtime, int pno = 1)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkDaily_Show);
            ViewBag.search_begintime = search_begintime == null ? "" : search_begintime.Value.ToString("yyyy-MM-dd");
            ViewBag.search_endtime = search_endtime == null ? "" : search_endtime.Value.ToString("yyyy-MM-dd");
            ViewBag.search_managerid = search_managerid;

            const int pageisze = 20;
            int? currmanagerid = User.CurrUserId();
            bool exist = PermissionProvider.Exist(SystemPermissionKey.WorkDaily_ShowOther);
            if (exist)
            {
                currmanagerid = null;
            }
            var model = workdailybll.GetPate(currmanagerid, search_managerid, search_begintime, search_endtime, pno, pageisze);
            return View(model);
        }

        public ActionResult Add()
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkDaily_Add);
            ViewBag.defaultworkdate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.managerid = Token.Id.ToString();
            ViewBag.managername = Token.Name;
            return View();
        }


        [HttpPost]
        public ActionResult Add(ManageDomain.Models.WorkDaily model)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkDaily_Add);
            try
            {
                if (model == null)
                {
                    throw new MException("无效参数");
                }
                model.ManagerId = User.CurrUserId();
                model = workdailybll.Add(model);
                return RedirectToAction("Edit", new { workdailyid = model.WorkDailyId });
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Edit(int workdailyid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkDaily_Show);
            var model = workdailybll.Detail(workdailyid);
            if (model == null)
            {
                throw new MException("无效参数");
            }
            ViewBag.CanEdit = workdailybll.CanEdit(model);
            return View("Add", model);
        }


        [HttpPost]
        public ActionResult Edit(ManageDomain.Models.WorkDaily model)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkDaily_Update);
            try
            {
                if (model == null)
                {
                    throw new MException("无效参数");
                }
                var updateresult = workdailybll.Update(model);
                if (updateresult)
                {
                    ViewBag.msg = "修改成功";
                }
                else
                {
                    ViewBag.msg = "修改失败";
                }
                model = workdailybll.Detail(model.WorkDailyId);
                ViewBag.CanEdit = workdailybll.CanEdit(model);
                return View("Add", model);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View("Add", model);
            }
        }


        public JsonResult Delete(int workdailyid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkDaily_Delete);
            bool data = workdailybll.Delete(workdailyid,Token.Id);
            return JsonE(data);
        }
        public JsonResult checkdate(int? WorkDailyId, DateTime date)
        {
            var model = workdailybll.GetByDate(User.CurrUserId(), date);
            if (model != null && WorkDailyId != null && WorkDailyId.Value == model.WorkDailyId)
            {
                model = null;
            }
            if (model == null)
            {
                return JsonE(0);
            }
            else
            {
                return JsonE(model.WorkDailyId);
            } 
        }

        public JsonResult BuildDailyFromWork(DateTime? date)
        {
            if (date == null)
                date = DateTime.Now;
            string data = workdailybll.BuildFromWorkItem(User.CurrUserId(), date.Value);
            return JsonE(data);
        }

        public ActionResult Report(DateTime? begintime, DateTime? endtime, int? groupid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.WorkDaily_Report);
            const int one_max_days = 15;
            const int default_days = 6;
            var usertags = new ManageDomain.BLL.ManagerBll().GetManagerTags(Token.Id);
            if (groupid == null && usertags.Count > 0)
                groupid = usertags[0].UserTagId;
            if (endtime == null)
                endtime = DateTime.Now;//
            if (begintime == null)
                begintime = endtime.Value.AddDays(-default_days);
            endtime = DateTime.Parse(endtime.Value.ToString("yyyy-MM-dd"));
            begintime = DateTime.Parse(begintime.Value.ToString("yyyy-MM-dd"));

            DateTime realendtime = endtime.Value;
            DateTime realbegintime = begintime.Value;
            DateTime nextendtime = realbegintime;
            bool hasmore = false;
            if ((endtime.Value - begintime.Value).TotalDays >= one_max_days)
            {
                realbegintime = realendtime.AddDays(-one_max_days+1);
                nextendtime = realbegintime.AddDays(-1);
                hasmore = true;
            } 

            ViewBag.groupid = groupid.ToString();
            ViewBag.tags = usertags;
            ViewBag.begintime = begintime.Value.ToString("yyyy-MM-dd");
            ViewBag.endtime = endtime.Value.ToString("yyyy-MM-dd");
            ViewBag.hasmore = hasmore;
            ViewBag.nextbegintime = begintime.Value.ToString("yyyy-MM-dd");
            ViewBag.nextendtime = nextendtime.ToString("yyyy-MM-dd");

            ViewBag.realbegintime = realbegintime;
            ViewBag.realendtime = realendtime;

            var data = workdailybll.GetGroupDaily(groupid ?? 0, realbegintime, realendtime);
            if (Request.IsAjaxRequest())
            {
                return PartialView("ReportMore", data);
            }
            return View(data);
        }

    }
}
