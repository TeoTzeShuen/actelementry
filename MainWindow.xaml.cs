using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using AssettoCorsaSharedMemory;

namespace actelementry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AssettoCorsa ac = new AssettoCorsa();
        public int curRPM;
        public int MaxRPM;



        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Activated(object sender, EventArgs e)
        {

            ac.Start();

            if (ac.IsRunning)
            {
                LabelStatus.Content = "Connected!";
                LabelStatus.Foreground = Color.Aquamarine.ToBrush();

                everything();
            }
            else
            {
                LabelStatus.Content = "Is AC Running?";
                LabelStatus.Foreground = Color.Crimson.ToBrush();
            }

        }

        public void everything()
        {
            ac.StaticInfoInterval = 5000; // Get StaticInfo updates ever 5 seconds
            ac.StaticInfoUpdated += ac_StaticInfoUpdated; // Add event listener for StaticInfo
            ac.PhysicsInterval = 10;
            ac.PhysicsUpdated += ac_PhysicsUpdated;

            
        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            if (ac.IsRunning == false)
            {
                ac.Start();
                everything();
            }
            else
            {
                LabelStatus.Content = LabelStatus.Content = "Already Running!";
                LabelStatus.Foreground = Color.Aquamarine.ToBrush();
            }
        }

        public void ac_StaticInfoUpdated(object sender, StaticInfoEventArgs e)
        {
            MaxRPM = e.StaticInfo.MaxRpm;
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => this.RPMmeter.Maximum = MaxRPM));
        }

        public void ac_PhysicsUpdated(object sender, PhysicsEventArgs e)
        {
            curRPM = e.Physics.Rpms;
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() => this.RPMmeter.Value = curRPM));
            }
            catch
            {
                LabelStatus.Content = "Error! Has AC Closed?";
                LabelStatus.Foreground = Color.Crimson.ToBrush();
            }
            
        }

    }

    //stupid wpf
    public static class colorconv
    {
        public static System.Windows.Media.Brush ToBrush(this System.Drawing.Color color)
        {
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
}
