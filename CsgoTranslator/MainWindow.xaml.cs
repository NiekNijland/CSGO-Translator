using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using CsgoTranslator.Controllers;
using CsgoTranslator.Enums;
using CsgoTranslator.Helpers;
using CsgoTranslator.Models;

namespace CsgoTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _checkTimer = new DispatcherTimer();
        public MainWindow()
        {
            LogsController.Logs = new LinkedList<Log>();
            LogsController.Chats = new List<Chat>();
            LogsController.Commands = new List<Command>();

            InitializeComponent();
            _checkTimer.Interval = TimeSpan.FromSeconds(3);
            _checkTimer.Tick += TimerTick;
            _checkTimer.Tick += UpdateTelnetConnectionStatus;
            _checkTimer.Start();

            LogsController.Chats = new List<Chat>();
            ChatView.ItemsSource = LogsController.Chats;
            OptionsManager.ValidateSettings();

            TelnetHelper.Connect();
            UpdateTelnetConnectionStatus(null, null);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            LogsController.LoadLogs(30);
            ChatView.Items.Refresh();
            ExecuteCommands();
        }

        private static void ExecuteCommands()
        {
            foreach (var command in LogsController.Commands.Where(command => !command.Executed))
            {
                command.Execute();
                command.Executed = true;
            }
        }

        private void UpdateTelnetConnectionStatus(object sender, EventArgs e)
        {
            if (TelnetHelper.Connected)
            {
                lbl_telnet_status.Content = "Connected";
            }
            else if(OptionsManager.SendTranslationsFrom != TelnetGrant.Undefined)
            {
                lbl_telnet_status.Content = "Disconnected";
                TelnetHelper.Connect();
            }
            else
            {
                lbl_telnet_status.Content = "Disabled";
            }
        }

        private void BtnOptions_Click(object sender, RoutedEventArgs e)
        {
            _checkTimer.Stop();
            new OptionsWindow().ShowDialog();
            LogsController.Chats.Clear();
            _checkTimer.Start();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
