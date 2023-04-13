using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 同步异步
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 同步按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSync_Click(object sender, EventArgs e)
        {
            Console.WriteLine("-------------------------同步------------------------------");
            //同步方法执行时是在主线程上跑的，所以窗体不能移动
            //同步方法是等待上一行代码执行完毕之后才会执行下一行代码
            Console.WriteLine($"****************btnSync_Click Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            for (int i = 0; i < 5; i++)
            {
                string name = string.Format($"btnSync_Click_{i}");
                this.DoSomethingLong(name);
            }
             Console.WriteLine($"***************btnSync_Click End    {Thread.CurrentThread.ManagedThreadId}");
        }

        /// <summary>
        /// 异步按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnASync_Click(object sender, EventArgs e)
        {
            //用于测试其异步执行时窗体可动性
            //AsyncTest1();
            //用于测试其与同步执行时的差异
            AsyncTest2();
        }

        /// <summary>
        /// 异步测试1：测试其异步执行时窗体可动性
        /// </summary>
        void AsyncTest1()
        {
            Console.WriteLine("--------------------------异步-----------------------------");
            Console.WriteLine($"***************btnAsync_Click Start {Thread.CurrentThread.ManagedThreadId}");
            Action<string> action = this.DoSomethingLong;
            // 调用委托(同步调用)
            action.Invoke("同步状态：btnAsync_Click_1");
            // 异步调用委托
            action.BeginInvoke("异步状态：btnAsync_Click_2", null, null);
            //当执行到action.BeginInvoke("btnAsync_Click_2",null,null);这句代码的时候，程序并没有等待这段代码执行完就执行了下面的End，没有阻塞程序的执行
            Console.WriteLine($"***************btnAsync_Click End    {Thread.CurrentThread.ManagedThreadId}");
        }

        /// <summary>
        /// 异步测试2：查看同步异步效率及差异
        /// </summary>
        void AsyncTest2()
        {
            Console.WriteLine("--------------------------异步-----------------------------");
            Console.WriteLine($"***************btnAsync_Click Start {Thread.CurrentThread.ManagedThreadId}");
            Action<string> action = this.DoSomethingLong;
            for (int i = 0; i < 5; i++)
            {
                //Thread.Sleep(5);
                string name = string.Format($"btnAsync_Click_{i}");
                action.BeginInvoke(name, null, null);
            }
            //当执行到action.BeginInvoke("btnAsync_Click_2",null,null);这句代码的时候，程序并没有等待这段代码执行完就执行了下面的End，没有阻塞程序的执行
            Console.WriteLine($"***************btnAsync_Click End    {Thread.CurrentThread.ManagedThreadId}");
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
    }
}
