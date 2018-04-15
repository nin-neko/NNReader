using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;


namespace NNReader.IO
{
    class Locator : ILocator
    {
        public static Locator Default { get; } = new Locator();

        private Locator()
        {
            var asm = Assembly.GetEntryAssembly();
            this.BootstrapperPath = asm.Location;
            this.BootstrapperDirectory = Path.GetDirectoryName(this.BootstrapperPath);
        }

        public string BootstrapperPath { get; }
        public string BootstrapperDirectory { get; }
    }
}
