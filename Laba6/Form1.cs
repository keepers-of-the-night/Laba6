using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laba6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetWindows.WindowsToListBox(listBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CloseWindows.KillWinByT(textBox1.Text, listBox1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetWindows.WindowsToListBox(listBox1);
        }
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
        int x, int y, int width, int height, uint uFlags);

        private const uint SHOWWINDOW = 0x0040;

        private void resizeItunes()
        {
            System.Diagnostics.Process[] itunesProcesses =
                System.Diagnostics.Process.GetProcessesByName(textBox1.Text);

            if (itunesProcesses.Length > 0)
            {
                SetWindowPos(itunesProcesses[0].MainWindowHandle, this.Handle,
                    0, 0, Screen.GetWorkingArea(this).Width * 2 / 3,
                    Screen.GetWorkingArea(this).Height, SHOWWINDOW);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            resizeItunes();
        }
    }
    public class CloseWindows
    {
        // Methods
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        public static string GetWindowText(IntPtr hWnd)
        {
            int len = GetWindowTextLength(hWnd) + 1;
            StringBuilder sb = new StringBuilder(len);
            len = GetWindowText(hWnd, sb, len);
            return sb.ToString(0, len);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        public static void KillWinByT(string NameOfWindow, ListBox l)
        {
            int pid;
            string search = NameOfWindow;
            string currWindow = null;
            if (!EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                currWindow = GetWindowText(hWnd);
                if (!(string.IsNullOrEmpty(currWindow) || !currWindow.Contains(search)))
                {
                    GetWindowThreadProcessId(hWnd, out pid);
                    Process.GetProcessById(pid).Kill();
                    GetWindows.WindowsToListBox(l);
                }
                return true;
            }, IntPtr.Zero))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        // Nested Types
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    }

    public static class GetWindows
    {
        // Methods
        public static void WindowsToListBox(ListBox el)
        {
            el.Items.Clear();
            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                if (IsWindowVisible(hWnd) && (GetWindowTextLength(hWnd) != 0))
                {
                    el.Items.Add(GetWindowText(hWnd));
                }
                return true;
            }, IntPtr.Zero);
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        public static string GetWindowText(IntPtr hWnd)
        {
            int len = GetWindowTextLength(hWnd) + 1;
            StringBuilder sb = new StringBuilder(len);
            len = GetWindowText(hWnd, sb, len);
            return sb.ToString(0, len);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        // Nested Types
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    }
}