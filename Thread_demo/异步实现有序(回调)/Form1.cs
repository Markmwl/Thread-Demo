using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 异步实现有序_回调_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 异步回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAsyncAdvanced_Click(object sender, EventArgs e)
        {
            //发现异步方法是无序的不能满足需求
            //AsyncAdvancedTest1();
            //所以需要将代码改为以下处理
            //AsyncAdvancedTest2();
            //验证BeginInvoke的返回值IAsyncResult类型是否是AsyncCallback委托的入参
            //IAsyncResult BeginInvoke(string,AsyncCallback,object)
            //所以，BeginInvoke的返回值就是它本身的第二个入参AsyncCallback委托的入参
            AsyncAdvancedTest3();
        }

        void AsyncAdvancedTest1()
        {
            Console.WriteLine($"****************btnAsyncAdvanced_Click Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            Action<string> action = this.DoSomethingLong;
            action.BeginInvoke("btnAsyncAdvanced_Click", null, null);
            // 需求：异步多线程执行完之后再打印出下面这句
            Console.WriteLine($"到这里计算已经完成了。{Thread.CurrentThread.ManagedThreadId.ToString("00")}。");
            Console.WriteLine($"****************btnAsyncAdvanced_Click End {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
        }

        void AsyncAdvancedTest2()
        {
            Console.WriteLine($"****************btnAsyncAdvanced_Click Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            Action<string> action = this.DoSomethingLong;
            //BeginInvoke的第二个参数就是一个回调(什么是回调？将方法或函数当入参传入方法中，也就是在方法执行完毕后的结尾在执行回调)，那么AsyncCallback究竟是什么呢？
            //发现AsyncCallback就是一个委托，参数类型是IAsyncResult:表示异步操作的状态(interface)
            // 定义一个回调
            AsyncCallback callback = p =>
            {
                // 需求：异步多线程执行完之后再打印出下面这句
                Console.WriteLine($"到这里计算已经完成了。{Thread.CurrentThread.ManagedThreadId.ToString("00")}。");
            };
            action.BeginInvoke("btnAsyncAdvanced_Click", callback, null);
            Console.WriteLine($"****************btnAsyncAdvanced_Click End {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            //上面的截图中可以看出，这就是我们想要的效果，而且打印是子线程输出的，但是程序究竟是怎么实现的呢？我们可以进行如下的猜想：

            //程序执行到BeginInvoke的时候，会申请一个基于线程池的线程，这个线程会完成委托的执行(在这里就是执行DoSomethingLong()方法)，
            //在委托执行完以后，这个线程又会去执行callback回调的委托，执行callback委托需要一个IAsyncResult类型的参数，
            //这个IAsyncResult类型的参数是如何来的呢？鼠标右键放到BeginInvoke上面，查看返回值：
            //发现BeginInvoke的返回值就是IAsyncResult类型的。那么这个返回值是不是就是callback委托的参数呢？
            //将代码修改为AsyncAdvancedTest3
        }

        void AsyncAdvancedTest3()
        {
            Console.WriteLine($"****************btnAsyncAdvanced_Click Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            Action<string> action = this.DoSomethingLong;
            IAsyncResult asyncResult = null;
            AsyncCallback callback = p =>
            {
                // 需求：异步多线程执行完之后再打印出下面这句
                // 比较两个变量是否是同一个使用object.ReferenceEquals
                Console.WriteLine(object.ReferenceEquals(p, asyncResult));
                Console.WriteLine($"到这里计算已经完成了。{Thread.CurrentThread.ManagedThreadId.ToString("00")}。");
            };
            // 回调作为参数
            asyncResult = action.BeginInvoke("btnAsyncAdvanced_Click", callback, null);
            Console.WriteLine($"****************btnAsyncAdvanced_Click End {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="name"></param>
        private void DoSomethingLong(string name)
        {
            Console.WriteLine($"****************DoSomethingLong {name} Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            long lResult = 0;
            for (int i = 0; i < 1000000000; i++)
            {
                lResult += i;
            }
            Console.WriteLine($"****************DoSomethingLong {name}   End {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {lResult}***************");
        }

        /// <summary>
        /// 获取委托异步调用的返回值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAsyncReturnVlaue_Click(object sender, EventArgs e)
        {
            // 定义一个无参数、int类型返回值的委托
            Func<int> func = () =>
            {
                Thread.Sleep(2000);
                return DateTime.Now.Day;
            };
            // 输出委托同步调用的返回值
            Console.WriteLine($"func.Invoke()={func.Invoke()}");
            // 委托的异步调用
            IAsyncResult asyncResult = func.BeginInvoke(p =>
            {
                Console.WriteLine(p.AsyncState);
            }, "异步调用返回值"+DateTime.Now.Year);
            // 输出委托异步调用的返回值
            Console.WriteLine($"func.EndInvoke(asyncResult)={func.EndInvoke(asyncResult)}");


            Func<string> funca = () =>
            {
                return "123";
            };
            Console.WriteLine("同步返回值：" + funca.Invoke());
            //因为异步BeginInvoke的返回对象是IAsyncResult，而获取委托异步调用EndInvoke的入参需要IAsyncResult
            Console.WriteLine("获取异步返回值：" + funca.EndInvoke(funca.BeginInvoke(c => { Console.WriteLine("用户定义@object：" + c.AsyncState + ",ps:当你看到我的时候就证明异步方法已经处理完毕"); }, "456"))); 
        }
    }
}
