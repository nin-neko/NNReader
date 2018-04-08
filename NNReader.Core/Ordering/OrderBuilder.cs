using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using Prism.Ioc;


namespace NNReader.Ordering
{
    class OrderBuilder : IOrderBuilder
    {
        private readonly IContainerProvider container;
        private readonly IOrderDispatcher orderDispatcher;

        public OrderBuilder(IContainerProvider container)
        {
            this.container = container;
            this.orderDispatcher = container.Resolve<IOrderDispatcher>();
        }

        public IOrderDiapatchable From(string orderName)
        {
            var order = container.Resolve<IOrder>(orderName);
            return new OrderDispatchable(orderDispatcher, order);
        }

        public IOrderDiapatchable From(Type orderType)
        {
            var order = (IOrder)container.Resolve(orderType);
            return new OrderDispatchable(orderDispatcher, order);
        }

        class OrderDispatchable : IOrderDiapatchable
        {
            private readonly IOrderDispatcher orderDispatcher;
            private readonly IOrder order;

            public OrderDispatchable(IOrderDispatcher orderDispatcher, IOrder order)
            {
                this.orderDispatcher = orderDispatcher;
                this.order = order;
            }

            public IOrderDiapatchable With<T>(string name, T value)
            {
                order.With(name, value);
                return this;
            }

            public async Task DispatchAsync() => await orderDispatcher.DispatchAsync(order);
        }
    }    
}
