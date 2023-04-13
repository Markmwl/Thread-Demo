using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DdelegateDemo
{
    class Program
    {
        //定义委托
        public delegate void DelTest(int id);
        //声明委托
        public static DelTest Test;


        static void GetID(int id)
        {
            Console.WriteLine("你的ID是："+id);
        }
        static void Main(string[] args)
        {
            Test = new DelTest(GetID);
            //同步
            Test.Invoke(24);
            //异步
            Test.BeginInvoke(12, null,null);
            Console.ReadKey();
        }
    }
}
