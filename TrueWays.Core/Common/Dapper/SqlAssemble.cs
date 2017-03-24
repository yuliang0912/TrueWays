namespace TrueWays.Core.Common.Dapper
{
    public class SqlAssemble
    {
        public string Column { get; set; }

        public object Value { get; set; }

        public string Operator { get; set; }
    }

    public enum OperatorEum
    {
        //等于
        Equal = 1,
        //大于
        Gt = 2,
        //小于
        Lt = 3,
        //大于等于
        GtAndEq = 4,
        //小于等于
        LtAndEq = 5,
        //模糊匹配
        Matching = 6
    }
}
