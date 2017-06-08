using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Code.Infrastructure;

namespace DDD.Test.Code.Events
{
    public class Order_Deleted_Event : IEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
