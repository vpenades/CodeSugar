using System;



namespace ExampleLibrary
{
    public class ClassExample
    {
        public void ExampleOfStreamWrite()
        {
            using(var m = new System.IO.MemoryStream())
            {
                m.WriteAllBytes(new byte[1024]);
                m.WriteLeS32(10);
                m.WriteLeF32(0.1f);
                m.WriteString("hello world");
            }
        }


        public void Other()
        {
            ContentFilesExample.ExampleInternals.GetFileText("hello");
            ExampleReader.GetDataText();            
        }
    }
}
