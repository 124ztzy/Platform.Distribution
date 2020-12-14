using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace Platform.Distribution
{
    //主节点
    public class Master
    {
        //启动主节点
        public void Start(string ip)
        {
            _listener = new TcpListener(IPEndPoint.Parse(ip));
            _listener.Start();
            Program.Log($"listening {_listener.LocalEndpoint}");
            ThreadPool.QueueUserWorkItem(obj => {
                while(true) 
                {
                    if(_listener.Pending()) 
                    {
                        MasterSalve salve = new MasterSalve(_listener.AcceptTcpClient());
                        _salves.Add(salve.EndPoint.ToString(), salve);
                        Program.Log($"accepted {salve.EndPoint}");
                    }
                    foreach(MasterSalve salve in _salves.Values)
                    {
                        if(salve.Stream.DataAvailable)
                        {
                            object value = _formatter.Deserialize(salve.Stream);
                            if(value is double capacity)
                            {
                                salve.Capacity = capacity;
                                salve.ReportTime = DateTime.Now;
                            }
                            else if(value is object[] outputs)
                            {
                                Program.Log($"received {outputs[0]} {salve.EndPoint} input {outputs[1]} output {outputs[2]}");
                                _jobs[outputs[0].ToString()].AddResult(outputs[1], outputs[2]);
                            }
                        }
                    }
                }
            });
        }
        //回复客户端
        public void SendTo(string name, object value)
        {
            if(_salves.TryGetValue(name, out MasterSalve salve))
                SendTo(salve, value);
            else
                Program.Log($"not found salve {name}");
        }
        public void SendTo(MasterSalve salve, object value)
        {
            _formatter.Serialize(salve.Stream, value);
        }
        //选择负载最低的有效节点
        public MasterSalve SelectSalve()
        {
            MasterSalve select = null; 
            DateTime now = DateTime.Now;
            foreach(MasterSalve salve in _salves.Values)
            {
                if((now - salve.ReportTime).Seconds <= 60)
                {
                    if(select == null)
                        select = salve;
                    else if(salve.Capacity < select.Capacity)
                        select = salve;
                }
            }
            return select;
        }
        

        //运行任务
        public void RunJob(string dllPath, string className, object arg)
        {
            ThreadPool.QueueUserWorkItem(obj => {
                // try
                // {
                    Assembly assembly = Assembly.LoadFile(dllPath);
                    Type type = Type.GetType(className);
                    IJob job = (IJob)Activator.CreateInstance(type);
                    _jobs.Add(job.JobName, job);
                    //拆分任务
                    job.Split(arg);
                    Program.Log($"{job.JobName} slicesCount {job.Slices.Count} salvesCount {_salves.Count}");
                    //分发执行
                    foreach(JobSlice slice in job.Slices)
                    {
                        MasterSalve salve = SelectSalve();
                        slice.MachineName = salve.EndPoint.ToString();
                        slice.AssemblyPath = dllPath;
                        slice.ClassName = className;
                        slice.MethodName = "Execute";
                        Program.Log($"{slice.MachineName} allot {slice.MachineName} input {slice.MethodInput}");
                        SendTo(salve, new object[] { job.JobName, dllPath, className, slice.MethodInput });
                    }

                    while(job.Progress < 1)
                    {
                        Thread.Sleep(1000);
                    }
                    object output = job.Combine();
                    Program.Log($"{job.JobName} result {output}");
                // }
                // catch(Exception e)
                // {
                //     Console.WriteLine($"{DateTime.Now} {Thread.CurrentThread.ManagedThreadId} {e}");
                // }
            });
        }


        //
        private TcpListener _listener;
        private Dictionary<string, MasterSalve> _salves = new Dictionary<string, MasterSalve>();
        private Dictionary<string, IJob> _jobs = new Dictionary<string, IJob>();
        private BinaryFormatter _formatter = new BinaryFormatter();
    }


    //连接主节点的次节点
    public class MasterSalve
    {
        //构造函数
        public MasterSalve(TcpClient client)
        {
            TcpClient = client;
            Stream = client.GetStream();
            EndPoint = client.Client.RemoteEndPoint;
        }


        //连接信息
        public TcpClient TcpClient { get; private set; }
        public NetworkStream Stream { get; private set; }
        public EndPoint EndPoint { get; private set; }


        //负载
        public double Capacity { get; set; }
        //报告时间
        public DateTime ReportTime { get; set; }
    }
}