using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Text.RegularExpressions;//正则表达式
using System.Net.NetworkInformation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Net.Http;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        SerialPort sp = null;
        bool isOpen = false;
        bool isSetProperty = false;
        private object toolStripStatusLabel1;


        private Socket socketSend;
        private bool IsStart;
        private Action<string> ShowMsgAction;
        public Form1()
        {
            InitializeComponent();

            ShowMsgAction += new Action<string>(ShowMsg);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
        
        
        int wifiSetN1 = 1, wifiSetSuccess = 0;
        byte[] textchar = new byte[1];
        int num2 = 0;


        private void btnSendData_Click(object sender, EventArgs e)
        {
            
            if (isOpen)
            {
                try
                {
                    if (!checkBox1.Checked)//如果没有选中十六进制发送
                    {
                        if (!checkBox2.Checked)//未选中回车换行
                        {
                            string str = tbxSendData.Text;//获取输入字符串
                            string result = str.Substring(0, 2);//提取头两个字符
                            bool firsttow = str.StartsWith("AT");//判断是否为AT
                            if (firsttow && flag)
                            {
                                if (tbxSendData.Text == "AT")
                                {
                                    tbxRecvData.AppendText("OK" + "\r\n");
                                }

                                else if (tbxSendData.Text == "AT+CWMODE=2")
                                {
                                    if (wifiSetN1 >= 2)
                                    {
                                        tbxRecvData.AppendText("no change" + "\r\n");
                                    }
                                    if (wifiSetN1 == 1)
                                    {
                                        tbxRecvData.AppendText("OK" + "\r\n");
                                        wifiSetN1 = 2;
                                    } //
                                }
                                else if (tbxSendData.Text == "AT+RST")
                                {
                                    if (wifiSetN1 >= 3)
                                    {
                                        tbxRecvData.AppendText("no change" + "\r\n");
                                    }
                                    if (wifiSetN1 == 2)
                                    {
                                        tbxRecvData.AppendText("OK" + "\r\n");
                                        wifiSetN1 = 3;
                                    } //
                                }
                                else if (tbxSendData.Text == ("AT+CIPMUX=1"))
                                {
                                    if (wifiSetN1 >= 4)
                                    {
                                        tbxRecvData.AppendText("no change" + "\r\n");
                                    }
                                    if (wifiSetN1 == 3)
                                    {
                                        tbxRecvData.AppendText("OK" + "\r\n");
                                        wifiSetN1 = 4;
                                    } //
                                }
                                else if (tbxSendData.Text == "AT+CIPFSR")
                                {
                                    if (wifiSetN1 >= 5)
                                    {
                                        tbxRecvData.AppendText("no change" + "\r\n");
                                    }
                                    if (wifiSetN1 == 4)
                                    {
                                        tbxRecvData.AppendText("OK" + "\r\n");
                                        wifiSetN1 = 5;
                                    } //
                                }
                                else if (tbxSendData.Text == "AT+CIPSERVER=1,8899")
                                {
                                    if (wifiSetN1 >= 6)
                                    {
                                        tbxRecvData.AppendText("no change" + "\r\n");
                                    }
                                    if (wifiSetN1 == 5)
                                    {
                                        tbxRecvData.AppendText("OK" + "\r\n");
                                        wifiSetN1 = 6;
                                    } //
                                }
                                else if (tbxSendData.Text == "AT+GMR")
                                {
                                    if (wifiSetN1 >= 7)
                                    {
                                        tbxRecvData.AppendText("no change" + "\r\n");
                                    }
                                    if (wifiSetN1 == 6)
                                    {
                                        tbxRecvData.AppendText("OK" + "\r\n");
                                        wifiSetN1 = 7;
                                    } //
                                }
                                else
                                    tbxRecvData.AppendText("There isn't this command!" + tbxSendData.Text + "\r\n");
                                if (wifiSetN1 == 7)
                                {
                                    tbxRecvData.AppendText("WIFI configure is successful!" + "\r\n");
                                    wifiSetN1 = 0;
                                    wifiSetSuccess = 1;
                                }
                            }
                            
                            else
                            {
                                sp.Encoding = Encoding.UTF8;//将串口发送的形式改为UTF-8

                                //sp.Encoding = Encoding.ASCII;//将串口发送的形式改为ASCII

                                sp.Write(tbxSendData.Text);//串口发送 （发送框里的东西）
                            }
                        }
                        else//选中回车换行
                        {
                            tbxSendData.AppendText("\r\n");//在发送文本框中回车换行
                        }
                    }
                    else//选择十六进制发送的时候
                    {

                        string buf = tbxSendData.Text;
                        string bartenm = @"\s";//正则表达式
                        string replace = "";

                        Regex rgx = new Regex(bartenm);
                        string senddata = rgx.Replace(buf, replace);//替换完成
                        num2 = (senddata.Length - senddata.Length % 2) / 2;


                        for (int a = 0; a < num2; a++)
                        {
                            textchar[0] = Convert.ToByte(senddata.Substring(a * 2, 2), 16);//每两个字符转换一次
                            sp.Write(textchar, 0, 1);
                        }
                        

                        if (senddata.Length % 2 != 0)
                        {
                            textchar[0] = Convert.ToByte(senddata.Substring(tbxSendData.Text.Length - 1, 2), 16);
                            sp.Write(textchar, 0, 1);
                            num2++;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("发送数据时发生错误！", "错误提示");
                    return;
                }
            }
            else
            {
                MessageBox.Show("串口未打开错误提示！", "错误提示");
            }
            if (!CheckSendData())
            {
                MessageBox.Show("请输入要发送的数据", "错误提示");
            }
        }

        bool hex16 = true;

        private void checkBox2_CheckedChanged(object sender, EventArgs e)//串口16进制发送选择
        {
            if (hex16)
            {
                byte[] bytes = Encoding.Default.GetBytes(tbxSendData.Text);//字符串转16进制
                string hex = BitConverter.ToString(bytes).Replace("-", " ");//每两个字符之间加空格
                tbxSendData.Text = hex;
                hex16 = false;
            }
            else
            {
                string hexString = tbxSendData.Text;
                byte[] bytes = hexString.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();//16进制转化为字符串 
                string result = Encoding.Default.GetString(bytes);
                tbxSendData.Text = result;
                hex16 = true;
            }

        }

        private void btnCheckCom_Click(object sender, EventArgs e)
        {
            bool comExistence = false;
            cbxCOMPort.Items.Clear();
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    SerialPort sp = new SerialPort("COM" + (i + 1).ToString());//可用COM的显示
                    sp.Open();
                    sp.Close();
                    cbxCOMPort.Items.Add("COM" + (i + 1).ToString());//COM口的选择
                    comExistence = true;
                }

                catch (Exception)
                {
                    continue;
                }
            }
            if (comExistence)
            {
                cbxCOMPort.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("没有找到可用串口！", "错误提示！");
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        int count, time;

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.timeBox3.Checked == true)
            {
                
                count = 0;
                //string str = numericUpDown1.Text;

                time = Int32.Parse(this.numericUpDown1.Value.ToString());//获取选择的秒数
                if(time==0)
                {
                    MessageBox.Show("定时不能为0", "错误提示");
                }
                else
                {
                    timer1.Start();//开始了就会循环调用timer函数，直到停止
                    
                }

            }
            else
            {
                timer1.Stop();//结束停止时间控件
                MessageBox.Show("自定发送已关闭");
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            for (int i = 0; i < 20; i++)
            {
                cbxCOMPort.Items.Add("COM" + (i + 1).ToString());
            }
            cbxCOMPort.SelectedIndex = 0;

            cbxBaudRate.Items.Add("1200");
            cbxBaudRate.Items.Add("2400");
            cbxBaudRate.Items.Add("4800");
            cbxBaudRate.Items.Add("9600");
            cbxBaudRate.Items.Add("19200");
            cbxBaudRate.Items.Add("38400");
            cbxBaudRate.Items.Add("115200");
            cbxBaudRate.SelectedIndex = 6;

            cbxStopBits.Items.Add("0");
            cbxStopBits.Items.Add("1");
            cbxStopBits.Items.Add("1.5");
            cbxStopBits.Items.Add("2");
            cbxStopBits.SelectedIndex = 1;

            cbxParity.Items.Add("无");
            cbxParity.Items.Add("奇校验");
            cbxParity.Items.Add("偶校验");
            cbxParity.SelectedIndex = 0;

            cbxDataBits.Items.Add("8");
            cbxDataBits.Items.Add("7");
            cbxDataBits.Items.Add("6");
            cbxDataBits.Items.Add("5");
            cbxDataBits.SelectedIndex = 0;

            //rbnChar.Checked = true;
            /*添加时间显示*/
            timer1.Interval = 1000;
            timer1.Start();

        }
        private bool CheckPortSetting()
        {
            if (cbxCOMPort.Text.Trim() == "") return false;
            if (cbxBaudRate.Text.Trim() == "") return false;
            if (cbxStopBits.Text.Trim() == "") return false;
            if (cbxParity.Text.Trim() == "") return false;
            if (cbxDataBits.Text.Trim() == "") return false;
            return true;
        }

        private bool CheckSendData()
        {
            if (tbxSendData.Text.Trim() == "") return false;
            return true;
        }

        private void SetProperty()
        {
            sp = new SerialPort();
            sp.PortName = cbxCOMPort.Text.Trim();
            sp.BaudRate = Convert.ToInt32(cbxBaudRate.Text.Trim());
            if (cbxStopBits.Text.Trim() == "0")
            {
                sp.StopBits = StopBits.None;
            }
            else if (cbxStopBits.Text.Trim() == "1.5")
            {
                sp.StopBits = StopBits.OnePointFive;
            }
            else if (cbxStopBits.Text.Trim() == "2")
            {
                sp.StopBits = StopBits.Two;
            }
            else
            {
                sp.StopBits = StopBits.One;
            }

            sp.DataBits = Convert.ToInt16(cbxDataBits.Text.Trim());

            if (cbxParity.Text.Trim() == "奇校验")
            {
                sp.Parity = Parity.Odd;

            }
            else if (cbxParity.Text.Trim() == "偶校验")
            {
                sp.Parity = Parity.Even;
            }
            else
            {
                sp.Parity = Parity.None;
            }
            sp.ReadTimeout = -1;
            sp.RtsEnable = true;

            sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
        }



        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs eg)
        {

            System.Threading.Thread.Sleep(100);

            this.Invoke((EventHandler)delegate//异步执行 一个线程
            {
                //if (!rbnHex.Checked)//如果未选中name为rbnHex的控件
                if (ckHEX)
                {
                    //tbxRecvData.Text += sp.ReadLine();
                    StringBuilder sb = new StringBuilder();
                    long rec_count = 0;
                    int num = sp.BytesToRead;
                    byte[] recbuf = new byte[num];
                    rec_count += num;

                    sp.Read(recbuf, 0, num);
                    sb.Clear();

                    try
                    {
                        Invoke((EventHandler)delegate
                        {
                            sb.Append(Encoding.UTF8.GetString(recbuf));  //将整个数组解码为UTF8数组

                            //sb.Append(Encoding.ASCII.GetString(recbuf));  //将整个数组解码为ASCII数组

                            tbxRecvData.AppendText(DateTime.Now.ToString() + "\r\n");//输出接收字符串时的本地时间
                            tbxRecvData.AppendText(sb.ToString() + "\r\n");//将字符串输出到输出窗口

                            if (flag)//开启wifi
                            {
                                Send(sb.ToString());//TCP字符串发送
                            }
                            
                        }
                        
                        );
                    }

                    catch
                    {
                        MessageBox.Show("请勾选换行", "错误提示");
                    }
                }
                //else if (rbnHex.Checked)//如果选中HEX接收
                else if (!ckHEX)
                {
                    Byte[] ReceivedData = new Byte[sp.BytesToRead];
                    sp.Read(ReceivedData, 0, ReceivedData.Length);

                    String RecvDataText = null; 

                    for (int i = 0; i < ReceivedData.Length; i++)
                    {
                        RecvDataText += (ReceivedData[i].ToString("X2") + " ");//数组里接收到的数据转化为16进制
                    }
                    tbxRecvData.AppendText(DateTime.Now.ToString() + "\r\n");//输出接收字符串时的本地时间
                    tbxRecvData.AppendText(RecvDataText + "\r\n");

                    if (flag)//WIFI模式开启
                    {
                        //Send(RecvDataText);//TCP16进制发送

                        string message = RecvDataText;//获取发送窗口中的数据
                        message = message.Replace(" ", "");//美两个字符之间使用空格隔开
                        byte[] buffer = new byte[message.Length / 2];
                        for (int i = 0; i < message.Length; i += 2)//循环写入转化完成的数据
                            buffer[i / 2] = (byte)Convert.ToByte(message.Substring(i, 2), 16);
                        socketSend.Send(buffer);//以上正确发送16进制
                    }

                }
                sp.DiscardInBuffer();
            });
        }

        private void btnOpenCom_Click(object sender, EventArgs e)
        {
            if (isOpen == false)
            {
                if (!CheckPortSetting())//此函数观察所有的设置控件，观察是否有空白
                {
                    MessageBox.Show("串口未设置", "错误提示");
                    return;
                }
                if (!isSetProperty)
                {
                    SetProperty();
                    isSetProperty = true;
                }
                try
                {
                    sp.Open();//打开串口
                    isOpen = true;
                    btnOpenCom.Text = "关闭串口";//改变控件Text
                    cbxCOMPort.Enabled = false;//将控件设为不可选中状态
                    cbxBaudRate.Enabled = false;
                    cbxDataBits.Enabled = false;
                    cbxParity.Enabled = false;
                    cbxStopBits.Enabled = false;
                    //rbnChar.Enabled = false;
                    //rbnHex.Enabled = false;
                }
                catch (Exception)
                {
                    isSetProperty = false;
                    isOpen = false;
                    MessageBox.Show("串口无效或已被占用", "错误提示");
                }
            }
            else if (isOpen == true)
            {

                try
                {
                    if (!timeBox3.Checked)
                    {
                        sp.Close();//关闭端口
                        isOpen = false;
                        btnOpenCom.Text = "打开串口";
                        cbxCOMPort.Enabled = true;
                        cbxBaudRate.Enabled = true;
                        cbxDataBits.Enabled = true;
                        cbxParity.Enabled = true;
                        cbxStopBits.Enabled = true;
                        //rbnChar.Enabled = true;
                        //rbnHex.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("请先关闭自动发送", "错误提示");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("关闭串口时发生错误", "错误提示");
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            count++;//自增
            if(time - count>0)
            {
                label6.Text = (time - count).ToString() + "秒";//获取倒计时总秒数减去已经过去的秒数，实现倒计时

            }
            
            
            if (count == time)//当我们的count等于time说明剩余0秒
            {
                if (!checkBox1.Checked)//如果没有选中十六进制发送
                {
                    sp.Encoding = Encoding.UTF8;//将串口发送的形式改为UTF-8
                    sp.Write(tbxSendData.Text);//串口发送 （发送框里的东西）
                    count = 0;
                }
                else
                {
                    string buf = tbxSendData.Text;
                    string bartenm = @"\s";//正则表达式
                    string replace = "";

                    Regex rgx = new Regex(bartenm);
                    string senddata = rgx.Replace(buf, replace);//替换完成
                    num2 = (senddata.Length - senddata.Length % 2) / 2;


                    for (int a = 0; a < num2; a++)
                    {
                        textchar[0] = Convert.ToByte(senddata.Substring(a * 2, 2), 16);//每两个字符转换一次
                        sp.Write(textchar, 0, 1);
                    }


                    if (senddata.Length % 2 != 0)
                    {
                        textchar[0] = Convert.ToByte(senddata.Substring(tbxSendData.Text.Length - 1, 2), 16);
                        sp.Write(textchar, 0, 1);
                        num2++;
                    }
                    count = 0;
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            btnSendData_Click(btnSendData, new EventArgs());
        }

        private void btnClearData_Click(object sender, EventArgs e)
        {
            if (!timeBox3.Checked)
            {
                tbxRecvData.Text = "";
                tbxSendData.Text = "";
            }
            else
            {
                MessageBox.Show("请先关闭自动发送", "错误提示");
            }
        }

        

        private void tbxRecvData_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbxSendData_TextChanged(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void rbnHex_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //创建客户端Socket，获得远程ip和端口号
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//新建客户端Socket
                IPAddress ip = IPAddress.Parse(textBox3.Text);//从控件中获取IP地址
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(textBox4.Text));//从控件中获取串口号，并将IP和端口号其导入到Socket之中

                socketSend.Connect(point);//建立连接
                ShowMsg("连接成功!");
                //开启新的线程，不停的接收服务器发来的消息
                Thread c_thread = new Thread(Received);
                c_thread.IsBackground = true;
                c_thread.Start();
                IsStart = true;
                ShowBtnState();
            }
            catch (Exception ex)
            {
                ShowMsg("连接失败:" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (IsStart == false)
            {
                MessageBox.Show("未连接！", "错误提示");
            }
            else
            {
                socketSend.Close();
                IsStart = false;
                //ShowBtnState();
                ShowMsg("信息:断开连接!");
            }
            
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (tcphex)//如果没有选中十六进制发送
            {
                if (!IsStart) return;
                string info = textBox2.Text;
                if (info == "") return;
                //Send(info);
                byte[] buffer = Encoding.UTF8.GetBytes(info);
                //byte[] buffer = Encoding.ASCII.GetBytes(info);
                socketSend.Send(buffer);
            }

            else//选择十六进制发送的时候
            {
                //int num2 = 0;
                //byte[] textchar = new byte[1];
                //string buf = textBox2.Text;
                //string bartenm = @"\s";//正则表达式
                //string replace = "";

                //Regex rgx = new Regex(bartenm);//
                //string senddata = rgx.Replace(buf, replace);
                //num2 = (senddata.Length - senddata.Length % 2) / 2;

                //for (int a = 0; a < num2; a++)
                //{
                //    textchar[0] = Convert.ToByte(senddata.Substring(a * 2, 2), 16);

                //}
                //socketSend.Send(textchar);

                //if (senddata.Length % 2 != 0)
                //{
                //    textchar[0] = Convert.ToByte(senddata.Substring(tbxSendData.Text.Length - 1, 2), 16);
                //    socketSend.Send(textchar);
                //    num2++;
                //}
                //socketSend.Send(textchar);

                string message = textBox2.Text;//获取发送窗口中的数据
                message = message.Replace(" ", "");//美两个字符之间使用空格隔开
                byte[] buffer = new byte[message.Length / 2];
                for (int i = 0; i < message.Length; i += 2)//循环写入转化完成的数据
                    buffer[i / 2] = (byte)Convert.ToByte(message.Substring(i, 2), 16);
                socketSend.Send(buffer);//以上正确发送16进制

            }


        }

        private void rbnChar_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Received()
        {
            while (IsStart)
            {
                try
                {
                    if (!socketSend.Connected) return;
                    byte[] buffer = new byte[1024 * 1024 * 3];
                    //实际接收到的有效字节数
                    int len = socketSend.Receive(buffer);
                    if (len == 0)
                    {
                        break;
                    }

                    if (hexshow)//字符串显示
                    {
                        //string str = Encoding.UTF8.GetString(buffer, 0, len)
                        //this.BeginInvoke(ShowMsgAction, socketSend.RemoteEndPoint + ":" + str);//字符串显示
                        string hexString = BitConverter.ToString(buffer, 0, len).Replace("-", " ");
                        
                        string String = hexString;
                        byte[] bytes = hexString.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();//16进制转化为字符串
                        string result = Encoding.Default.GetString(bytes);
                        
                        this.BeginInvoke(ShowMsgAction, socketSend.RemoteEndPoint + ":" + result);//显示

                        if (flag)
                        {
                            sp.Write(result);//串口发送字符串 （发送框里的东西）
                        }

                    }
                    else
                    {
                        string hexString = BitConverter.ToString(buffer, 0, len).Replace("-", " ");
                        this.BeginInvoke(ShowMsgAction, socketSend.RemoteEndPoint + ":" + hexString);//16进制显示
                        if (flag)
                        {
                            //sp.Write(hexString);//串口发送16进制 （发送框里的东西）
                            byte[] textchar = new byte[1];
                            int num2 = 0;
                            string buf = hexString;
                            string bartenm = @"\s";//正则表达式
                            string replace = "";

                            Regex rgx = new Regex(bartenm);
                            string senddata = rgx.Replace(buf, replace);//替换完成
                            num2 = (senddata.Length - senddata.Length % 2) / 2;


                            for (int a = 0; a < num2; a++)
                            {
                                textchar[0] = Convert.ToByte(senddata.Substring(a * 2, 2), 16);//每两个字符转换一次
                                sp.Write(textchar, 0, 1);
                            }


                            if (senddata.Length % 2 != 0)
                            {
                                textchar[0] = Convert.ToByte(senddata.Substring(tbxSendData.Text.Length - 1, 2), 16);
                                sp.Write(textchar, 0, 1);
                                num2++;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.BeginInvoke(ShowMsgAction, ex.Message);
                }
            }
        }

        private void Send(string msg)//功能不正常
        {
            try
            {
                byte[] buffer = new byte[1024 * 1024 * 3];
                buffer = Encoding.UTF8.GetBytes(msg);

                //buffer = Encoding.ASCII.GetBytes(msg);

                socketSend.Send(buffer);
            }
            catch (Exception ex)
            {
                this.BeginInvoke(ShowMsgAction, ex.Message);
            }
        }
        private void ShowMsg(string msg)
        {
            string info = string.Format("{0}:{1}\r\n", DateTime.Now.ToString("G"), msg);

            textBox1.AppendText(info);
            
        }

        bool flag = false;

        private void button1_Click(object sender, EventArgs e)
        {
            //如果flag为false，表示按钮显示“开”
            if (flag == false)
            {
                //改变flag为true
                flag = true;
                //改变按钮文字为“关”
                
            }
            //否则，表示按钮显示“关”
            else
            {
                //改变flag为false
                flag = false;
                //改变按钮文字为“开”
                
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        bool tcphex = true;
        private void checkBox3_CheckedChanged_1(object sender, EventArgs e)//TCP16进制发送选择
        {
            if (tcphex)
            {
                byte[] bytes = Encoding.Default.GetBytes(textBox2.Text);//字符串转16进制
                string hex = BitConverter.ToString(bytes).Replace("-", " ");
                textBox2.Text = hex;
                tcphex = false;
            }
            else
            {
                string hexString = textBox2.Text;
                byte[] bytes = hexString.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();//16进制转化为字符串
                string result = Encoding.Default.GetString(bytes);
                textBox2.Text = result;
                tcphex = true;
            }
        }

        private void groupBox8_Enter(object sender, EventArgs e)
        {

        }

        bool hexshow = true;

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if(hexshow)
            {
                hexshow = false;
            }
            else
            {
                hexshow = true;
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            TcpClient client = new TcpClient("192.168.31.219", 8888);
            //TcpClient client = new TcpClient("192.168.31.30", 8080);

            // 获取网络流
            NetworkStream stream = client.GetStream();

            // 发送消息
            //string message = textBox2.Text;
            //message = message.Replace(" ", "");
            //byte[] buffer = new byte[message.Length / 2];
            //for (int i = 0; i < message.Length; i += 2)
            //    buffer[i / 2] = (byte)Convert.ToByte(message.Substring(i, 2), 16);
            //stream.Write(buffer, 0, buffer.Length);//以上正确发送16进制
            string message2 = textBox2.Text;
            byte[] buffer = Encoding.UTF8.GetBytes(message2);
            stream.Write(buffer, 0, buffer.Length);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox3.Text = "169.254.7.186";
            textBox4.Text = "8888";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox3.Text = "192.168.31.30";
            textBox4.Text = "8080";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox3.Text = "121.36.42.100";
            textBox4.Text = "1883";
        }

        bool ckHEX = true;
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (ckHEX)
            {
                ckHEX = false;
            }
            else
            {
                ckHEX = true;
            }
        }


       

        private void timer3_Tick(object sender, EventArgs e)
        {
            count1++;//自增

            if (time1 - count1 > 0)
            {
                label9.Text = (time1 - count1).ToString() + "秒";//获取倒计时总秒数减去已经过去的秒数，实现倒计时

            }


            if (count1 == time1)//当我们的count等于time说明剩余0秒
            {
                if (tcphex)//如果没有选中十六进制发送
                {
                    if (!IsStart) return;
                    string info = textBox2.Text;
                    if (info == "") return;
                    //Send(info);
                    byte[] buffer = Encoding.UTF8.GetBytes(info);
                    socketSend.Send(buffer);
                    count1 = 0;
                }

                else//选择十六进制发送的时候
                {
                    string message = textBox2.Text;//获取发送窗口中的数据
                    message = message.Replace(" ", "");//美两个字符之间使用空格隔开
                    byte[] buffer = new byte[message.Length / 2];
                    for (int i = 0; i < message.Length; i += 2)//循环写入转化完成的数据
                        buffer[i / 2] = (byte)Convert.ToByte(message.Substring(i, 2), 16);
                    socketSend.Send(buffer);//以上正确发送16进制
                    count1 = 0;
                }
            }
            
        } 
        
        
        int count1, time1;

        private void button9_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (flag == false)
            {
                //改变flag为true
                flag = true;
               
                
            }
            //否则，表示按钮显示“关”
            else
            {
                //改变flag为false
                flag = false;
                
                
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox6.Checked == true)
            {
                count1 = 0;
                //string str = numericUpDown1.Text;

                time1 = Int32.Parse(this.numericUpDown2.Value.ToString());//获取选择的秒数
                if (time1 == 0)
                {
                    MessageBox.Show("定时不能为0", "错误提示");
                }
                else
                {
                    timer3.Start();//开始了就会循环调用timer函数，直到停止

                }

            }
            else
            {
                timer3.Stop();//结束停止时间控件
                MessageBox.Show("自定发送已关闭");
            }


        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void ShowBtnState()
        {
            //button1.Enabled = !IsStart;
            //button2.Enabled = IsStart;
        }

    }
}
