using System;

namespace Code.Infrastructure
{
    public class ActionDelegatedEventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : IEvent
    {
        private Action<TEvent> func;

        public ActionDelegatedEventHandler(Action<TEvent> fc)
        {
            func = fc;
        }

        public void Handle(TEvent tev)
        {
            func(tev);
        }
    }
}