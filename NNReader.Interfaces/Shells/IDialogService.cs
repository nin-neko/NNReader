using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace NNReader.Shells
{
    public interface IDialogService : INotifyPropertyChanged
    {
        bool IsOpen { get; set; }
    }
}
