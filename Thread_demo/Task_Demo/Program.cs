using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //实例化的Task对象，需要调用Start来启动任务
            Task t = new Task(() =>
            {
                Console.WriteLine("任务开始工作……");
                //模拟工作过程
                Thread.Sleep(3000);
            });
            t.Start();
            t.Wait(2000);
            Console.WriteLine(t.IsCompleted);
            t.ContinueWith((task) =>
            {//ContinueWith类似callback,执行完异步时在执行
                Console.WriteLine("任务完成，完成时候的状态为：");
                Console.WriteLine("IsCanceled={0}\tIsCompleted={1}\tIsFaulted={2}", task.IsCanceled, task.IsCompleted, task.IsFaulted);
            });
            //等待我执行完毕才能执行别的
            //t.Wait();
            //直到任务完成或超时间隔（以先达到者为准）为止,由于下面的示例将启动一个睡眠时间为两秒的任务，但定义了一秒的超时值，因此调用线程将会阻塞，直到超时过期以及任务已完成执行。
            //t.Wait(2000);
            // Wait on a single task with a timeout specified.
            Task taskA = Task.Run(() => Thread.Sleep(2000));
            try
            {
                taskA.Wait(1000);       // Wait for 1 second.
                bool completed = taskA.IsCompleted;
                Console.WriteLine("Task A completed: {0}, Status: {1}",
                                 completed, taskA.Status);
                if (!completed)
                    Console.WriteLine("任务完成前超时");
            }
            catch (AggregateException)
            {
                Console.WriteLine("Exception in taskA.");
            }


            //使用Task.Factory.StarNew，则不用调用Start方法来启动任务
            //无参
           var taskwc = Task.Factory.StartNew(() => { Console.WriteLine("创建Task.Factory.StarNew异步调用的无参方法！"); });
            //有参
            var taskyc = Task.Factory.StartNew<string>(() => { return"创建Task.Factory.StarNew异步调用的有参方法！"; });
            Console.WriteLine(taskyc.Result);

            //等待任意一个执行任务完成
            Task.WaitAny(taskwc, taskyc);
            Console.WriteLine("WaitAny Task finished");

            //等待两异步都完成
            Task.WaitAll(taskwc, taskyc);
            Console.WriteLine("WaitAll Task finished");
            

            //Task.Run的跟Task.Factory.StarNew和new Task相差不多，不同的是前两种是放进线程池立即执行，而Task.Run则是等线程池空闲后在执行。
            //Run方法只接受无参的Action和Func委托，另外两个接受一个object类型的参数。
            Task.Run(() => { Console.WriteLine("无参无返回值run"); });
            //有返回值 Task.Run<int>(() => { return 1; }); => Task.Run(() => { return 1; });
            Task.Run(() => { return 1; });

            //Action不具有返回值且只有一个参数,Func是不具有参数同时返回值是由TResult来决定的

            //task的取消
            //首先创建一个取消task的令牌的实例，在不启动task直接取消：
            //var tokenSource = new CancellationTokenSource();//创建取消task实例
            //var testTask = new Task(() =>
            //{
            //    for (int i = 0; i < 6; i++)
            //    {
            //        System.Threading.Thread.Sleep(1000);
            //    }
            //}, tokenSource.Token);
            //Console.WriteLine(testTask.Status);
            //tokenSource.Token.Register(() => {
            //    Console.WriteLine("task is to cancel");
            //});
            //tokenSource.Cancel();
            //Console.WriteLine(testTask.Status);

            //如果task启动了真的取消了task？并没有
            var tokenSource = new CancellationTokenSource();//创建取消task实例
            var testTask = new Task(() =>
            {
                for (int i = 0; i < 6; i++)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }, tokenSource.Token);
            Console.WriteLine(testTask.Status);
            testTask.Start();
            Console.WriteLine(testTask.Status);
            tokenSource.Token.Register(() => {
                Console.WriteLine("task is to cancel");
            });
            tokenSource.Cancel();
            Console.WriteLine(testTask.Status);
            for (int i = 0; i < 10; i++)
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine(testTask.Status);
            }

            //task的嵌套
            //var parentTask = new Task(() => {
            //    var childTask = new Task(() => {
            //        System.Threading.Thread.Sleep(2000);
            //        Console.WriteLine("childTask to start");
            //    });
            //    childTask.Start();
            //    Console.WriteLine("parentTask to start");
            //});
            //parentTask.Start();
            //parentTask.Wait();
            //Console.WriteLine("end");
            //此时为普通关联，父task和子task没影响
            var parentTask = new Task(() => {
                var childTask = new Task(() => {
                    System.Threading.Thread.Sleep(2000);
                    Console.WriteLine("childTask to start");
                }, TaskCreationOptions.AttachedToParent);
                childTask.Start();
                Console.WriteLine("parentTask to start");
            });
            parentTask.Start();
            parentTask.Wait();
            Console.WriteLine("end");
            //此时为父task和子task关联，wait会一直等待父子task执行完

            //task死锁的问题
            //我们可以设置最大等待时间，如果超过了等待时间，就不再等待，下面我们来修改代码，设置最大等待时间为5秒(项目中可以根据实际情况设置)，如果超过5秒就输出哪个任务出错了

            //对Spinlock的使用
            //举例来说Parallel.for和Parallel.foreach是线程不安全的，有可能达不到你的预期，此时就需要加锁来解决此问题，我们可以加lock和spinlock(自旋锁)来解决
            SpinLock slock = new SpinLock(false);
            var testLock = new object();
            long sum1 = 0;
            long sum2 = 0;
            long sum3 = 0;
            Parallel.For(0, 100000, i =>
            {
                sum1 += i;
            });

            Parallel.For(0, 100000, i =>
            {
                bool lockTaken = false;
                try
                {
                    //若发生异常则重写lockTaken再到finally中接收该错误信息并且释放锁
                    slock.Enter(ref lockTaken);
                    sum2 += i;
                }
                finally
                {
                    if (lockTaken)
                        slock.Exit(false);
                }
            });
            Parallel.For(0, 100000, i =>
            {
                //lock锁，当前线程只允许一条进入，待该线程完成后才能使下一条进入
                lock (testLock)
                {
                    sum3 += i;
                };
            });
            Console.WriteLine("Num1的值为:{0}", sum1);
            Console.WriteLine("Num2的值为:{0}", sum2);
            Console.WriteLine("Num3的值为:{0}", sum3);

            //多线程
            new System.Threading.Thread(Done).Start();
            new System.Threading.Thread(Done).Start();

            AsynAndAwait.M1();

            Console.ReadKey();
        }

        //需要加锁的静态全局变量
        private static bool _isOK = false;
        //lock只能锁定一个引用类型变量
        private static object _lock = new object();

        static void Done()
        {
            //lock只能锁定一个引用类型变量
            lock (_lock)
            {
                if (!_isOK)
                {
                    Console.WriteLine("OK");
                    _isOK = true;
                }
            }
        }

        //线程安全性
        //这是线程不安全，直接调用外部参数
        static void TestRun1(string Name, int Age)
        {
            Task.Factory.StartNew(() => Console.WriteLine("name:{0} age:{1}", Name, Age));
        }

        //如果你确定底层封装好了，可以像上面那样写，但建议写成下面这种
        static void TestRun2(string Name, int Age)
        {
            Task.Factory.StartNew(obj =>
            {
                var o = (dynamic)obj;
                Console.WriteLine("name:{0} age:{1}", o.Name, o.Age);
            }, new { Name, Age });
        }
    }


    public static class AsynAndAwait
    {
        //step 1
        private static int count = 0;
        //用async和await保证多线程下静态变量count安全 --- async必须和await连用
        //C# 中的 Async 和 Await 关键字是异步编程的核心。 通过这两个关键字，可以使用 .NET Framework、.NET Core 或 Windows 运行时中的资源，轻松创建异步方法（几乎与创建同步方法一样轻松）。 
        //使用 async 关键字定义的异步方法简称为“异步方法”
        public async static void M1()
        {
            //async and await将多个线程进行串行处理
            //等到await之后的语句执行完成后
            //才执行本线程的其他语句
            //step 2 
            //注意Task为.net4.0时引入的而Task.Run是.net4.5时引入的
            await Task.Run(new Action(M2));
            Console.WriteLine("Current Thread ID is {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            //step 6
            count++;
            //step 7
            Console.WriteLine("M1 Step is {0}", count);
        }

        public static void M2()
        {
            Console.WriteLine("Current Thread ID is {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            //step 3
            System.Threading.Thread.Sleep(3000);
            //step 4
            count++;
            //step 5
            Console.WriteLine("M2 Step is {0}", count);
        }
    }
}
