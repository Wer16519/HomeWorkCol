using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace FileTimeClient
{
    public partial class ClientForm : Form
    {
        private uint WM_FILETIME_REQUEST;
        private uint WM_FILETIME_RESPONSE;

        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("kernel32.dll")]
        private static extern bool GetSystemTimeAsFileTime(out MyFILETIME lpSystemTimeAsFileTime);

        // Структуры должны быть одинаковыми в клиенте и сервере
        [StructLayout(LayoutKind.Sequential)]
        public struct MyFILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MySYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MyCOPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        public class FileTimeData
        {
            public bool Success { get; set; }
            public MySYSTEMTIME SystemTime { get; set; }
            public MyFILETIME OriginalFileTime { get; set; }
        }

        public ClientForm()
        {
            InitializeComponent();
            InitializeMessages();
            this.Text = "FileTimeToSystemTime Client";
        }

        private void InitializeMessages()
        {
            WM_FILETIME_REQUEST = RegisterWindowMessage("WM_FILETIME_REQUEST");
            WM_FILETIME_RESPONSE = RegisterWindowMessage("WM_FILETIME_RESPONSE");
        }
        private void btnConvert_Click_1(object sender, EventArgs e)
        {

            GetSystemTimeAsFileTime(out MyFILETIME fileTime);

            IntPtr serverHandle = FindWindow(null, "FileTimeToSystemTime Server");

            if (serverHandle == IntPtr.Zero)
            {
                MessageBox.Show("Server not found! Make sure server is running.");
                return;
            }

            try
            {
                IntPtr fileTimePtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(fileTime));
                Marshal.StructureToPtr(fileTime, fileTimePtr, false);

                PostMessage(serverHandle, WM_FILETIME_REQUEST, this.Handle, fileTimePtr);
                Marshal.FreeCoTaskMem(fileTimePtr);

                listBoxResults.Items.Add("FILETIME conversion request sent");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private void btnGetCurrentTime_Click_1(object sender, EventArgs e)
        {
            // Локальный вызов для демонстрации
            GetSystemTimeAsFileTime(out MyFILETIME fileTime);
            listBoxResults.Items.Add($"Current FILETIME: Low={fileTime.dwLowDateTime}, High={fileTime.dwHighDateTime}");
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x004A) // WM_COPYDATA
            {
                HandleCopyData(ref m);
                return;
            }

            base.WndProc(ref m);
        }

        private void HandleCopyData(ref Message m)
        {
            try
            {
                MyCOPYDATASTRUCT cds = (MyCOPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(MyCOPYDATASTRUCT));

                if ((uint)cds.dwData == WM_FILETIME_RESPONSE)
                {
                    byte[] buffer = new byte[cds.cbData];
                    Marshal.Copy(cds.lpData, buffer, 0, cds.cbData);

                    FileTimeData data = DeserializeData(buffer);
                    DisplayResult(data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing response: {ex.Message}");
            }
        }

        private FileTimeData DeserializeData(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                FileTimeData data = new FileTimeData();
                data.Success = reader.ReadBoolean();
                data.OriginalFileTime = new MyFILETIME
                {
                    dwLowDateTime = reader.ReadUInt32(),
                    dwHighDateTime = reader.ReadUInt32()
                };
                data.SystemTime = new MySYSTEMTIME
                {
                    wYear = reader.ReadUInt16(),
                    wMonth = reader.ReadUInt16(),
                    wDayOfWeek = reader.ReadUInt16(),
                    wDay = reader.ReadUInt16(),
                    wHour = reader.ReadUInt16(),
                    wMinute = reader.ReadUInt16(),
                    wSecond = reader.ReadUInt16(),
                    wMilliseconds = reader.ReadUInt16()
                };
                return data;
            }
        }

        private void DisplayResult(FileTimeData data)
        {
            string result;
            if (data.Success)
            {
                result = $"Success: {data.SystemTime.wDay:00}.{data.SystemTime.wMonth:00}.{data.SystemTime.wYear} " +
                        $"{data.SystemTime.wHour:00}:{data.SystemTime.wMinute:00}:{data.SystemTime.wSecond:00}";
            }
            else
            {
                result = "Conversion error";
            }

            if (listBoxResults.InvokeRequired)
            {
                listBoxResults.Invoke(new Action(() =>
                {
                    listBoxResults.Items.Add($"Result: {result}");
                    txtResult.Text = $"Success: {data.Success}\r\n" +
                                   $"Date: {data.SystemTime.wDay:00}.{data.SystemTime.wMonth:00}.{data.SystemTime.wYear}\r\n" +
                                   $"Time: {data.SystemTime.wHour:00}:{data.SystemTime.wMinute:00}:{data.SystemTime.wSecond:00}";
                }));
            }
            else
            {
                listBoxResults.Items.Add($"Result: {result}");
                txtResult.Text = $"Success: {data.Success}\r\n" +
                               $"Date: {data.SystemTime.wDay:00}.{data.SystemTime.wMonth:00}.{data.SystemTime.wYear}\r\n" +
                               $"Time: {data.SystemTime.wHour:00}:{data.SystemTime.wMinute:00}:{data.SystemTime.wSecond:00}";
            }
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {

        }
    }
}