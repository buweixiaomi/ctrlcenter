using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDomain
{
   
   public  enum COMMANDTYPE
    {
       /// <summary>
       /// 发布项目
       /// </summary>
       [Description("publishapp")]
        PUBLISHAPP,
       /// <summary>
       /// 备份项目
       /// </summary>
       [Description("backupapp")]
       BACKUPAPP,
       /// <summary>
       /// 回退项目
       /// </summary>
       [Description("rollbackapp")]
       ROLLBACKAPP,

       /// <summary>
       /// 更新配置
       /// </summary>
       [Description("updateconfig")]
       UPDATECONFIG,

       /// <summary>
       /// CMD
       /// </summary>
       [Description("cmd")]
       CMD,
       /// <summary>
       /// 运行任务
       /// </summary>
       [Description("starttask")]
       STARTTASK,

       /// <summary>
       /// 停止任务
       /// </summary>
       [Description("stoptask")]
       STOPTASK,
        
       /// <summary>
       /// 卸载任务
       /// </summary>
       [Description("deletetask")]
       DELETETASK,
    }
}
