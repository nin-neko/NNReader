using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NNReader.Ordering
{
    public interface IOrderDispatcher
    {
        Task DispatchAsync(IOrder order);
        Task DispatchAsync(Action action);
    }

    public interface IOrder
    {
        IOrder With<T>(string name, T value);
        void Invoke();
    }
}
