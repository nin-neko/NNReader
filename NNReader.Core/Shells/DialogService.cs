using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Mvvm;


namespace NNReader.Shells
{
    class DialogService : BindableBase, IDialogService
    {
        private bool isOpen;
        public bool IsOpen
        {
            get => isOpen;
            set => this.SetProperty(ref isOpen, value);
        }
    }
}
