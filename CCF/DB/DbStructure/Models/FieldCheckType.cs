using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF.DB.DbStructure
{
    [Flags]
    public enum FieldCheckType
    {
        None = 0,
        Length = 1,
        Range = 2,
        PrecisionAndScale = 4,
        Custom = 8
    }
}
