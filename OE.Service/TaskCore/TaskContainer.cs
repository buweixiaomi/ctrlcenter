using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Core;

namespace OE.Service.TaskCore
{
    public class TaskContainer
    {
        public List<TaskItemContent> Tasks = new List<TaskItemContent>();
        ServiceTaskContext context = new ServiceTaskContext();
        Quartz.IScheduler Scheduler = null;
        private object taskslock = new object();
        private static TaskContainer instance;
        private bool istarting = false;
        private object configlock = new object();
        public bool TaskConfigIsRuning { get; private set; }
        static TaskContainer()
        {
            instance = new TaskContainer();
        }

        private TaskContainer()
        {
            var ssf = new StdSchedulerFactory();
            Scheduler = ssf.GetScheduler();
            Scheduler.Start();
        }
        public static TaskContainer Instance()
        {
            return instance;
        }

        public bool Init()
        {
            List<int[]> tasks = new List<int[]>();
            ApiSdk.TaskApi taskapi = new ApiSdk.TaskApi();
            var result = taskapi.GetTasksSummary();
            if (result.code > 0)
            {
                tasks.AddRange(result.data.Select(x => new int[] { x.TaskID, x.State }).ToList());
            }

            if (!tasks.Exists(x => x[0] == OE.Service.Tasks.PerformanceTask.STATIC_TASK_ID))
            {
                tasks.Add(new int[] { OE.Service.Tasks.PerformanceTask.STATIC_TASK_ID, Configrations.Config.GetUnionConfig(Configrations.ConfigConst.Performance_Run_Name, "true").ToLower() == "false" ? 0 : 1 });
            }

            Task tk = ResumeTask(tasks);
            if (tk != null)
            {
                try { tk.Wait(); }
                finally { }
            }
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        new Commands.Task.UploadTasksStateCommand().Exec(null);
                        CCF.WatchLog.Loger.Log("任务心跳", "完成心跳");
                    }
                    catch (Exception ex)
                    {
                        CCF.WatchLog.Loger.Error("任务心跳失败", ex);
                    }
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(15));
                }
            });
            TaskConfigIsRuning = true;
            return true;
        }

        private Task ResumeTask(List<int[]> task_states)
        {
            if (istarting == true)
                return null;
            CCF.WatchLog.Loger.Log("ResumeTask", "正在恢复任务");
            var systask = System.Threading.Tasks.Task.Factory.StartNew(() =>
           {
               try
               {
                   istarting = true;
                   lock (taskslock)
                   {
                       //删除不用的Task
                       foreach (var a in GetConfigItems())
                       {
                           var servertask = task_states.FirstOrDefault(x => x[0] == a.TaskID);
                           if (servertask == null)
                           {
                               Delete(a.TaskID);
                           }
                       }
                       try
                       {
                           foreach (var a in task_states)
                           {
                               switch (a[1])
                               {
                                   case -1:
                                       CCF.WatchLog.Loger.Log("ResumeTask", "Delete" + a[0]);
                                       Delete(a[0]);
                                       break;
                                   case 0:
                                       CCF.WatchLog.Loger.Log("ResumeTask", "Stop" + a[0]);
                                       Stop(a[0]);
                                       break;
                                   case 1:
                                       CCF.WatchLog.Loger.Log("ResumeTask", "Start" + a[0]);
                                       Start(a[0]);
                                       break;
                                   default:
                                       break;
                               }
                           }
                       }
                       catch { }
                   }
                   SaveConfigItems(Tasks.Select(x => x.TaskConfig).ToList());
               }
               finally
               {
                   istarting = false;
               }
           });
            return systask;
        }

        public bool Delete(int ID)
        {
            lock (taskslock)
            {
                CCF.WatchLog.Loger.Log("删除任务:" + ID, "");
                Stop(ID);
                var cacheitems = GetConfigItems();
                var a = cacheitems.FirstOrDefault(x => x.TaskID == ID);
                if (a != null)
                {
                    int ind = a.Dll.LastIndexOf('.');
                    if (ind <= 0)
                        return true;
                    string taskdllpath = GetTaskPath(a); // AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + a.TaskID + "_" + a.Dll.Substring(0, ind) + "\\";
                    if (System.IO.Directory.Exists(taskdllpath))
                    {
                        System.IO.Directory.Delete(taskdllpath, true);
                    }
                    cacheitems.Remove(a);
                    SaveConfigItems(cacheitems);
                }

                try
                {
                    new Commands.Task.UploadTasksStateCommand().Exec(null);
                }
                catch (Exception ex) { }
                return true;
            }
        }

        public bool Start(int ID)
        {
            lock (taskslock)
            {
                CCF.WatchLog.Loger.Log("开始任务:" + ID, "");
                var task = Tasks.FirstOrDefault(x => x.TaskID == ID);
                if (task != null)
                    return true;
                task = PrepareTask(ID);
                if (task == null)
                {
                    throw new ServiceException(ServiceErrorCode.Error, "初始化任务失败！");
                }
                try
                {
                    AppDomainSetup setup = new AppDomainSetup();
                    //   setup.ApplicationName = "task_" + task.TaskConfig.Name;
                    setup.ApplicationBase = task.BaseDir;
                    //    setup.PrivateBinPath = Path.Combine(task.BaseDir, "");
                    //  setup.CachePath = setup.ApplicationBase;
                    //setup.ShadowCopyDirectories = AppDomain.CurrentDomain.BaseDirectory;
                    setup.ShadowCopyFiles = "true";
                    if (System.IO.File.Exists(Path.Combine(task.BaseDir, task.TaskConfig.Dll) + ".config"))
                    {
                        setup.ConfigurationFile = Path.Combine(task.BaseDir, task.TaskConfig.Dll) + ".config";
                    }
                    task.TaskDomain = AppDomain.CreateDomain("task_", null, setup);
                    AppDomain.MonitoringIsEnabled = true;
                    //task.TaskDomain = AppDomain.CreateDomain("task_" + task.TaskID, null, setup);

                    var taskinstance = (CCF.Task.TaskBase)task.TaskDomain.CreateInstanceFromAndUnwrap(Path.Combine(task.BaseDir, task.TaskConfig.Dll), task.TaskConfig.ClassFullName);
                    try
                    {
                        taskinstance.InitGlobalConfig(CCF.Utils.DataSerialize.SerializeJson(Configrations.Config.unionConfig));
                        taskinstance.InitTaskConfig(task.TaskConfig.ConfigJson);
                        taskinstance.Init(new object[] { Configrations.Config.ClientID, Utils.Utils.SerializeObject(Configrations.Config.unionConfig) });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Init时出错", ex);
                    }
                    task.Task = taskinstance;
                    JobDetailImpl jobdetail = new JobDetailImpl("job_" + task.TaskID, typeof(TaskJob));
                    jobdetail.Description = task.TaskID.ToString();
                    Quartz.ITrigger triger = null;
                    if (task.TaskConfig.IsRunOnce || (task.TaskConfig.RunCron ?? "").ToLower() == "runonce")
                    {
                        task.TaskConfig.IsRunOnce = true;
                        var ttriger = new Quartz.Impl.Triggers.SimpleTriggerImpl("trigger_" + task.TaskID.ToString());
                        ttriger.RepeatCount = 1;
                        ttriger.RepeatInterval = TimeSpan.FromSeconds(1);
                        triger = ttriger;
                    }
                    else
                    {
                        var ttriger = new Quartz.Impl.Triggers.CronTriggerImpl("trigger_" + task.TaskID.ToString());
                        ttriger.CronExpressionString = task.TaskConfig.RunCron;
                        triger = ttriger;
                    }
                    if (!Tasks.Contains(task))
                        Tasks.Add(task);
                    var cacheitems = GetConfigItems();
                    if (!cacheitems.Exists(x => x.TaskID == ID))
                    {
                        cacheitems.Add(task.TaskConfig);
                        SaveConfigItems(cacheitems);
                    }
                    this.Scheduler.ScheduleJob(jobdetail, triger);

                    try
                    {
                        new Commands.Task.UploadTasksStateCommand().Exec(null);
                    }
                    catch (Exception ex) { }
                    return true;
                }
                catch (Exception ex)
                {
                    CCF.WatchLog.Loger.Error("启动任务失败", ex);
                    return false;
                }
            }
        }
        public bool Stop(int ID)
        {
            lock (taskslock)
            {
                CCF.WatchLog.Loger.Log("停止任务:" + ID, "");
                var task = Tasks.FirstOrDefault(x => x.TaskID == ID);
                if (task == null)
                    return false;
                try
                {
                    try
                    {
                        task.Task.Stop();
                        task.Task.IsRunning = false;
                    }
                    catch (Exception ex)
                    {
                        CCF.WatchLog.Loger.Error(" task.Task.Stop() 失败" + ID, ex);
                    }
                    try
                    {
                        Scheduler.PauseTrigger(new TriggerKey("trigger_" + ID));// 停止触发器
                    }
                    catch { }
                    try
                    {
                        Scheduler.UnscheduleJob(new TriggerKey("trigger_" + ID));// 移除触发器  
                    }
                    catch { }
                    try
                    {
                        Scheduler.DeleteJob(new JobKey("job_" + ID));// 删除任务
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    CCF.WatchLog.Loger.Error("停止任务失败1 stop" + ID, ex);
                }
                finally
                {
                    try
                    {
                        Tasks.Remove(task);
                        AppDomain.Unload(task.TaskDomain);
                    }
                    catch (Exception ex)
                    {
                        CCF.WatchLog.Loger.Error("停止任务失败2 Unload" + ID, ex);
                    }
                }
            }
            try
            {
                new Commands.Task.UploadTasksStateCommand().Exec(null);
            }
            catch (Exception ex) { }
            return true;
        }

        public TaskItemContent PrepareTask(int ID)
        {
            if (ID <= 0)
            {
                var localtask = TryLoadLocalTask(ID);
                if (localtask != null)
                {
                    localtask.TaskID = localtask.TaskConfig.TaskID;
                }
                return localtask;
            }
            try
            {
                #region 得到任务信息
                ApiSdk.TaskApi taskapi = new ApiSdk.TaskApi();
                var rdata = taskapi.GetTaskDetail(ID);
                if (rdata.code <= 0)
                {
                    throw new ServiceException(ServiceErrorCode.Error, "得取任务详情失败！");
                }
                var item = new TaskItemContent();
                item.TaskID = ID;
                item.TaskConfig = new TaskItem()
                {
                    ClassFullName = rdata.data.ClassFullName,
                    ConfigJson = rdata.data.ConfigJson,
                    Dll = rdata.data.Dll,
                    DownloadFileName = rdata.data.DownloadFileName,
                    LastServerState = rdata.data.LastServerState,
                    RunCron = rdata.data.RunCron,
                    TaskID = rdata.data.TaskID,
                    Url = rdata.data.Url,
                    Name = rdata.data.Name,
                    Remark = rdata.data.Remark
                };
                #endregion

                int ind = item.TaskConfig.Dll.LastIndexOf('.');
                if (ind <= 0)
                {
                    throw new ServiceException(ServiceErrorCode.Error, "启动dll文件类型错误！");
                }
                string taskdllpath = GetTaskPath(item.TaskConfig);// AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\TasksDll\\" + item.TaskConfig.TaskID + "_" + item.TaskConfig.Dll.Substring(0, ind) + "\\";
                string taskstartdllfullname = taskdllpath + item.TaskConfig.Dll;

                #region 下载任务文件
                if (!System.IO.File.Exists(taskstartdllfullname))
                {
                    //ApiSdk.TaskApi taskapi = new ApiSdk.TaskApi();
                    var rdata2 = new ApiSdk.CommApi().DownloadFile(item.TaskConfig.Url);
                    if (rdata2.code <= 0)
                    {
                        throw new ServiceException(ServiceErrorCode.Error, "下载任务文件失败:" + rdata.msg);
                    }
                    string fillename = Configrations.Config.GetTempFileName();
                    using (var filestream = new System.IO.FileStream(fillename, System.IO.FileMode.CreateNew))
                    {
                        filestream.Write(rdata2.data, 0, rdata2.data.Length);
                        filestream.Flush();
                    }
                    if (!System.IO.Directory.Exists(taskdllpath))
                        System.IO.Directory.CreateDirectory(taskdllpath);
                    Utils.Utils.UnZip(fillename, taskdllpath, "", true);
                    System.IO.File.Delete(fillename);
                }
                #endregion
                item.BaseDir = taskdllpath;
                return item;
            }
            catch (Exception ex)
            {
                throw new ServiceException(ServiceErrorCode.Error, "初始化任务失败:" + ex.Message);
            }
        }

        private string GetTaskConfigPath()
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + Configrations.ConfigConst.TaskConfigFileName;
            return filepath;
        }

        private string GetTaskPath(TaskItem ti)
        {
            int ind = ti.Dll.LastIndexOf('.');
            string taskdllpath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\TasksDll\\" + ti.TaskID + "_" + ti.Dll.Substring(0, ind) + "\\";
            return taskdllpath;
        }

        private List<TaskItem> GetConfigItems()
        {
            string filepath = GetTaskConfigPath();
            if (!System.IO.File.Exists(filepath))
                return new List<TaskItem>();
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TaskItem>>(System.IO.File.ReadAllText(filepath, Encoding.UTF8));
            return model;
        }

        private void SaveConfigItems(List<TaskItem> configs)
        {
            lock (configlock)
            {
                if (configs == null)
                    configs = new List<TaskItem>();
                System.IO.File.WriteAllText(GetTaskConfigPath(), Newtonsoft.Json.JsonConvert.SerializeObject(configs), Encoding.UTF8);
            }
        }

        private TaskItemContent TryLoadLocalTask(int ID)
        {
            if (ID == OE.Service.Tasks.PerformanceTask.STATIC_TASK_ID)
            {
                var pertask = new TaskItemContent();
                pertask.TaskConfig = new TaskItem()
                {
                    ClassFullName = typeof(OE.Service.Tasks.PerformanceTask).FullName,
                    ConfigJson = "",
                    Dll = typeof(OE.Service.Tasks.PerformanceTask).Assembly.ManifestModule.Name,
                    DownloadFileName = "",
                    IsRunOnce = false,
                    LastServerState = 1,
                    Name = typeof(OE.Service.Tasks.PerformanceTask).Name,
                    Remark = "",
                    RunCron = Configrations.Config.GetUnionConfig(Configrations.ConfigConst.Performance_RunCorn_Name, Configrations.ConfigConst.Performance_RunCorn_Default),
                    TaskID = ID,
                    Url = ""
                };

                string taskdllpath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\TasksDll\\" + pertask.TaskConfig.TaskID + "_" + pertask.TaskConfig.Dll.Substring(0, pertask.TaskConfig.Dll.LastIndexOf('.')) + "\\";
                if (System.IO.Directory.Exists(taskdllpath))
                {
                    System.IO.Directory.Delete(taskdllpath, true);
                }
                string taskstartdllfullname = taskdllpath + pertask.TaskConfig.Dll;

                if (!System.IO.File.Exists(taskstartdllfullname))
                {
                    Utils.Utils.CopyDir(AppDomain.CurrentDomain.BaseDirectory, taskdllpath, false);
                }
                pertask.BaseDir = taskdllpath;// AppDomain.CurrentDomain.BaseDirectory;
                return pertask;
            }
            return null;
        }

        public void ExistService()
        {
            try
            {
                CCF.WatchLog.Loger.Log("正在退出所有任务...", "");
                foreach (var a in Tasks.Select(x => x.TaskID).ToList())
                {
                    try
                    {
                        Stop(a);
                    }
                    catch { }
                }
                CCF.WatchLog.Loger.Log("退出所有任务完成", "");

                CCF.WatchLog.Loger.Log("关闭任务容器...", "");
                Scheduler.Shutdown(false);
                CCF.WatchLog.Loger.Log("关闭任务容器成功", "");
            }
            catch (Exception ex)
            {
                CCF.WatchLog.Loger.Error("关闭任务调试出错", ex);
            }
        }
    }
}
