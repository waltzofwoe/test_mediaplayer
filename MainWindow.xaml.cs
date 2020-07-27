using System;
using System.Collections.Generic;
using System.IO;
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

namespace Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var engine = new MediaEngine(_mediaElement);
            mediaController = new MediaController(engine);
        }

        MediaController mediaController;

        private void LoadSchedule_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                var file = File.ReadAllLines("config.txt");
                mediaController.LoadShedule(file);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message} {e.InnerException?.Message}");
            }
        }
    }
}
