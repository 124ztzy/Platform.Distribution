using System;
using System.IO;
using System.Threading;

namespace Platform.Distribution
{
    //主程序
    public class Program
    {
        //开始函数
        public static void Main(string[] args)
        {
            Console.WriteLine("Platform.Distribution 20.12.9 running...");
            Execute(args);
            while(true)
            {
                string[] commands = Console.ReadLine().Split(' ');
                Execute(commands);
            }
        }


        //执行命令
        private static Master _master = new Master();
        private static Salve _salve = new Salve();
        public static void Execute(string[] commands)
        {
            if(commands.Length > 0)
            {
                switch(commands[0])
                {
                    case ">start-master":
                        if(commands.Length >= 2)
                            _master.Start(commands[1]);
                        break;
                    case ">send-to":
                         if(commands.Length >= 3)
                            _master.SendTo(commands[1], commands[2]);
                        break;
                    case ">run-job":
                        if(commands.Length >= 3)
                            _master.RunJob(commands[1], commands[2]);
                        break;

                    case ">test":
                        MemoryStream stream = new MemoryStream();
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, new byte[] { 123, 234 });
                        stream.Seek(0, SeekOrigin.Begin);
                        object value = formatter.Deserialize(stream);
                        break;

                    case ">start-slave":
                        if(commands.Length >= 2)
                            _salve.Start(commands[1], null);
                        break;
                    case ">send":
                        if(commands.Length >= 2)
                            _salve.Send(commands[1]);
                        break;
                }
            }
        }


        //输出日志
        public static void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now} {Thread.CurrentThread.ManagedThreadId:D3} {message}");
        }

        
    }
}
