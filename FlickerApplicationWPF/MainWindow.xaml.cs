﻿using System;
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
using System.Windows.Threading;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;

namespace FlickerApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static FTDevice myFlicer = new FTDevice();
        private static FormSplash formSplash = new FormSplash();
        public DispatcherTimer dispTimer = new DispatcherTimer();
        public DispatcherTimer dataTimer = new DispatcherTimer();
        private static WindowData windowData = new WindowData();

        public MainWindow()
        {
            InitializeComponent();
            enableControls(true, false, false, false);
            formSplash.Show();
            formSplash.Topmost = true; 
            dispTimer.Tick += new EventHandler(dispTimer_Tick);
            dispTimer.Interval = new TimeSpan(0, 0, 2);
            dispTimer.Start();
            
            
        }

        void dispTimer_Tick(object sender, EventArgs e)
        {
            formSplash.Close();
            dispTimer.Stop();
            //Console.WriteLine("Timer event");            
            CommandManager.InvalidateRequerySuggested();
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
                        myFlicer.FtReceiverInit();
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
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                folderDialog.Description = "Select folder for flicker measurements";
                DialogResult result = folderDialog.ShowDialog();
                //folderDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    enableControls(false, false, false, false);
                    Console.WriteLine("FolderOpened");
                    DateTime dataCzas = DateTime.Now;
                    string flickerFilePath = folderDialog.SelectedPath + "\\" + "Flicker_[" + dataCzas.Year.ToString() + "_" + dataCzas.Month.ToString() + "_" + dataCzas.Day.ToString() + "].txt";

                    System.IO.StreamWriter flickerFile = new System.IO.StreamWriter(flickerFilePath);
                    
                    // Send Command to device
                    

                    //lock application and wait for all data (?) <---------------
                                        
                    //flickerFile.Write(Environment.NewLine + "kolejny tekst");
                    windowData.Show();
                    windowData.textBox1.Text = "Hello";

                    //dataTimer.Tick += new EventHandler(dataTimer_Tick);
                    //dataTimer.Interval = new TimeSpan(0, 0, 5);
                    //dataTimer.Start();
                    
                    StartReadingQueue();
                                       

                    myFlicer.writeData("R");
                    textStatus.Text += Environment.NewLine + "R-Command Send";
                    textStatus.ScrollToEnd();

                    flickerFile.Close(); 
                }
                else
                {
                    Console.WriteLine("Folder not selected");
                }
                enableControls(false, true, true, true);
            }
        }

        BackgroundWorker ftdiQueueReader = new BackgroundWorker();
        

        private void StartReadingQueue()
        {
            ftdiQueueReader.WorkerSupportsCancellation = true;
            
            ftdiQueueReader.DoWork += new DoWorkEventHandler(ftdiQueueReader_DoWork);
            if (ftdiQueueReader.IsBusy != true)
            {
                ftdiQueueReader.RunWorkerAsync();
            }
        }

        void ftdiQueueReader_DoWork(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker worker = sender as BackgroundWorker;
            

            if (myFlicer.receivedData.Count > 0)
            {
                byte singleChar;

                while (myFlicer.receivedData.Count > 0)
                {
                    singleChar = Convert.ToByte(myFlicer.receivedData.Dequeue());
                    windowData.textBox1.Text += singleChar;
                }
            }
            CommandManager.InvalidateRequerySuggested();
        }

        

        public void saveReceivedData()
        {
            enableControls(false, true, true, true);
            
        }



        private void btErase_Click(object sender, RoutedEventArgs e)
        {
            if (myFlicer.isOpened())
            {
                myFlicer.writeData("E");
                textStatus.Text += Environment.NewLine + "E-Command Send";
                textStatus.ScrollToEnd();
            }
        }

        private void btSynchro_Click(object sender, RoutedEventArgs e)
        {
            if (myFlicer.isOpened())
            {
                DateTime time = DateTime.Now;
                Console.WriteLine(time.ToString());

                string textToDevice = "T" + time.ToString();

                myFlicer.writeData(textToDevice);
                textStatus.Text += Environment.NewLine + time.ToString();
                textStatus.ScrollToEnd();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            windowData.Close();
            
        }
    }
}
