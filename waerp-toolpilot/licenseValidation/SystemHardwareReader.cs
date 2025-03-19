using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace waerp_toolpilot.License
{
    internal class SystemHardwareReader
    {

        internal static bool CompareSavedHash()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot");
            if (key.GetValue("CH").ToString() == GetSystemHash())
            {
                key.Close();
                return true;
            }
            else
            {
                key.Close();
                return false;
            }

        }
        internal static void WriteNewSystemHash()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\toolpilot", true);
            key.SetValue("CH", GetSystemHash());
            key.Close();
        }
        internal static string GetSystemHash()
        {
            string cpuid = string.Empty;
            string macAddress = string.Empty;
            string osSerial = string.Empty;
            string mbSerial = string.Empty;
            string ssdSerial = string.Empty;



            // creating ManagementClass to get an Instance to the WMI-objects

            // first read out the processor id
            ManagementClass man = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = man.GetInstances();

            foreach (ManagementObject mob in moc)
            {
                if (mob != null)
                {
                    if (mob["processorID"] != null)
                    {
                        cpuid = mob["processorID"].ToString();
                    }
                }
            }

            // second read out the mac-address
            man = new ManagementClass("Win32_NetworkAdapter");
            moc = man.GetInstances();

            foreach (ManagementObject mob in moc)
            {
                if (mob != null)
                {
                    if (mob["MacAddress"] != null)
                    {
                        macAddress = mob["MacAddress"].ToString();
                        break;
                    }
                }
            }


            // thrid read out the product id from the operating system
            man = new ManagementClass("Win32_operatingsystem");
            moc = man.GetInstances();

            foreach (ManagementObject mob in moc)
            {
                if (mob != null)
                {
                    osSerial = mob["SerialNumber"].ToString();
                }
            }


            // fourth read out the motherboard ID
            man = new ManagementClass("Win32_BaseBoard");
            moc = man.GetInstances();

            foreach (ManagementObject mob in moc)
            {
                if (mob != null)
                {
                    mbSerial = mob["SerialNumber"].ToString();
                }
            }

            // fifth read out the SSD ID
            man = new ManagementClass(@"Win32_logicaldisk");
            moc = man.GetInstances();

            foreach (ManagementObject mob in moc)
            {
                if (mob != null)
                {
                    if (mob["DeviceID"].Equals("C:"))
                    {
                        ssdSerial = mob["VolumeSerialNumber"].ToString();
                    }
                }
            }

            // create Seed
            string keySeed = "toolpilot" + cpuid + macAddress + osSerial + mbSerial + ssdSerial;

            SHA512 sha512Hash = SHA512.Create();

            byte[] sourceBytes = Encoding.UTF8.GetBytes(keySeed);
            byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);

            // create unique key
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }

    }
}
