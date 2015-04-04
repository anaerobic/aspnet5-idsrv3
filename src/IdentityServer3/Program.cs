using System;
using Microsoft.Owin.Hosting;

namespace AspNet5Host
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "http://192.168.1.20:5004";
            using (WebApp.Start<Startup_Mono>(url))
            {
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
        }
    }
}