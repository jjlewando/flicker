using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTD2XX_NET;

namespace FlickerApplication
{
    class FTDevice
    {                
        // Create new instance of the FTDI device class
        private static FTDI myFtdiDevice;
        private bool opened { get; set; }

        public FTDevice()
        {
            myFtdiDevice = new FTDI();
            opened = false; 
            
        }

        public bool isOpened()
        {
            return opened; 
        }


        public UInt32 GetNumberOfDevices()
        {
            UInt32 ftdiDeviceCount = 0;
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            
            // Determine the number of FTDI devices connected to the machine
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            // Check status
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                Console.WriteLine("Number of FTDI devices: " + ftdiDeviceCount.ToString());
                Console.WriteLine("");
                return ftdiDeviceCount;
            }
            else
            {
                // Wait for a key press
                Console.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
                Console.ReadKey();
                return 0;                
            }            
        }

        public FTDI.FT_DEVICE_INFO_NODE[] GetDevices(UInt32 numberOfDevices)
        {
            // Allocate storage for device info list
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[numberOfDevices];
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;


            // Populate our device list
            ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);

            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                for (UInt32 i = 0; i < numberOfDevices; i++)
                {
                    Console.WriteLine("Device Index: " + i.ToString());
                    Console.WriteLine("Flags: " + String.Format("{0:x}", ftdiDeviceList[i].Flags));
                    Console.WriteLine("Type: " + ftdiDeviceList[i].Type.ToString());
                    Console.WriteLine("ID: " + String.Format("{0:x}", ftdiDeviceList[i].ID));
                    Console.WriteLine("Location ID: " + String.Format("{0:x}", ftdiDeviceList[i].LocId));
                    Console.WriteLine("Serial Number: " + ftdiDeviceList[i].SerialNumber.ToString());
                    Console.WriteLine("Description: " + ftdiDeviceList[i].Description.ToString());
                    Console.WriteLine("");
                    
                }
            }
            return ftdiDeviceList;
        }

        public bool OpenDefaultDevice(FTDI.FT_DEVICE_INFO_NODE[] devList, UInt32 deviceIndex)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;            

            // Open first device in our list by serial number
            ftStatus = myFtdiDevice.OpenBySerialNumber(devList[0].SerialNumber);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to open device (error " + ftStatus.ToString() + ")");
                //Console.ReadKey();
                return false;
            }

            // Set up device data parameters
            // Set Baud rate to 9600
            ftStatus = myFtdiDevice.SetBaudRate(9600);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to set Baud rate (error " + ftStatus.ToString() + ")");
                //Console.ReadKey();
                return false;
            }

            // Set data characteristics - Data bits, Stop bits, Parity
            ftStatus = myFtdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to set data characteristics (error " + ftStatus.ToString() + ")");
                //Console.ReadKey();
                return false;
            }

            // Set flow control - set RTS/CTS flow control
            ftStatus = myFtdiDevice.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_NONE, 0x00, 0x00);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to set flow control (error " + ftStatus.ToString() + ")");
                //Console.ReadKey();
                return false;
            }

            // Set read timeout to 5 seconds, write timeout to infinite
            ftStatus = myFtdiDevice.SetTimeouts(5000, 0);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to set timeouts (error " + ftStatus.ToString() + ")");
                Console.ReadKey();
                return false;
            }
            opened = true; 
            return true; 
        }

        public bool writeData(byte[] dataBuffer)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
            
            UInt32 numBytesWritten = 0;
            // Note that the Write method is overloaded, so can write string or byte array data
            ftStatus = myFtdiDevice.Write(dataBuffer, dataBuffer.Length, ref numBytesWritten);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to write to device (error " + ftStatus.ToString() + ")");
                //Console.ReadKey();
                return false;
            }
            return true;
        }

        public bool writeData(string dataBuffer)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

            UInt32 numBytesWritten = 0;
            // Note that the Write method is overloaded, so can write string or byte array data
            ftStatus = myFtdiDevice.Write(dataBuffer, dataBuffer.Length, ref numBytesWritten);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                // Wait for a key press
                Console.WriteLine("Failed to write to device (error " + ftStatus.ToString() + ")");
                //Console.ReadKey();
                return false;
            }
            return true;
        }


        public bool Close()
        {
            myFtdiDevice.Close();
            opened = false;
            return true;
        }






    }
}
