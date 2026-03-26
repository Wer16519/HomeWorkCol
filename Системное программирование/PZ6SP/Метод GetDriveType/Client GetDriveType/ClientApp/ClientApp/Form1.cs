using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace ClientApp
{
    public partial class ClientForm : Form
    {
        private uint WM_GETDRIVETYPE_REQUEST;
        private uint WM_GETDRIVETYPE_RESPONSE;

        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public ClientForm()
        {
            InitializeComponent();
            InitializeMessages();
            this.Text = "GetDriveType Client";
        }

        private void InitializeMessages()
        {
            WM_GETDRIVETYPE_REQUEST = RegisterWindowMessage("WM_GETDRIVETYPE_REQUEST");
            WM_GETDRIVETYPE_RESPONSE = RegisterWindowMessage("WM_GETDRIVETYPE_RESPONSE");
        }

        private void btnGetDriveType_Click_1(object sender, EventArgs e)
                {
            string drivePath = txtDrivePath.Text;
            if (string.IsNullOrEmpty(drivePath))
            {
                drivePath = "C:\\";
            }

            IntPtr serverHandle = FindWindow(null, "GetDriveType Server");

            if (serverHandle == IntPtr.Zero)
            {
                MessageBox.Show("Server not found! Make sure server is running.");
                return;
            }

            try
            {
                // Безопасная передача строки
                byte[] drivePathBytes = Encoding.UTF8.GetBytes(drivePath + "\0");
                IntPtr drivePathPtr = Marshal.AllocCoTaskMem(drivePathBytes.Length);
                Marshal.Copy(drivePathBytes, 0, drivePathPtr, drivePathBytes.Length);

                PostMessage(serverHandle, WM_GETDRIVETYPE_REQUEST, this.Handle, drivePathPtr);
                Marshal.FreeCoTaskMem(drivePathPtr);

                listBoxResults.Items.Add($"Request sent for: {drivePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
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
                COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));

                if ((uint)cds.dwData == WM_GETDRIVETYPE_RESPONSE)
                {
                    byte[] buffer = new byte[cds.cbData];
                    Marshal.Copy(cds.lpData, buffer, 0, cds.cbData);

                    DriveTypeData data = DeserializeData(buffer);
                    DisplayResult(data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing response: {ex.Message}");
            }
        }

        private DriveTypeData DeserializeData(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                return new DriveTypeData
                {
                    DrivePath = reader.ReadString(),
                    DriveType = reader.ReadUInt32(),
                    Description = reader.ReadString()
                };
            }
        }

        private void DisplayResult(DriveTypeData data)
        {
            string result = $"Drive: {data.DrivePath}, Type: {data.DriveType} ({data.Description})";

            if (listBoxResults.InvokeRequired)
            {
                listBoxResults.Invoke(new Action(() =>
                {
                    listBoxResults.Items.Add($"Result: {result}");
                    txtResult.Text = $"Path: {data.DrivePath}\r\nCode: {data.DriveType}\r\nDescription: {data.Description}";
                }));
            }
            else
            {
                listBoxResults.Items.Add($"Result: {result}");
                txtResult.Text = $"Path: {data.DrivePath}\r\nCode: {data.DriveType}\r\nDescription: {data.Description}";
            }
        }

        private void listBoxResults_SelectedIndexChanged(object sender, EventArgs e)
        {

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