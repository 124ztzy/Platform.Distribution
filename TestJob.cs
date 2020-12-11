using System;
using System.Collections.Generic;
using System.Threading;

namespace Platform.Distribution
{

    //测试job
    public class TestJob : Job<object, string, int, int>
    {
        public override IReadOnlyCollection<string> Split(object args)
        {
            Console.WriteLine("task split");
            return new string[] {
                "task1",
                "task2",
                "task3",
            };
        }

        public override int Execute(string input)
        {
            int random = new Random().Next(10, 60);
            Console.WriteLine("task execute waiting {0}s", random);
            Thread.Sleep(random * 1000);
            return random;
        }


        public override int Combine(IReadOnlyDictionary<string, int> output)
        {
            int count = 0;
            foreach(int value in output.Values)
            {
                count += value;
            }
            Console.WriteLine("task combine {0}", count);
            return count;
        }


        //
        public override string JobName => "测试任务";
    }

}