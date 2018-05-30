using Microsoft.Win32;
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

namespace meniuprincipal
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        int i = 1;

        public Window2() => InitializeComponent();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (i == 0)
            {
                i = 1;
                butt4.Content = "🔈";
                mediaPlayer.Pause();
            }
            else
            {
                butt4.Content = "🔉";
                i = 0;
                mediaPlayer.Play();
            }

        }
        private void butt5_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                mediaPlayer.Open(new Uri(openFileDialog.FileName));
                mediaPlayer.Play();
            }
        }

        private void butt1_Click(object sender, RoutedEventArgs e)
        {
            TankArena.MainWindow wind = new TankArena.MainWindow();

            wind.Show();
        }
    }
}
