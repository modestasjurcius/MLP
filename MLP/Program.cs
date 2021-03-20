using System;

namespace MLP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MLP starting...");

            var app = new Services.MLP("D:\\Mokslai\\4-kursas\\2-semestras\\duomenu-gavyba\\nd3\\glass.data");
        }
    }
}
