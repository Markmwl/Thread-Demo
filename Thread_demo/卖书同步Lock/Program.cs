using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace 卖书同步Lock
{
    class Program
    {
        static void Main(string[] args)
        {
            Sellbooks sellbooks = new Sellbooks();
            Thread threadA = new Thread(new ParameterizedThreadStart(sellbooks.Sellbook));
            Thread threadB = new Thread(new ParameterizedThreadStart(sellbooks.Sellbook));
            threadA.Start("threadA");
            threadB.Start("threadB");
            Console.ReadKey();
        }
    }

    class Sellbooks
    {
        int num = 1;
        public void Sellbook(object obj)
        {
            //c#语言的关键字Lock，它可以把一段代码定义为互斥段，互斥段在一个时刻内只允许一个线程进入执行，而其他线程必须等待
            lock (this)
            {
                if (num > 0)
                {
                    Thread.Sleep(300);
                    num -= 1;
                    Console.WriteLine(obj+":卖出1本书，剩余{0}本！", num);
                }
                else
                {
                    Console.WriteLine(obj + ":书卖完了！");
                }
            }

        }
    }
}
