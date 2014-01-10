using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIoNetServer
{
    public class WebSocketData
    {
        public string Key;
        public string Data;

        public WebSocketData(string key, string data)
        {
            this.Data = data;
            this.Key = key;
        }
    }
}
