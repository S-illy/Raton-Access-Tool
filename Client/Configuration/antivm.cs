using System;
using System.Management;

public class VMCheck
{
    public static void Analyze()
    {
        if (CheckVMWare() || CheckVirtualBox() || CheckHyperV() || CheckVMInfo())
        {
            Environment.Exit(0);
        }
    }

    private static bool CheckVMWare()
    {
        try
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["Manufacturer"]?.ToString().ToLower().Contains("vmware") == true)
                    {
                        return true;
                    }
                }
            }
        }
        catch { }
        return false;
    }

    private static bool CheckVirtualBox()
    {
        try
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["Manufacturer"]?.ToString().ToLower().Contains("innotek") == true)
                    {
                        return true;
                    }
                }
            }
        }
        catch { }
        return false;
    }

    private static bool CheckHyperV()
    {
        try
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["Model"]?.ToString().ToLower().Contains("virtual machine") == true)
                    {
                        return true;
                    }
                }
            }
        }
        catch { }
        return false;
    }

    private static bool CheckVMInfo()
    {
        try
        {
            string[] vmManufacturers = { "VMware", "VirtualBox", "Hyper-V", "KVM", "QEMU", "Xen" };
            foreach (string manufacturer in vmManufacturers)
            {
                if (System.IO.Directory.Exists(@"C:\\Program Files\\" + manufacturer))
                {
                    return true;
                }
            }
        }
        catch { }
        return false;
    }
}
