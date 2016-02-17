namespace nsds.RedisDictionaryService
{
    #region DEPENDENCY

    using System;
    using System.Threading;

    #endregion

    public class AsyncFuture<T> : IAsyncFuture
    {
        public Exception Exception;

        private readonly ManualResetEvent Event = new ManualResetEvent(false);

        private AsyncTask task;

        private T result;

        public AsyncFuture(AsyncTask task)
        {
            this.task = task;
            this.Run();
        }

        public delegate T AsyncTask();

        public T Get()
        {
            this.Event.WaitOne();
            if (this.Exception != null)
            {
                throw this.Exception;
            }

            return this.result;
        }

        private void Run()
        {
            ThreadPool.QueueUserWorkItem(state => this.InnerRun());
        }

        private void InnerRun()
        {
            try
            {
                this.result = this.task();
                this.task = null;
            }
            catch (Exception ex)
            {
                this.Exception = ex;
            }

            this.Event.Set();
        }
    }
}