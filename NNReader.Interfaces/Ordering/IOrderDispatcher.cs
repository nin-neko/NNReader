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
        //Task DispatchAsync(Action action);
    }

    public interface IOrderChainable<TOrder> where TOrder : IOrderChainable<TOrder>
    {
        TOrder With<T>(string name, T value);
    }

    public interface IOrderDiapatchable : IOrderChainable<IOrderDiapatchable>
    {
        Task DispatchAsync();
    }

    public interface IOrder : IOrderChainable<IOrder>
    {
        Task InvokeAsync();
    }

    public interface IOrderBuilder
    {
        IOrderDiapatchable From(string orderName);
        IOrderDiapatchable From(Type orderType);
    }

    public static class IOrderBuilderExtensions
    {
        public static IOrderDiapatchable From<TOrder>(this IOrderBuilder self) where TOrder : IOrder
            => self.From(typeof(TOrder));
    }
}
