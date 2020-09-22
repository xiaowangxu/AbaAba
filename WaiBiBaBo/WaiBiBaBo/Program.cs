using System;
using System.Net;
using System.Net.Sockets;
using ChatRoomService;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Service ss = new Service();
            ss.Start();
            Console.ReadLine();
        }
    }
}
