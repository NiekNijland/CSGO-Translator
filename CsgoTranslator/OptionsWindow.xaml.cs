using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CsgoTranslator.Enums;
using CsgoTranslator.Helpers;
using CsgoTranslator.MinimalisticTelnet;

namespace CsgoTranslator
{
    /**
     * <summary>
     *Interaction logic for OptionsWindow.xaml
     * </summary>
     */
    public partial class OptionsWindow 
    {
        private ChatType TransToRadioButtons
        {
            get => RbTransToAll.IsChecked != null && (bool) (RbTransToAll.IsChecked) ? ChatType.All : ChatType.Team;
            set
            {
                if (value == ChatType.Team)
                {
                    RbTransToTeam.IsChecked = true;    
                }
                else
                {
                    RbTransToAll.IsChecked = true;
                }
            }
        }
        private TelnetGrant CommandsFromRadioButtons
        {
            get
            {
                if (RbCommandsBoth.IsChecked != null && (bool) RbCommandsBoth.IsChecked)
                    return TelnetGrant.BothChats;
                
                if(RbCommandsTeam.IsChecked != null && (bool) RbCommandsTeam.IsChecked)
                    return TelnetGrant.TeamChat;
                
                return TelnetGrant.Self;
            }
            set
            {
                switch (value)
                {
                    case TelnetGrant.BothChats:
                        RbCommandsBoth.IsChecked = true;
                        break;
                    case TelnetGrant.TeamChat:
                        RbCommandsTeam.IsChecked = true;
                        break;
                    default:
                        RbCommandsSelf.IsChecked = true;
                        break;
                }
            }
        }
        private TelnetGrant TransFromCheckBoxes
        {
            get
            {
                var returnValue = TelnetGrant.Undefined;

                if (CbTransFromAll.IsChecked != null && (bool) CbTransFromAll.IsChecked)
                    returnValue = TelnetGrant.AllChat;

                if (CbTransFromTeam.IsChecked != null && (bool) CbTransFromTeam.IsChecked)
                    return returnValue == TelnetGrant.Undefined ? TelnetGrant.TeamChat : TelnetGrant.BothChats;

                return returnValue;
            }
            set
            {
                switch (value)
                {
                    case TelnetGrant.BothChats:
                        CbTransFromAll.IsChecked = true;
                        CbTransFromTeam.IsChecked = true;
                        break;
                    case TelnetGrant.TeamChat:
                        CbTransFromTeam.IsChecked = true;
                        CbTransFromAll.IsChecked = false;
                        break;
                    case TelnetGrant.AllChat:
                        CbTransFromTeam.IsChecked = false;
                        CbTransFromAll.IsChecked = true;
                        break;
                    default:
                        CbTransFromAll.IsChecked = false;
                        CbTransFromTeam.IsChecked = false;
                        break;
                }
            }
        }
        private bool IgnoreOwnMessages
        {
            get => CbIgnoreOwnMessages.IsChecked != null && (bool) CbIgnoreOwnMessages.IsChecked;
            set => CbIgnoreOwnMessages.IsChecked = value;
        }
        private string OwnUsername
        {
            get => TbOwnUsername.Text.Trim();
            set
            {
                TbOwnUsername.Text = value;

                if (!string.IsNullOrEmpty(value))
                {
                    CbIgnoreOwnMessages.IsEnabled = true;
                    RbCommandsSelf.IsEnabled = true;
                }
                else
                {
                    CbIgnoreOwnMessages.IsEnabled = false;
                    CbIgnoreOwnMessages.IsChecked = false;
                    RbCommandsSelf.IsEnabled = false;
                    
                    if (RbCommandsSelf.IsChecked != null && (bool) RbCommandsSelf.IsChecked)
                        RbCommandsTeam.IsChecked = true;
                }
            }
        }
        
        public OptionsWindow()
        {
            InitializeComponent();
            LoadOptions();
        }

        private void LoadOptions()
        {
            TbFolderPath.Text = OptionsManager.InstallationPath;
            TbLang.Text = OptionsManager.Language;
            OwnUsername = OptionsManager.OwnUsername;
            TbTelnetPort.Text = OptionsManager.TelnetPort.ToString();
            TransToRadioButtons = OptionsManager.SendTranslationsTo;
            CommandsFromRadioButtons = OptionsManager.AllowCommandsFrom;
            TransFromCheckBoxes = OptionsManager.SendTranslationsFrom;
            IgnoreOwnMessages = OptionsManager.IgnoreOwnMessages;
        }

        private void BtnSaveOptions_Click(object sender, RoutedEventArgs e)
        {
            OptionsManager.InstallationPath = TbFolderPath.Text;
            OptionsManager.Language = TbLang.Text;
            OptionsManager.OwnUsername = OwnUsername;
            OptionsManager.TelnetPort = int.Parse(TbTelnetPort.Text);
            OptionsManager.SendTranslationsTo = TransToRadioButtons;
            OptionsManager.AllowCommandsFrom = CommandsFromRadioButtons;
            OptionsManager.SendTranslationsFrom = TransFromCheckBoxes;
            OptionsManager.IgnoreOwnMessages = IgnoreOwnMessages; 
            
            OptionsManager.Save();
            Close();
        }

        private void BtnSetDefault_Click(object sender, RoutedEventArgs e)
        {
            OptionsManager.SetDefault();
            LoadOptions();
        }
        
        private void TbTelnetPort_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("^[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void TbOwnUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox) sender).Text.Length > 0)
            {
                CbIgnoreOwnMessages.IsEnabled = true;
                RbCommandsSelf.IsEnabled = true;
            }
            else
            {
                CbIgnoreOwnMessages.IsEnabled = false;
                CbIgnoreOwnMessages.IsChecked = false;
                RbCommandsSelf.IsEnabled = false;
                
                if (RbCommandsSelf.IsChecked != null && (bool) RbCommandsSelf.IsChecked)
                    RbCommandsTeam.IsChecked = true;
            }
        }
    }
}
