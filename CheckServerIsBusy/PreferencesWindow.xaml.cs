using CheckServerIsBusy.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

namespace CheckServerIsBusy
{
    public partial class PreferencesWindow : Window
    {
        private Preferences Preferences { get; set; }

        public PreferencesWindow()
        {
            InitializeComponent();
            Preferences = Preferences.Load();
            FillListBox();
        }

        private void BtnImportPreferences_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Preferences = Preferences.Load(openFileDialog.FileName);
                FillListBox();
            }
        }

        private void BtnExportPreferences_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Preferences.Save(saveFileDialog.FileName);
            }
        }

        private void BtnSavePreferences_Click(object sender, RoutedEventArgs e)
        {
            Preferences.Save();
            Close();
        }

        private void BtnAddNewRow_Click(object sender, RoutedEventArgs e)
        {
            UserAssociations ua = new UserAssociations
            {
                IPAddress = TbIPAddress.Text.Trim(),
                UserName = TbUserName.Text.Trim()
            };
            (Preferences.Users as List<UserAssociations>).Add(ua);
            LbAssociations.Items.Add(ua);
        }

        private void FillListBox()
        {
            (Preferences.Users as List<UserAssociations>).ForEach(x => LbAssociations.Items.Add(x));
        }
    }
}
