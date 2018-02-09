using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCF.Task
{
    [Serializable]
    public class TaskBase : MarshalByRefObject
    {
        protected List<ConfigKeyValue> GlobalConfig = new List<ConfigKeyValue>();
        public void Log(string msg)
        {
            CCF.Task.Log.WriteLog(msg);
        }
        public virtual void Init(object[] args) { }
        public void InitGlobalConfig(string jsonstring)
        {
            GlobalConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConfigKeyValue>>(jsonstring);
        }
        public void InitTaskConfig(string config)
        {
            if (!string.IsNullOrWhiteSpace(config))
            {
                var jsonobj = Newtonsoft.Json.Linq.JObject.Parse(config ?? "");
                foreach (var a in jsonobj)
                {
                    CurrTaskConfig.Add(a.Key, a.Value.ToString());
                }
            }
        }
        public int TaskID { get; set; }
        protected readonly Dictionary<string, string> CurrTaskConfig = new Dictionary<string, string>();
        public const string TempConfigFileName = "tempConfig.json";
        public bool IsRunning { get; set; }
        public TaskContext TaskContext { get; set; }
        public string GetTempData(string key)
        {
            string filefullname = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + TempConfigFileName;
            if (!System.IO.File.Exists(filefullname))
                return null;
            Newtonsoft.Json.Linq.JObject jobj = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(filefullname, Encoding.Default));
            foreach (var a in jobj["configs"].Children())
            {
                if (a["Key"].ToString() == key)
                {
                    return a["Value"].ToString();
                }
            }
            return null;
        }

        protected void SetTempData(string key, string value)
        {
            List<ConfigKeyValue> configs = new List<ConfigKeyValue>();
            string filefullname = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + TempConfigFileName;
            if (System.IO.File.Exists(filefullname))
            {
                Newtonsoft.Json.Linq.JObject jobj = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(filefullname, Encoding.Default));
                foreach (var a in jobj["configs"].Children())
                {
                    configs.Add(new ConfigKeyValue() { Key = a["Key"].ToString(), Value = a["Value"].ToString() });
                }
            }
            if (configs.Exists(x => x.Key == key))
            {
                configs.Where(x => x.Key == key).FirstOrDefault().Value = value;
            }
            else
            {
                configs.Add(new ConfigKeyValue() { Key = key, Value = value });
            }
            System.IO.File.WriteAllText(filefullname, Newtonsoft.Json.JsonConvert.SerializeObject(new { configs = configs }), Encoding.Default);
        }

        protected string GetTaskConfig(string key, string defaultv)
        {
            string v = null;
            if (CurrTaskConfig.ContainsKey(key))
            {
                v = CurrTaskConfig[key];
            }
            if (string.IsNullOrWhiteSpace(v))
                return defaultv;
            return v;
        }

        public virtual void Run() { }

        public virtual void Stop() { }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public virtual void Dispose()
        {
        }

    }
}
