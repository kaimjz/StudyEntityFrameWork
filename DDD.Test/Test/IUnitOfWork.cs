using System;
using System.Collections.Generic;
using System.Text;

namespace DDD_Test.Test
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// 将操作提交到数据库
        /// </summary>
        void Save();

        /// <summary>
        /// 是否需要显示进行提交（save()）
        /// 默认为false，即在repository方法中自动完成提交，值为true时，表示需要显示调用save()方法
        /// </summary>
        bool IsExplicitSubmit { get; set; }
    }
}
