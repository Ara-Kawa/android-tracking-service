using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace listener
{
    public partial class Form1 : Form
    {
        client cl = new client() ;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            new Thread(
       () =>
       {
           Server begin = new Server();
           begin.SetupServer();
       }
   ).Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cl.connect();
        }

        private void button2_Click(object sender, EventArgs e)
        {
          
            cl.send(textBox1.Text);
        }
    }
}
