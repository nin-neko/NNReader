using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace NNReader.Diagnostics
{
    class ComponentService : IComponentService
    {
        public ComponentService()
        {
            var asm = Assembly.GetEntryAssembly();
            var asmName = asm.GetName();
            this.Version = asmName.Version.ToString(3);

            this.Components = new IComponent[]
            {
                new AngleSharp(),
                new CommonServiceLocator(),
                new ControlzEx(),
                new MahApps(),
                new MaterialDesignInXAMLToolkit(),
                new JsonNet(),
                new Prism(),
                new ReactiveProperty(),
                new ReactiveExtensions(),
                new CoreFx(),
                new Unity(),
            };
        }

        public string Version { get; }
        public IComponent[] Components { get; }
    }
}
