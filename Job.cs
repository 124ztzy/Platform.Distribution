using System;
using System.Collections;
using System.Collections.Generic;

namespace Platform.Distribution
{
    //作业接口
    public interface IJob
    {
        //拆分切片
        void Split(object jobIn);
        //执行切片
        object Execute(object sliceIn);
        //添加切片结果
        void AddResult(object sliceIn, object sliceOut);
        //合并切片结果
        object Combine();


        //作业名称
        string JobName { get; }
        //作业进度
        double Progress { get; }
        //作业切片
        ICollection<JobSlice> Slices { get; }
    }


    //作业
    public abstract class Job<JobIn, SliceIn, SliceOut, JobOut> : IJob
    {
        //分割切片
        public abstract IReadOnlyCollection<SliceIn> Split(JobIn jobIn);
        void IJob.Split(object jobIn)
        {
            IReadOnlyCollection<SliceIn> sliceIns = Split((JobIn)jobIn);
            foreach(SliceIn sliceIn in sliceIns)
            {
                _slices.Add(sliceIn, new JobSlice() { JobName = JobName, MethodInput = sliceIn });
            }
        }
        //切片执行
        public abstract SliceOut Execute(SliceIn sliceIn);
        object IJob.Execute(object sliceIn)
        {
            return Execute((SliceIn)sliceIn);
        }
        //添加切片执行结果
        void IJob.AddResult(object sliceIn, object sliceOut)
        {
            _results.Add((SliceIn)sliceIn, (SliceOut)sliceOut);
            JobSlice slice = _slices[(SliceIn)sliceIn];
            slice.MethodOutput = sliceOut;
            slice.EndTime = DateTime.Now;
        }
        //结果合并
        public abstract JobOut Combine(IReadOnlyDictionary<SliceIn, SliceOut> combineIn);
        object IJob.Combine()
        {
            Dictionary<SliceIn, SliceOut> data = new Dictionary<SliceIn, SliceOut>();
            foreach(JobSlice slice in _slices.Values)
            {
                data.Add((SliceIn)slice.MethodInput, (SliceOut)slice.MethodOutput);
            }
            return Combine(data);
        }


        //作业名称
        public abstract string JobName { get; }
        //作业切片
        ICollection<JobSlice> IJob.Slices { get => _slices.Values; }
        //作业进度
        public double Progress { get => _results.Count / _slices.Count; }


        private Dictionary<SliceIn, JobSlice> _slices = new Dictionary<SliceIn, JobSlice>();
        private Dictionary<SliceIn, SliceOut> _results = new Dictionary<SliceIn, SliceOut>();
    }


    //作业任务
    public class JobSlice
    {
        //执行机器
        public string MachineName { get; set; }
        //作业名称
        public string JobName { get; set; }
        //程序集
        public string AssemblyPath { get; set; }
        public byte[] AssemblyBytes { get; set; }
        //类名
        public string ClassName { get; set; }
        //函数名
        public string MethodName { get; set; }
        //函数输入输出
        public object MethodInput { get; set; }
        public object MethodOutput { get; set; }
        //开始时间、结束时间
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
    
}