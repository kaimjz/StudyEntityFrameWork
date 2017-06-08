using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Code.Infrastructure;
using DDD.Test.Code.Events;

namespace DDD.Test.Code.EventHandlers
{
    public class Test_EventHandler : IEventHandler<Order_Added_Event>
    {
        void IEventHandler<Order_Added_Event>.Handle(Order_Added_Event evt)
        {
            Console.WriteLine(string.Format("测试:订单号-{0},订单名:{1}", evt.Id, evt.Name));
        }
    }
}
