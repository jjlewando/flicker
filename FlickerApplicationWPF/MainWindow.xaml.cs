using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace FlickerApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static FTDevice myFlicer = new FTDevice();
        


        public MainWindow()
        {
            InitializeComponent();
            enableControls(true, false, false, false);        
        }

        private void enableControls(bool keyConnect, bool keyReed, bool keyErase, bool keySynchro)
        {
            //btConnect.IsEnabled = keyConnect;
            if (keyConnect == true) btConnect.Content = "Connect Flicker";
            else btConnect.Content = "Disconnect Flicker";

            btRead.IsEnabled = keyReed;
            btErase.IsEnabled = keyErase;
            btSynchro.IsEnabled = keySynchro;
        }

        private void buttonConnect(object sender, RoutedEventArgs e)
        {
            if (myFlicer.isOpened())
            {
                //Device is open
                myFlicer.Close();
                enableControls(true, false, false, false);
                textStatus.Text += Environment.NewLine + "Disconnected";
            }
            else
            {
                UInt32 numberOfDevices = myFlicer.GetNumberOfDevices();
                bool state;
                if (numberOfDevices != 0)
                {

                    state = myFlicer.OpenDefaultDevice(myFlicer.GetDevices(numberOfDevices), 0);
                    if (state)
                    {
                        textStatus.Text += Environment.NewLine + "Connected";
                        enableControls(false, true, true, true);
                    }
                    else
                    {
                        textStatus.Text += Environment.NewLine + "Connection Error";
                    }

                }
                else
                {
                    textStatus.Text += Environment.NewLine + "No devices";
                }
                
            }
            textStatus.ScrollToEnd();
        }

        private void btRead_Click(object sender, RoutedEventArgs e)
        {
            if (myFlicer.isOpened())
            {
                myFlicer.writeData("Hello");
                textStatus.Text += Environment.NewLine + "R-Command Send";
                textStatus.ScrollToEnd();
            }
        }

        private void btErase_Click(object sender, RoutedEventArgs e)
        {
            if (myFlicer.isOpened())
            {
                myFlicer.writeData("Bye");
                textStatus.Text += Environment.NewLine + "E-Command Send";
                textStatus.ScrollToEnd();
            }
        }

        private void btSynchro_Click(object sender, RoutedEventArgs e)
        {
            if (myFlicer.isOpened())
            {
                byte dtCode = 0xA0;
                DateTime time = DateTime.Now;
                Console.WriteLine(time.ToString());
                myFlicer.writeData(time.ToString());
                textStatus.Text += Environment.NewLine + time.ToString();
                textStatus.ScrollToEnd();
            }
        }
    }
}
