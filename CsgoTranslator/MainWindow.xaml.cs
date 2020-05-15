using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CsgoTranslator;

namespace CsgoTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Log> Logs { get; set; }
        private DispatcherTimer CheckTimer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            CheckTimer.Interval = TimeSpan.FromSeconds(3);
            CheckTimer.Tick += UpdateChat;
            CheckTimer.Start();
            this.Logs = new List<Log>();
            ChatView.ItemsSource = this.Logs;
            OptionsController.CheckIfSet();
        }

        private void UpdateChat(object sender, EventArgs e)
        {
            List<Log> logs = LogsController.GetLogs(100);
            if(logs != null)
            {
                foreach (Log l in logs)
                {
                    if (this.Logs.Where(x => x.OriginalMessage == l.OriginalMessage).Count() == 0)
                    {
                        l.Translate();
                        if(l.Message == null)
                        {
                            MessageBox.Show(this,"Too many translation requests where made\nTry again later\n\nClick OK to retry", "CSGO Translator - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            this.Logs.Insert(0, l);
                        }
                    }
                }
                ChatView.Items.Refresh();
            }
            else
            {
                MessageBox.Show(this, "Can't find console.log \nEnter the console command: con_logfile \"console.log\"\nor check csgo path in options window \n\nClick OK to continue", "CSGO Translator - Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }

        private void BtnOptions_Click(object sender, RoutedEventArgs e)
        {
            CheckTimer.Stop();
            new OptionsWindow().ShowDialog();
            this.Logs.Clear();
            CheckTimer.Start();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
