using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace CodeSugar
{
    internal class ICommandTests
    {
        [Test]
        public async Task TestICommands()
        {
            int result = 0;
            Action a1 = () => result = 1;
            a1.ToInputICommand().Execute(null);
            Assert.That(result, Is.EqualTo(1));

            result = 0;
            Func<Task> a2 = ()=> Task.Run( ()=> result = 1);
            a2.ToInputICommand().Execute(null);
            await Task.Delay(100);
            Assert.That(result, Is.EqualTo(1));

            result = 0;
            Func<int, Task> a3 = val => Task.Run(() => result = val);
            a3.ToInputICommand().Execute(5);
            await Task.Delay(100);
            Assert.That(result, Is.EqualTo(5));            
        }
    }
}
