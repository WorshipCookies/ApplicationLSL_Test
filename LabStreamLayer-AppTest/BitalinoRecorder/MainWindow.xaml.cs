﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace BitalinoRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /**
         * Create a Properties Section for Bitalino Device
         * This way we can make sure it is well managed and only enable certain functions once the device is correctly loaded.
         * **/
        private Bitalino _connectedDevice;

        private Bitalino connected_device
        {
            get { return _connectedDevice; }
            set
            {
                _connectedDevice = value;
                if(_connectedDevice != null)
                    OnVariableChange(value);
            }
        }

        /**
         * Event triggers once the device is connected.
         * **/
        public delegate void OnVariableChangeDelegate(Bitalino newValue);
        public event OnVariableChangeDelegate OnVariableChange;

        // MAC Address of the selected Device
        private string macAddress;

        // String buffer to read the obtained signals from the device as it is going to be running in its own thread.
        private static List<string> bufferList = new List<string>();

        // Streaming flag to effectively stop bitalino streaming. 
        private static bool IS_STREAMING = false;

        // Thread that will be streaming data
        private Thread streamingThread;

        public MainWindow()
        {
            InitializeComponent();
            ShowDevices();
            this.Closed += new EventHandler(OnWindowClosing);
            OnVariableChange += EnableStreamingButton;
        }

        private void ShowDevices()
        {
            Bitalino.DevInfo[] devices = Bitalino.find();

            foreach(Bitalino.DevInfo d in devices)
            {
                string[] nameCheck = d.name.Split('-');

               if(nameCheck.Length > 0 && nameCheck[0] == "BITalino")                
                    BlinoDeviceList.Items.Add(d.macAddr + "-" + d.name);
                
            }
        }


        private void StreamData()
        {
            Bitalino dev = connected_device;

            dev.start(1000, new int[] { 0 });

            Bitalino.Frame[] frames = new Bitalino.Frame[100];
            for (int i = 0; i < frames.Length; i++)
                frames[i] = new Bitalino.Frame(); // must initialize all elements in the array

            while (IS_STREAMING)
            {
                dev.read(frames);
                string s = "";
                foreach (Bitalino.Frame f in frames)
                {
                    
                }
            }

        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            // Connect user selected device if not device is currently connected
            if(BlinoDeviceList.SelectedValue != null && connected_device == null)
            {
                macAddress = BlinoDeviceList.SelectedValue.ToString().Split('-')[0];

                infoOutputBox.Text = "Connecting to Device ... " + macAddress;

                connected_device = new Bitalino(macAddress);

                infoOutputBox.Text = "Device " + macAddress + " Sucessfully Connected.";
            }
            // If a device is already connected warn the user.
            else if (connected_device != null)
            {
                infoOutputBox.Text = "Existing Device (" + macAddress + ") Already Connected";
            }
        }

        private void OnWindowClosing(object sender, System.EventArgs e)
        {
            // Need to make sure to disconnect and stop the device once the program is closed for efficiency!
            if(connected_device != null)
            {
                connected_device.Dispose();
            }
        }

        private void EnableStreamingButton(Bitalino device)
        {
            if(device != null)
            {
                startStreamingButton.IsEnabled = true;
            }
            else
            {
                startStreamingButton.IsEnabled = false;
            }
        }

        private void startStreamingButton_Click(object sender, RoutedEventArgs e)
        {
            if (IS_STREAMING)
            {
                IS_STREAMING = false;

                // Wait for Thread to Stop -- might want to delegate this to a specific event.
            }
            else
            {
                IS_STREAMING = true;
                // Run Thread
            }
        }
    }
}
