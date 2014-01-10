using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SocketIoNetServer;

namespace SocketIoNetServerTester
{
    public partial class Form1 : Form
    {
        SokcketServer ws;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ws = new SokcketServer("http://192.168.178.31:80","192.168.178.31",8181);
            
            ws.On("Connect",OnConnect);

            ws.On("newOn", (WebsocketClient from, WebSocketData data) =>
            {
                MessageBox.Show(data.Data);
            });

        }

        private void OnConnect(WebsocketClient from, WebSocketData data)
        {
            MessageBox.Show("ein client hat sich verbunden.... ");
        }

    }
}
