using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Common
{
    public class OrderModel
    {
        public string? Name { get; set; }

        public int Amount { get; set; }
    }
}
