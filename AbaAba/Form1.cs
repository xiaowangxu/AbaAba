using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
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
            client.Connect("127.0.0.1", 5566, delegate (string str) { Receive(str); }, delegate (Exception error) { });
        }
            
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && textBox1.Text != "")
            {
                client.Send(textBox1.Text);
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
    }
}
