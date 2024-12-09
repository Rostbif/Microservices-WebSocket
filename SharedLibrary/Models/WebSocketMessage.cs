using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    public class WebSocketMessage
    {
        public string? Action { get; set; }
        public string? Status { get; set; }
        public object Data { get; set; }
    }
}