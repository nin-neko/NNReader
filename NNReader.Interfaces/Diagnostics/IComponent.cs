using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NNReader.Diagnostics
{
    public interface IComponent
    {
        string Name { get; }
        string ProjectUrl { get; }
        string License { get; }
        string LicenseUrl { get; }
    }
}
