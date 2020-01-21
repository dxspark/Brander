using System;
using System.IO;

namespace Brander.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Constants.GreetingMessage);
            Console.WriteLine(DateTime.Now.ToString(Constants.DateFormat));

            Console.WriteLine(File.ReadAllText("ASCIIImage.txt"));
        }
    }
}
