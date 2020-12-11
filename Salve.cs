using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace Platform.Distribution
{
    //从节点
    public class Salve
    {
        //启动从节点
        public void Start(string ip, string dir)
        {
            _client = new TcpClient();
            _client.Connect(IPEndPoint.Parse(ip));
            ThreadPool.QueueUserWorkItem(obj => {
                while(true)
                {
                    NetworkStream stream = _client.GetStream();
                    if(stream.DataAvailable) 
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        object value = formatter.Deserialize(stream);
                        //Console.WriteLine($"{DateTime.Now} {Thread.CurrentThread.ManagedThreadId} received [{_client.Client.RemoteEndPoint}] {value}");
                        if(value is object[] inputs)
                            ThreadPool.QueueUserWorkItem(obj => RunTask(inputs[0].ToString(), inputs[1].ToString(), inputs[2].ToString(), inputs[3]));
                    }
                }
            });
            Console.WriteLine($"{DateTime.Now} {Thread.CurrentThread.ManagedThreadId} {_client.Client.LocalEndPoint} connected {_client.Client.RemoteEndPoint}");
        }
        //发送到服务器
        public void Send(object value)
        {
            NetworkStream stream = _client.GetStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, value);
        }


        //执行任务
        public void RunTask(string name, string dllPath, string className, object input)
        {
            Console.WriteLine($"{DateTime.Now} {Thread.CurrentThread.ManagedThreadId} {name} {dllPath} {className}");
            Console.WriteLine($"{DateTime.Now} {Thread.CurrentThread.ManagedThreadId} {name} input {input}");
            Assembly assembly = Assembly.LoadFile(dllPath);
            Type type = Type.GetType(className);
            IJob job = (IJob)Activator.CreateInstance(type);
            object output = job.Execute(input);
            Console.WriteLine($"{DateTime.Now} {Thread.CurrentThread.ManagedThreadId} {name} input {input} output {output}");
            Send(new object[] { name, input, output });
        }



        //tcp客户端
        private static TcpClient _client;
    }
}