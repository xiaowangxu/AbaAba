using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace UserSocket
{
    class UserSocket
    {
        Socket clientSocket;
        Action<string> callback = null;
        Action<Exception> errorcallback = null;

        public UserSocket()
        {
            clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);//初始化服务器
        }

        public void Connect(string Ip, int port, Action<string> callbackfunc, Action<Exception> errorcallbackfunc)
        {
            try 
            { 
                clientSocket.Connect(Ip, port); 
            }
            catch (Exception e)
            {

            }
            callback = callbackfunc;
            errorcallback = errorcallbackfunc;
            Console.WriteLine("连接成功");
            // ClientSocket.Bind(new IPEndPoint());

            Thread RecvThread = new Thread(ReceiveMessage);
            RecvThread.IsBackground = true;
            RecvThread.Start();
        }

        public void Send(String str)
        {
            DateTime time = DateTime.Now;

            StringWriter sw = new StringWriter();
            JsonWriter writer = new JsonTextWriter(sw);

            writer.WriteStartObject();
            writer.WritePropertyName("command");
            writer.WriteValue("send");
            writer.WritePropertyName("text");
            writer.WriteValue(str);
            writer.WritePropertyName("date");
            writer.WriteValue(time.ToString());
            writer.WriteEndObject();
            writer.Flush();

            string jsonText = sw.GetStringBuilder().ToString();

            try
            {
                clientSocket.Send(Encoding.UTF8.GetBytes(jsonText));
            }
            catch (Exception e)
            {

            }
        }


        private void ReceiveMessage()
        {
            try
            {
                byte[] strByte = new byte[500 * 1024];
                int len = clientSocket.Receive(strByte);
                if (callback != null)
                    callback.Invoke(Encoding.UTF8.GetString(strByte, 0, len));
                //Console.WriteLine();
            }
            catch (Exception e) //服务器关闭
            {
                //Console.WriteLine("服务器关闭");
                return;
            }
            ReceiveMessage();
        }
    }
}