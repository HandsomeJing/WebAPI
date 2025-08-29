using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearlerPlatform.Common.EventBusHelper
{
    /// <summary>
    /// 本地事件总线 - 用于应用内部不同模块间的消息传递
    /// 比如购物车变化时通知商品服务更新库存信息
    /// </summary>
    public class LocalEventBus<T>
        where T : class
    {
        public delegate Task LocalEventHandler(T t);
        /// <summary>事件处理器 - 当有消息发布时会触发这些处理器</summary>
        public event LocalEventHandler? localEventHandler;
        
        /// <summary>
        /// 发布事件消息 - 通知所有订阅者处理数据
        /// </summary>
        public async Task Publish(T t)
        {
            if (localEventHandler != null)
            {
                await localEventHandler(t);
            }
        }
    }
}