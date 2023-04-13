using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace 跨线程访问
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //6.
            //CheckForIllegalCrossThreadCalls = false;
        }

        //7.定义回调
        private delegate void setTextValueCallBack(int value);
        //8.声明回调
        private setTextValueCallBack setCallBack;
        private void button1_Click(object sender, EventArgs e)
        {
            //9.实例化回调
            setCallBack = new setTextValueCallBack(SetValue);
            //1.创建一个线程去执行这个方法:创建的线程默认是前台线程
            Thread thread = new Thread(new ThreadStart(Test));
            //2.Start方法标记这个线程就绪了，可以随时被执行，具体什么时候执行这个线程，由CPU决定
            //将线程设置为后台线程
            thread.IsBackground = true;
            thread.Start();
        }

        private void Test()
        {
            //4.报错：System.InvalidOperationException:“线程间操作无效: 从不是创建控件“textBox1”的线程访问它。”
            //产生错误的原因：textBox1是由主线程创建的，thread线程是另外创建的一个线程，在.NET上执行的是托管代码，C#强制要求这些代码必须是线程安全的，即不允许跨线程访问Windows窗体的控件。
            //5.解决方案：
            //5.1、在窗体的加载事件中，将C#内置控件(Control)类的CheckForIllegalCrossThreadCalls属性设置为false，屏蔽掉C#编译器对跨线程调用的检查。
            //使用上述的方法虽然可以保证程序正常运行并实现应用的功能，但是在实际的软件开发中，做如此设置是不安全的（不符合.NET的安全规范），
            //在产品软件的开发中，此类情况是不允许的。如果要在遵守.NET安全标准的前提下，实现从一个线程成功地访问另一个线程创建的空间，要使用C#的方法回调机制。
            //5.2、使用回调函数
            //详情见：多线程.docx 中 2.5 跨线程访问\2、使用回调函数
            for (int i = 0; i < 10000; i++)
            {
                //3.
                //this.textBox1.Text = i.ToString();
                //11.使用回调
                textBox1.Invoke(setCallBack, i);
            }
            //
        }

        /// <summary>
        /// 10.定义回调使用的方法
        /// </summary>
        /// <param name="value"></param>
        private void SetValue(int value)
        {
            this.textBox1.Text = value.ToString();
        }
    }
}
