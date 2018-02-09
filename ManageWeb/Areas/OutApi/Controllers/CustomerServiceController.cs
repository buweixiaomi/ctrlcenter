using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageWeb.Areas.OutApi.Controllers
{
    public class CustomerServiceController : OutApiBaseController
    {
        //
        // GET: /OutApi/CustomerService/
        static object feedbacklock = new object();
        [ValidateInput(false)]
        public JsonResult FeedBack(Models.FeedBackData model)
        {
            lock (feedbacklock)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.5));
                if (model == null)
                {
                    return JsonError("无效参数！");
                }
                int cusid = 0;
                string cusname = "";
                if (!string.IsNullOrWhiteSpace(model.CusNo))
                {
                    var cusmodel = new ManageDomain.BLL.CustomerBll().GetCusDetailByCusNo(model.CusNo);
                    if (cusmodel != null)
                    {
                        cusid = cusmodel.CusId;
                        cusname = cusmodel.CustomerName;
                    }
                }

                model.Title = model.Title ?? "";
                model.Content = model.Content ?? "";
                if (string.IsNullOrWhiteSpace(model.Content))
                {
                    return JsonError("请填写反馈内容！");
                }
                ManageDomain.BLL.FeedbackBll fbbll = new ManageDomain.BLL.FeedbackBll();
                fbbll.Add(new ManageDomain.Models.Feedback()
                {
                    Content = model.Content,
                    Title = string.IsNullOrEmpty(model.Title) ? "[无标题]" : model.Title,
                    cusId = cusid,
                    CusName = cusname,
                    FromSource = 1,
                    ManagerId = 0,
                    CheckManagerId = 0,
                    CheckManagerName = "",
                    CheckRemark = "",
                    CheckTime = null,
                    CreateTime = DateTime.Now,
                    FeedbackId = 0,
                    FeedbackType = model.FeedbackType,
                    LastProcessTime = null,
                    ManagerName = "",
                    Remark = "",
                    State = 0,
                    WorkItemId = 0
                });
            }
            return JsonE("提示成功，谢谢你的反馈。");
        }

    }
}
