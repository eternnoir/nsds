namespace nsds.RedisDictionaryService
{
    #region DEPENDENCY

    using System;
    using System.Threading;

    #endregion

    public class AsyncFuture<T> : IAsyncFuture
    {
        public delegate T AsyncTask();

        private readonly ManualResetEvent Event = new ManualResetEvent(false);

        public Exception Exception;

        private AsyncTask task;

        private T result;

        private AsyncFuture(AsyncTask task)
        {
            this.task = task;
        }

        public static AsyncFuture<T> Execute(AsyncTask task)
        {
            return new AsyncFuture<T>(task).Run();
        }

        public T Get()
        {
            this.Event.WaitOne();
            if (this.Exception != null)
            {
                throw this.Exception;
            }

            return this.result;
        }

        private AsyncFuture<T> Run()
        {
            ThreadPool.QueueUserWorkItem(state => this.InnerRun());
            return this;
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