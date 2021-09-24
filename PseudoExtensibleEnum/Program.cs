using System;

namespace PseudoExtensibleEnum
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    [PseudoExtensible]
    public enum BaseEnum
    {
        Unspecified, First, Second, Something = 100
    }

    [PseudoExtend(typeof(BaseEnum))]
    public enum ExtensionEnum
    {
        Third = 3, SomethingElse = 128
    }
}
