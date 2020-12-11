using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Platform.Distribution
{
    //作业接口
    public interface IJob
    {
        //拆分
        ICollection Split(object jobIn);

        //执行
        object Execute(object sliceIn);

        //合并
        object Combine(IDictionary combineIn);


        //切片执行结果
        IDictionary Results { get; }
        //作业名称
        string JobName { get; }
        
    }


    //作业
    public abstract class Job<JobIn, SliceIn, SliceOut, JobOut> : IJob
    {
        //拆分
        public abstract IReadOnlyCollection<SliceIn> Split(JobIn jobIn);
        ICollection IJob.Split(object jobIn)
        {
            return (ICollection)Split((JobIn)jobIn);
        }

        //执行
        public abstract SliceOut Execute(SliceIn sliceIn);
        object IJob.Execute(object sliceIn)
        {
            return Execute((SliceIn)sliceIn);
        }

        //合并
        public abstract JobOut Combine(IReadOnlyDictionary<SliceIn, SliceOut> combineIn);
        object IJob.Combine(IDictionary combineIn)
        {
            return Combine((IReadOnlyDictionary<SliceIn, SliceOut>)combineIn);
        }


        //作业切片缓存
        IDictionary IJob.Results { get => _slices; }
        private Dictionary<SliceIn, SliceOut> _slices = new Dictionary<SliceIn, SliceOut>();
        //作业名称
        public abstract string JobName { get; }
    }


    //作业切片
    public class JobSlice
    {
        //
        public string Name { get; set; }
        //dll
        public string DllPath { get; set; }
        //类名
        public string ClassName { get; set; }
        //函数名
        public string MethodName { get; set; }
        //函数输入
        public object MethodInput { get; set; }
        //函数输出
        public object MethodOutput { get; set; }
    }
}