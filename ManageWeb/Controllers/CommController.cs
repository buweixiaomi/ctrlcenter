using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace ManageWeb.Controllers
{
    public class CommController : ManageBaseController
    {
        //
        // GET: /Comm/

        public ActionResult Select(string type, int projectid = 0)
        {
            object model = null;
            switch (type)
            {
                case "customer":
                    model = new ManageDomain.BLL.CustomerBll().GetMinCustomers(1000);
                    ViewBag.type = "customer";
                    ViewBag.model = model;
                    return View(model);
                case "server":
                    model = new ManageDomain.BLL.ServerMachineBll().GetMiniServers(1000);
                    ViewBag.type = "server";
                    ViewBag.model = model;
                    return View(model);
                case "project":
                    model = new ManageDomain.BLL.ProjectBll().GetMiniProjects(1000);
                    ViewBag.type = "project";
                    ViewBag.model = model;
                    return View(model);
                case "manager":
                    model = new ManageDomain.BLL.ManagerBll().GetManagerTop(1000);
                    ViewBag.type = "manager";
                    ViewBag.model = model;
                    return View(model);
                case "projectversion":
                    model = new ManageDomain.BLL.ProjectBll().GetProjectVersions(projectid);
                    ViewBag.type = "projectversion";
                    ViewBag.model = model;
                    return View(model);
            }
            return View(model);
        }

        public ActionResult UpdateImg()
        {
            JsonEntity result = new JsonEntity();
            if (Request.Files == null || Request.Files.Count == 0)
            {
                result.code = -1;
                result.msg = "";
                result.data = "";
            }
            else
            {
                HttpPostedFileBase file = Request.Files[0];
                string filename = file.FileName;
                List<byte> bs = new List<byte>();
                int b = 0;
                while ((b = file.InputStream.ReadByte()) != -1)
                {
                    bs.Add((byte)b);
                }
                string outmsg = "";
                bool uploadresult = ManageDomain.FileUpload.Upload(filename, bs.ToArray(), ManageDomain.UpLoadMode.PUBLIC_STATIC, out outmsg);
                if (uploadresult)
                {
                    var jobj = Newtonsoft.Json.Linq.JObject.Parse(outmsg);
                    if (jobj["code"].Value<int>() > 0)
                    {
                        result.code = 1;
                        result.data = ManageDomain.FileUpload.BuildFullUrl(jobj["data"].Value<string>());
                        result.msg = "";
                    }
                    else {
                        result.code = -1;
                        result.data = "出错";
                        result.msg = "出错";
                    }
                }
                else
                {
                    result.code = -1;
                    result.data = outmsg;
                    result.msg = outmsg;
                }
            }
            ViewBag.jsondata = ManageDomain.Pub.SerializeObject(result);
            return View();
        }
    }
}
