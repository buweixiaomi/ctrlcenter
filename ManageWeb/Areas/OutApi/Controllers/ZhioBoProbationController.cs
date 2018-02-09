using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace ManageWeb.Areas.OutApi.Controllers
{
    public class ZhioBoProbationController : OutApiBaseController
    {
        //
        // GET: /OutApi/ZhioBoProbation/
        ManageDomain.BLL.ZhiBoProbationBll BLL = new ManageDomain.BLL.ZhiBoProbationBll();
        public JsonResult Probation(ManageDomain.Models.ZhiBoProbation model)
        {
            if (model == null)
            {
                return Json(new JsonEntity { code = -1, msg = "参数无效" });
            }
            if (string.IsNullOrWhiteSpace(model.Profession))
            {
                return Json(new JsonEntity { code = -1, msg = "请填写所属行业" });
            }
            if (string.IsNullOrWhiteSpace(model.Mobile))
            {
                return Json(new JsonEntity { code = -1, msg = "请填写手机号" });
            }
            var result = BLL.Add(new ManageDomain.Models.ZhiBoProbation
            {
                Name = model.Name ?? "",
                Profession = model.Profession,
                Mobile = model.Mobile,
                CompanyNum = model.CompanyNum,
                QQ = model.QQ ?? ""
            });
            return Json(new JsonEntity { code = 1, msg = "成功" });
        }

    }
}
