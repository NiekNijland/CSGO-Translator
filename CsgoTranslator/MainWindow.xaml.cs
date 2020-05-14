using System;
using System.Collections.Generic;
using System.Data;
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
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += UpdateChat;
            timer.Start();
            this.Logs = new List<Log>();
            ChatView.ItemsSource = this.Logs;
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
                MessageBox.Show(this, "Can't find console.log \nEnter the console command: con_logfile \"console.log\"\nor check csgo install location \n\nClick OK to continue", "CSGO Translator - Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.No);
            }

        }
    }
}
