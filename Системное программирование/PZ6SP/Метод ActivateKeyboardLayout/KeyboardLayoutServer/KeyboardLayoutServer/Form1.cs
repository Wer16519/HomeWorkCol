using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardLayoutServer
{
    public partial class ServerForm : Form
    {
        private uint WM_ACTIVATEKEYBOARDLAYOUT_REQUEST;
        private uint WM_ACTIVATEKEYBOARDLAYOUT_RESPONSE;

        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, uint Flags);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

        [DllImport("user32.dll")]
        private static extern int GetKeyboardLayoutName(StringBuilder pwszKLID);

        private int requestCount = 0;

        public ServerForm()
        {
            InitializeComponent();
            InitializeMessages();

            // Устанавливаем фиксированное имя окна для поиска
            this.Text = "ActivateKeyboardLayout Server";
            this.Name = "ActivateKeyboardLayout Server";

            LogMessage("Server initialized and ready");
        }

        private void InitializeMessages()
        {
            WM_ACTIVATEKEYBOARDLAYOUT_REQUEST = RegisterWindowMessage("WM_ACTIVATEKEYBOARDLAYOUT_REQUEST");
            WM_ACTIVATEKEYBOARDLAYOUT_RESPONSE = RegisterWindowMessage("WM_ACTIVATEKEYBOARDLAYOUT_RESPONSE");

            LogMessage($"Registered message: WM_ACTIVATEKEYBOARDLAYOUT_REQUEST = {WM_ACTIVATEKEYBOARDLAYOUT_REQUEST}");
            LogMessage($"Registered message: WM_ACTIVATEKEYBOARDLAYOUT_RESPONSE = {WM_ACTIVATEKEYBOARDLAYOUT_RESPONSE}");
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ACTIVATEKEYBOARDLAYOUT_REQUEST)
            {
                HandleKeyboardLayoutRequest(ref m);
            }

            base.WndProc(ref m);
        }

        private void HandleKeyboardLayoutRequest(ref Message m)
        {
            try
            {
                requestCount++;
                UpdateCounters();

                string layoutId = Marshal.PtrToStringAnsi(m.LParam);
                LogMessage($"Received layout change request: {layoutId} from client 0x{m.WParam.ToInt64():X8}");

                bool success = ChangeKeyboardLayout(layoutId);
                string currentLayout = GetCurrentKeyboardLayout();

                SendResponse(m.WParam, success, currentLayout);

                LogMessage($"Layout change {(success ? "successful" : "failed")}. Current layout: {currentLayout}");
            }
            catch (Exception ex)
            {
                LogMessage($"Error handling request: {ex.Message}");
                SendResponse(m.WParam, false, "ERROR");
            }
        }

        private bool ChangeKeyboardLayout(string layoutId)
        {
            try
            {
                const uint KLF_ACTIVATE = 0x00000001;

                IntPtr hkl = LoadKeyboardLayout(layoutId, KLF_ACTIVATE);
                if (hkl == IntPtr.Zero)
                {
                    LogMessage($"Failed to load keyboard layout: {layoutId}");
                    return false;
                }

                IntPtr result = ActivateKeyboardLayout(hkl, KLF_ACTIVATE);
                if (result == IntPtr.Zero)
                {
                    LogMessage($"Failed to activate keyboard layout: {layoutId}");
                    return false;
                }

                LogMessage($"Successfully activated layout: {layoutId} (Handle: 0x{hkl.ToInt64():X8})");
                return true;
            }
            catch (Exception ex)
            {
                LogMessage($"Exception in ChangeKeyboardLayout: {ex.Message}");
                return false;
            }
        }

        private string GetCurrentKeyboardLayout()
        {
            StringBuilder layoutName = new StringBuilder(9);
            if (GetKeyboardLayoutName(layoutName) != 0)
            {
                return layoutName.ToString();
            }
            return "UNKNOWN";
        }

        private void SendResponse(IntPtr clientHandle, bool success, string currentLayout)
        {
            try
            {
                byte[] layoutBytes = Encoding.ASCII.GetBytes(currentLayout);
                IntPtr responsePtr = Marshal.AllocHGlobal(layoutBytes.Length + 1);
                Marshal.Copy(layoutBytes, 0, responsePtr, layoutBytes.Length);
                Marshal.WriteByte(responsePtr, layoutBytes.Length, 0);

                IntPtr result = SendMessage(clientHandle, WM_ACTIVATEKEYBOARDLAYOUT_RESPONSE,
                    success ? (IntPtr)1 : (IntPtr)0, responsePtr);

                Marshal.FreeHGlobal(responsePtr);

                LogMessage($"Response sent to client. Success: {success}, Result: {result}");
            }
            catch (Exception ex)
            {
                LogMessage($"Error sending response: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            if (logListBox.InvokeRequired)
            {
                logListBox.Invoke(new Action<string>(LogMessage), message);
            }
            else
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                logListBox.Items.Add($"[{timestamp}] {message}");
                logListBox.SelectedIndex = logListBox.Items.Count - 1;
                logListBox.SelectedIndex = -1;
            }
        }

        private void UpdateCounters()
        {
            if (requestsCountLabel.InvokeRequired)
            {
                requestsCountLabel.Invoke(new Action(UpdateCounters));
            }
            else
            {
                requestsCountLabel.Text = $"Requests: {requestCount}";
            }
        }

        private void clearLogButton_Click(object sender, EventArgs e)
        {
            logListBox.Items.Clear();
            LogMessage("Log cleared");
        }

        private void startServerButton_Click(object sender, EventArgs e)
        {
            this.Text = "ActivateKeyboardLayout Server";

            statusLabel.Text = "Server is running... Waiting for requests";
            startServerButton.Enabled = false;
            stopServerButton.Enabled = true;
            LogMessage("=== SERVER STARTED ===");
            LogMessage("Server name: 'ActivateKeyboardLayout Server'");
            LogMessage("Waiting for client connections...");
        }

        private void stopServerButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Server stopped";
            startServerButton.Enabled = true;
            stopServerButton.Enabled = false;
            LogMessage("Server stopped");
        }
    }
}