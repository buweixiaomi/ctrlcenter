using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain
{
    public enum SystemPermissionKey
    {
        None,


        [PermissionKey("", "员工管理", "")]
        Manager,
        [PermissionKey("manager.add", "添加员工", "员工管理")]
        Manager_Add,
        [PermissionKey("manager.delete", "删除员工", "员工管理")]
        Manager_Delete,
        [PermissionKey("manager.show", "查看员工", "员工管理")]
        Manager_Show,
        [PermissionKey("manager.update", "修改员工", "员工管理")]
        Manager_Update,

        [PermissionKey("", "分组标签", "")]
        UserTag,
        [PermissionKey("usertag.show", "查看分组标签", "分组标签")]
        UserTag_Show,
        [PermissionKey("usertag.add", "添加分组标签", "分组标签")]
        UserTag_Add,
        [PermissionKey("usertag.delete", "删除分姐标签", "分组标签")]
        UseTag_Delete,

        [PermissionKey("", "客户管理", "")]
        Customer,
        [PermissionKey("customer.add", "添加客户", "客户管理")]
        Customer_Add,
        [PermissionKey("customer.delete", "删除客户", "客户管理")]
        Customer_Delete,
        [PermissionKey("customer.show", "查看客户", "客户管理")]
        Customer_Show,
        [PermissionKey("customer.update", "修改客户", "客户管理")]
        Customer_Update,

        //[PermissionKey("feedback.show", "查看客户反馈问题", "客户管理")]
        //FeedBack_Show,
        //[PermissionKey("feedback.edit", "修改反馈问题", "客户管理")]
        //FeedBack_Update,
        //[PermissionKey("feedback.add", "添加反馈问题", "客户管理")]
        //FeedBack_Add,


        [PermissionKey("", "客户服务记录", "")]
        Customer_ServiceLog,
        [PermissionKey("customer.servicelog.show", "查看服务记录", "客户服务记录")]
        Customer_ServiceLog_Show,
        [PermissionKey("customer.servicelog.add", "添加服务记录", "客户服务记录")]
        Customer_ServiceLog_Add,
        [PermissionKey("customer.servicelog.delete", "删除服务记录", "客户服务记录")]
        Customer_ServiceLog_Delete,
        [PermissionKey("customer.probation.show", "查看18直播试用记录", "客户服务记录")]
        Customer_Probation_Show,


        [PermissionKey("", "客户反馈列表", "")]
        Customer_Feedback,
        [PermissionKey("customer.feedback.show", "查看客户反馈信息", "客户反馈列表")]
        Customer_Feedback_Show,
        [PermissionKey("customer.feedback.add", "添加客户反馈信息", "客户反馈列表")]
        Customer_Feedback_Add,
        [PermissionKey("customer.feedback.update", "修改客户反馈信息", "客户反馈列表")]
        Customer_Feedback_Update,
        [PermissionKey("customer.feedback.delete", "删除客户反馈信息", "客户反馈列表")]
        Customer_Feedback_Delete,
        [PermissionKey("customer.feedback.check", "审核客户反馈信息", "客户反馈列表")]
        Customer_Feedback_Check,

        [PermissionKey("", "服务器管理", "")]
        Server,
        [PermissionKey("server.show", "查看服务器", "服务器管理")]
        Server_Show,
        [PermissionKey("server.add", "添加服务器", "服务器管理")]
        Server_Add,
        [PermissionKey("server.update", "修改服务器", "服务器管理")]
        Server_Update,
        [PermissionKey("server.delete", "删除服务器", "服务器管理")]
        Server_Delete,
        [PermissionKey("operationlog.show", "查看操作日志", "服务器管理")]
        OperationLog_Show,


        [PermissionKey("", "开发项目管理", "")]
        Project,
        [PermissionKey("project.show", "查看项目", "开发项目管理")]
        Project_Show,
        [PermissionKey("project.add", "添加项目", "开发项目管理")]
        Project_Add,
        [PermissionKey("project.update", "修改项目", "开发项目管理")]
        Project_Update,
        [PermissionKey("project.delete", "删除项目", "开发项目管理")]
        Project_Delete,
        
        [PermissionKey("", "服务器项目管理", "")]
        ServerProject,
        [PermissionKey("serverproject.show", "查看服务器项目", "服务器项目管理")]
        ServerProject_Show,
        [PermissionKey("serverproject.add", "添加服务器项目", "服务器项目管理")]
        ServerProject_Add,
        [PermissionKey("serverproject.update", "更新服务器项目", "服务器项目管理")]
        ServerProject_Update,
        [PermissionKey("serverproject.delete", "删除服务器项目", "服务器项目管理")]
        ServerProject_Delete,

        [PermissionKey("", "命令管理", "")]
        Command,
        [PermissionKey("command.show", "查看命令", "命令管理")]
        Command_Show,
        [PermissionKey("command.add", "添加命令", "命令管理")]
        Command_Add,
        [PermissionKey("command.delete", "删除命令", "命令管理")]
        Command_Delete,


        [PermissionKey("", "工作任务", "")]
        WorkItem,
        [PermissionKey("workitem.show", "查看工作任务", "工作任务")]
        WorkItem_Show,
        [PermissionKey("workitem.add", "添加工作任务", "工作任务")]
        WorkItem_Add,
        [PermissionKey("workitem.update", "更新工作任务", "工作任务")]
        WorkItem_Update,
        [PermissionKey("workitem.delete", "删除工作任务", "工作任务")]
        WorkItem_Delete,
        [PermissionKey("workitem.summary", "任务汇总", "工作任务")]
        WorkItem_summary,

        [PermissionKey("workitem.execwork", "执行工作任务", "工作任务")]
        WorkItem_ExecWork,

        [PermissionKey("", "工作日志", "")]
        WorkDaily,
        [PermissionKey("workdaily.show", "查看工作日志", "工作日志")]
        WorkDaily_Show,
        [PermissionKey("workdaily.showother", "查看别人工作日志", "工作日志")]
        WorkDaily_ShowOther,
        [PermissionKey("workdaily.add", "添加工作日志", "工作日志")]
        WorkDaily_Add,
        [PermissionKey("workdaily.update", "修改工作日志", "工作日志")]
        WorkDaily_Update,
        [PermissionKey("workdaily.delete", "删除工作日志", "工作日志")]
        WorkDaily_Delete,
        [PermissionKey("workdaily.deleteother", "删除别人的工作日志", "工作日志")]
        WorkDaily_DeleteOther,
        [PermissionKey("workdaily.report", "工作日志报表", "工作日志")]
        WorkDaily_Report,

        [PermissionKey("", "监控日志", "")]
        WatchLog,
        [PermissionKey("watchlog.commlog", "普通日志", "监控日志")]
        WatchLog_CommLog,
        [PermissionKey("watchlog.errorlog", "错误日志", "监控日志")]
        WatchLog_ErrorLog,
        [PermissionKey("watchlog.timewatch", "耗时日志", "监控日志")]
        WatchLog_TimeWatch,

        [PermissionKey("", "任务管理", "")]
        Task,
        [PermissionKey("task.show", "查看任务", "任务管理")]
        Task_Show,
        [PermissionKey("task.add", "添加任务", "任务管理")]
        Task_Add,
        [PermissionKey("task.update", "修改任务", "任务管理")]
        Task_Update,
        [PermissionKey("task.delete", "删除任务", "任务管理")]
        Task_Delete,

        [PermissionKey("Performance.show", "查看服务器性能", "")]
        Performance,

        [PermissionKey("webexception.show", "显示网站操作异常", "")]
        ShowWebException,




    }

    public class PermissionKeyAttribute : Attribute
    {
        public string Key { get; private set; }
        public string Name { get; private set; }

        public string Group { get; private set; }
        public string ParentGroup { get; private set; }
        public PermissionKeyAttribute(string key, string name)
            : this(key, name, "")
        {
        }

        public PermissionKeyAttribute(string key, string name, string groupname)
        {
            this.Key = key;
            this.Name = name;
            this.Group = groupname ?? "";
        }
    }
}
