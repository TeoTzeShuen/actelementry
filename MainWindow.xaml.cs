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
            ac.StaticInfoInterval = 10000; // Get StaticInfo updates ever 10 seconds
            ac.StaticInfoUpdated += ac_StaticInfoUpdated; // Add event listener for StaticInfo
            ac.PhysicsInterval = 50; //mess around with this value - higher = lower cpu usage
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

            Dispatcher.Invoke(() => {
                this.RPMmeter.Maximum = MaxRPM;
            });

        }

        public void ac_PhysicsUpdated(object sender, PhysicsEventArgs e)
        {
            curRPM = e.Physics.Rpms;
            curGear = e.Physics.Gear - 1;
            PitLimit = e.Physics.PitLimiterOn;
            DRS = e.Physics.Drs;

            Dispatcher.Invoke(() => 
            {
                try
                {
                    MainLabelUpdater();

                    this.RPMmeter.Value = curRPM;
                    GearLabel.Content = curGear.ToString();
                    LabelStatus.Content = "Connected!";
                    LabelStatus.Foreground = Color.Aquamarine.ToBrush();

                    RPMlabel.Content = curRPM.ToString();
                    RPMlabel.Foreground = Color.Gray.ToBrush();

                }
                catch
                {
                    LabelStatus.Content = "Error!";
                    LabelStatus.Foreground = Color.Crimson.ToBrush();
                }
            });
        }

        public void MainLabelUpdater()
        {
            Dispatcher.Invoke(() =>
            {
                if (PitLimit > 0)
                {
                    MainLabel.Content = "PIT";
                    MainLabel.Foreground = Color.Crimson.ToBrush();
                }

                if (DRS > 0)
                {
                    MainLabel.Content = "DRS";
                    MainLabel.Foreground = Color.LawnGreen.ToBrush();
                }

                if ((curRPM / MaxRPM) > 0.5)
                {
                    MainLabel.Content = "SHIFT";
                    MainLabel.Foreground = Color.Crimson.ToBrush();
                }

                else
                {
                    //keep last state
                }

            });
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
