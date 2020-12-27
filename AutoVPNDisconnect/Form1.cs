using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Runtime.InteropServices;

//todo:
//-the window cannot be brought up from the task bar
//- the "Cancel" button will not close app
//- verify if the app is always killed uppon exit (new feature)
//- the VPN is not automatically connected when 

namespace AutoVPNDisconnect
{
    public partial class Form1 : Form
    {
        //IResourceWriter writer = new ResourceWriter("Resource1.resx");
        
        public Form1()
        {
            InitializeComponent();
            
            var contents = "";
            DialogResult DR = MessageBox.Show("Should I reconnect to VPN when VPN disconnected?", "Auto VPN reconnect", MessageBoxButtons.YesNoCancel);
            try
            {
                var c = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VPN.txt";
                if (File.Exists(c))
                    contents = File.ReadAllText(c);
                else contents = null;
            }
            catch (Exception e) { }
            if (contents != null)
            {
                var login_data = contents.Split(' ');
                this.textBox4.Text = login_data[0];
                this.textBox5.Text = login_data[1];
                this.maskedTextBox1.Text = login_data[2];
            }

            if (DR == DialogResult.Yes) this.checkBox3.Checked = true;
            else if (DR == DialogResult.No) this.checkBox3.Checked = false;
            else Application.Exit();

            this.Icon = SystemIcons.Shield;
            this.ShowIcon = true;
            //this.notifyIcon1.Icon = SystemIcons.Shield;
             toolStripStatusLabel1.Text = "Status: ";
            this.timer1.Interval = 1;
            this.Visible = false;
            this.FormClosing += Form1_FormClosing;
            string path =
            AutoVPNDisconnect.Resource1.ResourceManager.GetString("path");
            
            string IP = AutoVPNDisconnect.Resource1.ResourceManager.GetString("ip");
            
            if (path == null || path == "") { }
            else
            {
                this.textBox1.Text = path;
            }
            if (IP == null || IP == "") { }
            else
            {
                this.textBox2.Text = IP;
            }
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkBox4.Checked)
                AppTaskKill();
            if (checkBox4.Checked)
                System.Diagnostics.Process.Start("cmd.exe", "/c rasdial.exe /d");
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.DefaultExt = "exe";
            opf.CheckFileExists = true;
            opf.CheckPathExists = true;
            opf.Multiselect = false;
            opf.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            
            if(opf.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = opf.FileName;


            }
        }
        public  List<string> GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            List<String> list = new List<string>();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    list.Add(ip.ToString());
                }
            }
            return list;
            //throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        
        void AppTaskKill()
        {
            System.Diagnostics.Process.Start("cmd.exe", "/c taskkill /F /IM " + textBox1.Text.Substring(textBox1.Text.LastIndexOf("\\") + 1));

        }
        bool firstLoop = true;
    void VPNDisconnect()
        {

            /*
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "tasklist";
            p.StartInfo.CreateNoWindow = false;

            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            this.textBox3.Text = output;

    */      List<string> list = this.GetLocalIPAddress();
            this.listBox1.Items.AddRange(list.ToArray());

            bool flag = false;
            bool flag_found = false;
            var allProcceses = Process.GetProcesses();
            var cos = textBox1.Text.Substring(textBox1.Text.LastIndexOf("\\") + 1, textBox1.Text.Length + 2 - textBox1.Text.LastIndexOf("."));
            foreach (Process p in allProcceses)
            {
                //Clipboard.SetText(Clipboard.GetText() + p.ProcessName+"\n");
                if (p.ProcessName.Contains(textBox1.Text.Substring(textBox1.Text.LastIndexOf("\\") + 1, textBox1.Text.Length + 2 - textBox1.Text.LastIndexOf("."))))
                    flag_found = true;
            }

            foreach (var item in list)
            {
                if (item.IndexOf(textBox2.Text) == 0)
                {
                    flag = true;
                }
            }
            if (flag == true) { toolStripStatusLabel1.Text = "Status: VPN CONNECTED"; notifyIcon1.Text = "Status: VPN CONNECTED";
                                this.notifyIcon1.Icon = SystemIcons.Shield;
                                }
            else { toolStripStatusLabel1.Text = "Status: VPN DISCONNECTED"; notifyIcon1.Text = "Status: VPN DISCONNECTED";
                                 this.notifyIcon1.Icon = SystemIcons.Error;
            }
            if(firstLoop) this.notifyIcon1.ShowBalloonTip(2000, "VPN Failsafe Killswitch v1.0", "VPN Failsafe Killswitch running in the background..", new ToolTipIcon());
            firstLoop = false;

            if (flag == false && flag_found == true)
            {
                //System.Diagnostics.Process.Start("cmd.exe", "/c taskkill /F /IM " + textBox1.Text.Substring(textBox1.Text.LastIndexOf("\\") + 1));
                //moved to:
                AppTaskKill();


                //Process myprc = GetaProcess(textBox1.Text.Substring(textBox1.Text.LastIndexOf("\\") + 1,textBox1.Text.Length+2- textBox1.Text.LastIndexOf(".")));
                // try
                // {
                //   myprc.Kill();
                //}
                // catch (NullReferenceException) { }
            }
            else
            {


            }
            if (flag_last != null && flag_last == false && flag == true && checkBox2.Checked) System.Diagnostics.Process.Start(textBox1.Text);
            if (flag_last != null && flag_last == true && flag == false && checkBox3.Checked) Connect_VPN();
            flag_last = flag;


        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Interval = 2000;
            this.listBox1.Items.Clear();
            VPNDisconnect();

        }
        bool? flag_last = null;
        private Process GetaProcess(string processname)
        {
            Process[] aProc = Process.GetProcesses();//Process.GetProcessesByName(processname);
            //bool flag_found = false;
            foreach (Process p in aProc)
            {
                if (p.ProcessName == textBox1.Text.Substring(textBox1.Text.LastIndexOf("\\") + 1))
                    //flag_found = true;
                     return p;
            }
            if (aProc.Length > 0)
                return aProc[0];

            else return null;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                
                this.Visible = !this.Visible;
                //if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
                //this.Activate();
                //this.BringToFront();
                //this.TopMost = true;
                //this.Focus();
                //else if (this.WindowState == FormWindowState.Normal) this.WindowState = FormWindowState.Minimized;
                //this.Show();
            }

                if (e.Button == MouseButtons.Right) {
                var r = WinAPI.GetTrayRectangle();
                contextMenuStrip1.Show(new Point(r.Right, r.Top));
                    }
                   
        }
        private void Connect_VPN()
        {
            try { 
            var c = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VPN.txt";
            var contents = File.ReadAllText(c);
        


            System.Diagnostics.Process.Start("rasdial.exe", contents);
            }
            catch (Exception e)
            {
                MessageBox.Show("Please create a file VPN.txt in %appdata% with following contents :VPNConnectionName VPNUsername VPNPassword");
            }
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            Connect_VPN();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkBox4.Checked) VPNDisconnect();
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var c = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VPN.txt";
                var contents = this.textBox4.Text + " "+ this.textBox5.Text + " " + this.maskedTextBox1.Text;
                File.WriteAllText(c, contents);

            }
            catch (Exception exc) { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Connect_VPN();
        }
    }

    public class WinAPI
    {
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public override string ToString()
            {
                return "(" + left + ", " + top + ") --> (" + right + ", " + bottom + ")";
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);


        public static IntPtr GetTrayHandle()
        {
            IntPtr taskBarHandle = WinAPI.FindWindow("Shell_TrayWnd", null);
            if (!taskBarHandle.Equals(IntPtr.Zero))
            {
                return WinAPI.FindWindowEx(taskBarHandle, IntPtr.Zero, "TrayNotifyWnd", IntPtr.Zero);
            }
            return IntPtr.Zero;
        }

        public static Rectangle GetTrayRectangle()
        {
            WinAPI.RECT rect;
            WinAPI.GetWindowRect(WinAPI.GetTrayHandle(), out rect);
            return new Rectangle(new Point(rect.left, rect.top), new Size((rect.right - rect.left) + 1, (rect.bottom - rect.top) + 1));
        }
    }
}
