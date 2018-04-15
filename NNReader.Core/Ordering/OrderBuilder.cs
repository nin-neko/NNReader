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
            return new OrderDispatchable(container, orderDispatcher, order);
        }

        public IOrderDiapatchable From(Type orderType)
        {
            var order = (IOrder)container.Resolve(orderType);
            return new OrderDispatchable(container, orderDispatcher, order);
        }

        class OrderDispatchable : IOrderDiapatchable
        {
            private readonly IContainerProvider container;
            private readonly IOrderDispatcher orderDispatcher;
            private readonly IOrder order;

            public OrderDispatchable(IContainerProvider container, IOrderDispatcher orderDispatcher, IOrder order)
            {
                this.container = container;
                this.orderDispatcher = orderDispatcher;
                this.order = order;
            }

            public IOrderDiapatchable With<T>(string name, T value)
            {
                order.With(name, value);
                return this;
            }

            public IOrderDiapatchable Next(string orderName)
            {
                var o = container.Resolve<IOrder>(orderName);
                return new AggregatedDispatchable(container, orderDispatcher, o, new[] { order });
            }

            public async Task DispatchAsync() => await orderDispatcher.DispatchAsync(order);
        }

        class AggregatedDispatchable : IOrderDiapatchable
        {
            private readonly IContainerProvider container;
            private readonly IOrderDispatcher orderDispatcher;
            private readonly IOrder rootOrder;
            private readonly IOrder[] orders;

            public AggregatedDispatchable(IContainerProvider container, IOrderDispatcher orderDispatcher, IOrder rootOrder, IEnumerable<IOrder> orders)
            {
                this.container = container;
                this.orderDispatcher = orderDispatcher;
                this.rootOrder = rootOrder;
                this.orders = orders.ToArray();
            }

            public async Task DispatchAsync()
            {
                foreach (var o in orders)
                {
                    await orderDispatcher.DispatchAsync(o);
                }
                await orderDispatcher.DispatchAsync(rootOrder);
            }

            public IOrderDiapatchable Next(string orderName)
            {
                var o = container.Resolve<IOrder>(orderName);
                return new AggregatedDispatchable(container, orderDispatcher, o, orders.Concat(new[] { rootOrder }));
            }

            public IOrderDiapatchable With<T>(string name, T value)
            {
                rootOrder.With(name, value);
                return this;
            }
        }
    }    
}
