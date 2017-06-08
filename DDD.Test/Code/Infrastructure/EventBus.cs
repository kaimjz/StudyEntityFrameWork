using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Code.Infrastructure
{
    /// <summary>
    /// 事件总线 发布与订阅处理逻辑 核心功能代码
    /// </summary>
    public class EventBus
    {
        #region 变量及构造方法

        public EventBus()
        {
        }

        //类私有字段
        private static EventBus eventBus = null;

        //订阅事件锁
        private readonly object lockObject = new object();

        /// <summary>
        /// 对于事件数据的存储，目前采用内存字典
        /// </summary>
        private static Dictionary<Type, List<object>> EventHandlers = new Dictionary<Type, List<object>>();

        #endregion

        #region 判断是否相等

        /// <summary>
        /// 检查两个事件处理程序是否相等。如果事件处理程序是一个行动的授权，只是 比较的对象，等于重写（因为它是被比较两代表。否则， 将使用事件处理程序的类型，因为我 们不需要为每个特定事件注册相同类型的事件处理程序一次以上。
        /// </summary>
        private readonly Func<object, object, bool> EventHandlerEquals = (o1, o2) =>
        {
            var o1Type = o1.GetType();
            var o2Type = o2.GetType();
            if (o1Type.IsGenericType && o1Type.GetGenericTypeDefinition() == typeof(ActionDelegatedEventHandler<>) &&
            o2Type.IsGenericType && o2Type.GetGenericTypeDefinition() == typeof(ActionDelegatedEventHandler<>))
            {
                return o1.Equals(o2);
            }
            return o1Type == o2Type;
        };

        #endregion

        #region 初始化事件总线

        /// <summary>
        /// 初始化空的事件总件
        /// </summary>
        public static EventBus Instance
        {
            get
            {
                return eventBus ?? (eventBus = new EventBus());
            }
        }

        /// <summary>
        /// 通过XML文件初始化事件总线，订阅信自在XML里配置
        /// </summary>
        /// <returns></returns>
        public static EventBus InstanceXml
        {
            get
            {
                if (eventBus == null)
                {
                    var root = XElement.Load(ConfigurationManager.AppSettings["BusPath"]);
                    foreach (var evt in root.Elements("Event"))
                    {
                        var handlers = new List<object>();
                        Type publishEventType = Type.GetType(evt.Element("PublishEvent").Value);
                        foreach (var subscribedEvt in evt.Elements("SubscribedEvents"))
                        {
                            foreach (var concreteEvt in subscribedEvt.Elements("SubscribedEvent"))
                            {
                                handlers.Add(Activator.CreateInstance(Type.GetType(concreteEvt.Value)));
                            }
                        }
                        EventHandlers[publishEventType] = handlers;
                    }
                    eventBus = new EventBus();
                }
                return eventBus;
            }
        }

        /// <summary>
        /// 全局统一注册所有事件处理程序，实现了IEventHandler的
        /// </summary>
        public static EventBus InstanceAll
        {
            get
            {
                if (eventBus == null)
                {
                    eventBus = new EventBus();
                    var Excepts = new string[] { "IEventHandler`1", "ActionDelegatedEventHandler`1" };
                    //todo 验证两张方式的用时情况
                    var typess = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()
                    .Where(t => t.GetInterfaces().Where(p => p.Name == typeof(IEventHandler<>).Name).Count() > 0))
                    .Where(i => !Excepts.Contains(i.Name))
                    .ToArray();

                    var types = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => t.GetInterfaces().Where(p => p.Name == typeof(IEventHandler<>).Name).Count() > 0)
                    .Where(i => !Excepts.Contains(i.Name))
                    .ToArray();
                    foreach (var item in types)
                    {
                        if (!item.ContainsGenericParameters)
                        {
                            var eventHandler = Activator.CreateInstance(item);
                            foreach (var type in item.GetInterfaces().Where(i => i.Name != "IEventHandlers"))
                            {
                                eventBus.Subscribe(type, eventHandler);
                            }
                        }
                    }
                    eventBus = new EventBus();
                }
                return eventBus;
            }
        }

        #endregion

        #region 事件订阅&取消订阅，可以扩展

        /// <summary>
        /// 订阅非泛型版
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventHandler"></param>
        public void Subscribe(Type type, object eventHandler)
        {
            lock (lockObject)
            {
                var eventType = type.GetGenericArguments()[0];
                if (EventHandlers.ContainsKey(eventType))
                {
                    var handlers = EventHandlers[eventType];
                    if (handlers != null)
                    {
                        if (!handlers.Exists(deh => EventHandlerEquals(deh, eventHandler)))
                            handlers.Add(eventHandler);
                    }
                    else
                    {
                        handlers = new List<Object>
                        {
                            eventHandler
                        };
                    }
                }
                else
                    EventHandlers.Add(eventType, new List<object> { eventHandler });
            }
        }

        /// <summary>
        /// 订阅事件实体
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="handler"></param>
        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : class, IEvent
        {
            lock (lockObject)
            {
                var eventType = typeof(TEvent);
                if (EventHandlers.ContainsKey(eventType))
                {
                    var handlers = EventHandlers[eventType];
                    if (handlers == null)
                    {
                        handlers = new List<object>
                        {
                            eventHandler
                        };
                    }
                    else
                    {
                        if (!handlers.Exists(p => EventHandlerEquals(p, eventHandler)))
                        {
                            handlers.Add(eventHandler);
                        }
                    }
                }
                else
                {
                    EventHandlers.Add(eventType, new List<object>() { eventHandler });
                }
            }
        }

        /// <summary>
        /// 订阅事件委托
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlerFunc"></param>
        public void Subscribe<TEvent>(Action<TEvent> eventHandlerFunc)
            where TEvent : class, IEvent
        {
            Subscribe(new ActionDelegatedEventHandler<TEvent>(eventHandlerFunc));
        }

        /// <summary>
        /// 订阅事件列表
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlers"></param>
        public void Subscribe<TEvent>(IEnumerable<IEventHandler<TEvent>> eventHandlers)
           where TEvent : class, IEvent
        {
            foreach (var eventHandler in eventHandlers)
            {
                Subscribe<TEvent>(eventHandler);
            }
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandler"></param>
        public void Unsubscribe<TEvent>(IEventHandler<TEvent> eventHandler)
            where TEvent : class, IEvent
        {
            lock (lockObject)
            {
                var eventType = typeof(TEvent);
                if (EventHandlers.ContainsKey(eventType))
                {
                    var handlers = EventHandlers[eventType];
                    if (handlers != null && handlers.Exists(deh => EventHandlerEquals(deh, eventHandler)))
                    {
                        var handlerToRemove = handlers.First(deh => EventHandlerEquals(deh, eventHandler));
                        handlers.Remove(handlerToRemove);
                    }
                }
            }
        }

        /// <summary>
        /// 取消订阅事件列表
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlers"></param>
        public void Unsubscribe<TEvent>(IEnumerable<IEventHandler<TEvent>> eventHandlers)
          where TEvent : class, IEvent
        {
            foreach (var eventHandler in eventHandlers)
                Unsubscribe(eventHandler);
        }

        /// <summary>
        /// 取消订阅实体
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlerFunc"></param>
        public void Unsubscribe<TEvent>(Action<TEvent> eventHandlerFunc)
            where TEvent : class, IEvent
        {
            Unsubscribe(new ActionDelegatedEventHandler<TEvent>(eventHandlerFunc));
        }

        #endregion

        #region 事件发布

        /// <summary>
        /// 发布事件，支持异步事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="tev">实体</param>
        public void Publish<TEvent>(TEvent tev)
            where TEvent : class, IEvent
        {
            if (tev == null)
            {
                throw new Exception("发布事件为null");
            }
            var eventType = tev.GetType();
            if (EventHandlers.ContainsKey(eventType) && EventHandlers[eventType] != null && EventHandlers[eventType].Count > 0)
            {
                var handlers = EventHandlers[eventType];
                foreach (var handler in handlers)
                {
                    var eventHandler = handler as IEventHandler<TEvent>;
                    if (eventHandler == null)
                    {
                        continue;
                    }
                    if (eventHandler.GetType().IsDefined(typeof(HandlesAsyncAttribute), false))
                    {
                        Task.Factory.StartNew((o) => eventHandler.Handle((TEvent)o), tev);
                    }
                    else
                    {
                        eventHandler.Handle(tev);
                    }
                }
            }
        }

        /// <summary>
        /// 发布事件,支持异步事件,并有回调方法
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evnt">实体</param>
        /// <param name="callback">回调</param>
        /// <param name="timeout">超时时间</param>
        public void Publish<TEvent>(TEvent evnt, Action<TEvent, bool, Exception> callback, TimeSpan? timeout = null)
           where TEvent : class, IEvent
        {
            if (evnt == null)
                throw new ArgumentNullException("evnt");
            var eventType = evnt.GetType();
            if (EventHandlers.ContainsKey(eventType) && EventHandlers[eventType] != null && EventHandlers[eventType].Count > 0)
            {
                var handlers = EventHandlers[eventType];
                List<Task> tasks = new List<Task>();
                try
                {
                    foreach (var handler in handlers)
                    {
                        var eventHandler = handler as IEventHandler<TEvent>;
                        if (eventHandler.GetType().IsDefined(typeof(HandlesAsyncAttribute), false))
                        {
                            tasks.Add(Task.Factory.StartNew((o) => eventHandler.Handle((TEvent)o), evnt));
                        }
                        else
                        {
                            eventHandler.Handle(evnt);
                        }
                    }
                    if (tasks.Count > 0)
                    {
                        if (timeout == null)
                        {
                            Task.WaitAll(tasks.ToArray());
                        }
                        else
                        {
                            Task.WaitAll(tasks.ToArray(), timeout.Value);
                        }
                    }
                    callback(evnt, true, null);
                }
                catch (Exception ex)
                {
                    callback(evnt, false, ex);
                }
            }
            else
            {
                callback(evnt, false, null);
            }
        }

        #endregion
    }
}