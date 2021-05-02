using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using CsgoTranslator.Controllers;
using CsgoTranslator.Enums;
using CsgoTranslator.EventArgs;
using CsgoTranslator.Exceptions;
using CsgoTranslator.Helpers;
using CsgoTranslator.Models;

namespace CsgoTranslator
{
    /**
     * <summary>
     * Interaction logic for MainWindow.xaml
     * </summary>
     */
    public partial class MainWindow
    {
        private readonly DispatcherTimer _checkTimer = new DispatcherTimer();
        
        public static EventHandler<TranslatorExceptionEventArgs> ErrorEncountered;
        public static EventHandler<TranslatorExceptionEventArgs> Succeeded;

        public MainWindow()
        {
            InitializeComponent();

            ErrorEncountered += OnErrorEncountered;
            Succeeded += OnSucceeded;

            LogsController.Logs = new LinkedList<Log>();
            LogsController.Chats = new List<Chat>();
            LogsController.Commands = new List<Command>();

            Loaded += LoadWindow;
        }

        private void OnErrorEncountered(object sender, TranslatorExceptionEventArgs args)
        {
           Dispatcher.Invoke(() => { LblError.Content = args.Exception.Message; }); 
        }
        
        private void OnSucceeded(object sender, TranslatorExceptionEventArgs e)
        { 
            Dispatcher.Invoke(() => {
                if ((string) LblError.Content == e.Exception.Message)
                    LblError.Content = "";
            });
        }

        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            ChatView.ItemsSource = LogsController.Chats;

            _checkTimer.Interval = TimeSpan.FromSeconds(3);
            _checkTimer.Tick += TimerTick;
            _checkTimer.Start();

            OptionsManager.ValidateSettings();
        }

        private async void TimerTick(object sender, System.EventArgs e)
        {
            await LogsController.LoadLogsAsync(30);
            ChatView.Items.Refresh();

            /* Do not await because these functions have no return */
            UpdateTelnetAsync();
            ExecuteCommandsAsync();
        }

        private static async Task ExecuteCommandsAsync()
        {
            /* await each task because otherwise the order of responses might be incorrect */
            foreach (var command in LogsController.Commands.Where(command => !command.Executed))
            {
                await Task.Run(() => command.Execute());
                command.Executed = true;
            }
        }

        private async Task UpdateTelnetAsync()
        {
            if (TelnetHelper.Connected)
            {
                LblTelnetStatus.Content = "Connected";
            }
            else if(OptionsManager.SendTranslationsFrom != TelnetGrant.Undefined)
            {
                LblTelnetStatus.Content = "Disconnected";
                await Task.Run(() => TelnetHelper.Connect());
            }
            else
            {
                LblTelnetStatus.Content = "Disabled";
            }
        }

        private void BtnOptions_Click(object sender, RoutedEventArgs e)
        {
            new OptionsWindow().ShowDialog();
            LogsController.Chats.Clear();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
