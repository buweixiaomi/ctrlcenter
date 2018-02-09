using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManageDomain;

namespace ManageWeb
{
    public class MenuBar
    {
        public static List<ManageDomain.MenuGroup> Menu = new List<ManageDomain.MenuGroup>();
        static MenuBar()
        {
            Menu.Add(
                new ManageDomain.MenuGroup()
            {
                Title = "员工管理",
                Items = new List<ManageDomain.MenuItem>(){
               new ManageDomain.MenuItem(){ Text = "员工列表",  PermissionKey =  PermissionProvider.GetPermissionKey( SystemPermissionKey.Manager_Show),Url = "/manager/index" , UrlKey = "manager.index"},
                new ManageDomain.MenuItem(){ Text = "新增/编辑员工",  PermissionKey =  PermissionProvider.GetPermissionKey(SystemPermissionKey.Manager_Add)+","+ PermissionProvider.GetPermissionKey(SystemPermissionKey.Manager_Update),Url = "/manager/edit" , UrlKey = "manager.edit"},
                new ManageDomain.MenuItem(){ Text = "分组标签管理",  PermissionKey =  PermissionProvider.GetPermissionKey(SystemPermissionKey.UserTag_Show),Url = "/manager/tag" , UrlKey = "manager.tag"}
              }
            });
            Menu.Add(
                new ManageDomain.MenuGroup()
            {
                Title = "客户管理",
                Items = new List<ManageDomain.MenuItem>(){
               new ManageDomain.MenuItem(){ Text = "客户列表",  PermissionKey =  PermissionProvider.GetPermissionKey( SystemPermissionKey.Customer_Show),Url = "/customer/index" , UrlKey = "customer.index"},
                new ManageDomain.MenuItem(){ Text = "新增/编辑客户",  PermissionKey =  PermissionProvider.GetPermissionKey(SystemPermissionKey.Customer_Add)+","+ PermissionProvider.GetPermissionKey( SystemPermissionKey.Customer_Update),Url = "/customer/edit" , UrlKey = "customer.edit"},
                new ManageDomain.MenuItem(){ Text = "客户服务记录",  PermissionKey =  PermissionProvider.GetPermissionKey(SystemPermissionKey.Customer_ServiceLog_Show),Url = "/cusservice/index" , UrlKey = "cusservice.index"},
                  new ManageDomain.MenuItem(){ Text = "18直播试用记录",  PermissionKey =  PermissionProvider.GetPermissionKey(SystemPermissionKey.Customer_Probation_Show),Url = "/zhiboprobation/index" , UrlKey = "zhiboprobation.index"},
                
              }
            });
            Menu.Add(
                new ManageDomain.MenuGroup()
            {
                Title = "服务器管理",
                Items = new List<ManageDomain.MenuItem>(){
               new ManageDomain.MenuItem(){ Text = "服务器列表",  PermissionKey =  PermissionProvider.GetPermissionKey( SystemPermissionKey.Server_Show),Url = "/servermachine/index" , UrlKey = "servermachine.index"},
                new ManageDomain.MenuItem(){ Text = "开发项目列表",  PermissionKey =  PermissionProvider.GetPermissionKey(SystemPermissionKey.Project_Show) ,Url = "/project/index" , UrlKey = "project.index"},
                new ManageDomain.MenuItem(){ Text = "服务器项目管理",  PermissionKey =  PermissionProvider.GetPermissionKey(SystemPermissionKey.ServerProject_Show),Url = "/serverproject/index" , UrlKey = "serverproject.index"},
                new ManageDomain.MenuItem(){ Text = "命令操作",  PermissionKey =  PermissionProvider.GetPermissionKey(SystemPermissionKey.Command_Add),Url = "/cmd/operate" , UrlKey = "cmd.operate"},
                new ManageDomain.MenuItem(){ Text = "命令列表",  PermissionKey =  PermissionProvider.GetPermissionKey(SystemPermissionKey.Command_Show),Url = "/cmd/index" , UrlKey = "cmd.index"},
              //   new ManageDomain.MenuItem(){ Text = "操作日志列表",  PermissionKey =  PermissionProvider.GetPermissionKey(SystemPermissionKey.OperationLog_Show),Url = "/operationlog/index" , UrlKey = "operationlog.index"},
                 new ManageDomain.MenuItem(){ Text = "性能监控",  PermissionKey =  PermissionProvider.GetPermissionKey(SystemPermissionKey.Performance),Url = "/performance/index" , UrlKey = "performance.index,performance.detail"},
                   new ManageDomain.MenuItem(){ Text = "任务列表",  PermissionKey =  PermissionProvider.GetPermissionKey(SystemPermissionKey.Task_Show),Url = "/taskdll/index" , UrlKey = "taskdll.index"}
              }
            }
            );


            Menu.Add(
                new ManageDomain.MenuGroup()
                {
                    Title = "工作管理",
                    Items = new List<ManageDomain.MenuItem>(){
                new ManageDomain.MenuItem(){ Text = "客户反馈列表",  PermissionKey =  PermissionProvider.GetPermissionKey( SystemPermissionKey.Customer_Feedback_Show),Url = "/feedback/index" , UrlKey = "feedback.index"},
                new ManageDomain.MenuItem(){ Text = "工作任务",  PermissionKey =  PermissionProvider.GetPermissionKey( SystemPermissionKey.WorkItem_Show),Url = "/workitem/index" , UrlKey = "workitem.index"},
                new ManageDomain.MenuItem(){ Text = "工作任务汇总",  PermissionKey =  PermissionProvider.GetPermissionKey( SystemPermissionKey.WorkItem_summary),Url = "/workitem/summary" , UrlKey = "workitem.summary"},
                new ManageDomain.MenuItem(){ Text = "工作日志",  PermissionKey =  PermissionProvider.GetPermissionKey( SystemPermissionKey.WorkDaily_Show),Url = "/workdaily/index" , UrlKey = "workdaily.index"},
                new ManageDomain.MenuItem(){ Text = "添加工作日志",  PermissionKey =  PermissionProvider.GetPermissionKey( SystemPermissionKey.WorkDaily_Add),Url = "/workdaily/add" , UrlKey = "workdaily.add"},
                new ManageDomain.MenuItem(){ Text = "工作日志报表",  PermissionKey =  PermissionProvider.GetPermissionKey( SystemPermissionKey.WorkDaily_Report),Url = "/workdaily/report" , UrlKey = "workdaily.report"}
              }
                }
            );
            Menu.Add(
             new ManageDomain.MenuGroup()
             {
                 Title = "监控日志",
                 Items = new List<ManageDomain.MenuItem>(){
                new ManageDomain.MenuItem(){ Text = "普通日志",  PermissionKey =  PermissionProvider.GetPermissionKey( SystemPermissionKey.WatchLog_CommLog),Url = "/WatchLog/CommLog" , UrlKey = "watchlog.commlog"},
                new ManageDomain.MenuItem(){ Text = "错误日志",  PermissionKey =  PermissionProvider.GetPermissionKey( SystemPermissionKey.WatchLog_ErrorLog),Url = "/WatchLog/ErrorLog" , UrlKey = "watchlog.errorlog"},
                new ManageDomain.MenuItem(){ Text = "耗时日志",  PermissionKey =  PermissionProvider.GetPermissionKey( SystemPermissionKey.WatchLog_TimeWatch),Url = "/WatchLog/TimeWatch" , UrlKey = "watchlog.timewatch"},        
                new ManageDomain.MenuItem(){ Text = "耗时分析",  PermissionKey = "",Url = "/WatchLog/TimeWatchAna" , UrlKey = ""}               
              }
             }
         );
        }

        public static HtmlString GetUserMenu(string urlkey)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            int id = PermissionProvider.GetManagerId();
            var rdata = GetUserMenu(id, urlkey);
            sw.Stop();
            System.Diagnostics.Trace.WriteLine("用时:" + sw.Elapsed.TotalMilliseconds + "ms;");
            return rdata;
        }

        public static HtmlString GetUserMenu(int userid, string urlkey)
        {
            List<ManageDomain.MenuGroup> currmenu = new List<MenuGroup>();
            #region
            foreach (var a in Menu)
            {
                var currgroup = new MenuGroup() { Title = a.Title, Items = new List<MenuItem>() };
                foreach (var b in a.Items)
                {
                    if (string.IsNullOrEmpty(b.PermissionKey))
                    {
                        currgroup.Items.Add(b);
                    }
                    string[] ks = b.PermissionKey.Split(',');
                    bool allexist = true;
                    foreach (var k in ks)
                    {
                        if (!PermissionProvider.ExistWidthCache(userid, k))
                        {
                            allexist = false;
                            break;
                        }
                    }
                    if (allexist)
                    {
                        currgroup.Items.Add(b);
                    }
                }
                if (currgroup.Items.Count > 0)
                    currmenu.Add(currgroup);
            }
            #endregion
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var a in currmenu)
            {
                sb.AppendLine("<div class=\"cc-menu-group\">");
                sb.AppendLine("\t<p class=\"cc-list-group-header\">" + a.Title + "</p>");
                sb.AppendLine("\t <div class=\"list-group\">");
                foreach (var b in a.Items)
                {
                    sb.AppendFormat("\t\t<a href=\"{0}\" class=\"list-group-item cc-list-group-item{1}\">{2}</a>\r\n", b.Url, b.UrlKey.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).Contains(urlkey.ToLower()) ? " active" : "", b.Text);
                }
                sb.AppendLine("\t </div>");
                sb.AppendLine("</div>");
            }
            return new HtmlString(sb.ToString());
        }

    }
}