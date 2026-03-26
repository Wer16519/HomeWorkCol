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

namespace KeyboardLayoutClient
{
    public partial class ClientForm : Form
    {
        private uint WM_ACTIVATEKEYBOARDLAYOUT_REQUEST;
        private uint WM_ACTIVATEKEYBOARDLAYOUT_RESPONSE;

        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern int GetKeyboardLayoutName(StringBuilder pwszKLID);

        public ClientForm()
        {
            InitializeComponent();
            InitializeMessages();
            UpdateCurrentLayout();

            // Заполняем комбобокс предустановленными раскладками
            presetLayoutsComboBox.Items.AddRange(new object[] {
                "English (US) - 00000409",
                "Russian - 00000419",
                "German - 00000407",
                "French - 0000040C",
                "Spanish - 0000040A",
                "Japanese - 00000411",
                "Chinese - 00000804"
            });
            presetLayoutsComboBox.SelectedIndex = 0;

            this.Text = "ActivateKeyboardLayout Client";
            CheckServerConnection();
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
            if (m.Msg == WM_ACTIVATEKEYBOARDLAYOUT_RESPONSE)
            {
                HandleKeyboardLayoutResponse(ref m);
            }

            base.WndProc(ref m);
        }

        private void HandleKeyboardLayoutResponse(ref Message m)
        {
            try
            {
                bool success = m.WParam != IntPtr.Zero;
                string currentLayout = Marshal.PtrToStringAnsi(m.LParam);

                LogMessage($"Received response from server: Success={success}, CurrentLayout={currentLayout}");

                if (success)
                {
                    statusLabel.Text = $"Last operation: Success - Layout changed to {currentLayout}";
                    UpdateCurrentLayout();
                }
                else
                {
                    statusLabel.Text = "Last operation: Failed - Could not change layout";
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error handling response: {ex.Message}");
            }
        }

        private void sendRequestButton_Click(object sender, EventArgs e)
        {
            string layoutId = layoutIdTextBox.Text.Trim();

            if (string.IsNullOrEmpty(layoutId))
            {
                MessageBox.Show("Please enter a layout ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SendKeyboardLayoutRequest(layoutId);
        }

        private void SendKeyboardLayoutRequest(string layoutId)
        {
            try
            {
                IntPtr serverHandle = FindServerWindow();
                if (serverHandle == IntPtr.Zero)
                {
                    LogMessage("Error: Server not found. Make sure server is running.");
                    statusLabel.Text = "Error: Server not found";
                    return;
                }

                LogMessage($"Sending layout change request to server: {layoutId}");

                byte[] layoutBytes = Encoding.ASCII.GetBytes(layoutId);
                IntPtr layoutPtr = Marshal.AllocHGlobal(layoutBytes.Length + 1);
                Marshal.Copy(layoutBytes, 0, layoutPtr, layoutBytes.Length);
                Marshal.WriteByte(layoutPtr, layoutBytes.Length, 0);

                IntPtr result = SendMessage(serverHandle, WM_ACTIVATEKEYBOARDLAYOUT_REQUEST,
                    this.Handle, layoutPtr);

                Marshal.FreeHGlobal(layoutPtr);

                LogMessage($"Request sent to server. Result: {result}");
                statusLabel.Text = $"Request sent - waiting for response...";
            }
            catch (Exception ex)
            {
                LogMessage($"Error sending request: {ex.Message}");
                statusLabel.Text = "Error sending request";
            }
        }

        private IntPtr FindServerWindow()
        {
            // Ищем окно сервера по точному заголовку
            IntPtr hwnd = FindWindow(null, "ActivateKeyboardLayout Server");

            if (hwnd == IntPtr.Zero)
            {
                LogMessage("Server window not found with exact match, trying partial search...");

                // Альтернативный поиск по части имени
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
                foreach (System.Diagnostics.Process process in processes)
                {
                    if (!string.IsNullOrEmpty(process.MainWindowTitle) &&
                        process.MainWindowTitle.Contains("ActivateKeyboardLayout"))
                    {
                        LogMessage($"Found server process: {process.MainWindowTitle}");
                        return process.MainWindowHandle;
                    }
                }

                LogMessage("Server not found in running processes");
            }
            else
            {
                LogMessage($"Server found successfully: 0x{hwnd.ToInt64():X8}");
            }

            return hwnd;
        }

        private void refreshLayoutButton_Click(object sender, EventArgs e)
        {
            UpdateCurrentLayout();
        }

        private void UpdateCurrentLayout()
        {
            try
            {
                StringBuilder layoutName = new StringBuilder(9);
                if (GetKeyboardLayoutName(layoutName) != 0)
                {
                    currentLayoutLabel.Text = $"Current Layout: {layoutName.ToString()}";
                    LogMessage($"Current keyboard layout: {layoutName.ToString()}");
                }
                else
                {
                    currentLayoutLabel.Text = "Current Layout: Unknown";
                    LogMessage("Failed to get current keyboard layout");
                }
            }
            catch (Exception ex)
            {
                currentLayoutLabel.Text = "Current Layout: Error";
                LogMessage($"Error getting current layout: {ex.Message}");
            }
        }

        private void presetLayoutsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (presetLayoutsComboBox.SelectedItem != null)
            {
                string selected = presetLayoutsComboBox.SelectedItem.ToString();
                string layoutId = selected.Split('-')[1].Trim();
                layoutIdTextBox.Text = layoutId;
            }
        }

        private void layoutIdTextBox_TextChanged(object sender, EventArgs e)
        {
            // Validation can be added here
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
        private void CheckServerConnection()
        {
            IntPtr serverHandle = FindServerWindow();
            if (serverHandle == IntPtr.Zero)
            {
                statusLabel.Text = "Status: Server NOT FOUND - Start server first";
                statusLabel.ForeColor = Color.Red;
                sendRequestButton.Enabled = false;
            }
            else
            {
                statusLabel.Text = "Status: Server CONNECTED - Ready to send requests";
                statusLabel.ForeColor = Color.Green;
                sendRequestButton.Enabled = true;
            }
        }

        private void CheckConnectionButton_Click(object sender, EventArgs e)
        {
            CheckServerConnection();
        }


    }
}