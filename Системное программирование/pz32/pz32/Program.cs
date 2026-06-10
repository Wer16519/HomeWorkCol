using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;

class pz32
{
    // ======================== Константы DDEML ========================
    private const uint DDE_FMT_TEXT = 1;
    private const uint XTYP_CONNECT = 0x0060;
    private const uint XTYP_POKE = 0x0090;
    private const uint XTYP_REQUEST = 0x0080;
    private const uint XTYP_EXECUTE = 0x0050;
    private const uint DDE_FACK = 0x8000;
    private const uint APPCLASS_STANDARD = 0x0000;
    private const uint DNS_REGISTER = 0x0001;
    private const uint DNS_UNREGISTER = 0x0002;
    private const uint DMLERR_NO_ERROR = 0;
    private const uint DMLERR_NO_CONV_ESTABLISHED = 0x4006;
    private const uint QCONVINFO = 0xFFFFFFFF;
    private const int TIMEOUT = 5000;

    // ======================== Импорт API-функций ========================
    [DllImport("user32.dll")]
    private static extern uint DdeInitialize(ref uint pidInst, DdeCallback pfnCallback, uint afCmd, uint ulRes);

    [DllImport("user32.dll")]
    private static extern bool DdeUninitialize(uint idInst);

    [DllImport("user32.dll")]
    private static extern IntPtr DdeCreateStringHandle(uint idInst, string psz, int iCodePage);

    [DllImport("user32.dll")]
    private static extern bool DdeFreeStringHandle(uint idInst, IntPtr hsz);

    [DllImport("user32.dll")]
    private static extern IntPtr DdeConnect(uint idInst, IntPtr hszService, IntPtr hszTopic, IntPtr pCC);

    [DllImport("user32.dll")]
    private static extern bool DdeDisconnect(IntPtr hConv);

    [DllImport("user32.dll")]
    private static extern IntPtr DdeClientTransaction(IntPtr pData, uint cbData, IntPtr hConv,
        IntPtr hszItem, uint uFmt, uint uType, uint dwTimeout, out IntPtr pdwResult);

    [DllImport("user32.dll")]
    private static extern IntPtr DdeCreateDataHandle(uint idInst, byte[] pSrc, uint cb, uint cbOff,
        IntPtr hszItem, uint uFmt, uint afCmd);

    [DllImport("user32.dll")]
    private static extern IntPtr DdeAccessData(IntPtr hData, out uint pcbData);

    [DllImport("user32.dll")]
    private static extern bool DdeFreeDataHandle(IntPtr hData);

    [DllImport("user32.dll")]
    private static extern uint DdeGetLastError(uint idInst);

    [DllImport("user32.dll")]
    private static extern bool DdeNameService(uint idInst, IntPtr hsz1, IntPtr hsz2, uint afCmd);

    [DllImport("user32.dll")]
    private static extern uint DdeQueryConvInfo(IntPtr hConv, uint idTransaction, ref CONVINFO pConvInfo);

    [DllImport("user32.dll")]
    private static extern bool DdeImpersonateClient(IntPtr hConv);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern uint DdeQueryString(uint idInst, IntPtr hsz, StringBuilder psz, uint cchMax, int iCodePage);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    [StructLayout(LayoutKind.Sequential)]
    private struct CONVINFO
    {
        public uint cb;
        public IntPtr hUser;
        public IntPtr hConvPartner;
        public IntPtr hszSvcPartner;
        public IntPtr hszServiceReq;
        public IntPtr hszTopic;
        public IntPtr hszItem;
        public uint wFmt;
        public uint wType;
        public uint wStatus;
        public uint wConvst;
        public uint wLastError;
    }

    private delegate uint DdeCallback(uint uType, uint uFmt, IntPtr hConv,
        IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2);

    // Глобальные переменные
    private static uint _idInst = 0;
    private static DdeCallback _callback;
    private static bool _serverRunning = true;
    private static string _lastReceivedData = "";

    static void Main()
    {
        Console.WriteLine("=== Практическое занятие 32: DDEML ===\n");

        // Инициализация DDEML
        _callback = new DdeCallback(DdeCallbackHandler);
        uint res = DdeInitialize(ref _idInst, _callback, APPCLASS_STANDARD, 0);
        if (res != DMLERR_NO_ERROR)
        {
            Console.WriteLine("Ошибка инициализации DDE: " + res);
            return;
        }
        Console.WriteLine("[OK] DDEML инициализирован\n");

        // Запуск DDE-сервера (Задание 4)
        Thread serverThread = new Thread(new ThreadStart(RunDDEServer));
        serverThread.IsBackground = true;
        serverThread.Start();
        Thread.Sleep(1000);

        // ======================== ЗАДАНИЕ 1 ========================
        Console.WriteLine("========== ЗАДАНИЕ 1 ==========");
        Console.WriteLine("Отправка и получение данных между приложениями");
        Task1_SendAndReceiveData();
        Console.WriteLine();

        // ======================== ЗАДАНИЕ 2 ========================
        Console.WriteLine("========== ЗАДАНИЕ 2 ==========");
        Console.WriteLine("Функция обратного вызова реализована в DdeCallbackHandler()");
        Console.WriteLine("Она вызывается при получении данных сервером");
        Console.WriteLine("Последние полученные данные: '" + _lastReceivedData + "'\n");

        // ======================== ЗАДАНИЕ 3 ========================
        Console.WriteLine("========== ЗАДАНИЕ 3 ==========");
        Console.WriteLine("Использование DdeClientTransaction для отправки данных");
        Task3_ClientTransaction();
        Console.WriteLine();

        // ======================== ЗАДАНИЕ 4 ========================
        Console.WriteLine("========== ЗАДАНИЕ 4 ==========");
        Console.WriteLine("DDE-сервер запущен в отдельном потоке");
        Console.WriteLine("Сервер зарегистрирован через DdeNameService()\n");

        // ======================== ЗАДАНИЕ 5 ========================
        Console.WriteLine("========== ЗАДАНИЕ 5 ==========");
        Task5_MultithreadedServer();
        Console.WriteLine();

        // ======================== ЗАДАНИЕ 6 ========================
        Console.WriteLine("========== ЗАДАНИЕ 6 ==========");
        Task6_ConnectAndDisconnect();
        Console.WriteLine();

        // ======================== ЗАДАНИЕ 7 ========================
        Console.WriteLine("========== ЗАДАНИЕ 7 ==========");
        Task7_AsyncExecute();
        Console.WriteLine();

        // ======================== ЗАДАНИЕ 8 ========================
        Console.WriteLine("========== ЗАДАНИЕ 8 ==========");
        Task8_QueueLimits();
        Console.WriteLine();

        // ======================== ЗАДАНИЕ 9 ========================
        Console.WriteLine("========== ЗАДАНИЕ 9 ==========");
        Task9_QueryInfo();
        Console.WriteLine();

        // ======================== ЗАДАНИЕ 10 =======================
        Console.WriteLine("========== ЗАДАНИЕ 10 =========");
        Task10_ErrorHandling();
        Console.WriteLine();

        // Очистка
        _serverRunning = false;
        DdeUninitialize(_idInst);
        Console.WriteLine("\n[OK] DDEML деинициализирован. Нажмите любую клавишу для выхода.");
        Console.ReadKey();
    }

    // ======================== ЗАДАНИЕ 1 ========================
    static void Task1_SendAndReceiveData()
    {
        IntPtr hszService = DdeCreateStringHandle(_idInst, "DDEServer", 1251);
        IntPtr hszTopic = DdeCreateStringHandle(_idInst, "DNETopic", 1251);

        // Подключаемся как клиент
        IntPtr hConv = DdeConnect(_idInst, hszService, hszTopic, IntPtr.Zero);
        if (hConv != IntPtr.Zero)
        {
            // Отправка данных (POKE)
            string dataToSend = "Привет от клиента!";
            byte[] bytes = Encoding.Default.GetBytes(dataToSend);
            IntPtr hData = DdeCreateDataHandle(_idInst, bytes, (uint)bytes.Length, 0, IntPtr.Zero, DDE_FMT_TEXT, 0);
            IntPtr hszItem = DdeCreateStringHandle(_idInst, "TestItem", 1251);
            IntPtr pResult;

            IntPtr ret = DdeClientTransaction(IntPtr.Zero, 0, hConv, hszItem, DDE_FMT_TEXT, XTYP_POKE, TIMEOUT, out pResult);
            Console.WriteLine("Отправка данных: " + ((ret != IntPtr.Zero) ? "УСПЕХ" : "ОШИБКА"));

            DdeFreeDataHandle(hData);
            DdeFreeStringHandle(_idInst, hszItem);
            DdeDisconnect(hConv);
        }
        else
        {
            Console.WriteLine("Не удалось подключиться к серверу");
        }

        DdeFreeStringHandle(_idInst, hszService);
        DdeFreeStringHandle(_idInst, hszTopic);
    }

    // ======================== ЗАДАНИЕ 3 ========================
    static void Task3_ClientTransaction()
    {
        IntPtr hszService = DdeCreateStringHandle(_idInst, "DDEServer", 1251);
        IntPtr hszTopic = DdeCreateStringHandle(_idInst, "DNETopic", 1251);
        IntPtr hConv = DdeConnect(_idInst, hszService, hszTopic, IntPtr.Zero);

        if (hConv != IntPtr.Zero)
        {
            IntPtr hszItem = DdeCreateStringHandle(_idInst, "TransactionItem", 1251);
            IntPtr pResult;

            // XTYP_POKE - отправка данных
            IntPtr ret = DdeClientTransaction(IntPtr.Zero, 0, hConv, hszItem, DDE_FMT_TEXT, XTYP_POKE, TIMEOUT, out pResult);
            Console.WriteLine("DdeClientTransaction (POKE): " + ((ret != IntPtr.Zero) ? "УСПЕХ" : "ОШИБКА"));

            // XTYP_REQUEST - запрос данных
            ret = DdeClientTransaction(IntPtr.Zero, 0, hConv, hszItem, DDE_FMT_TEXT, XTYP_REQUEST, TIMEOUT, out pResult);
            if (ret != IntPtr.Zero)
            {
                uint size;
                IntPtr dataPtr = DdeAccessData(pResult, out size);
                byte[] received = new byte[size];
                Marshal.Copy(dataPtr, received, 0, (int)size);
                Console.WriteLine("DdeClientTransaction (REQUEST): Получено '" + Encoding.Default.GetString(received) + "'");
                DdeFreeDataHandle(pResult);
            }
            else
            {
                Console.WriteLine("DdeClientTransaction (REQUEST): ОШИБКА");
            }

            DdeFreeStringHandle(_idInst, hszItem);
            DdeDisconnect(hConv);
        }
        DdeFreeStringHandle(_idInst, hszService);
        DdeFreeStringHandle(_idInst, hszTopic);
    }

    // ======================== ЗАДАНИЕ 5 ========================
    static void Task5_MultithreadedServer()
    {
        Console.WriteLine("Многопоточность в DDE-сервере реализована через:");
        Console.WriteLine("1. Отдельный поток для сервера (RunDDEServer)");
        Console.WriteLine("2. Использование DdeImpersonateClient для имитации клиента");

        // Демонстрация DdeImpersonateClient
        IntPtr hszService = DdeCreateStringHandle(_idInst, "DDEServer", 1251);
        IntPtr hszTopic = DdeCreateStringHandle(_idInst, "DNETopic", 1251);
        IntPtr hConv = DdeConnect(_idInst, hszService, hszTopic, IntPtr.Zero);

        if (hConv != IntPtr.Zero)
        {
            bool impersonated = DdeImpersonateClient(hConv);
            Console.WriteLine("DdeImpersonateClient: " + (impersonated ? "Успешно" : "Ошибка"));
            if (impersonated)
            {
                Console.WriteLine("  Идентификатор текущего потока: " + GetCurrentThreadId());
            }
            DdeDisconnect(hConv);
        }

        DdeFreeStringHandle(_idInst, hszService);
        DdeFreeStringHandle(_idInst, hszTopic);
    }

    // ======================== ЗАДАНИЕ 6 ========================
    static void Task6_ConnectAndDisconnect()
    {
        IntPtr hszService = DdeCreateStringHandle(_idInst, "DDEServer", 1251);
        IntPtr hszTopic = DdeCreateStringHandle(_idInst, "DNETopic", 1251);

        // DdeConnect - установка соединения
        IntPtr hConv = DdeConnect(_idInst, hszService, hszTopic, IntPtr.Zero);
        Console.WriteLine("DdeConnect: " + ((hConv != IntPtr.Zero) ? ("УСПЕХ (дескриптор: " + hConv + ")") : "ОШИБКА"));

        // DdeDisconnect - разрыв соединения
        if (hConv != IntPtr.Zero)
        {
            bool result = DdeDisconnect(hConv);
            Console.WriteLine("DdeDisconnect: " + (result ? "УСПЕХ" : "ОШИБКА"));
        }

        DdeFreeStringHandle(_idInst, hszService);
        DdeFreeStringHandle(_idInst, hszTopic);
    }

    // ======================== ЗАДАНИЕ 7 ========================
    static void Task7_AsyncExecute()
    {
        IntPtr hszService = DdeCreateStringHandle(_idInst, "DDEServer", 1251);
        IntPtr hszTopic = DdeCreateStringHandle(_idInst, "DNETopic", 1251);
        IntPtr hConv = DdeConnect(_idInst, hszService, hszTopic, IntPtr.Zero);

        if (hConv != IntPtr.Zero)
        {
            // Асинхронное выполнение команды через XTYP_EXECUTE
            string command = "[CalculateSum(10,20)]";
            byte[] cmdBytes = Encoding.Default.GetBytes(command);
            IntPtr hCmdData = DdeCreateDataHandle(_idInst, cmdBytes, (uint)cmdBytes.Length, 0, IntPtr.Zero, DDE_FMT_TEXT, 0);
            IntPtr pResult;

            IntPtr ret = DdeClientTransaction(IntPtr.Zero, 0, hConv, IntPtr.Zero, DDE_FMT_TEXT, XTYP_EXECUTE, TIMEOUT, out pResult);
            Console.WriteLine("Асинхронная команда (XTYP_EXECUTE): " + ((ret != IntPtr.Zero) ? "ОТПРАВЛЕНА" : "ОШИБКА"));

            DdeFreeDataHandle(hCmdData);
            DdeDisconnect(hConv);
        }
        DdeFreeStringHandle(_idInst, hszService);
        DdeFreeStringHandle(_idInst, hszTopic);
    }

    // ======================== ЗАДАНИЕ 8 ========================
    static void Task8_QueueLimits()
    {
        Console.WriteLine("Функция DdeSetQueueLimits отсутствует в современных версиях user32.dll");
        Console.WriteLine("Альтернатива: управление очередью сообщений через настройки DDEML при инициализации");
        Console.WriteLine("  APPCMD_CLIENTONLY - ограничение клиентской части");
        Console.WriteLine("  APPCMD_FILTERONLY - фильтрация сообщений");
    }

    // ======================== ЗАДАНИЕ 9 ========================
    static void Task9_QueryInfo()
    {
        IntPtr hszService = DdeCreateStringHandle(_idInst, "DDEServer", 1251);
        IntPtr hszTopic = DdeCreateStringHandle(_idInst, "DNETopic", 1251);
        IntPtr hConv = DdeConnect(_idInst, hszService, hszTopic, IntPtr.Zero);

        if (hConv != IntPtr.Zero)
        {
            CONVINFO info = new CONVINFO();
            info.cb = (uint)Marshal.SizeOf(typeof(CONVINFO));
            uint result = DdeQueryConvInfo(hConv, QCONVINFO, ref info);

            if (result != 0)
            {
                Console.WriteLine("DdeQueryConvInfo (аналог DdeQueryInfo) - информация о соединении:");
                Console.WriteLine("  Размер структуры: " + info.cb);
                Console.WriteLine("  Статус соединения: 0x" + info.wStatus.ToString("X4"));
                Console.WriteLine("  Тип транзакции: 0x" + info.wType.ToString("X4"));
                Console.WriteLine("  Состояние: 0x" + info.wConvst.ToString("X4"));
                Console.WriteLine("  Последняя ошибка: " + info.wLastError);
            }
            else
            {
                Console.WriteLine("DdeQueryConvInfo не удалась, ошибка: " + DdeGetLastError(_idInst));
            }
            DdeDisconnect(hConv);
        }
        DdeFreeStringHandle(_idInst, hszService);
        DdeFreeStringHandle(_idInst, hszTopic);
    }

    // ======================== ЗАДАНИЕ 10 =======================
    static void Task10_ErrorHandling()
    {
        Console.WriteLine("Обработка ошибок с помощью DdeGetLastError:");

        // Попытка выполнить операцию с неверным соединением
        IntPtr pResult;
        IntPtr ret = DdeClientTransaction(IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero, DDE_FMT_TEXT, XTYP_REQUEST, TIMEOUT, out pResult);

        if (ret == IntPtr.Zero)
        {
            uint lastError = DdeGetLastError(_idInst);
            Console.WriteLine("  DdeClientTransaction вернула NULL");
            Console.WriteLine("  DdeGetLastError: " + lastError + " (0x" + lastError.ToString("X4") + ")");

            // Расшифровка кода ошибки
            string errorDesc = "";
            if (lastError == 0)
                errorDesc = "Нет ошибки";
            else if (lastError == 0x4001 || lastError == 0x4006)
                errorDesc = "DMLERR_NO_CONV_ESTABLISHED - Соединение не установлено";
            else if (lastError == 0x4000)
                errorDesc = "DMLERR_SERVER_DIED - Сервер недоступен";
            else if (lastError == 0x4003)
                errorDesc = "DMLERR_INVALIDPARAMETER - Неверный параметр";
            else
                errorDesc = "Неизвестная ошибка";

            Console.WriteLine("  Описание: " + errorDesc);
        }

        // Демонстрация обработки в callback
        Console.WriteLine("  Ошибки также обрабатываются в DdeCallbackHandler()");
    }

    // ======================== DDE Callback (Задание 2) ========================
    private static uint DdeCallbackHandler(uint uType, uint uFmt, IntPtr hConv,
        IntPtr hsz1, IntPtr hsz2, IntPtr hData, IntPtr dwData1, IntPtr dwData2)
    {
        try
        {
            switch (uType)
            {
                case XTYP_CONNECT:
                    // Проверка и принятие соединения
                    string serviceName = GetStringFromHSZ(hsz1);
                    string topicName = GetStringFromHSZ(hsz2);
                    Console.WriteLine("[Callback] Запрос на подключение: Service=" + serviceName + ", Topic=" + topicName);
                    return DDE_FACK;

                case XTYP_POKE:
                    // Получение данных от клиента
                    if (hData != IntPtr.Zero)
                    {
                        uint size;
                        IntPtr dataPtr = DdeAccessData(hData, out size);
                        byte[] received = new byte[size];
                        Marshal.Copy(dataPtr, received, 0, (int)size);
                        _lastReceivedData = Encoding.Default.GetString(received);
                        Console.WriteLine("[Callback] ПОЛУЧЕНЫ ДАННЫЕ (POKE): '" + _lastReceivedData + "'");
                    }
                    return DDE_FACK;

                case XTYP_REQUEST:
                    // Ответ на запрос данных
                    string response = "Ответ сервера: " + DateTime.Now.ToString("HH:mm:ss");
                    byte[] respBytes = Encoding.Default.GetBytes(response);
                    IntPtr hResp = DdeCreateDataHandle(_idInst, respBytes, (uint)respBytes.Length, 0, hsz2, DDE_FMT_TEXT, 0);
                    Console.WriteLine("[Callback] ОТВЕТ НА ЗАПРОС (REQUEST): '" + response + "'");
                    return (uint)hResp;

                case XTYP_EXECUTE:
                    // Выполнение команды
                    if (hData != IntPtr.Zero)
                    {
                        uint size;
                        IntPtr dataPtr = DdeAccessData(hData, out size);
                        byte[] cmdData = new byte[size];
                        Marshal.Copy(dataPtr, cmdData, 0, (int)size);
                        string command = Encoding.Default.GetString(cmdData);
                        Console.WriteLine("[Callback] ВЫПОЛНЕНИЕ КОМАНДЫ (EXECUTE): '" + command + "'");
                    }
                    return DDE_FACK;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Callback] ОШИБКА: " + ex.Message);
            return 0;
        }
        return 0;
    }

    // Вспомогательная функция для получения строки из HSZ
    private static string GetStringFromHSZ(IntPtr hsz)
    {
        if (hsz == IntPtr.Zero) return "";
        StringBuilder sb = new StringBuilder(255);
        DdeQueryString(_idInst, hsz, sb, 254, 1251);
        return sb.ToString();
    }

    // ======================== Серверная часть (Задание 4) ========================
    private static void RunDDEServer()
    {
        IntPtr hszService = DdeCreateStringHandle(_idInst, "DDEServer", 1251);

        // Регистрация сервера в системе
        bool registered = DdeNameService(_idInst, hszService, IntPtr.Zero, DNS_REGISTER);
        Console.WriteLine("[Сервер] DdeNameService: " + (registered ? "ЗАРЕГИСТРИРОВАН" : "ОШИБКА РЕГИСТРАЦИИ"));
        Console.WriteLine("[Сервер] Имя сервера: DDEServer");
        Console.WriteLine("[Сервер] Ожидание подключений...");

        while (_serverRunning)
        {
            Thread.Sleep(100);
        }

        DdeNameService(_idInst, hszService, IntPtr.Zero, DNS_UNREGISTER);
        DdeFreeStringHandle(_idInst, hszService);
        Console.WriteLine("[Сервер] Остановлен");
    }
}