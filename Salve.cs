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
            _stream = _client.GetStream();
            ThreadPool.QueueUserWorkItem(obj => {
                while(true)
                {
                    if(_stream.DataAvailable) 
                    {
                        object value = _formatter.Deserialize(_stream);
                        //Program.Log($"received [{_client.Client.RemoteEndPoint}] {value}");
                        if(value is object[] inputs)
                            ThreadPool.QueueUserWorkItem(obj => RunTask(inputs[0].ToString(), inputs[1].ToString(), inputs[2].ToString(), inputs[3]));
                    }
                }
            });
            Program.Log($"{_client.Client.LocalEndPoint} connected {_client.Client.RemoteEndPoint}");
        }
        //发送到服务器
        public void Send(object value)
        {
            _formatter.Serialize(_stream, value);
        }


        //执行任务
        public void RunTask(string name, string dllPath, string className, object input)
        {
            Program.Log($"{name} {dllPath} {className}");
            Program.Log($"{name} input {input}");
            Assembly assembly = Assembly.LoadFile(dllPath);
            Type type = Type.GetType(className);
            IJob job = (IJob)Activator.CreateInstance(type);
            object output = job.Execute(input);
            Program.Log($"{name} input {input} output {output}");
            Send(new object[] { name, input, output });
        }



        //tcp客户端
        private TcpClient _client;
        private NetworkStream _stream;
        private BinaryFormatter _formatter = new BinaryFormatter();
    }
}