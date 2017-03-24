using System;

namespace TrueWays.Core.Common.Dapper
{
    /// <summary>
    /// 忽略的字段,不会被Dapper解析
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreFieldAttribute : Attribute
    {
        
    }
}
