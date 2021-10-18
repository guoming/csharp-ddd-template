using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using CSharp.DDD.Template.Domain.Core.Commands;

namespace CSharp.DDD.Template.Domain.Commands.SendSmsCommand
{
    /// <summary>
    /// 发送SMS
    /// </summary>
    public class SendSmsCommand : BaseCommand<bool>
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

    public class SendSmsCommandValidator : AbstractValidator<SendSmsCommand>
    {
        public SendSmsCommandValidator()
        {
            RuleFor(command => command.TemplateCode).NotEmpty();
            RuleFor(command => command.PhoneNumber).NotEmpty();
            RuleFor(command => command.PhoneAreaCode).NotEmpty().GreaterThanOrEqualTo(0);
            RuleFor(command => command.ParamterList).Must((dict) => {

                return dict?.Count > 0;

            });
        }
    }
}
