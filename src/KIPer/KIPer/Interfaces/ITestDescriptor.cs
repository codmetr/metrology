﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Interfaces
{
    public interface ITestDescriptor
    {
        IEnumerable<ICheckPoint> Checks { get; } 
    }
}
