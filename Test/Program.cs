using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Parser p = new Parser.Parser();
            p.Parse("Cat has big claws and long tail");
            Console.ReadLine();
        }
    }
}
