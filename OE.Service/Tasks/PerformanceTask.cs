using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace OE.Service.Tasks
{
    public class PerformanceTask : CCF.Task.TaskBase
    {
        public override void Init(object[] args)
        {
            Configrations.Config.ClientID = args[0] as string;
            Configrations.Config.unionConfig = Utils.Utils.DeserializeObject<List<OE.Service.Configrations.ConfigKeyValue>>(args[1] as string);
            base.Init(args);

            base.Log("初始化性能任务");
            string monitorNetworks = "";
            if (this.GlobalConfig.Exists(x => x.Key.ToLower() == "monitorNetworks".ToLower()))
            {
                monitorNetworks = this.GlobalConfig.FirstOrDefault(x => x.Key.ToLower() == "monitorNetworks".ToLower()).Value ?? "";
            }
            try
            {
                counters.Add(new MenoryCounter());
                counters.Add(new DiskSpaceCounter());
                counters.Add(new CPUCounter());
                counters.Add(new NetWorkIOCounter(monitorNetworks));
                counters.Add(new RequestCounter());
                counters.Add(new DiskIO());
                System.Threading.Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                this.Log("初始化性能任务异常:" + ex.Message);
            }
            finally
            {
                this.Log("结束初始化性能任务");
                foreach (var a in counters)
                {
                    if (a.IsEnable == false)
                    {
                        this.Log(a.GetType().Name + "：性能计数顺始化初始化失败:" + a.Msg);
                    }
                }
            }
        }
        public const int STATIC_TASK_ID = -100;
        Dictionary<string, List<CountItem>> perkeys = new Dictionary<string, List<CountItem>>();
        object datalock = new object();

        public List<PerformanceCounterBase> counters = new List<PerformanceCounterBase>();
        public PerformanceTask()
        {
        }
        public override void Run()
        {
            if (this.IsRunning)
                return;
            this.IsRunning = true;
            InterRun();
            this.IsRunning = false;
        }

        private void InterRun()
        {
            try
            {
                base.Log("开始运行性能任务");
                List<CountItem> cis = new List<CountItem>();
                foreach (var a in counters)
                {
                    if (a.IsEnable)
                    {
                        cis.AddRange(a.GetCount());
                    }
                }
                int c = CCF.DB.LibConvert.StrToInt(this.GetTaskConfig("FullCountSend", Configrations.ConfigConst.Performance_FullCountSend_Default.ToString()));
                lock (datalock)
                {
                    perkeys.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), cis);
                    try
                    {
                        if (perkeys.Count >= c)
                        {
                            var uploadresult = new ApiSdk.CommApi().UploadData(Configrations.ConfigConst.PerformanceUploadType, Utils.Utils.SerializeObject(perkeys));
                            if (uploadresult.code > 0)
                            {
                                perkeys.Clear();
                            }
                            else
                            {
                                throw new Exception("上传数据失败！");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        base.Log("上传文件出错:" + ex.Message);
                        if (perkeys.Count >= Configrations.ConfigConst.MAX_CACHE_WAITSEND)
                        {
                            perkeys.Clear();
                        }
                    }
                    finally { }
                }
            }
            catch (Exception ex)
            {
                base.Log("性能任务运行出错:" + ex.Message);
                throw ex;
            }
            finally
            {
                this.Log("结束性能任务。");
            }
        }
    }

    public abstract class PerformanceCounterBase
    {
        protected bool _IsEnable = true;
        public bool IsEnable { get { return _IsEnable; } }
        public abstract List<CountItem> GetCount();
        public string Msg { get; protected set; }
    }
    public class CountItem
    {
        public CountItem()
        {
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public string Timestamp { get; set; }
        public string Key { get; set; }
        public string Subkey { get; set; }
        public double Total { get; set; }
        public double Used { get; set; }
        public double Available { get; set; }
    }

    public class MenoryCounter : PerformanceCounterBase
    {
        ComputerInfo cinfo = new ComputerInfo();
        public override List<CountItem> GetCount()
        {
            CountItem ci = new CountItem();
            ci.Total = cinfo.TotalPhysicalMemory / 1024d / 1024d;
            ci.Available = cinfo.AvailablePhysicalMemory / 1024d / 1024d;
            ci.Used = ci.Total - ci.Available;
            ci.Key = "memory";
            ci.Subkey = "";
            return new List<CountItem>() { ci };
        }

    }

    public class DiskSpaceCounter : PerformanceCounterBase
    {
        List<System.IO.DriveInfo> dvs = new List<System.IO.DriveInfo>();
        public DiskSpaceCounter()
        {
            foreach (var a in System.IO.DriveInfo.GetDrives())
            {
                dvs.Add(a);
            }
        }
        public override List<CountItem> GetCount()
        {
            List<CountItem> cis = new List<CountItem>();
            foreach (var a in dvs)
            {
                if (!a.IsReady)
                    continue;
                var ci = new CountItem()
               {
                   Key = "diskspace",
                   Subkey = a.Name,
                   Total = a.TotalSize / 1024d / 1024d,
                   Available = a.AvailableFreeSpace / 1024d / 1024
               };
                ci.Used = ci.Total - ci.Available;
                cis.Add(ci);
            }
            if (cis.Count > 0)
            {
                cis.Add(new CountItem()
                {
                    Key = "diskspace",
                    Subkey = "",
                    Total = cis.Sum(x => x.Total),
                    Used = cis.Sum(x => x.Used),
                    Available = cis.Sum(x => x.Available)
                });
            }
            return cis;
        }
    }

    public class CPUCounter : PerformanceCounterBase
    {
        System.Diagnostics.PerformanceCounter cpu_avaliable = null;
        public CPUCounter()
        {
            try
            {
                cpu_avaliable = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            }
            catch
            {
                _IsEnable = false;
            }
        }
        public override List<CountItem> GetCount()
        {
            CountItem ci = new CountItem();
            ci.Key = "cpu";
            ci.Subkey = "";
            ci.Total = 100;
            ci.Used = cpu_avaliable.NextValue();
            ci.Available = Math.Max(0, ci.Total - ci.Used);
            return new List<CountItem>() { ci };
        }
    }

    public class NetWorkIOCounter : PerformanceCounterBase
    {
        Dictionary<string, PerformanceCounter> pcs = new Dictionary<string, PerformanceCounter>();
        DateTime lastinittime = DateTime.Now;

        int autoinitmins = 60;
        private string[] monitorNetworks;
        public NetWorkIOCounter()
            : this("") { }
        public NetWorkIOCounter(string monitors)
        {
            monitorNetworks = CCF.Utils.StringHelper.SplitToStringList(monitors ?? "", new char[] { ',', ';' }).ToArray();
            Init();
        }


        private void Init()
        {
            lastinittime = DateTime.Now;
            pcs.Clear();
            try
            {
                PerformanceCounterCategory pcc = new PerformanceCounterCategory("Network Interface");
                foreach (var a in pcc.GetInstanceNames())
                {
                    if (pcc.InstanceExists(a) && (monitorNetworks.Length == 0 || monitorNetworks.Contains(a)))
                    {
                        try
                        {
                            pcs.Add("Received_" + a, new PerformanceCounter("Network Interface", "Bytes Received/sec", a, "."));
                        }
                        catch { }

                        try
                        {
                            pcs.Add("Sent_" + a, new PerformanceCounter("Network Interface", "Bytes Sent/sec", a, "."));
                        }
                        catch { }
                    }
                }
                CCF.WatchLog.Loger.Log("当前网络实例", string.Join("、", pcc.GetInstanceNames()));
            }
            catch (Exception ex)
            {
                this._IsEnable = false;
                Msg = ex.Message;
            }
        }
        public override List<CountItem> GetCount()
        {
            if ((DateTime.Now - lastinittime).TotalMinutes > autoinitmins)
            {
                Init();
                System.Threading.Thread.Sleep(100);
            }
            List<CountItem> ls = new List<CountItem>();
            foreach (var a in pcs)
            {
                ls.Add(new CountItem()
                {
                    Key = "networkio",
                    Subkey = a.Key,
                    Available = -1,
                    Total = -1,
                    Used = a.Value.NextValue() / 1024
                });
            }
            return ls;
        }
    }

    public class RequestCounter : PerformanceCounterBase
    {
        PerformanceCounter request = null;
        public RequestCounter()
        {
            try
            {
                request = new PerformanceCounter("Web Service", "Total Method Requests/sec", "_Total");
            }
            catch
            {
                _IsEnable = false;
            }
        }

        public override List<CountItem> GetCount()
        {
            CountItem ci = new CountItem();
            ci.Key = "httprequest";
            ci.Subkey = "";
            ci.Total = -1;
            ci.Available = -1;
            ci.Used = request.NextValue();
            return new List<CountItem>() { ci };
        }
    }

    public class DiskIO : PerformanceCounterBase
    {
        PerformanceCounter waitio = null;
        PerformanceCounter iowrite = null;
        PerformanceCounter ioread = null;
        public DiskIO()
        {
            try
            {
                waitio = new PerformanceCounter("PhysicalDisk", "Avg. Disk Queue Length", "_Total");
                iowrite = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
                ioread = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
            }
            catch
            {
                _IsEnable = false;
            }
        }

        public override List<CountItem> GetCount()
        {
            CountItem ci_queue = new CountItem();
            ci_queue.Key = "diskio";
            ci_queue.Subkey = "waitqueue";
            ci_queue.Total = -1;
            ci_queue.Available = -1;
            ci_queue.Used = waitio.NextValue();

            CountItem ci_read = new CountItem();
            ci_read.Key = "diskio";
            ci_read.Subkey = "read";
            ci_read.Total = -1;
            ci_read.Available = -1;
            ci_read.Used = ioread.NextValue() / 1024;

            CountItem ci_write = new CountItem();
            ci_write.Key = "diskio";
            ci_write.Subkey = "write";
            ci_write.Total = -1;
            ci_write.Available = -1;
            ci_write.Used = iowrite.NextValue() / 1024;

            return new List<CountItem>() { ci_queue, ci_read, ci_write };
        }
    }
}
