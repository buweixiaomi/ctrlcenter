using ManageDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Controllers
{
    public class CustomerController : ManageBaseController
    {
        //
        // GET: /Customer/
        ManageDomain.BLL.CustomerBll cusbll = new ManageDomain.BLL.CustomerBll();
        public ActionResult Index(string keywords, int pno = 1)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Show);
            ViewBag.keywords = keywords;
            const int pagesize = 20;

            var model = cusbll.PageCustomer(keywords ?? "", pno, pagesize);
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int cusid = 0)
        {
            ViewBag.managers = new ManageDomain.BLL.ManagerBll().GetManagerTop(1000);
            if (cusid > 0)
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Show);
                var model = cusbll.GetCusDetail(cusid);
                return View(model);
            }
            else
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Add);
                return View();
            }
        }

        [ValidateInput(false)]
        public ActionResult Edit(ManageDomain.Models.Customer model, string[] tag, string[] linkmanagers_managerid, string[] linkmanagers_title, string[] linkmanagers_remark)
        {
            ViewBag.managers = new ManageDomain.BLL.ManagerBll().GetManagerTop(1000);
            if (model == null)
            {
                ViewBag.msg = "无效参数！";
                return View();
            }
            if (linkmanagers_managerid == null)
                linkmanagers_managerid = new string[0];
            if (linkmanagers_title == null)
                linkmanagers_title = new string[0];
            if (linkmanagers_remark == null)
                linkmanagers_remark = new string[0];
            List<ManageDomain.Models.CustomerLinkManager> links = new List<ManageDomain.Models.CustomerLinkManager>();
            if (linkmanagers_managerid.Length == linkmanagers_remark.Length && linkmanagers_managerid.Length == linkmanagers_title.Length)
            {
                for (int i = 0; i < linkmanagers_managerid.Length; i++)
                {
                    links.Add(new ManageDomain.Models.CustomerLinkManager()
                    {
                        CusId = model.CusId,
                        ManagerId = CCF.DB.LibConvert.StrToInt(linkmanagers_managerid[i]),
                        Title = linkmanagers_title[i] ?? "",
                        Remark = linkmanagers_remark[i] ?? ""
                    });
                }
            }
            string _tag = ManageDomain.Pub.CombineTags(tag);
            model.Tag = _tag;
            if (model.CusId > 0)
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Update);
                int r = cusbll.UpdateCus(model, links);
            }
            else
            {
                //权限
                ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Add);
                model = cusbll.AddCus(model, links);
            }
            model = cusbll.GetCusDetail(model.CusId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int cusid = 0)
        {
            //权限
            ManageDomain.PermissionProvider.CheckExist(SystemPermissionKey.Customer_Delete);
            var model = cusbll.Delete(cusid);
            if (model)
            {
                return JsonE("OK");
            }
            else
            {
                return Json(new JsonEntity()
                {
                    code = -1,
                    msg = "删除失败"
                });
            }
        }
    }
}
