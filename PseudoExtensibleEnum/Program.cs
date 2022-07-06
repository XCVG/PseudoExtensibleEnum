using System;
using System.Linq;

namespace PseudoExtensibleEnum
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");            

            Console.WriteLine(string.Join(",", PxEnum.GetNames(typeof(BaseEnum))));
            Console.WriteLine(string.Join(",", PxEnum.GetValues(typeof(BaseEnum)).Cast<int>().Select(i => i.ToString())));

            Console.WriteLine(PxEnum.Parse(typeof(BaseEnum), "First"));
            Console.WriteLine((int)PxEnum.Parse(typeof(BaseEnum), "First"));
            Console.WriteLine((BaseEnum)PxEnum.Parse(typeof(BaseEnum), "First"));
            Console.WriteLine(PxEnum.Parse(typeof(BaseEnum), "SomethingElse"));
            Console.WriteLine((int)PxEnum.Parse(typeof(BaseEnum), "SomethingElse"));
            Console.WriteLine((BaseEnum)PxEnum.Parse(typeof(BaseEnum), "SomethingElse"));

            const string testJson1 = "{}";
            const string testJson2 = "{\"test\": \"Something\"}";
            const string testJson3 = "{\"test\": \"SomethingElse\"}";
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
