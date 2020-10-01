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
using Brushes = System.Windows.Media.Brushes;

namespace actelementry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AssettoCorsa ac = new AssettoCorsa();
        public int curRPM;
        public int curGear;
        public int MaxRPM;
        public int PitLimit;
        public float DRS;
        
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
            curGear = e.Physics.Gear - 1;
            PitLimit = e.Physics.PitLimiterOn;
            DRS = e.Physics.Drs;
            
            
            try
            {
                MainLabelUpdater();

                Dispatcher.Invoke(() => {
                    this.RPMmeter.Value = curRPM;
                    GearLabel.Content = curGear.ToString();
                    LabelStatus.Content = "Connected!";
                    LabelStatus.Foreground = Color.Aquamarine.ToBrush();
                });
            }
            catch
            {
                Dispatcher.Invoke(() => {
                    LabelStatus.Content = "Error!";
                    LabelStatus.Foreground = Color.Crimson.ToBrush();
                });
            }
            
        }

        public void MainLabelUpdater()
        {
            if (PitLimit == 1)
            {
                Dispatcher.Invoke(() => {
                    MainLabel.Content = "PIT";
                    MainLabel.Foreground = Color.Crimson.ToBrush();
                });
            }

            if (DRS == 1)
            {
                Dispatcher.Invoke(() => {
                    MainLabel.Content = "DRS";
                    MainLabel.Foreground = Color.LawnGreen.ToBrush();
                });
            }

            if (curRPM / MaxRPM > 0.8)
            {
                Dispatcher.Invoke(() => {
                    MainLabel.Content = "SHIFT";
                    MainLabel.Foreground = Color.Crimson.ToBrush();
                });
            }

            else
            {
                Dispatcher.Invoke(() => {
                    MainLabel.Content = curRPM.ToString();
                    MainLabel.Foreground = Color.White.ToBrush();
                });
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
