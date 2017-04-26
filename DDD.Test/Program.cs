using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Code.Infrastructure;
using DDD.Test.Code.EventHandlers;
using DDD.Test.Code.Events;
using System.Reflection;
namespace DDD.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //EventBus.Instance.Subscribe<OrderAddedEvent>(new OrderHandler_SendEMail());
            //EventBus.Instance.Subscribe<OrderDeletedEvent>(new OrderHandler_SendEMail());
            var entity1 = new OrderAddedEvent()
            {
                Id = Guid.NewGuid(),
                Name = "订单1111"
            };
            var entity2 = new OrderDeletedEvent()
            {
                Id = Guid.NewGuid(),
                Name = "订单2222"
            };
            //EventBus.InstanceXml.Publish(entity1);
            //EventBus.InstanceXml.Publish(entity2);
            EventBus.InstanceAll.Publish(entity1);
            EventBus.InstanceAll.Publish(entity2);
            Console.ReadKey();
        }
    }
}
