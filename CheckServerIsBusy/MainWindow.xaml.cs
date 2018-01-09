using CheckServerIsBusy.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CheckServerIsBusy
{
    public partial class MainWindow : Window
    {
        public Preferences Preferences { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledException;
            Preferences = Preferences.Load();
        }

        private void GlobalUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debugger.Break();
            using (FileStream stream = new FileStream($"{Environment.CurrentDirectory}\\{DateTime.Now.ToShortDateString()}.log", FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    sw.WriteLine(DateTime.Now);
                    sw.WriteLine(e.ExceptionObject.ToString());
                    sw.WriteLine("=================================================");
                }
            }
        }

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            ChangeWindowState();
        }

        private void ChangeWindowState()
        {
            if (WindowState == WindowState.Minimized)
            {
                Show();
                WindowState = WindowState.Normal;
                
            }
            else
            {
                WindowState = WindowState.Minimized;
                Hide();
            }
        }

        private void TBIMain_TrayRightMouseDown(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = (ContextMenu)FindResource("NotifierContextMenu");
            menu.IsOpen = true;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeWindowState();
            await SetTrayIcon();
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = new TimeSpan(0, 0, 10),
                IsEnabled = true
            };
            timer.Tick += async (object s, EventArgs a) =>
            {
                await SetTrayIcon();
            };
            timer.Start();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        private void MIExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private async Task SetTrayIcon()
        {
            try
            {
                if (await CheckServer.ServerIsBusyAsync())
                {
                    string userName = Preferences.Users.FirstOrDefault(x => x.IPAddress == CheckServer.ClientIPAddress)?.UserName;
                    string clientName = userName ?? CheckServer.ClientName;
                    TBIMain.Icon = Properties.Resources.Closed;
                    StringBuilder sb = new StringBuilder(100);
                    sb.AppendLine("Server is busy");
                    sb.Append(userName == null ? "End user computer name: " : "End user name: ");
                    sb.Append(clientName ?? "Not defined");
                    if (string.IsNullOrEmpty(userName))
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append($"End user IP:{ CheckServer.ClientIPAddress ?? "Not defined"}");
                    }
                    var idleTime = (int)CheckServer.IdleTime.TotalMinutes;
                    if (idleTime >= 5)
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append($"Idle time: {(int)idleTime} minutes");
                    }
                    TBIMain.ToolTipText = sb.ToString();
                    TbServerIsBusyStatus.Text = sb.Replace(Environment.NewLine, " ").ToString();
                }
                else
                {
                    const string serverIsOpen = "Server free";
                    TBIMain.Icon = Properties.Resources.Opened;
                    TBIMain.ToolTipText = serverIsOpen;
                    TbServerIsBusyStatus.Text = serverIsOpen;
                }
            }
            catch (Win32Exception)
            {
                const string fail = "Connection attempt is failed";
                TBIMain.Icon = Properties.Resources.ConnectionFailed;
                TBIMain.ToolTipText = fail;
                TbServerIsBusyStatus.Text = fail;
            }
        }

        private void MIPreferences_Click(object sender, RoutedEventArgs e)
        {
            PreferencesWindow pw = new PreferencesWindow();
            if (pw.ShowDialog() == true)
            {
                Preferences = Preferences.Load();
            }
        }

        private async void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            string messageText = TbMessage.Text;
            LblResult.Content = "";
            var result = await Task.Factory.StartNew(() 
                => CheckServer.SendMessage($"{messageText}{Environment.NewLine}{Environment.UserName}"));
            if (result)
            {
                LblResult.Content = "You can join to the server";
            }
            else
            {
                LblResult.Content = "You can't join to the server";
            }
        }
    }
}