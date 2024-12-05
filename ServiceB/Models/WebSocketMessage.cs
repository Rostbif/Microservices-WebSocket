using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceB.Models
{
    public class WebSocketMessage
    {
        public string Action { get; set; }
        public object Data { get; set; }
    }
}