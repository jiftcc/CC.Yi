﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Common.IOCOptions
{
    public class SqliteOptions
    {
        public string WriteUrl { get; set; }
        public List<string> ReadUrl { get; set; }
    }
}
