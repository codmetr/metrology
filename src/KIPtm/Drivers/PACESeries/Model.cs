﻿using System;

namespace PACESeries
{
    [Flags]
    public enum Model
    {
        PACE1000 = 1 >> 0,
        PACE5000 = 1 >> 1,
        PACE6000 = 1 >> 2,
    }
}