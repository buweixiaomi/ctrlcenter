using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.WatchLog
{
    public interface ILoger
    {
        void WriteLog(List<LogEntity> logs);
    }
}
