using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PseudoExtensibleEnum
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //test context creation
            PxEnum.RecreateCurrentContext();
            PxEnum.CurrentContext.LoadTypes(System.Reflection.Assembly.GetEntryAssembly());

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
            const string testJson4A = "{\"btest\": null}";
            const string testJson4 = "{\"test\": null, \"btest\": null}";
            const string testJson5 = "{\"test\": null}";
            const string testJson6 = "{\"test\": 1}";

            var testModel1 = JsonConvert.DeserializeObject<TestModel>(testJson1);
            var testModel2 = JsonConvert.DeserializeObject<TestModel>(testJson2);
            var testModel3 = JsonConvert.DeserializeObject<TestModel>(testJson3);
            //var testModel4A = JsonConvert.DeserializeObject<TestModel>(testJson4A);
            //var testModel4 = JsonConvert.DeserializeObject<TestModel>(testJson4);
            //var testModel5 = JsonConvert.DeserializeObject<TestModel>(testJson5);

            var testModel3Z = JsonConvert.DeserializeObject<TestModel2>(testJson3);
            var testModel5Z = JsonConvert.DeserializeObject<TestModel2>(testJson5);

            var testModel6 = JsonConvert.DeserializeObject<TestModel>(testJson6);

            var testModelArray2 = new TestModelArray()
            {
                Test = new int[] { 1, 2, 128, 4 }
            };
            string array2out = JsonConvert.SerializeObject(testModelArray2);
            Console.WriteLine(array2out);

            const string testJsonArray1 = "{\"test\": [1, \"Third\", \"Second\", 128]}";
            var testModelArray1 = JsonConvert.DeserializeObject<TestModelArray>(testJsonArray1);

            var testSimpleObjectModel = new TestModelObject()
            {
                Test = new Dictionary<int, float>()
                {
                    { 2, 4.5f },
                    { 3, 7.123f }
                }
            };
            string simpleObjectOut = JsonConvert.SerializeObject(testSimpleObjectModel);
            Console.WriteLine(simpleObjectOut);

            var converter = new PxEnumObjectConverter(typeof(TestModelObject));
            bool objectCanConvert = converter.CanConvert(testSimpleObjectModel.Test.GetType());

            const string testJsonSimpleObject = "{\"Test\": { \"2\": 2.5, \"Something\": 100, \"3\": 3.33, \"SomethingElse\": 200.823}}";
            var testSimpleObject1 = JsonConvert.DeserializeObject<TestModelObject>(testJsonSimpleObject);

            //TODO complex object test

            //"skip unknown" tests
            const string testJsonArrayUnknownKeys = "{\"test\": [1, \"Third\", \"Second\", 128, \"AAAAUnknown\", \"BBBUnknown\"]}";
            var testModelArrayUnknownKeys = JsonConvert.DeserializeObject<TestModelArraySkipUnknown>(testJsonArrayUnknownKeys);

            const string testJsonSimpleObjectUnknownKeys = "{\"Test\": { \"2\": 2.5, \"Something\": 100, \"3\": 3.33, \"SomethingElse\": 200.823, \"AAAAUnknown\": 105}}";
            var testSimpleObject2 = JsonConvert.DeserializeObject<TestModelObjectSkipUnknown>(testJsonSimpleObjectUnknownKeys);

            Console.WriteLine("all passed!");

        }
    }

    public class TestModel
    {
        [JsonConverter(typeof(PxEnumConverter), typeof(BaseEnum))]
        public int Test { get; set; }

        public BaseEnum BTest { get; set; }
    }

    public class TestModel2
    {
        [JsonConverter(typeof(PxEnumConverter), typeof(BaseEnum))]
        public int? Test { get; set; }
    }

    public class TestModelArray
    {
        [JsonConverter(typeof(PxEnumArrayConverter), typeof(BaseEnum))]
        public IList<int> Test { get; set; }
    }

    public class TestModelObject
    {
        [JsonConverter(typeof(PxEnumObjectConverter), typeof(BaseEnum))]
        public IReadOnlyDictionary<int, float> Test { get; set; }
    }

    public class TestModelComplexObject
    {
        [JsonConverter(typeof(PxEnumObjectConverter), typeof(BaseEnum))]
        public IReadOnlyDictionary<int, object> Test { get; set; }
    }
    public class TestModelArraySkipUnknown
    {
        [JsonConverter(typeof(PxEnumArrayConverter), typeof(BaseEnum), true, true)]
        public IList<int> Test { get; set; }
    }

    public class TestModelObjectSkipUnknown
    {
        [JsonConverter(typeof(PxEnumObjectConverter), typeof(BaseEnum), true, true)]
        public IReadOnlyDictionary<int, float> Test { get; set; }
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
