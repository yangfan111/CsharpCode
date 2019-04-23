using System;
using System.Collections;
using System.Threading;

namespace Utils.Concurrent
{
    
    /**
     * 信号事件实现的bolckingqueue 适合单个消费者，如果是多个消费者就不好控制顺序，会随机给任何一个消费者
     */
    public class BlockingQueue<T>
    {
        private Queue _queue;
        private Semaphore _semaphore;

        public BlockingQueue(int initNum = 128)
        {
            _semaphore = new Semaphore(0, Int32.MaxValue); //消息不知道有多少，设个足够大的吧，初始=0
            _queue = Queue.Synchronized(new Queue(initNum));
        }

        public int Count
        {
            get { return _queue.Count; }
        }

        public T Dequeue()
        {
            if(_semaphore.WaitOne())//请求信号量，信号量=0，就会阻塞了
                return (T) _queue.Dequeue();
            else
            {
                return default(T);
            }
        }
        public T Dequeue(int time)
        {
            if(_semaphore.WaitOne(time))//请求信号量，信号量=0，就会阻塞了
            return (T) _queue.Dequeue();
            else
            {
                return default(T);
            }
        }

        public void Enqueue(T t)
        {
            _queue.Enqueue(t); //出队
            _semaphore.Release(); //释放信号量
        }

        public void Clear()
        {
            while (_queue.Count>0)
            {
                Dequeue();
            }
        }
    }
}