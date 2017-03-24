﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueWays.Core
{
    public enum UserRole
    {
        管理员 = 1,
        客服 = 2,
        AllUser = 3
    }

    public enum OrderStatus
    {
        待受理 = 1,
        已受理 = 2,
        已拒绝 = 3,
        交易完毕 = 4
    }
}
