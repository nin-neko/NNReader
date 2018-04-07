using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using Prism.Ioc;


namespace NNReader.Ordering
{
    public class OrderDispatcher : IOrderDispatcher
    {
        private static readonly ExecutionDataflowBlockOptions options = new ExecutionDataflowBlockOptions()
        {
            EnsureOrdered = true,
            MaxDegreeOfParallelism = 1,
        };
        private readonly ActionBlock<IOrder> orderInvoker = new ActionBlock<IOrder>(o => o.Invoke(), options);

        public OrderDispatcher()
        {
        }

        public async Task DispatchAsync(IOrder order) => await orderInvoker.SendAsync(order);
        public async Task DispatchAsync(Action action) => await this.DispatchAsync(new DelegateOrder(action));
    }

    abstract class BaseOrder : IOrder
    {
        protected Dictionary<string, object> Contexts { get; } = new Dictionary<string, object>();
        [Unity.Attributes.Dependency]
        protected IContainerProvider Container { get; set; }

        public IOrder With<T>(string name, T value)
        {
            if (!name.EndsWith("Context")) name = name + "Context";
            this.Contexts[name] = value;
            return this;
        }

        public abstract void Invoke();
    }

    class DelegateOrder : BaseOrder
    {
        private readonly Action action;

        public DelegateOrder(Action action) => this.action = action;

        public override void Invoke() => action?.Invoke();
    }
}
