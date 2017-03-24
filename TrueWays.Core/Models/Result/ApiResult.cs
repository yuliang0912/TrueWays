using Newtonsoft.Json;

namespace TrueWays.Core.Models.Result
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiResult<T> : ApiResult
    {
        public ApiResult(T data)
        {
            Data = data;
        }

        public ApiResult()
        {
        }

        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }


    [JsonObject(MemberSerialization.OptIn)]
    public class ApiResult
    {
        public ApiResult(int ret, int errorCode, string message)
        {
            Ret = ret;
            ErrorCode = errorCode;
            Message = message;
        }

        public ApiResult() : this(0, 0, "success")
        {
        }

        /// <summary>
        /// 服务器状态码
        /// </summary>
        [JsonProperty(PropertyName = "ret")]
        public int Ret { get; set; }

        /// <summary>
        /// 接口内部状态码
        /// </summary>
        [JsonProperty(PropertyName = "errcode")]
        public int ErrorCode { get; set; }

        /// <summary>
        /// 简单信息描述
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
