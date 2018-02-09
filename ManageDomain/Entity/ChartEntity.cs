using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManageDomain.Entity
{
    public class ChartEntity
    {
        public Chart chart { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string ytitle { get; set; }

        public List<string> categories { get; set; }
        public string unit { get; set; }

        public List<Serie> series { get; set; }
        public ChartEntity()
        {
            chart = new Chart();
        }
    }

    public class Serie
    {
        public string name { get; set; }
        public List<object> data { get; set; }

        public bool visible { get; set; }

        public Serie()
        {
            visible = true;
        }
    }

    public class Chart
    {
        public string zoomType { get; set; }
    }
}
