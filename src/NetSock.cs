using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace NetSock
{
    
    public class SokcketServer
    {
        private string _webSocketOrigin;
        private string _websocketHost;
        private int _websocketPort;
        
        private TcpListener _socket;
        private IList<WebsocketClient> _connectedClients = new List<WebsocketClient>();
        private Thread _connectionThread;
        private IList<On> _onList = new List<On>();
        
        public SokcketServer(string webSocketOrigin, string websocketHost, int websocketPort)
        {
            _websocketHost = websocketHost;
            _webSocketOrigin = webSocketOrigin;
            _websocketPort = websocketPort;
            
            
            _connectionThread = new Thread(new ThreadStart(Connection));
            
            _socket = new TcpListener(IPAddress.Parse(_websocketHost), _websocketPort);
            _socket.Start();
            _connectionThread.Start();         
        }

        private void Connection()
        {

            var client = _socket.AcceptTcpClient();
            if (client != null) _connectedClients.Add(new WebsocketClient(client, DataResivedandFireEvent));
        }

        private void DataResivedandFireEvent(WebsocketClient client, string data)
        {
            //in date wird es späder eine eigeschaft geben mit den key....

            var temp = data.Substring(1,data.Length -2);
            var keyValue = temp.Split(',');
            var key = keyValue[0].Split(':')[1];
            var vaulue = keyValue[1].Split(':')[1];

            WebSocketData Data = new WebSocketData(key, vaulue);

            if(Data == null)
            {
                return;
            }

            foreach(var item in _onList)
            {
                if(item.GetOn() == Data.Key)
                {
                    item.Call(client,Data);
                }
            }
        }

        //so der plan....
        public void Emit(string send, WebSocketData dat, WebsocketClient client)
        {

        }

        public void On(string key, Action<WebsocketClient,WebSocketData> callback)
        {
            if(!string.IsNullOrEmpty(key) || callback != null)
            {
                _onList.Add(new On(key, callback));
            }
        }
    }
    
}
