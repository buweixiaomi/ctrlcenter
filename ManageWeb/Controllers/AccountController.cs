using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ManageWeb.Controllers
{
    public class AccountController : ManageBaseController
    {
        //
        // GET: /Account/

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string loginname, string loginpwd)
        {
            try
            {
                ViewBag.loginname = loginname;
                if (string.IsNullOrEmpty(loginname))
                {
                    throw new Exception("请输入登录名");
                }
                ManageDomain.BLL.ManagerBll managerbll = new ManageDomain.BLL.ManagerBll();
                var model = managerbll.LoginIn(loginname, loginpwd);
                if (model == null)
                {
                    throw new Exception("登录失败！");
                }
                ManageDomain.Entity.LoginTokenModel tokenmodel = new ManageDomain.Entity.LoginTokenModel()
                {
                    Id = model.ManagerId,
                    Name = model.Name,
                    SubName = model.SubName ?? ""
                };
                string name = ManageDomain.Pub.SerializeObject(tokenmodel);
                FormsAuthentication.SetAuthCookie(name, false, "/");

                ManageDomain.PermissionProvider.InitManagerKeys(model.ManagerId);
                new ManageDomain.BLL.OperationLogBll().AddLog(new ManageDomain.Models.OperationLog()
                {
                    Createtime = DateTime.Now,
                    Module = "登录",
                    OperationName = model.Name,
                    OperationTitle = "用户登录",
                    OperationContent = model.Name + "登录"
                });
                return RedirectToAction("index", "home");
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            if (Token != null)
                ManageDomain.PermissionProvider.RemoveManagerKeys(CCF.DB.LibConvert.ObjToInt(Token.Id));
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Profile()
        {
            ManageDomain.BLL.ManagerBll bll = new ManageDomain.BLL.ManagerBll();
            var model = bll.GetManagerDetail(Token.Id);
            ViewBag.manager = model;
            return View();
        }


        [HttpPost]
        public ActionResult Profile(string oldpwd, string newpwd, string newpwd2)
        {
            oldpwd = (oldpwd ?? "").Trim();
            newpwd = (newpwd ?? "").Trim();
            newpwd2 = (newpwd2 ?? "").Trim();
            if (newpwd != newpwd2)
            {
                ViewBag.msg = "两次密码不一致！";
                return View();
            }
            ManageDomain.BLL.ManagerBll bll = new ManageDomain.BLL.ManagerBll();
            try
            {
                bool isok = bll.ChangePwd(Token.Id, oldpwd, newpwd);
                ViewBag.msg = "修改成功";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
    }
}
