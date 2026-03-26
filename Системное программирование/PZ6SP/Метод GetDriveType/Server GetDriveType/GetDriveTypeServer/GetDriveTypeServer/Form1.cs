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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace GetDriveTypeServer
{
    public partial class ServerForm : Form
    {
        private uint WM_GETDRIVETYPE_REQUEST;
        private uint WM_GETDRIVETYPE_RESPONSE;

        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern uint GetDriveType(string lpRootPathName);

        public ServerForm()
        {
            InitializeComponent();
            InitializeMessages();
            this.Text = "GetDriveType Server";
        }

        private void InitializeMessages()
        {
            WM_GETDRIVETYPE_REQUEST = RegisterWindowMessage("WM_GETDRIVETYPE_REQUEST");
            WM_GETDRIVETYPE_RESPONSE = RegisterWindowMessage("WM_GETDRIVETYPE_RESPONSE");
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_GETDRIVETYPE_REQUEST)
            {
                HandleDriveTypeRequest(m);
                return;
            }

            base.WndProc(ref m);
        }

        private void HandleDriveTypeRequest(Message m)
        {
            IntPtr clientHandle = m.WParam;

            // Безопасное получение строки
            string drivePath = GetStringFromPointer(m.LParam);
            if (string.IsNullOrEmpty(drivePath))
            {
                drivePath = "C:\\";
            }

            uint driveType = GetDriveType(drivePath);
            string description = GetDriveTypeDescription(driveType);

            DriveTypeData data = new DriveTypeData
            {
                DrivePath = drivePath,
                DriveType = driveType,
                Description = description
            };

            SendDriveTypeResponse(clientHandle, data);
        }

        private string GetStringFromPointer(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return "C:\\";

            try
            {
                // Получаем длину строки
                int length = 0;
                while (Marshal.ReadByte(ptr, length) != 0)
                {
                    length++;
                }

                if (length == 0)
                    return "C:\\";

                byte[] buffer = new byte[length];
                Marshal.Copy(ptr, buffer, 0, length);
                return Encoding.UTF8.GetString(buffer);
            }
            catch
            {
                return "C:\\";
            }
        }

        private string GetDriveTypeDescription(uint driveType)
        {
            switch (driveType)
            {
                case 0: return "Unknown type";
                case 1: return "Drive does not exist";
                case 2: return "Removable drive";
                case 3: return "Local drive";
                case 4: return "Network drive";
                case 5: return "CD-ROM";
                case 6: return "RAM disk";
                default: return "Unknown type";
            }
        }

        private void SendDriveTypeResponse(IntPtr clientHandle, DriveTypeData data)
        {
            try
            {
                byte[] buffer = SerializeData(data);

                COPYDATASTRUCT cds = new COPYDATASTRUCT
                {
                    dwData = (IntPtr)WM_GETDRIVETYPE_RESPONSE,
                    cbData = buffer.Length,
                    lpData = Marshal.AllocCoTaskMem(buffer.Length)
                };

                Marshal.Copy(buffer, 0, cds.lpData, buffer.Length);

                IntPtr cdsPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cds));
                Marshal.StructureToPtr(cds, cdsPtr, false);

                SendMessage(clientHandle, 0x004A, this.Handle, cdsPtr);

                Marshal.FreeCoTaskMem(cds.lpData);
                Marshal.FreeCoTaskMem(cdsPtr);

                // Логируем в UI потоке
                if (listBoxLog.InvokeRequired)
                {
                    listBoxLog.Invoke(new Action(() =>
                        listBoxLog.Items.Add($"Processed request for: {data.DrivePath}")));
                }
                else
                {
                    listBoxLog.Items.Add($"Processed request for: {data.DrivePath}");
                }
            }
            catch (Exception ex)
            {
                if (listBoxLog.InvokeRequired)
                {
                    listBoxLog.Invoke(new Action(() =>
                        listBoxLog.Items.Add($"Error: {ex.Message}")));
                }
                else
                {
                    listBoxLog.Items.Add($"Error: {ex.Message}");
                }
            }
        }

        private byte[] SerializeData(DriveTypeData data)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(data.DrivePath ?? "C:\\");
                writer.Write(data.DriveType);
                writer.Write(data.Description ?? "Unknown");
                return ms.ToArray();
            }
        }

        private void btnStart_Click_1(object sender, EventArgs e)
        {
            listBoxLog.Items.Add($"{DateTime.Now}: Server started");
            btnStart.Enabled = false;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }

    public class DriveTypeData
    {
        public string DrivePath { get; set; }
        public uint DriveType { get; set; }
        public string Description { get; set; }
    }
}
