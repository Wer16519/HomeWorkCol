using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileTimeServer
{
    public partial class ServerForm : Form
    {
        private uint WM_FILETIME_REQUEST;
        private uint WM_FILETIME_RESPONSE;

        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern bool FileTimeToSystemTime(ref MyFILETIME lpFileTime, out MySYSTEMTIME lpSystemTime);

        // Переименовал структуры чтобы избежать конфликтов
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

        // Структура для WM_COPYDATA
        [StructLayout(LayoutKind.Sequential)]
        public struct MyCOPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        // Класс для данных
        public class FileTimeData
        {
            public bool Success { get; set; }
            public MySYSTEMTIME SystemTime { get; set; }
            public MyFILETIME OriginalFileTime { get; set; }
        }

        public ServerForm()
        {
            InitializeComponent();
            InitializeMessages();
            this.Text = "FileTimeToSystemTime Server";
        }

        private void InitializeMessages()
        {
            WM_FILETIME_REQUEST = RegisterWindowMessage("WM_FILETIME_REQUEST");
            WM_FILETIME_RESPONSE = RegisterWindowMessage("WM_FILETIME_RESPONSE");
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_FILETIME_REQUEST)
            {
                HandleFileTimeRequest(m);
                return;
            }

            base.WndProc(ref m);
        }

        private void HandleFileTimeRequest(Message m)
        {
            IntPtr clientHandle = m.WParam;

            try
            {
                MyFILETIME fileTime = (MyFILETIME)Marshal.PtrToStructure(m.LParam, typeof(MyFILETIME));

                bool success = FileTimeToSystemTime(ref fileTime, out MySYSTEMTIME systemTime);

                FileTimeData data = new FileTimeData
                {
                    Success = success,
                    SystemTime = systemTime,
                    OriginalFileTime = fileTime
                };

                SendFileTimeResponse(clientHandle, data);
            }
            catch (Exception ex)
            {
                if (listBoxLog.InvokeRequired)
                {
                    listBoxLog.Invoke(new Action(() =>
                        listBoxLog.Items.Add($"Error processing request: {ex.Message}")));
                }
                else
                {
                    listBoxLog.Items.Add($"Error processing request: {ex.Message}");
                }
            }
        }

        private void SendFileTimeResponse(IntPtr clientHandle, FileTimeData data)
        {
            try
            {
                byte[] buffer = SerializeData(data);

                MyCOPYDATASTRUCT cds = new MyCOPYDATASTRUCT
                {
                    dwData = (IntPtr)WM_FILETIME_RESPONSE,
                    cbData = buffer.Length,
                    lpData = Marshal.AllocCoTaskMem(buffer.Length)
                };

                Marshal.Copy(buffer, 0, cds.lpData, buffer.Length);

                IntPtr cdsPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cds));
                Marshal.StructureToPtr(cds, cdsPtr, false);

                SendMessage(clientHandle, 0x004A, this.Handle, cdsPtr);

                Marshal.FreeCoTaskMem(cds.lpData);
                Marshal.FreeCoTaskMem(cdsPtr);

                string logMessage = $"Processed FILETIME request. Success: {data.Success}";
                if (listBoxLog.InvokeRequired)
                {
                    listBoxLog.Invoke(new Action(() => listBoxLog.Items.Add(logMessage)));
                }
                else
                {
                    listBoxLog.Items.Add(logMessage);
                }
            }
            catch (Exception ex)
            {
                if (listBoxLog.InvokeRequired)
                {
                    listBoxLog.Invoke(new Action(() =>
                        listBoxLog.Items.Add($"Error sending response: {ex.Message}")));
                }
                else
                {
                    listBoxLog.Items.Add($"Error sending response: {ex.Message}");
                }
            }
        }

        private byte[] SerializeData(FileTimeData data)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(data.Success);
                writer.Write(data.OriginalFileTime.dwLowDateTime);
                writer.Write(data.OriginalFileTime.dwHighDateTime);
                writer.Write(data.SystemTime.wYear);
                writer.Write(data.SystemTime.wMonth);
                writer.Write(data.SystemTime.wDayOfWeek);
                writer.Write(data.SystemTime.wDay);
                writer.Write(data.SystemTime.wHour);
                writer.Write(data.SystemTime.wMinute);
                writer.Write(data.SystemTime.wSecond);
                writer.Write(data.SystemTime.wMilliseconds);
                return ms.ToArray();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            listBoxLog.Items.Add($"{DateTime.Now}: Server started");
            btnStart.Enabled = false;
        }
    }
}