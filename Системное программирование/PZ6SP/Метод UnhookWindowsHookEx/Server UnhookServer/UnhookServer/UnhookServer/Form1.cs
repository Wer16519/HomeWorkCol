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
using System.Diagnostics;

namespace UnhookServer
{
    public partial class ServerForm : Form
    {
        private uint WM_UNHOOK_REQUEST;
        private uint WM_UNHOOK_RESPONSE;
        private uint WM_CREATEHOOK_REQUEST;

        private Dictionary<IntPtr, IntPtr> clientHooks = new Dictionary<IntPtr, IntPtr>();

        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        // Делегат для hook процедуры
        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

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

        public ServerForm()
        {
            InitializeComponent();
            InitializeMessages();
            this.Text = "UnhookWindowsHookEx Server";
        }

        private void InitializeMessages()
        {
            WM_UNHOOK_REQUEST = RegisterWindowMessage("WM_UNHOOK_REQUEST");
            WM_UNHOOK_RESPONSE = RegisterWindowMessage("WM_UNHOOK_RESPONSE");
            WM_CREATEHOOK_REQUEST = RegisterWindowMessage("WM_CREATEHOOK_REQUEST");
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CREATEHOOK_REQUEST)
            {
                HandleCreateHookRequest(m);
                return;
            }
            else if (m.Msg == WM_UNHOOK_REQUEST)
            {
                HandleUnhookRequest(m);
                return;
            }

            base.WndProc(ref m);
        }

        private void HandleCreateHookRequest(Message m)
        {
            IntPtr clientHandle = m.WParam;
            UnhookData data = new UnhookData
            {
                ClientHandle = clientHandle,
                Operation = "CREATE",
                Success = false,
                ErrorMessage = ""
            };

            try
            {
                // Пробуем разные типы hook'ов
                IntPtr hookHandle = IntPtr.Zero;
                string hookType = "";

                // Сначала пробуем WH_KEYBOARD (2)
                hookHandle = SetWindowsHookEx(2, KeyboardHookProcedure, IntPtr.Zero, GetCurrentThreadId());
                hookType = "WH_KEYBOARD";

                // Если не получилось, пробуем WH_MOUSE (7)
                if (hookHandle == IntPtr.Zero)
                {
                    hookHandle = SetWindowsHookEx(7, MouseHookProcedure, IntPtr.Zero, GetCurrentThreadId());
                    hookType = "WH_MOUSE";
                }

                // Если все еще не получилось, пробуем WH_GETMESSAGE (3)
                if (hookHandle == IntPtr.Zero)
                {
                    hookHandle = SetWindowsHookEx(3, GetMessageHookProcedure, IntPtr.Zero, GetCurrentThreadId());
                    hookType = "WH_GETMESSAGE";
                }

                if (hookHandle != IntPtr.Zero)
                {
                    clientHooks[clientHandle] = hookHandle;
                    data.Success = true;
                    data.HookHandle = hookHandle;
                    data.ErrorMessage = $"Hook created successfully: {hookType}";

                    if (listBoxLog.InvokeRequired)
                    {
                        listBoxLog.Invoke(new Action(() =>
                            listBoxLog.Items.Add($"Hook created for client: {clientHandle} ({hookType})")));
                    }
                    else
                    {
                        listBoxLog.Items.Add($"Hook created for client: {clientHandle} ({hookType})");
                    }
                }
                else
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    data.ErrorMessage = $"Failed to create hook. Error code: {errorCode}";

                    if (listBoxLog.InvokeRequired)
                    {
                        listBoxLog.Invoke(new Action(() =>
                            listBoxLog.Items.Add($"Failed to create hook for client: {clientHandle}. Error: {errorCode}")));
                    }
                    else
                    {
                        listBoxLog.Items.Add($"Failed to create hook for client: {clientHandle}. Error: {errorCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                data.ErrorMessage = $"Exception: {ex.Message}";

                if (listBoxLog.InvokeRequired)
                {
                    listBoxLog.Invoke(new Action(() =>
                        listBoxLog.Items.Add($"Exception creating hook: {ex.Message}")));
                }
                else
                {
                    listBoxLog.Items.Add($"Exception creating hook: {ex.Message}");
                }
            }

            SendUnhookResponse(clientHandle, data);
        }

        private void HandleUnhookRequest(Message m)
        {
            IntPtr clientHandle = m.WParam;
            IntPtr hookHandle = m.LParam;

            UnhookData data = new UnhookData
            {
                ClientHandle = clientHandle,
                HookHandle = hookHandle,
                Operation = "UNHOOK",
                Success = false,
                ErrorMessage = ""
            };

            try
            {
                if (clientHooks.ContainsKey(clientHandle) && clientHooks[clientHandle] == hookHandle && hookHandle != IntPtr.Zero)
                {
                    bool success = UnhookWindowsHookEx(hookHandle);
                    if (success)
                    {
                        clientHooks.Remove(clientHandle);
                        data.Success = true;
                        data.ErrorMessage = "Hook removed successfully";

                        if (listBoxLog.InvokeRequired)
                        {
                            listBoxLog.Invoke(new Action(() =>
                                listBoxLog.Items.Add($"Hook removed for client: {clientHandle}")));
                        }
                        else
                        {
                            listBoxLog.Items.Add($"Hook removed for client: {clientHandle}");
                        }
                    }
                    else
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        data.ErrorMessage = $"Failed to remove hook. Error code: {errorCode}";
                    }
                }
                else
                {
                    data.ErrorMessage = "Hook not found or invalid handle";
                }
            }
            catch (Exception ex)
            {
                data.ErrorMessage = $"Exception: {ex.Message}";
            }

            SendUnhookResponse(clientHandle, data);
        }

        // Процедура для keyboard hook
        private IntPtr KeyboardHookProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Всегда передаем сообщение дальше
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        // Процедура для mouse hook
        private IntPtr MouseHookProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Всегда передаем сообщение дальше
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        // Процедура для get message hook
        private IntPtr GetMessageHookProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Всегда передаем сообщение дальше
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private void SendUnhookResponse(IntPtr clientHandle, UnhookData data)
        {
            try
            {
                byte[] buffer = SerializeData(data);

                MyCOPYDATASTRUCT cds = new MyCOPYDATASTRUCT
                {
                    dwData = (IntPtr)WM_UNHOOK_RESPONSE,
                    cbData = buffer.Length,
                    lpData = Marshal.AllocCoTaskMem(buffer.Length)
                };

                Marshal.Copy(buffer, 0, cds.lpData, buffer.Length);

                IntPtr cdsPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cds));
                Marshal.StructureToPtr(cds, cdsPtr, false);

                SendMessage(clientHandle, 0x004A, this.Handle, cdsPtr);

                Marshal.FreeCoTaskMem(cds.lpData);
                Marshal.FreeCoTaskMem(cdsPtr);

                // Обновляем счетчик клиентов
                UpdateClientsCount();
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

        private void UpdateClientsCount()
        {
            if (labelClientsCount.InvokeRequired)
            {
                labelClientsCount.Invoke(new Action(() =>
                    labelClientsCount.Text = $"Active Hooks: {clientHooks.Count}"));
            }
            else
            {
                labelClientsCount.Text = $"Active Hooks: {clientHooks.Count}";
            }
        }

        private byte[] SerializeData(UnhookData data)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(data.ClientHandle.ToInt64());
                writer.Write(data.HookHandle.ToInt64());
                writer.Write(data.Operation ?? "");
                writer.Write(data.Success);
                writer.Write(data.ErrorMessage ?? "");
                return ms.ToArray();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            listBoxLog.Items.Add($"{DateTime.Now}: Server started");
            listBoxLog.Items.Add("Note: Hooks may require administrator privileges");
            btnStart.Enabled = false;
            UpdateClientsCount();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Удаляем все hooks при закрытии формы
            foreach (var hook in clientHooks.Values)
            {
                if (hook != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(hook);
                }
            }
            base.OnFormClosing(e);
        }
    }
}
