using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace NetSock
{
    public class WebsocketClient
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private Thread _resiveThred;
        Action<WebsocketClient,string> _callback;

        public WebsocketClient(TcpClient client, Action<WebsocketClient,string> callback)
        {
            _callback = callback;
            _tcpClient = client;
            _stream = _tcpClient.GetStream();

            if(Handshake())
            {
                _resiveThred = new Thread(new ThreadStart(DataResive));
                _resiveThred.Start();
            }

            Thread.Sleep(5000);

            Send("hallo vom lieben server");
        }

        public void Send(string data)
        {
            Byte[] response = Encoding.UTF8.GetBytes(data);
            _stream.Write(response,0,response.Length);
        }

        private bool Handshake()
        {
            while (true)
            {
                while (!_stream.DataAvailable) ;

                Byte[] bytes = new Byte[_tcpClient.Available];

                _stream.Read(bytes, 0, bytes.Length);

                String data = Encoding.UTF8.GetString(bytes);

                if (new Regex("^GET").IsMatch(data))
                {
                    Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine
                        + "Connection: Upgrade" + Environment.NewLine
                        + "Upgrade: websocket" + Environment.NewLine
                        + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                            SHA1.Create().ComputeHash(
                                Encoding.UTF8.GetBytes(
                                    new Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                                )
                            )
                        ) + Environment.NewLine
                        + Environment.NewLine);

                    _stream.Write(response, 0, response.Length);
                    return true;
                }
            }
        }

        private void DataResive()
        {
            while (true)
            {
                while (!_stream.DataAvailable) ;

                Byte[] bytes = new Byte[_tcpClient.Available];

                _stream.Read(bytes, 0, bytes.Length);

                int resivetDataLang = bytes[1] - 128;

                var readLang = resivetDataLang;

                var maskstart = 2;
                var masklang = 4;
                var startread = 6;
                
                if(resivetDataLang == 126)
                {
                    readLang = bytes[2] + bytes[3];
                    maskstart = 4;
                    startread = 8;
                }
                else if (resivetDataLang == 127)
                {
                    readLang = bytes[2] + bytes[3] + bytes[4] + bytes[5];
                    maskstart = 6;
                    startread = 10;
                }

                var getMessage = new List<Byte>();
                int rindex = 0;
                while (readLang != rindex)
                {
                    getMessage.Add(bytes[startread + rindex]);
                    rindex++;
                }
                
                var keys = new List<Byte>();
                int kindex = 0;
                while (masklang != kindex)
                {
                    keys.Add(bytes[maskstart + kindex]);
                    kindex++;
                }

                Byte[] decoded = getMessage.ToArray();
                Byte[] encoded = getMessage.ToArray();
                Byte[] key = keys.ToArray();

                for (int i = 0; i < encoded.Length; i++) {
                    decoded[i] = (Byte)(encoded[i] ^ key[i % 4]);
                }

                _callback(this, System.Text.Encoding.UTF8.GetString(decoded));
            }
        }

        
       
    }
}
