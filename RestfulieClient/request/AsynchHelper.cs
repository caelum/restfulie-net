using System;
using System.Collections.Generic;
using System.Threading;

namespace RestfulieClient.request
{
    public static class AsynchHelper
    {
        private static readonly IList<IAsyncResult> AsynchResults = new List<IAsyncResult>();

        public static T WaitForAsynchResponse<T>(Func<AsyncCallback, IAsyncResult> beginDelegate, Func<IAsyncResult, object, T> endDelegate, TimeSpan timeout) {
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            Exception exception = null;
            int index = AsynchResults.Count;
            T response = default(T);

            AsyncCallback callback = state => {
                try {
                    response = endDelegate(state, AsynchResults[index]);
                }
                catch (Exception e) {
                    exception = e;
                }
                finally {
                    waitHandle.Set();
                }
            };

            IAsyncResult result = beginDelegate(callback);

            if (result.CompletedSynchronously)
                return response;

            AsynchResults.Add(result);

            bool hasSignal = waitHandle.WaitOne(timeout);

            if (!hasSignal)
                throw new TimeoutException("No response received in time");

            if (exception != null) throw exception;

            AsynchResults.RemoveAt(index);

            return response;
        }

        public static T WaitForAsynchResponse<T>(Func<AsyncCallback, IAsyncResult> beginDelegate, Func<IAsyncResult, object, T> endDelegate) {
            return WaitForAsynchResponse(beginDelegate, endDelegate, TimeSpan.FromSeconds(5));
        }
    }
}
