using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NNReader.Serialization
{
    public interface IDeserializer<T>
    {
        Task LoadAsync(T target);
    }
}
