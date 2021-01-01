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
        //Creating global variable
        class Global
        {
            public static string filename;

        }
        public delegate void Add(String Message);

        Add mess;
        public void AddMessage(String Message)
        {
            listBox1.Items.Add(Message);
        }
        //Intialising components
        public Form1()
        {
            InitializeComponent();
            mess = new Add(AddMessage);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //Start Server Button On click
        private void button1_Click(object sender, EventArgs e)

        {
            //Queues a method for execution. The method executes when a thread pool thread becomes available.
            System.Threading.ThreadPool.QueueUserWorkItem(StartTcpServer);
            Message("Server Started...");
            System.Threading.ThreadPool.QueueUserWorkItem(StartTcpServer2);
        }
        //Function to get IP addres of the device
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
        //Function to pass string message to list box component
        public void Message(string data)
        {
            listBox1.BeginInvoke(mess, data);

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //Function to establish connection ,It recieves file name 
        public void StartTcpServer2(object state)

        {  
            //Listen at the specified IP and port no
            TcpListener filelistener = new TcpListener(IPAddress.Parse(GetIP()),5050);
            filelistener.Start();

            //Incoming client connected
            TcpClient client = filelistener.AcceptTcpClient();
            Message("Client connection accepted from :" + client.Client.RemoteEndPoint + ".");
            

            //Get the incoming data through a network stream
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[1500];

            //read incoming stream
            int bytesRead = nwStream.Read(buffer, 0, 1500);

            //convert the data received into a string
            Global.filename = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            //Displaying file name in list box component 
            Message(Global.filename);

            

        }
        //Function to establish connection ,It recieves file 
        public void StartTcpServer(object state)

        {   //Listen at the specified IP and port no
            TcpListener filelistener = new TcpListener(IPAddress.Parse(GetIP()), 8085);
            filelistener.Start();

            //Incoming client connected
            TcpClient client = filelistener.AcceptTcpClient();
            Message("Client connection accepted from :" + client.Client.RemoteEndPoint + ".");
            byte[] buffer = new byte[1500];
            int bytesread = 1;

            //Creates a TextWriter for writing characters to a stream
            StreamWriter writer = new StreamWriter(textBox1.Text + "\\" + Global.filename);

            while (bytesread > 0)
            {   //Reads data from the NetworkStream and stores it to a byte array.
                bytesread = client.GetStream().Read(buffer, 0, buffer.Length);
                if (bytesread == 0)
                    break;
                //Writes data to the NetworkStream from a specified range of a byte array.
                writer.BaseStream.Write(buffer, 0, buffer.Length);
                Message(bytesread + " Received. ");
            }
            writer.Close();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }
        //For selecting destination, Opens folder dialog on click
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog op = new FolderBrowserDialog();
            op.ShowDialog();
            textBox1.Text = op.SelectedPath;
        }
    }
}
