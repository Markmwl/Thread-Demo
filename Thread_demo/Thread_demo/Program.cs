using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Thread_demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //ThreadStart是一个无参的、返回值为void的委托:public delegate void ThreadStart()
            //传入内部方法
            //创建一个线程去执行这个方法:创建的线程默认是前台线程
            Thread thread = new Thread(new ThreadStart(test));
            //Start方法标记这个线程就绪了，可以随时被执行，具体什么时候执行这个线程，由CPU决定
            thread.Start();
            Console.WriteLine(thread.ManagedThreadId);

            //传入实例
            DemoClass demo = new DemoClass();
            thread = new Thread(new ThreadStart(demo.Gettxt));
            thread.Start();
            Console.WriteLine(thread.ManagedThreadId);

            //使用匿名delegate
            thread = new Thread(new ThreadStart(delegate () { Console.WriteLine("我是匿名delegate创建的委托"); }));
            thread.Start();
            Console.WriteLine(thread.ManagedThreadId);

            //传入lamda
            thread = new Thread(new ThreadStart(() => { Console.WriteLine("我是lamda表达式创建的委托！"); }));
            thread.Start();
            Console.WriteLine(thread.ManagedThreadId);

            //ParameterizedThreadStart是一个有参的、返回值为void的委托:public delegate void ParameterizedThreadStart(Object obj)
            //使用ParameterizedThreadStart需要注意入参，如果入参不传则为null，如果调用无参方法则会报错(使用ThreadStart)
            //调用内部方法
            thread = new Thread(new ParameterizedThreadStart(testName));
            thread.Start("ParameterizedThreadStart");

            //调用实例方法
            thread = new Thread(new ParameterizedThreadStart(demo.Getname));
            thread.Start("ParameterizedThreadStart");

            threadtest();

            backthread();

            Console.ReadKey();
        }

        static void test()
        {
            Console.WriteLine("我是内部方法！");
        }

        static void testName(object obj)
        {
            Console.WriteLine("我是有参内部方法！" + obj);
        }

        /// <summary>
        /// 线程
        /// </summary>
        static void threadtest()
        {
            Console.WriteLine("----------------------------------------");
            //获取正在运行的线程
            Thread thread = Thread.CurrentThread;
            //设置线程的名字
            thread.Name = "主线程";
            //获取当前线程的唯一标识符
            int id = thread.ManagedThreadId;
            //获取当前线程的状态
            ThreadState state = thread.ThreadState;
            //获取当前线程的优先级
            ThreadPriority priority = thread.Priority;
            string strMsg = string.Format("Thread ID:{0}\n" + "Thread Name:{1}\n" +
                "Thread State:{2}\n" + "Thread Priority:{3}\n", id, thread.Name,
                 state, priority);

            Console.WriteLine(strMsg);
        }

        /// <summary>
        /// 演示前台、后台线程
        /// </summary>
        static void backthread()
        {
            Console.WriteLine("=====================================");
            backthread a = new backthread(10);

            //创建前台进程
            Thread Qthread = new Thread(new ThreadStart(a.Runloop));
            //设置进程名称
            Qthread.Name = "前台线程";


            //创建后台进程
            a = new backthread(20);
            Thread Hthread = new Thread(new ThreadStart(a.Runloop));
            //设置进程名称
            Hthread.Name = "后台线程";
            //设置进程为后台线程：必须在start执行前调用
            Hthread.IsBackground = true;

            Qthread.Start();
            Hthread.Start();
            //前台线程执行完，后台线程未执行完，程序自动结束
            //把Hthread.IsBackground = true注释掉，运行结果：主线程执行完毕后(Main函数)，程序并未结束，而是要等所有的前台线程结束以后才会结束。
            //后台线程一般用于处理不重要的事情，应用程序结束时，后台线程是否执行完成对整个应用程序没有影响。如果要执行的事情很重要，需要将线程设置为前台线程
        }

    }

    class DemoClass
    {
        public void Gettxt()
        {
            Console.WriteLine("我是DemoClass类的Gettxt方法！");
        }

        public void Getname(object obj)
        {
            Console.WriteLine("我是DemoClass类的Getname方法！" + obj);
        }
    }

    class backthread
    {
        int Count;
        public backthread(int count)
        {
            Count = count;
        }

        public void Runloop()
        {
            //获取当前线程名称
            string threadname = Thread.CurrentThread.Name;
            for (int i = 0; i < this.Count; i++)
            {
                Console.WriteLine("{0}线程，计数:{1}", threadname,i.ToString());
                //将线程挂起指定毫秒数
                Thread.Sleep(500);
            }

            Console.WriteLine("{0}线程执行结束,完成计数！", threadname);
        }
    }
}
