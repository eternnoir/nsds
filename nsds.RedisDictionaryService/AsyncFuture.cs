namespace nsds.RedisDictionaryService
{
    #region DEPENDENCY

    using System;
    using System.Threading;

    #endregion

    public class AsyncFuture<T> : IAsyncFuture
    {
        public Exception Exception;

        public volatile bool Done;

        private readonly ManualResetEvent Event = new ManualResetEvent(false);

        private AsyncTask task;

        private T result;

        private AsyncFuture(AsyncTask task)
        {
            this.task = task;
        }

        public delegate T AsyncTask();

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
            
            this.Done = true;
            this.Event.Set();
        }
    }
}