using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NNReader.Diagnostics
{
    abstract class BaseComponent : IComponent
    {
        public string Name { get; protected set; } = "";

        public string ProjectUrl { get; protected set; } = "";

        public string License { get; protected set; } = "";

        public string LicenseUrl { get; protected set; } = "";
    }

    abstract class BaseMitComponent : BaseComponent
    {
        public BaseMitComponent() => this.License = "MIT";
    }

    abstract class BaseMsplComponent : BaseComponent
    {
        public BaseMsplComponent() => this.License = "MS-PL";
    }

    abstract class BaseApacheComponent : BaseComponent
    {
        public BaseApacheComponent() => this.License = "Apache License 2.0";
    }

    sealed class AngleSharp : BaseMitComponent
    {
        public AngleSharp()
        {
            this.Name = "AngleSharp";
            this.ProjectUrl = "https://github.com/AngleSharp/AngleSharp";
            this.LicenseUrl = "https://github.com/AngleSharp/AngleSharp/blob/master/LICENSE";
        }
    }

    sealed class CommonServiceLocator : BaseMsplComponent
    {
        public CommonServiceLocator()
        {
            this.Name = "CommonServiceLocator";
            this.ProjectUrl = "https://github.com/unitycontainer/commonservicelocator";
            this.LicenseUrl = "https://github.com/unitycontainer/commonservicelocator/blob/master/LICENSE";
        }
    }

    sealed class ControlzEx : BaseMitComponent
    {
        public ControlzEx()
        {
            this.Name = "ControlzEx";
            this.ProjectUrl = "https://github.com/ControlzEx/ControlzEx";
            this.LicenseUrl = "https://github.com/ControlzEx/ControlzEx/blob/develop/LICENSE";
        }
    }

    sealed class MahApps : BaseMitComponent
    {
        public MahApps()
        {
            this.Name = "MahApps.Metro";
            this.ProjectUrl = "https://github.com/MahApps/MahApps.Metro";
            this.LicenseUrl = "https://github.com/MahApps/MahApps.Metro/blob/develop/LICENSE";
        }
    }

    sealed class MaterialDesignInXAMLToolkit : BaseMitComponent
    {
        public MaterialDesignInXAMLToolkit()
        {
            this.Name = "Material Design In XAML Toolkit";
            this.ProjectUrl = "https://github.com/ButchersBoy/MaterialDesignInXamlToolkit";
            this.LicenseUrl = "https://github.com/ButchersBoy/MaterialDesignInXamlToolkit/blob/master/LICENSE";
        }
    }

    sealed class JsonNet : BaseMitComponent
    {
        public JsonNet()
        {
            this.Name = "Json.NET";
            this.ProjectUrl = "https://github.com/JamesNK/Newtonsoft.Json";
            this.LicenseUrl = "https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md";
        }
    }

    sealed class Prism : BaseMitComponent
    {
        public Prism()
        {
            this.Name = "Prism";
            this.ProjectUrl = "https://github.com/PrismLibrary/Prism";
            this.LicenseUrl = "https://github.com/PrismLibrary/Prism/blob/master/LICENSE";
        }
    }

    sealed class ReactiveProperty : BaseMitComponent
    {
        public ReactiveProperty()
        {
            this.Name = "ReactiveProperty";
            this.ProjectUrl = "https://github.com/runceel/ReactiveProperty";
            this.LicenseUrl = "https://github.com/runceel/ReactiveProperty/blob/master/LICENSE.txt";
        }
    }

    sealed class ReactiveExtensions : BaseApacheComponent
    {
        public ReactiveExtensions()
        {
            this.Name = "Reactive Extensions";
            this.ProjectUrl = "https://github.com/Reactive-Extensions/Rx.NET";
            this.LicenseUrl = "https://www.apache.org/licenses/LICENSE-2.0";
        }
    }

    sealed class CoreFx : BaseMitComponent
    {
        public CoreFx()
        {
            this.Name = ".NET Core Libraries";
            this.ProjectUrl = "https://github.com/dotnet/corefx";
            this.LicenseUrl = "https://github.com/dotnet/corefx/blob/master/LICENSE.TXT";
        }
    }

    sealed class Unity : BaseApacheComponent
    {
        public Unity()
        {
            this.Name = "Unity";
            this.ProjectUrl = "https://github.com/unitycontainer/unity";
            this.LicenseUrl = "https://www.apache.org/licenses/LICENSE-2.0";
        }
    }
}
