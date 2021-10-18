using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CSharp.DDD.Template.Domain.Core.Enums
{
    public enum EnumApiStatus
    {
        #region 默认业务状态 0~1
        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功")]
        BizOK = 0,

        /// <summary>
        /// 操作失败
        /// </summary>
        [Description("操作失败")]
        BizError = 1
        #endregion
    }
}
