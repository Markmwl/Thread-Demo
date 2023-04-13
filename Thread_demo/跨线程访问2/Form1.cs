using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 跨线程访问2
{
    public partial class Form1 : Form
    {
        //注意：此种方式仅限于.NET4.5以上版本使用
        public Form1()
        {
            InitializeComponent();
            button1.Click += button2_Click;
        }
        /// <summary>
        /// 1:将事件改为异步事件，然后赋值时设置其等待await即可
        /// </summary>
        /// <param name="sender">必须要记住：用async异步修饰的方法必须用await来修饰Task.Run()</param>
        /// <param name="e">单个异步方法中可以有多个await</param>
        private async void button1_Click(object sender, EventArgs e)
        {
            var t = Task.Run(()=> {
                return "hello";
            });
            //await Task.Run(()=> { System.Threading.Thread.Sleep(2000); });
            txt.Text = await t;
        }

        /// <summary>
        /// 2:使用invoke来进行异步任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var t = Task.Run(() => {
                return "world";
            });

            t.ContinueWith(OnDoSomthingIsComplete);
        }

        private void OnDoSomthingIsComplete(Task<string> t)
        {
            Action action = () => { txt.Text = t.Result; };
            txt.Invoke(action);
        }
    }
}
