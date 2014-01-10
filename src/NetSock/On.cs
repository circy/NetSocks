using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSock
{
    class On
    {
        private string _on;
        private Action<WebsocketClient, WebSocketData> _callback;

        public On(string on, Action<WebsocketClient, WebSocketData> callback)
        {
            _on = on;
            _callback = callback;
        }

        public string GetOn()
        {
            return _on;
        }

        public void Call(WebsocketClient client, WebSocketData data)
        {
            _callback(client, data);
        }
    }
}
