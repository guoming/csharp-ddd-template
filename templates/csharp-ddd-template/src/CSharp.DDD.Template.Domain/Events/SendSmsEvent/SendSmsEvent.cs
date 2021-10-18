using Newtonsoft.Json;
using System.Collections.Generic;

namespace CSharp.DDD.Template.Domain.Events
{
    /// <summary>
    /// 短信定义
    /// </summary>
    public class SendSmsEvent
    {
        [JsonProperty("phone_number")]
        /// <summary>
        /// 电话号码
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 国际电话区号
        /// </summary>
        [JsonProperty("phone_area_code")]
        public int PhoneAreaCode { get; set; }


        /// <summary>
        /// key:参数名称,value:参数值
        /// </summary>
        [JsonProperty("paramter_list")]
        public Dictionary<string, string> ParamterList { get; set; }

        /// <summary>
        /// 模板编码
        /// </summary>
        [JsonProperty("template_code")]
        public string TemplateCode { get; set; }
    }
}
