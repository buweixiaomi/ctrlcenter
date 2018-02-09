using ManageDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Controllers
{
    public class FeedbackController : ManageBaseController
    {
        //
        // GET: /Feedback/
        ManageDomain.BLL.FeedbackBll feebll = new ManageDomain.BLL.FeedbackBll();
        public ActionResult Index(string keyword, int? search_state = 0, int pno = 1)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Feedback_Show);
            ViewBag.search_state = search_state;
            ViewBag.keywords = keyword;
            keyword = Request.QueryString["keywords"];
            const int pagesize = 20;
            var model = feebll.PageFeedback(keyword ?? "", search_state, pno, pagesize);
            return View(model);

        }
        [HttpGet]
        public ActionResult Edit()
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Feedback_Add);
            return View();
        }

        [HttpPost]
        public ActionResult Edit(ManageDomain.Models.Feedback model, string feeId = "")
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Feedback_Add);
            ViewBag.feeId = feeId;
            try
            {
                if (model == null)
                    throw new MException(MExceptionCode.BusinessError, "无效参数！");
                if (model.cusId <= 0)
                {
                    throw new MException(MExceptionCode.BusinessError, "请选择客户！");
                }

                model.ManagerId = Convert.ToInt32(Token.Id);
                model.ManagerName = Token.Name;
                model.Content = ManageDomain.Pub.URLDecode(model.Content ?? "");
                model.FromSource = 0;
                model = feebll.Add(model);
                return RedirectToAction("Index", new { cusserviceid = model.FeedbackId });
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Delete(int feedbackid)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Feedback_Delete);
            var bll = new ManageDomain.BLL.FeedbackBll();
            int r = bll.Delete(feedbackid);
            if (r > 0)
            {
                return Json(new { code = 1 });
            }
            else
            {
                return Json(new { code = -1, msg = "删除失败" });
            }
        }

        [HttpGet]
        public ActionResult Detail(int feedbackId = 0)
        {
            if (feedbackId > 0)
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Feedback_Show);
                var model = feebll.GetFeedbackDetail(feedbackId);
                return View(model);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Detail(ManageDomain.Models.Feedback model, string feeId = "")
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Feedback_Update);
            ViewBag.feeId = feeId;
            try
            {
                if (model == null)
                    throw new MException(MExceptionCode.BusinessError, "无效参数！");

                model.Content = ManageDomain.Pub.URLDecode(model.Content ?? "");
                bool f = feebll.Edit(model);
                if (f)
                {
                    return RedirectToAction("Index", new { cusserviceid = model.FeedbackId });
                }
                else
                {
                    throw new MException(MExceptionCode.BusinessError, "更新失败！");

                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View(model);
            }
        }


        public JsonResult Check(int feedbackid, int checktype, string checkremark)
        {            
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Feedback_Check);
            int workitemid = 0;
            bool isok = feebll.MakeResolve(feedbackid, checktype, Token.Id, Token.Name, checkremark ?? "", out workitemid);
            if (isok)
            {
                if (checktype == 2)
                {
                    return Json(new JsonEntity()
                    {
                        code = 2,
                        data = "/workitem/edit?workitemid=" + workitemid,
                        msg = "成功"
                    });
                }
                else
                {
                    return Json(new JsonEntity()
                    {
                        code = 1,
                        msg = "成功"
                    });
                }
            }
            else
            {
                return Json(new JsonEntity()
                {
                    code = -1,
                    msg = "审核失败"
                });
            }
        }

    }
}
