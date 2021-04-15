using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace CsgoTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer CheckTimer = new DispatcherTimer();
        private DispatcherTimer TelnetTimer = new DispatcherTimer();
        public MainWindow()
        {
            LogsController.Logs = new LinkedList<Log>();
            LogsController.Chats = new List<Chat>();
            LogsController.Commands = new List<Command>();

            InitializeComponent();
            CheckTimer.Interval = TimeSpan.FromSeconds(3);
            CheckTimer.Tick += TimerTick;
            CheckTimer.Tick += UpdateTelnetConnectionStatus;
            CheckTimer.Start();

            LogsController.Chats = new List<Chat>();
            ChatView.ItemsSource = LogsController.Chats;
            OptionsController.CheckIfSet();

            TelnetHelper.Connect();
            UpdateTelnetConnectionStatus(null, null);
        }

        public void TimerTick(object sender, EventArgs e)
        {
            LogsController.LoadLogs(30);
            ChatView.Items.Refresh();
            ExecuteCommands();
        }

        private void ExecuteCommands()
        {
            foreach (var command in LogsController.Commands)
            {
                if (!command.Executed)
                {
                    command.Execute();
                    command.Executed = true;
                }
            }
        }

        private void UpdateTelnetConnectionStatus(object sender, EventArgs e)
        {
            if (TelnetHelper.Connected)
            {
                lbl_telnet_status.Content = "Connected";
            }
            else
            {
                lbl_telnet_status.Content = "Disconnected";
                TelnetHelper.Connect();
            }
        }

        private void BtnOptions_Click(object sender, RoutedEventArgs e)
        {
            CheckTimer.Stop();
            new OptionsWindow().ShowDialog();
            LogsController.Chats.Clear();
            CheckTimer.Start();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
