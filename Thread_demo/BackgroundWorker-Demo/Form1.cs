using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BackgroundWorker_Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            progressBar1.Minimum = 0;          //设置progressbar的最小值
            progressBar1.Maximum = 100;        //设置progressbar的最大值

            backgroundWorker1.WorkerReportsProgress = true;        //可以汇报进度，只有置true，我们才可以和UI界面进行交互，否则backgroundWorker无法操控其他控件
            backgroundWorker1.WorkerSupportsCancellation = true;   //可以被取消

        }

        /// <summary>
        /// 启动线程，线程的主要逻辑，调用RunWorkerAsync()时触发该事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //throw new NotImplementedException();
            //每间隔500ms之后，进度增加10%
            //注意！由于backgroundWorker是一个后台线程，所以他没有办法直接操控其他控件
            //我们必须调用ReportProgress，然后在这个函数里面才可以和UI进行交互
            for (int i = 0; i <= 10; i++)
            {
                if (backgroundWorker1.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(500);
                    backgroundWorker1.ReportProgress(i * 10);
                }
            }
        }

        /// <summary>
        /// 报告进度，如果想要和UI进行交互必须要通过这个函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //设置进度条进度和result_result来显示进度
            resultLabel.Text = e.ProgressPercentage.ToString() + "%";
            progressBar1.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// 结束，当线程运行完毕、发生异常和调用CancelAsync()方法这三种方式都会触发该事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //backgroundWorker结束
            if (e.Cancelled == true)
            {
                resultLabel.Text = "Canceled!";
            }
            else if (e.Error != null)
            {
                resultLabel.Text = "Error: " + e.Error.Message;
            }
            else
            {
                resultLabel.Text = "Done!";
            }
        }

        private void btnstart_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();    //启动backgroundWorker
            }
        }

        private void btncacel_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                backgroundWorker1.CancelAsync();       //取消backgroundWorker
            }
        }
    }
}
