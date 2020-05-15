using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace CsgoTranslator
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
            this.tbFolderPath.Text = OptionsController.GetPath();
            this.tbLang.Text = OptionsController.GetLang();
        }

        private void BtnSaveOptions_Click(object sender, RoutedEventArgs e)
        {
            OptionsController.SavePath(tbFolderPath.Text);
            OptionsController.SaveLang(tbLang.Text);
            this.Close();
        }

        private void BtnSetDefault_Click(object sender, RoutedEventArgs e)
        {
            OptionsController.SaveDefault();
            this.Close();
        }
    }
}
