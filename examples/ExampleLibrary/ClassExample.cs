using System;

using ExampleLibrary;

namespace LibraryExample
{
    public class ClassExample
    {
        public void ExampleOfStreamWrite()
        {
            using(var m = new System.IO.MemoryStream())
            {
                m.WriteAllBytes(new byte[1024]);
                m.WriteValue<int>(10);
                m.WriteValue<float>(0.1f);
                m.WriteString("hello world");
            }
        }

    }
}
