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

namespace UnhookClient
{
    public partial class ClientForm : Form
    {
        private uint WM_UNHOOK_REQUEST;
        private uint WM_UNHOOK_RESPONSE;
        private uint WM_CREATEHOOK_REQUEST;

        private IntPtr currentHook = IntPtr.Zero;

        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string className, string windowName);

        // Структура для WM_COPYDATA
        [StructLayout(LayoutKind.Sequential)]
        public struct MyCOPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        // Класс для данных
        public class UnhookData
        {
            public IntPtr ClientHandle { get; set; }
            public IntPtr HookHandle { get; set; }
            public string Operation { get; set; }
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }

        public ClientForm()
        {
            InitializeComponent();
            InitializeMessages();
            this.Text = "UnhookWindowsHookEx Client";
            UpdateCurrentHookDisplay();
        }

        private void InitializeMessages()
        {
            WM_UNHOOK_REQUEST = RegisterWindowMessage("WM_UNHOOK_REQUEST");
            WM_UNHOOK_RESPONSE = RegisterWindowMessage("WM_UNHOOK_RESPONSE");
            WM_CREATEHOOK_REQUEST = RegisterWindowMessage("WM_CREATEHOOK_REQUEST");
        }

        private void btnCreateHook_Click(object sender, EventArgs e)
        {
            IntPtr serverHandle = FindWindow(null, "UnhookWindowsHookEx Server");

            if (serverHandle == IntPtr.Zero)
            {
                MessageBox.Show("Server not found! Make sure server is running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool success = PostMessage(serverHandle, WM_CREATEHOOK_REQUEST, this.Handle, IntPtr.Zero);
            if (success)
            {
                listBoxResults.Items.Add("Hook creation request sent");
            }
            else
            {
                listBoxResults.Items.Add("Failed to send hook creation request");
            }
        }

        private void btnUnhook_Click(object sender, EventArgs e)
        {
            if (currentHook == IntPtr.Zero)
            {
                MessageBox.Show("No active hook to unhook!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            IntPtr serverHandle = FindWindow(null, "UnhookWindowsHookEx Server");

            if (serverHandle == IntPtr.Zero)
            {
                MessageBox.Show("Server not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool success = PostMessage(serverHandle, WM_UNHOOK_REQUEST, this.Handle, currentHook);
            if (success)
            {
                listBoxResults.Items.Add($"Unhook request sent for: {currentHook}");
            }
            else
            {
                listBoxResults.Items.Add("Failed to send unhook request");
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
                MyCOPYDATASTRUCT cds = (MyCOPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(MyCOPYDATASTRUCT));

                if ((uint)cds.dwData == WM_UNHOOK_RESPONSE)
                {
                    byte[] buffer = new byte[cds.cbData];
                    Marshal.Copy(cds.lpData, buffer, 0, cds.cbData);

                    UnhookData data = DeserializeData(buffer);
                    DisplayResult(data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing response: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private UnhookData DeserializeData(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                UnhookData data = new UnhookData();
                data.ClientHandle = new IntPtr(reader.ReadInt64());
                data.HookHandle = new IntPtr(reader.ReadInt64());
                data.Operation = reader.ReadString();
                data.Success = reader.ReadBoolean();
                data.ErrorMessage = reader.ReadString();
                return data;
            }
        }

        private void DisplayResult(UnhookData data)
        {
            if (data.Operation == "CREATE" && data.Success)
            {
                currentHook = data.HookHandle;
            }
            else if (data.Operation == "UNHOOK" && data.Success)
            {
                currentHook = IntPtr.Zero;
            }

            string result = $"{data.Operation}: {(data.Success ? "Success" : "Failed")}";

            if (listBoxResults.InvokeRequired)
            {
                listBoxResults.Invoke(new Action(() =>
                {
                    UpdateDisplay(data, result);
                }));
            }
            else
            {
                UpdateDisplay(data, result);
            }
        }

        private void UpdateDisplay(UnhookData data, string result)
        {
            listBoxResults.Items.Add($"Result: {result}");
            if (!string.IsNullOrEmpty(data.ErrorMessage))
            {
                listBoxResults.Items.Add($"Error: {data.ErrorMessage}");
            }

            txtResult.Text = $"Operation: {data.Operation}\r\n" +
                           $"Success: {data.Success}\r\n" +
                           $"Hook Handle: {data.HookHandle}\r\n" +
                           $"Error: {data.ErrorMessage}\r\n" +
                           $"Current Hook: {currentHook}";

            UpdateCurrentHookDisplay();
        }

        private void UpdateCurrentHookDisplay()
        {
            labelCurrentHook.Text = $"Current Hook: {(currentHook == IntPtr.Zero ? "None" : currentHook.ToString())}";
            labelCurrentHook.ForeColor = currentHook == IntPtr.Zero ? System.Drawing.Color.Red : System.Drawing.Color.Green;
        }

        private void btnTestHook_Click(object sender, EventArgs e)
        {
            // Тестовая кнопка для проверки работы hook'а
            if (currentHook != IntPtr.Zero)
            {
                MessageBox.Show($"Hook is active: {currentHook}\nTry pressing keys or moving mouse to test.", "Hook Test",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No active hook. Create a hook first.", "Hook Test",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            listBoxResults.Items.Clear();
            txtResult.Clear();
        }
    }
}