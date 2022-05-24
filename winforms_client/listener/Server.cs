using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace listener
{
    class Server
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 8700;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
    

        public  void SetupServer()
        {

            Console.WriteLine("Setting up server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server setup complete");
        }
        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }

            clientSockets.Add(socket);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Client connected, waiting for request...");
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.Close();
                clientSockets.Remove(current);
                return;
            }

            byte[] recBuf = new byte[received];
            if (received <= 0)
            {
                // normal disconnect
                return;
            }
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Received Text: " + text);


            if (text.Contains(",")) // client sent coordinates 
            {
                string[] split = text.Split(',');
                string latitude =  split[0];
                string longitude = split[1];
                Process.Start("chrome.exe", "http://maps.google.com/?q=" + latitude + "," + longitude); //opens google maps in chrome with the coordinates that was sent. This is just for testing
                Form form1 =(Form1) Application.OpenForms["Form1"];

                form1.Controls["button2"].BeginInvoke(new MethodInvoker(() =>
                {
                    form1.Controls["button2"].Text = "Tracking";
                }));
                Console.WriteLine("Text is a get location request");
                try
                {
                  
                    Class1.ConnectSQLServer();
                    SqlCommand cmd = new SqlCommand("insert into tbl_location (location,from_date) values (@location,@from_date)", Class1.myCon);

                    cmd.Parameters.AddWithValue("@location", longitude + ", " + latitude);
                    cmd.Parameters.AddWithValue("@from_date", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    Class1.DisConnectSQLServer();

                }
                catch (SqlException ex) { throw new Exception(ex.Message); }
            }
            else if (text =="Stopped") // tracking is stopped
            {

                Form form1 = (Form1)Application.OpenForms["Form1"];

                form1.Controls["button2"].BeginInvoke(new MethodInvoker(() =>
                {
                    form1.Controls["button2"].Text = "Start Tracking";
                   
                }));
        
             
            }
           
            else
            {
                Console.WriteLine("Text is an invalid request");
                byte[] data = Encoding.ASCII.GetBytes("Invalid request");
                current.Send(data);
                Console.WriteLine("Warning Sent");
            }

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }
    }
}
