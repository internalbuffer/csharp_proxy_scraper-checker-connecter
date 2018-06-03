/*
 C# Proxy scraper, checker and connecter!
 This is code written on 3-6-2018
 */

using System;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Proxy
{
    class Program
    {
        public static void Main()
        {
            Console.Title = "sProxy";
            Console.Write("sProxy\nScraper, Checker, Connections\n\nFunction:         ");

            var function = Console.ReadLine().ToLower();

            if (function != "" && function.Length != 1)
            {
                if (function[0] == 's')
                {
                    Proxys.scraper();
                }
                else if (function[0] == 'c' && function[1] == 'h')
                {
                    Proxys.checker();
                }
                else if (function[0] == 'c' && function[1] == 'o')
                {
                    Proxys.connect();
                }


            } else
            {
                Console.Clear();
                Main();
            }
        }


       
    }

    class Proxys
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        public static void connect()
        {
            RegistryKey regKey = default(RegistryKey);
            string[] list = new string[] { };
            var proxytmp = "";
            Console.Clear();
            if (!File.Exists("proxys.txt"))
            {
                Console.Write("It looks like you dont got a proxy list to connect to , use the scraper first!\nPress any key to go back to the main panel.");
                Console.ReadKey();
            }
            else
            {
            
                regKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                if (regKey.GetValue("ProxyEnable").ToString() == "1")
                {
                    Console.Write("You are currently connected to the proxy server: " + regKey.GetValue("ProxyServer").ToString() + "\n");
                    Console.Write("Disconnect from the proxy server? (Yes/No):");
                    var responsetemp = Console.ReadLine().ToLower();
                    if (responsetemp[0] == 'y')
                    {
                        var currproxtemp = regKey.GetValue("ProxyServer").ToString();
                        regKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                        regKey.SetValue("ProxyEnable", false, RegistryValueKind.DWord);
                        regKey.Close();
                        InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
                        InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
                        Console.Write("You have been disconnected from proxy: " + currproxtemp + "\n\nPress a key to go back to the main panel");
                        Console.ReadKey();
                        Console.Clear();
                        Program.Main();
                    }
                }
                else
                {
                    Console.Write("Connect to random proxy server? (Yes/No):");
                    regKey.Close();
                    var response = Console.ReadLine().ToLower();

                    if (response[0] == 'y')
                    {
                        Random tmprand = new Random();
                        var temp = File.ReadAllText("proxys.txt");
                        list = temp.Split('\n');
                        proxytmp = list[tmprand.Next(0, list.Length - 1)];

                        regKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                        regKey.SetValue("ProxyEnable", true, RegistryValueKind.DWord);
                        regKey.SetValue("ProxyServer", proxytmp.Split(':')[0] + ":" + proxytmp.Split(':')[1]);
                        regKey.Close();
                        InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
                        InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
                        Console.Write("You have been connected to proxy: " + proxytmp + "\n\nPress a key to go back to the main panel");
                        Console.ReadKey();
                        Console.Clear();
                        Program.Main();

                    }
                    else
                    {

                    }
                }
            }
         }

        public static void scraper()
        {
            Console.Clear();
            WebClient client = new WebClient();
            int counter = 0;
            var sProxys = "";
            string[] urls = new string[] { "https://free-proxy-list.net/", "https://www.sslproxies.org/", "https://www.vpngids.nl/artikel/lijst-met-gratis-proxy-servers/" };
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    var result = client.DownloadString(urls[i]);
                    if (i == 2)
                    {
                        foreach (Match m in Regex.Matches(result, "[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}</td><td align=\"center\">[0-9]{1,5}"))
                        {
                            counter++;
                            sProxys += m + "\n";

                            sProxys = sProxys.Replace("</td><td align=\"center\">", ":");
                        }
                    }
                    else
                    {
                        foreach (Match m in Regex.Matches(result, "[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}</td><td>[0-9]{1,5}"))
                        {
                            counter++;
                            sProxys += m + "\n";

                            sProxys = sProxys.Replace("</td><td>", ":");
                        }
                    }
                    

                    Console.Write(sProxys);
                }
            }
            catch (Exception)
            {
                throw;
            }
            Console.Write("\nProxy's scraped: " + counter + "\nWanna save the proxys? (Yes/No):");
            var response = Console.ReadLine().ToLower();

            if (response[0] == 'y')
            {
                if (!File.Exists("proxys.txt"))
                {
                    File.WriteAllText("proxys.txt", sProxys);
                    Console.Write("File proxys.txt has been created\n\n");
                } else {
                    File.WriteAllText("proxys.txt", sProxys);
                    Console.Write("File proxys.txt already existed and has been overwritten with the new proxy's\n\n");
                }
            }
            Console.Write("Press a key to go back to the main panel");
            Console.ReadKey();
            Console.Clear();
            Program.Main();
        }

        public static void checker()
        {
            Console.Clear();
            int counter = 0;
            string[] list = new string[] { };
            var sProxys = "";
            if (!File.Exists("proxys.txt"))
            {
                Console.Write("It looks like you dont got a proxy list to check yet, use the scraper first!\nPress any key to go back to the main panel.");
                Console.ReadKey();
            } else
            {
                var temp = File.ReadAllText("proxys.txt");
                list = temp.Split('\n');
                for(int i = 0; i < list.Length - 1; i++)
                {
                    try
                    {
                        Ping check = new Ping();
                        var resulttemp = check.Send(list[i].Split(':')[0], 5);
                        if (resulttemp.Status != IPStatus.TimedOut)
                        {
                            sProxys += list[i] + "\n";
                            Console.Write(list[i] + "[" + resulttemp.RoundtripTime.ToString() + "ms]\n");
                            counter++;
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                        
                    }
                }
                Console.Write("\nProxy's working:" + counter + "\nWanna save the proxys? (Yes/No):");
                var response = Console.ReadLine().ToLower();

                if (response[0] == 'y')
                {
                    if (!File.Exists("proxys.txt"))
                    {
                        File.WriteAllText("proxys.txt", sProxys);
                        Console.Write("File proxys.txt has been created\n\n");
                    }
                    else
                    {
                        File.WriteAllText("proxys.txt", sProxys);
                        Console.Write("File proxys.txt already existed and has been overwritten with the new proxy's\n\n");
                    }
                }
                Console.Write("Press a key to go back to the main panel");
                Console.ReadKey();
                Console.Clear();
                Program.Main();
            }
        }
    }
}
