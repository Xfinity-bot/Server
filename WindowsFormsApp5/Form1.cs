using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
       
        public delegate void Add(String Message);

        Add mess;
        public void AddMessage(String Message)
        {
            listBox1.Items.Add(Message);
        }
        public Form1()
        {
            InitializeComponent();
            mess = new Add(AddMessage);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(StartTcpServer);
            Message("Server Started...");
        }
        public string GetIP()
        {
            string name = Dns.GetHostName();
            IPHostEntry entry = Dns.GetHostEntry(name);
            IPAddress[] addr = entry.AddressList;
            if (addr[1].ToString().Split('.').Length == 4)
            {
                return addr[1].ToString();
            }
            return addr[2].ToString();
        }

        public void Message(string data)
        {
            listBox1.BeginInvoke(mess, data);

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public void StartTcpServer(object state)
        {
            TcpListener filelistener = new TcpListener(IPAddress.Parse(GetIP()), 8085);
            filelistener.Start();
            TcpClient client = filelistener.AcceptTcpClient();
            Message("Client connection accepted from :" + client.Client.RemoteEndPoint + ".");
            byte[] buffer = new byte[1500];
            int bytesread = 1;

            StreamWriter writer = new StreamWriter("c:\\Users\\Garuda\\Downloads\\sample.rar");

            while (bytesread > 0)
            {
                bytesread = client.GetStream().Read(buffer, 0, buffer.Length);
                if (bytesread == 0)
                    break;
                writer.BaseStream.Write(buffer, 0, buffer.Length);
                Message(bytesread + " Received. ");
            }
            writer.Close();

        }
    }
}
