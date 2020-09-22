using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace ChatRoomService
{
    class Revobj
    {
        public string command;
        public string text;
        public string date;
    }
    class User
    {
        public Socket socketObj;
        public int Uid;
        public string nickname;
        public bool exited = false;

        public User(Socket obj, int uid)
        {
            socketObj = obj;
            Uid = uid;
            nickname = (socketObj.RemoteEndPoint as IPEndPoint).Address.ToString() + " at " + Uid.ToString();
        }

    }

    class Service
    {
        Socket socketSevice;
        List<User> userList = new List<User>();
        int Count = 0;

        public Service()
        {
            socketSevice = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
            socketSevice.Bind(localEndPoint);
            socketSevice.Listen(10);
            Console.WriteLine("服务器启动成功\t" + localEndPoint.ToString());

            //开启接受连接,用多线程
            Thread accThread = new Thread(Accept);
            accThread.IsBackground = true;
            accThread.Start();
        }

        private void Accept()
        {
            //接受连接
            Socket clientSocket = socketSevice.Accept();

            User newUser = new User(clientSocket, Count++);


            userList.Add(newUser);

            //发送Uid信息
            clientSocket.Send(Encoding.UTF8.GetBytes(IPToAddress(clientSocket) + ":\t" + newUser.Uid.ToString()));

            //打印已经连接IP地址
            Console.WriteLine(IPToAddress(clientSocket) + "\t" + "Uid: " +  newUser.Uid.ToString() + "\t\t连接");

            Thread RecvThread = new Thread(ReceMessage);
            RecvThread.IsBackground = true;
            RecvThread.Start(newUser);

            Accept();
        }
        //接收客户端信息
        private void ReceMessage(Object obj)
        {
            User user = obj as User;
            byte[] RevByte = new byte[1024 * 1024];//设定接受字符的长度
            string str = "";
            try
            {
                int len = user.socketObj.Receive(RevByte);//接受用户发送的内容
                str = Encoding.Default.GetString(RevByte, 0, len);
                Revobj Rev = JsonConvert.DeserializeObject<Revobj>(str);
                if (Rev.command == "send") { 
                    Broadcast(Rev.text, user);//广播给用户
                }
                else if (Rev.command == "nickname")
                {
                    user.nickname = Rev.text;
                }
                

                Console.WriteLine(str);
            }
            catch (Exception e)
            {
                Console.WriteLine(IPToAddress(user.socketObj) + "\t" + "Uid: " + user.Uid.ToString() + "\t\t退出");
                user.exited = true;
                userList.Remove(user);
            }
            if (user.exited)
            {
                return;
            }
            ReceMessage(user);
        }

        private void Broadcast(string userStr, User fromUser)
        {
            foreach (User user in userList)
            {
                if (user != fromUser)//将信息广播给其他用户
                {
                    user.socketObj.Send(Encoding.UTF8.GetBytes("(UID " + fromUser.Uid.ToString() + ") " + fromUser.nickname + ":  " + userStr));
                }
            }
        }

        private void BroadcastUserList(User toUser)
        {

        }

        //转换出连来客户的IP地址
        private string IPToAddress(Socket soket)
        {
            return (soket.RemoteEndPoint as IPEndPoint).Address.ToString();
        }
    }
}
