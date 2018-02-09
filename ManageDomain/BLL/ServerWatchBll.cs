using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ManageDomain.BLL
{
    public class ServerWatchBll
    {
        DAL.ServerWatchDal swdal = new DAL.ServerWatchDal();
        public int SaveWatchData(int serverid, string jsonstring)
        {
            int r = 0;
            JObject jobj = JObject.Parse(jsonstring);
            using (var dbconn = Pub.GetServerWatchConn())
            {
                List<string> insert_memory = new List<string>();
                List<string> insert_diskspace = new List<string>();
                List<string> insert_cpu = new List<string>();
                List<string> insert_networkio = new List<string>();
                List<string> insert_httprequest = new List<string>();
                List<string> insert_diskio = new List<string>();

                List<CCF.DB.ProcedureParameter> paras = new List<CCF.DB.ProcedureParameter>();

                #region 取得数量
                int paraid = 0;
                DateTime? newesttime = null;
                string serversummaryinfo = "";
                foreach (var a in jobj.Children<Newtonsoft.Json.Linq.JProperty>())
                {
                    StringBuilder stateinfo = new StringBuilder();
                    DateTime timestamp = Convert.ToDateTime(a.Name);
                    if (newesttime == null)
                        newesttime = timestamp;
                    Dictionary<string, Models.ServerWatch.DataNetWorkIO> nwiodata = new Dictionary<string, Models.ServerWatch.DataNetWorkIO>();

                    foreach (var b in a.Value.Children<Newtonsoft.Json.Linq.JToken>())
                    {
                        paraid++;
                        string paraother = paraid.ToString();
                        switch (b["key"].Value<string>())
                        {
                            case "memory":
                                insert_memory.Add(string.Format("(@memory_serverid{0},@memory_timestamp{0}, @memory_used{0},@memory_available{0},now())", paraother));
                                paras.Add(new CCF.DB.ProcedureParameter("memory_serverid" + paraother, serverid));
                                paras.Add(new CCF.DB.ProcedureParameter("memory_timestamp" + paraother, timestamp));
                                paras.Add(new CCF.DB.ProcedureParameter("memory_used" + paraother, b["used"].Value<decimal>()));
                                paras.Add(new CCF.DB.ProcedureParameter("memory_available" + paraother, b["available"].Value<decimal>()));

                                stateinfo.AppendFormat("内存:当前使用{0}MB,剩余{1}MB; ", b["used"].Value<decimal>().ToString("0"), b["available"].Value<decimal>().ToString("0"));
                                break;
                            case "diskspace":
                                insert_diskspace.Add(string.Format("(@diskspace_serverid{0},@diskspace_timestamp{0},@diskspace_subkey{0}, @diskspace_used{0},@diskspace_available{0},now())", paraother));
                                paras.Add(new CCF.DB.ProcedureParameter("diskspace_serverid" + paraother, serverid));
                                paras.Add(new CCF.DB.ProcedureParameter("diskspace_timestamp" + paraother, timestamp));
                                paras.Add(new CCF.DB.ProcedureParameter("diskspace_subkey" + paraother, b["subkey"].Value<string>() ?? ""));
                                paras.Add(new CCF.DB.ProcedureParameter("diskspace_used" + paraother, b["used"].Value<decimal>()));
                                paras.Add(new CCF.DB.ProcedureParameter("diskspace_available" + paraother, b["available"].Value<decimal>()));

                                if (!string.IsNullOrWhiteSpace(b["subkey"].Value<string>() ?? ""))
                                {
                                    stateinfo.AppendFormat("磁盘[{0}]可用空间:{1}MB; ", b["subkey"].Value<string>(), b["available"].Value<decimal>().ToString("0"));
                                }
                                break;
                            case "cpu":
                                insert_cpu.Add(string.Format("(@cpu_serverid{0},@cpu_timestamp{0},@cpu_userange{0},now())", paraother));
                                paras.Add(new CCF.DB.ProcedureParameter("cpu_serverid" + paraother, serverid));
                                paras.Add(new CCF.DB.ProcedureParameter("cpu_timestamp" + paraother, timestamp));
                                paras.Add(new CCF.DB.ProcedureParameter("cpu_userange" + paraother, (int)b["used"].Value<double>()));

                                stateinfo.AppendFormat("CPU:当前使用率{0}%; ", (int)b["used"].Value<double>());
                                break;
                            case "networkio":
                                string tnewworkiosubkey = b["subkey"].Value<string>();
                                string newworksubkeytype = tnewworkiosubkey.Substring(0, tnewworkiosubkey.IndexOf('_'));
                                string newworksubkeyName = tnewworkiosubkey.Substring(tnewworkiosubkey.IndexOf('_') + 1);

                                if (!nwiodata.ContainsKey(newworksubkeyName))
                                {
                                    nwiodata[newworksubkeyName] = new Models.ServerWatch.DataNetWorkIO();
                                    nwiodata[newworksubkeyName].serverid = serverid;
                                    nwiodata[newworksubkeyName].timestamp = timestamp;
                                    nwiodata[newworksubkeyName].subkey = newworksubkeyName;
                                }
                                if (newworksubkeytype.ToLower() == "sent")
                                {
                                    nwiodata[newworksubkeyName].sent = b["used"].Value<decimal>();
                                }
                                else
                                {
                                    nwiodata[newworksubkeyName].received = b["used"].Value<decimal>();
                                }
                                break;
                            case "httprequest":
                                insert_httprequest.Add(string.Format("(@httprequest_serverid{0},@httprequest_timestamp{0},@httprequest_requestcount{0},now())", paraother));
                                paras.Add(new CCF.DB.ProcedureParameter("httprequest_serverid" + paraother, serverid));
                                paras.Add(new CCF.DB.ProcedureParameter("httprequest_timestamp" + paraother, timestamp));
                                paras.Add(new CCF.DB.ProcedureParameter("httprequest_requestcount" + paraother, (int)b["used"].Value<double>()));
                                stateinfo.AppendFormat("http并发数:{0}/s; ", (int)b["used"].Value<double>());
                                break;

                            case "diskio":
                                insert_diskio.Add(string.Format("(@diskio_serverid{0},@diskio_timestamp{0},@diskio_subkey{0},@diskio_iovalue{0},now())", paraother));

                                paras.Add(new CCF.DB.ProcedureParameter("diskio_serverid" + paraother, serverid));
                                paras.Add(new CCF.DB.ProcedureParameter("diskio_timestamp" + paraother, timestamp));
                                paras.Add(new CCF.DB.ProcedureParameter("diskio_subkey" + paraother, b["subkey"].Value<string>()));
                                paras.Add(new CCF.DB.ProcedureParameter("diskio_iovalue" + paraother, (int)b["used"].Value<decimal>()));
                                break;
                        }
                    }
                    foreach (var c in nwiodata)
                    {
                        paraid++;
                        string paraother = paraid.ToString();
                        insert_networkio.Add(string.Format("(@networkio_serverid{0},@networkio_timestamp{0},@networkio_subkey{0}, @networkio_sent{0},@networkio_received{0},now())", paraother));
                        paras.Add(new CCF.DB.ProcedureParameter("networkio_serverid" + paraother, serverid));
                        paras.Add(new CCF.DB.ProcedureParameter("networkio_timestamp" + paraother, timestamp));
                        paras.Add(new CCF.DB.ProcedureParameter("networkio_subkey" + paraother, c.Value.subkey));
                        paras.Add(new CCF.DB.ProcedureParameter("networkio_sent" + paraother, c.Value.sent));
                        paras.Add(new CCF.DB.ProcedureParameter("networkio_received" + paraother, c.Value.received));

                        stateinfo.AppendFormat("网络[{0}]:上行{1}KB/s,下行{2}KB/s; ", c.Key, c.Value.sent.ToString("0.0"), c.Value.received.ToString("0.0"));
                    }
                    if (timestamp.CompareTo(newesttime) >= 0)
                    {
                        newesttime = timestamp;
                        serversummaryinfo = stateinfo.ToString();
                    }
                }
                #endregion

                StringBuilder sbsql = new StringBuilder();
                if (insert_memory.Count > 0)
                {
                    sbsql.AppendFormat("INSERT INTO `datamemory`(`serverid`,`timestamp`,`used`,`available`,`createtime`) values{0} ;\r\n", string.Join(",\r\n", insert_memory));
                }
                if (insert_diskspace.Count > 0)
                {
                    sbsql.AppendFormat("INSERT INTO `datadiskspace`(`serverid`,`timestamp`,`subkey`,`used`,`available`,`createtime`) values{0} ;\r\n", string.Join(",\r\n", insert_diskspace));
                }
                if (insert_cpu.Count > 0)
                {
                    sbsql.AppendFormat("INSERT INTO `datacpu`(`serverid`,`timestamp`,`userange`,`createtime`) values{0} ;\r\n", string.Join(",\r\n", insert_cpu));
                }
                if (insert_networkio.Count > 0)
                {
                    sbsql.AppendFormat("INSERT INTO `datanetworkio`(`serverid`,`timestamp`,`subkey`,`sent`,`received`,`createtime`) values{0} ;\r\n", string.Join(",\r\n", insert_networkio));
                }
                if (insert_httprequest.Count > 0)
                {
                    sbsql.AppendFormat("INSERT INTO `datahttprequest`(`serverid`,`timestamp`,`requestcount`,`createtime`) values{0} ;\r\n", string.Join(",\r\n", insert_httprequest));
                }
                if (insert_diskio.Count > 0)
                {
                    sbsql.AppendFormat("INSERT INTO `datadiskio`(`serverid`,`timestamp`,`subkey`,`iovalue`,`createtime`) values{0} ;\r\n", string.Join(",\r\n", insert_diskio));
                }
                if (!string.IsNullOrEmpty(serversummaryinfo))
                {
                    swdal.AddServerSummary(dbconn, serverid, serversummaryinfo);
                }
                dbconn.ExecuteSql(sbsql.ToString(), paras);
            }
            return r;
        }

        public Models.PageModel<Models.ServerWatch.ServerStateInfo> GetServerPage(string keywords, int pno, int pagesize)
        {
            DAL.ServerMachineDal serverdal = new DAL.ServerMachineDal();
            using (var dbconn = Pub.GetConn())
            using (var watchdbconn = Pub.GetServerWatchConn())
            {
                int totalcount = 0;
                Models.PageModel<Models.ServerWatch.ServerStateInfo> models = new Models.PageModel<Models.ServerWatch.ServerStateInfo>();
                models.list = new List<Models.ServerWatch.ServerStateInfo>();
                models.PageNo = pno;
                models.PageSize = pagesize;
                models.TotalCount = totalcount;
                var services = serverdal.GetServerPage(dbconn, keywords, pno, pagesize, out totalcount);
                foreach (var s in services)
                {
                    var summary = swdal.GetServerSummary(watchdbconn, s.ServerId);
                    if (summary == null)
                        summary = new Models.ServerWatch.ServerStateInfo();
                    summary.Server = s;
                    models.list.Add(summary);
                }
                return models;
            }

        }

        public Entity.ChartEntity GetCpuChartData(int serverid, DateTime begintime, DateTime endtime)
        {
            using (var watchdbconn = Pub.GetServerWatchConn())
            {
                var datas = swdal.GetChartCpu(watchdbconn, serverid, begintime, endtime);
                datas.Reverse();
                Entity.ChartEntity rdata = new Entity.ChartEntity()
                {
                    categories = new List<string>(),
                    title = "CPU使用情况",
                    subtitle = begintime.ToString("yyyy-MM-dd HH:mm:ss") + "-" + endtime.ToString("yyyy-MM-dd HH:mm:ss"),
                    unit = "%",
                    ytitle = "使用率(%)",
                    series = new List<Entity.Serie>(),
                    chart = new Entity.Chart() { zoomType = "x" }
                };
                var serie = new Entity.Serie() { name = "CPU", data = new List<object>() };
                foreach (var a in datas)
                {
                    rdata.categories.Add(a.timestamp.ToString("yyyy/MM/dd HH:mm:ss"));
                    serie.data.Add(a.userange);
                }
                rdata.series.Add(serie);
                return rdata;
            }
        }

        public Entity.ChartEntity GetDiskSpaceChartData(int serverid, DateTime begintime, DateTime endtime)
        {
            using (var watchdbconn = Pub.GetServerWatchConn())
            {
                var datas = swdal.GetChartDiskSpace(watchdbconn, serverid, begintime, endtime);
                datas.Reverse();
                Entity.ChartEntity rdata = new Entity.ChartEntity()
                {
                    categories = new List<string>(),
                    title = "硬盘空间使用情况",
                    subtitle = begintime.ToString("yyyy-MM-dd HH:mm:ss") + "-" + endtime.ToString("yyyy-MM-dd HH:mm:ss"),
                    unit = "MB",
                    ytitle = "空间(MB)",
                    series = new List<Entity.Serie>(),
                    chart = new Entity.Chart() { zoomType = "x" }
                };
                bool existcd = false;
                rdata.categories = datas.Select(x => x.timestamp.ToString("yyyy-MM-dd HH:mm:ss")).Distinct().ToList();
                foreach (var a in datas.GroupBy(x => x.subkey))
                {
                    var serie1 = new Entity.Serie() { name = (a.Key == "" ? "整体" : a.Key) + "可用", data = new List<object>() };
                    var serie2 = new Entity.Serie() { name = (a.Key == "" ? "整体" : a.Key) + "已用", data = new List<object>() };
                    serie2.visible = false;
                    if (a.Key.ToLower().StartsWith("c:") || a.Key.ToLower().StartsWith("d:"))
                    {
                        serie1.visible = true;
                        existcd = true;
                    }
                    else
                    {
                        serie1.visible = false;
                    }
                    foreach (var b in rdata.categories)
                    {
                        var t = a.FirstOrDefault(x => x.timestamp.ToString("yyyy-MM-dd HH:mm:ss") == b);
                        if (t == null)
                        {
                            serie1.data.Add(0);
                            serie2.data.Add(0);
                        }
                        else
                        {
                            serie1.data.Add(t.available);
                            serie2.data.Add(t.used);
                        }
                    }
                    rdata.series.Add(serie1);
                    rdata.series.Add(serie2);
                }
                if (!existcd)
                {
                    foreach (var a in rdata.series)
                    {
                        a.visible = true;
                    }
                }
                return rdata;
            }
        }


        public Entity.ChartEntity GetDiskIOChartData(int serverid, DateTime begintime, DateTime endtime)
        {
            using (var watchdbconn = Pub.GetServerWatchConn())
            {
                var datas = swdal.GetChartDiskIO(watchdbconn, serverid, begintime, endtime);
                datas.Reverse();
                Entity.ChartEntity rdata = new Entity.ChartEntity()
                {
                    categories = new List<string>(),
                    title = "硬盘IO情况",
                    subtitle = begintime.ToString("yyyy-MM-dd HH:mm:ss") + "-" + endtime.ToString("yyyy-MM-dd HH:mm:ss"),
                    unit = "(KB/s)",
                    ytitle = "队列长度、速度(KB/s)",
                    series = new List<Entity.Serie>(),
                    chart = new Entity.Chart() { zoomType = "x" }
                };
                rdata.categories = datas.Select(x => x.timestamp.ToString("yyyy-MM-dd HH:mm:ss")).Distinct().ToList();
                foreach (var a in datas.GroupBy(x => x.subkey))
                {
                    var serie1 = new Entity.Serie() { name = a.Key, data = new List<object>() };

                    foreach (var b in rdata.categories)
                    {
                        var t = a.FirstOrDefault(x => x.timestamp.ToString("yyyy-MM-dd HH:mm:ss") == b);
                        if (t == null)
                        {
                            serie1.data.Add(0);
                        }
                        else
                        {
                            serie1.data.Add(t.iovalue);
                        }
                    }
                    rdata.series.Add(serie1);
                }
                return rdata;
            }
        }

        public Entity.ChartEntity GetMemoryChartData(int serverid, DateTime begintime, DateTime endtime)
        {
            using (var watchdbconn = Pub.GetServerWatchConn())
            {
                var datas = swdal.GetChartMemory(watchdbconn, serverid, begintime, endtime);
                datas.Reverse();
                Entity.ChartEntity rdata = new Entity.ChartEntity()
                {
                    categories = new List<string>(),
                    title = "内存使用情况",
                    subtitle = begintime.ToString("yyyy-MM-dd HH:mm:ss") + "-" + endtime.ToString("yyyy-MM-dd HH:mm:ss"),
                    unit = "MB",
                    ytitle = "内存(MB)",
                    series = new List<Entity.Serie>(),
                    chart = new Entity.Chart() { zoomType = "x" }
                };
                var serie1 = new Entity.Serie() { name = "内存已用", data = new List<object>() };
                var serie2 = new Entity.Serie() { name = "内存剩余", data = new List<object>() };
                foreach (var a in datas)
                {
                    rdata.categories.Add(a.timestamp.ToString("yyyy/MM/dd HH:mm:ss"));
                    serie1.data.Add(a.used);
                    serie2.data.Add(a.available);
                }
                rdata.series.Add(serie1);
                rdata.series.Add(serie2);
                return rdata;
            }
        }

        public Entity.ChartEntity GetNetworkIOChartData(int serverid, DateTime begintime, DateTime endtime)
        {
            using (var watchdbconn = Pub.GetServerWatchConn())
            {
                var datas = swdal.GetChartNetworkIO(watchdbconn, serverid, begintime, endtime);
                datas.Reverse();
                Entity.ChartEntity rdata = new Entity.ChartEntity()
                {
                    categories = new List<string>(),
                    title = "网速使用情况",
                    subtitle = begintime.ToString("yyyy-MM-dd HH:mm:ss") + "-" + endtime.ToString("yyyy-MM-dd HH:mm:ss"),
                    unit = "kb",
                    ytitle = "速度(KB/s)",
                    series = new List<Entity.Serie>(),
                    chart = new Entity.Chart() { zoomType = "x" }
                };
                rdata.categories = datas.Select(x => x.timestamp.ToString("yyyy-MM-dd HH:mm:ss")).Distinct().ToList();
                foreach (var a in datas.GroupBy(x => x.subkey))
                {
                    var serie1 = new Entity.Serie() { name = a.Key + "上行", data = new List<object>() };
                    var serie2 = new Entity.Serie() { name = a.Key + "下行", data = new List<object>() };

                    foreach (var b in rdata.categories)
                    {
                        var t = a.FirstOrDefault(x => x.timestamp.ToString("yyyy-MM-dd HH:mm:ss") == b);
                        if (t == null)
                        {
                            serie1.data.Add(0);
                            serie2.data.Add(0);
                        }
                        else
                        {
                            serie1.data.Add(t.sent);
                            serie2.data.Add(t.received);
                        }
                    }
                    rdata.series.Add(serie1);
                    rdata.series.Add(serie2);
                }
                return rdata;
            }
        }

        public Entity.ChartEntity GetHttpRequestChartData(int serverid, DateTime begintime, DateTime endtime)
        {
            using (var watchdbconn = Pub.GetServerWatchConn())
            {
                var datas = swdal.GetChartHttpRequest(watchdbconn, serverid, begintime, endtime);
                datas.Reverse();
                Entity.ChartEntity rdata = new Entity.ChartEntity()
                {
                    categories = new List<string>(),
                    title = "Http并发量",
                    subtitle = begintime.ToString("yyyy-MM-dd HH:mm:ss") + "-" + endtime.ToString("yyyy-MM-dd HH:mm:ss"),
                    unit = "request/s",
                    ytitle = "并发数(request/s)",
                    series = new List<Entity.Serie>(),
                    chart = new Entity.Chart() { zoomType = "x" }
                };
                var serie = new Entity.Serie() { name = "Http并发量", data = new List<object>() };
                foreach (var a in datas)
                {
                    rdata.categories.Add(a.timestamp.ToString("yyyy/MM/dd HH:mm:ss"));
                    serie.data.Add(a.requestcount);
                }
                rdata.series.Add(serie);
                return rdata;
            }
        }
    }
}
