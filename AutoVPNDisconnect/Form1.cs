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


namespace AutoVPNDisconnect
{
    public partial class Form1 : Form
    {
        //IResourceWriter writer = new ResourceWriter("Resource1.resx");

        public Form1()
        {
            InitializeComponent();
            this.notifyIcon1.Icon = SystemIcons.Shield;
            this.notifyIcon1.ShowBalloonTip(10000, "VPN Failsafe Killswitch", "VPN Failsafe Killswitch running in the background..", new ToolTipIcon());
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Interval = 2000;
            this.listBox1.Items.Clear();
            List<string> list = this.GetLocalIPAddress();
            this.listBox1.Items.AddRange(list.ToArray());

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

    */
            bool flag = false;
            bool flag_found = false;
            var allProcceses = Process.GetProcesses();
            foreach (Process p in allProcceses)
            {
                if (p.ProcessName == textBox1.Text.Substring(textBox1.Text.LastIndexOf("\\") + 1, textBox1.Text.Length + 2 - textBox1.Text.LastIndexOf(".")))
                    flag_found = true;
            }

            foreach (var item in list)
            {
                if (item.IndexOf(textBox2.Text) == 0)
                {
                    flag = true;
                }
            }
            if (flag == true) { toolStripStatusLabel1.Text = "Status: VPN CONNECTED"; notifyIcon1.Text = "Status: VPN CONNECTED"; }
            else {toolStripStatusLabel1.Text = "Status: VPN DISCONNECTED"; notifyIcon1.Text = "Status: VPN DISCONNECTED"; }

            if (flag == false&&flag_found==true)
            {
                System.Diagnostics.Process.Start("cmd.exe", "/c taskkill /F /IM " + textBox1.Text.Substring(textBox1.Text.LastIndexOf("\\") + 1));

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
            if(e.Button == MouseButtons.Left)
                
            this.Visible = !this.Visible;
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
                MessageBox.Show("Please create a file VPN.txt in %appdata% with contents like:VPNConnectionName VPNUsername VPNPassword");
            }
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            Connect_VPN();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
