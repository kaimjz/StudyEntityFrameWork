﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Code.Infrastructure
{
    /// <summary>
    /// 事件处理接口
    /// </summary>
    /// <typeparam name="TEvent">继承IEvent对象的事件源对象</typeparam>
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        void Handle(TEvent evt);
    }
}
