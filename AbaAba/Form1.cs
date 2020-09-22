using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserSocket;

namespace AbaAba
{
    public partial class Form1 : Form
    {
        private UserSocket.UserSocket client = new UserSocket.UserSocket();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            client.Connect("fe80::a4dd:9a42:d19c:e8a1%4", 11111, delegate (string str) { Receive(str); }, delegate (Exception error) { });
        }
            
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && textBox1.Text != "")
            {
                client.Send("send",textBox1.Text);
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.SelectionColor = Color.Red;
                richTextBox1.AppendText(textBox1.Text + "\n");
                textBox1.Text = "";
            }
        }

        private void Receive(string str)
        {
            richTextBox1.Text += str + "\n";
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            client.Send("nickname", textBox2.Text);
        }
    }
}
