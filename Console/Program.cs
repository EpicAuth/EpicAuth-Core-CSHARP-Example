using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EpicAuth
{
    class Program
    {
        public static api EpicAuthApp = new api(
            name: "", // Application Name
            ownerid: "", // Account ID
            version: "" // Application version.
                        //path: @"Your_Path_Here"
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern ushort GlobalFindAtom(string lpString);

        static void Main(string[] args)
        {
            PerformSecurityChecks();

            Console.Title = "Loader";
            Console.WriteLine("\n\n Connecting..");
            EpicAuthApp.Initialize();

            HandleAutoUpdate();

            if (!EpicAuthApp.response.success)
            {
                Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
                Thread.Sleep(1500);
                TerminateProcess(GetCurrentProcess(), 1);
                return;
            }

            DisplayMainMenu();
            ProcessUserSelection();

            HandleAutoUpdate();

            if (!EpicAuthApp.response.success)
            {
                Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
                Thread.Sleep(2500);
                TerminateProcess(GetCurrentProcess(), 1);
                return;
            }

            Console.WriteLine("\n Logged In!");

            if (string.IsNullOrEmpty(EpicAuthApp.response.message))
            {
                TerminateProcess(GetCurrentProcess(), 1);
                return;
            }

            DisplayUserInformation();
            HandleTwoFactorAuthentication();

            Console.WriteLine("\n Closing in five seconds...");
            Thread.Sleep(-1);
            Environment.Exit(0);
        }

        private static void DisplayMainMenu()
        {
            Console.Write("\n [1] Login\n [2] Register\n [3] Upgrade\n [4] License key only\n [5] Forgot password\n\n Choose option: ");
        }

        private static void ProcessUserSelection()
        {
            string username, password, licenseKey, emailAddress, twoFactorCode;

            if (!int.TryParse(Console.ReadLine(), out int selectedOption))
            {
                Console.WriteLine("\n\n Invalid Selection");
                Thread.Sleep(2500);
                TerminateProcess(GetCurrentProcess(), 1);
                return;
            }

            switch (selectedOption)
            {
                case 1:
                    HandleLogin(out username, out password, out twoFactorCode);
                    EpicAuthApp.login(username, password, twoFactorCode);
                    break;
                case 2:
                    HandleRegister(out username, out password, out licenseKey, out emailAddress);
                    EpicAuthApp.register(username, password, licenseKey, emailAddress);
                    break;
                case 3:
                    HandleUpgrade(out username, out licenseKey);
                    EpicAuthApp.upgrade(username, licenseKey);
                    Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                    return;
                case 4:
                    HandleLicenseOnly(out licenseKey, out twoFactorCode);
                    EpicAuthApp.license(licenseKey, twoFactorCode);
                    break;
                case 5:
                    HandleForgotPassword(out username, out emailAddress);
                    EpicAuthApp.forgot(username, emailAddress);
                    Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                    return;
                default:
                    Console.WriteLine("\n\n Invalid Selection");
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                    return;
            }
        }

        private static void HandleLogin(out string username, out string password, out string twoFactorCode)
        {
            Console.Write("\n\n Enter username: ");
            username = Console.ReadLine();
            Console.Write("\n\n Enter password: ");
            password = Console.ReadLine();
            Console.Write("\n\n Enter 2fa code: (not using 2fa? Just press enter) ");
            twoFactorCode = Console.ReadLine();
        }

        private static void HandleRegister(out string username, out string password, out string licenseKey, out string emailAddress)
        {
            Console.Write("\n\n Enter username: ");
            username = Console.ReadLine();
            Console.Write("\n\n Enter password: ");
            password = Console.ReadLine();
            Console.Write("\n\n Enter license: ");
            licenseKey = Console.ReadLine();
            Console.Write("\n\n Enter email (just press enter if none): ");
            emailAddress = Console.ReadLine();
        }

        private static void HandleUpgrade(out string username, out string licenseKey)
        {
            Console.Write("\n\n Enter username: ");
            username = Console.ReadLine();
            Console.Write("\n\n Enter license: ");
            licenseKey = Console.ReadLine();
        }

        private static void HandleLicenseOnly(out string licenseKey, out string twoFactorCode)
        {
            Console.Write("\n\n Enter license: ");
            licenseKey = Console.ReadLine();
            Console.Write("\n\n Enter 2fa code: (not using 2fa? Just press enter");
            twoFactorCode = Console.ReadLine();
        }

        private static void HandleForgotPassword(out string username, out string emailAddress)
        {
            Console.Write("\n\n Enter username: ");
            username = Console.ReadLine();
            Console.Write("\n\n Enter email: ");
            emailAddress = Console.ReadLine();
        }

        private static void DisplayUserInformation()
        {
            Console.WriteLine("\n User data:");
            Console.WriteLine(" Username: " + EpicAuthApp.user_data.username);

            if (EpicAuthApp.user_data.subscriptions != null && EpicAuthApp.user_data.subscriptions.Count > 0)
            {
                Console.WriteLine(" License: " + EpicAuthApp.user_data.subscriptions[0].key);
            }

            Console.WriteLine(" IP address: " + EpicAuthApp.user_data.ip);
            Console.WriteLine(" Hardware-Id: " + EpicAuthApp.user_data.hwid);
            Console.WriteLine(" Created at: " + ConvertUnixTimeToDateTime(long.Parse(EpicAuthApp.user_data.createdate)));

            if (!string.IsNullOrEmpty(EpicAuthApp.user_data.lastlogin))
            {
                Console.WriteLine(" Last login at: " + ConvertUnixTimeToDateTime(long.Parse(EpicAuthApp.user_data.lastlogin)));
            }

            Console.WriteLine(" Your subscription(s):");
            if (EpicAuthApp.user_data.subscriptions != null)
            {
                for (int index = 0; index < EpicAuthApp.user_data.subscriptions.Count; index++)
                {
                    var subscription = EpicAuthApp.user_data.subscriptions[index];
                    Console.WriteLine(" Subscription name: " + subscription.subscription +
                                    " - Expires at: " + ConvertUnixTimeToDateTime(long.Parse(subscription.expiry)) +
                                    " - Time left in seconds: " + subscription.timeleft);
                }
            }
        }

        private static void HandleTwoFactorAuthentication()
        {
            Console.Write("\n [1] Enable 2FA\n [2] Disable 2FA\n Choose option: ");

            if (!int.TryParse(Console.ReadLine(), out int twoFactorOption))
            {
                Console.WriteLine("\n\n Invalid Selection");
                Thread.Sleep(2500);
                TerminateProcess(GetCurrentProcess(), 1);
                return;
            }

            switch (twoFactorOption)
            {
                case 1:
                    EpicAuthApp.enable2fa();
                    break;
                case 2:
                    Console.Write("Enter your 6 digit authorization code: ");
                    string authorizationCode = Console.ReadLine();
                    EpicAuthApp.disable2fa(authorizationCode);
                    break;
                default:
                    Console.WriteLine("\n\n Invalid Selection");
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                    return;
            }
        }

        public static bool SubscriptionExists(string subscriptionName)
        {
            if (EpicAuthApp.user_data.subscriptions == null)
            {
                return false;
            }
            return EpicAuthApp.user_data.subscriptions.Exists(x => x.subscription == subscriptionName);
        }

        public static DateTime ConvertUnixTimeToDateTime(long unixTimestamp)
        {
            DateTime epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            try
            {
                epochDateTime = epochDateTime.AddSeconds(unixTimestamp).ToLocalTime();
            }
            catch
            {
                epochDateTime = DateTime.MaxValue;
            }
            return epochDateTime;
        }

        static void StartAtomMonitoring()
        {
            Thread monitoringThread = new Thread(() =>
            {
                const int checkIntervalMs = 60000;
                while (true)
                {
                    Thread.Sleep(checkIntervalMs);

                    ushort atomValue = GlobalFindAtom(EpicAuthApp.ownerid);
                    if (atomValue == 0)
                    {
                        TerminateProcess(GetCurrentProcess(), 1);
                    }
                }
            });

            monitoringThread.IsBackground = true;
            monitoringThread.Start();
        }

        static void PerformSecurityChecks()
        {
            StackTrace currentStackTrace = new StackTrace();
            StackFrame[] stackFrames = currentStackTrace.GetFrames();

            if (stackFrames != null)
            {
                foreach (StackFrame frame in stackFrames)
                {
                    MethodBase methodInfo = frame.GetMethod();
                    if (methodInfo != null && methodInfo.DeclaringType?.Assembly != Assembly.GetExecutingAssembly())
                    {
                        TerminateProcess(GetCurrentProcess(), 1);
                        return;
                    }
                }
            }

            Assembly harmonyAssembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(assembly => assembly.GetName().Name == "0Harmony");

            if (harmonyAssembly != null)
            {
                TerminateProcess(GetCurrentProcess(), 1);
                return;
            }

            StartAtomMonitoring();
        }

        static void HandleAutoUpdate()
        {
            const string invalidVersionMessage = "invalidver";

            if (EpicAuthApp.response.message == invalidVersionMessage)
            {
                if (!string.IsNullOrEmpty(EpicAuthApp.app_data.downloadLink))
                {
                    Console.WriteLine("\n Auto update avaliable!");
                    Console.WriteLine(" Choose how you'd like to auto update:");
                    Console.WriteLine(" [1] Open file in browser");
                    Console.WriteLine(" [2] Download file directly");

                    if (!int.TryParse(Console.ReadLine(), out int updateChoice))
                    {
                        Console.WriteLine(" Invalid selection, terminating program..");
                        Thread.Sleep(1500);
                        Environment.Exit(0);
                        return;
                    }

                    switch (updateChoice)
                    {
                        case 1:
                            Process.Start(EpicAuthApp.app_data.downloadLink);
                            Environment.Exit(0);
                            return;
                        case 2:
                            Console.WriteLine(" Downloading file directly..");
                            Console.WriteLine(" New file will be opened shortly..");

                            using (WebClient downloadClient = new WebClient())
                            {
                                string executablePath = Application.ExecutablePath;
                                string randomSuffix = GenerateRandomString();
                                string newFilePath = executablePath.Replace(".exe", $"-{randomSuffix}.exe");

                                downloadClient.DownloadFile(EpicAuthApp.app_data.downloadLink, newFilePath);

                                Process.Start(newFilePath);
                                Process.Start(new ProcessStartInfo()
                                {
                                    Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Application.ExecutablePath + "\"",
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    CreateNoWindow = true,
                                    FileName = "cmd.exe"
                                });
                            }
                            Environment.Exit(0);
                            return;
                        default:
                            Console.WriteLine(" Invalid selection, terminating program..");
                            Thread.Sleep(1500);
                            Environment.Exit(0);
                            return;
                    }
                }

                Console.WriteLine("\n Status: Version of this program does not match the one online. Furthermore, the download link online isn't set. You will need to manually obtain the download link from the developer.");
                Thread.Sleep(2500);
                Environment.Exit(0);
            }
        }

        static string GenerateRandomString()
        {
            StringBuilder randomStringBuilder = new StringBuilder(5);
            Random randomGenerator = new Random();
            const int asciiUpperCaseStart = 65;
            const int alphabetSize = 26;

            for (int characterIndex = 0; characterIndex < 5; characterIndex++)
            {
                int randomValue = (int)Math.Floor(alphabetSize * randomGenerator.NextDouble() + asciiUpperCaseStart);
                randomStringBuilder.Append((char)randomValue);
            }

            return randomStringBuilder.ToString();
        }
    }
}
