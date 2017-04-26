using System;
using Code.Infrastructure;
using DDD.Test.Code.Events;

namespace DDD.Test.Code.EventHandlers
{
    public class OrderHandler_SendEMail :
        IEventHandler<OrderAddedEvent>,
        IEventHandler<OrderDeletedEvent>
    {
       
        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="evt"></param>
        public void Handle(OrderAddedEvent evt)
        {
            Console.WriteLine(string.Format("添加订单:订单号-{0},订单名:{1}", evt.Id, evt.Name));
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="evt"></param>
        public void Handle(OrderDeletedEvent evt)
        {
            Console.WriteLine(string.Format("删除订单:订单号-{0},订单名:{1}", evt.Id, evt.Name));
        }
    }
}