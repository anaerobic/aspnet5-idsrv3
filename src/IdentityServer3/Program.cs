using System;
using Microsoft.Owin.Hosting;

namespace AspNet5Host
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:5004";
            using (WebApp.Start<Startup_Mono>(url))
            {
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
        }
    }
}