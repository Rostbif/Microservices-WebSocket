using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceB.Models
{
    public class WebSocketMessage2
    {
        public string? Action { get; set; }
        public string? Status { get; set; }
        public object Data { get; set; }
    }
}