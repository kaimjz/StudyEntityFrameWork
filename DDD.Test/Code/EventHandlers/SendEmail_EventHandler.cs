using System;
using Code.Infrastructure;
using DDD.Test.Code.Events;

namespace DDD.Test.Code.EventHandlers
{
    public class SendEmail_EventHandler :
        IEventHandler<Order_Added_Event>,
        IEventHandler<Order_Deleted_Event>
    {

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="evt"></param>
        void IEventHandler<Order_Added_Event>.Handle(Order_Added_Event evt)
        {
            Console.WriteLine(string.Format("添加订单:订单号-{0},订单名:{1}", evt.Id, evt.Name));
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="evt"></param>
        void IEventHandler<Order_Deleted_Event>.Handle(Order_Deleted_Event evt)
        {
            Console.WriteLine(string.Format("删除订单:订单号-{0},订单名:{1}", evt.Id, evt.Name));
        }
    }
}