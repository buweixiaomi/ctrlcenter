using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.Config
{
    public class ConfigModel
    {
        public List<Config.Database> sourcesdbs { get; set; }
        public List<Config.Database> destinationdbs { get; set; }

        public List<ImportActionMiniModel> importactions { get; set; }
    }
}
