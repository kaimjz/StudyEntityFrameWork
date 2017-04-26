using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public static class ExtensionFuncion
    {
        #region 操作枚举返回值

        /// <summary>
        /// 操作枚举返回值
        /// </summary>
        /// <param name="er"></param>
        /// <param name="strHandle"></param>
        /// <returns></returns>
        public static string GetResultString(this EnumResult er, string strHandle)
        {
            string strResult = string.Empty;
            switch (er)
            {
                case EnumResult.Error:
                case EnumResult.Fail:
                    strResult = strHandle + "失败!";
                    break;
                case EnumResult.NoData:
                    strResult = "资源已不存在,请刷新重试!";
                    break;
                case EnumResult.Success:
                    strResult = strHandle + "成功!";
                    break;
                default:
                    strResult = "操作有误,请重试!";
                    break;
            }
            return strResult;
        }

        #endregion
    }
}
