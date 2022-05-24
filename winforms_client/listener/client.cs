using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace listener
{
    
    class client {
     
        TcpClient _client;
         public void connect() {
             _client = new TcpClient("192.168.1.99", 8700);
         }
    
         
     public   void send(string text)
        {
            try
            {
                _client = new TcpClient("192.168.1.99", 8700);
                ASCIIEncoding encoder = new ASCIIEncoding();
                NetworkStream ns = _client.GetStream();
                ns.Write(Encoding.ASCII.GetBytes(text), 0, text.Length);
                ns.Flush();
                ns.Close();
              _client.Close();
            }
            catch (Exception ex) { }
          
        
           
       
        }

    }
}
