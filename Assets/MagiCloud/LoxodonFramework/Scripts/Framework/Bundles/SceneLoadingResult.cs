using System;

using Loxodon.Log;
using Loxodon.Framework.Execution;

namespace Loxodon.Framework.Bundles
{
    public enum LoadState
    {
        None = 0,
        AssetBundleLoaded,
        SceneActivationReady,
        Failed,
        Completed
    }

    public interface ISceneLoadingResult<TResult>
    {
        /// <summary>
        /// Gets the result of the asynchronous operation.
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Gets the cause of the asynchronous operation.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Returns <code>true</code> if the asynchronous operation is finished.
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// The task's progress.
        /// </summary>
        float Progress { get; }

        /// <summary>
        /// Scene loading state.
        /// </summary>
        LoadState State { get; }

        /// <summary>
        /// Priority lets you tweak in which order async operation calls will be performed.
        /// When multiple asynchronous operations are queued up, the operation with the higher priority will be executed first. Once an operation has been started on the background thread, changing the priority will have no effect anymore.
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        /// Allow scenes to be activated as soon as it is ready.
        /// </summary>
        bool AllowSceneActivation { get; set; }

        /// <summary>
        /// Wait for the result,suspends the coroutine.
        /// eg:
        /// IAsyncResult result;
        /// yiled return result.WaitForDone();
        /// </summary>
        /// <returns></returns>
        object WaitForDone();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress"></param>
        void OnProgressCallback(Action<float> callback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        void OnStateChangedCallback(Action<LoadState> callback);
    }

    public interface ISceneLoadingPromise<TResult>
    {
        /// <summary>
        /// The execution result
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Exception
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Returns  "true" if this task finished.
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// The task's progress.
        /// </summary>
        float Progress { get; set; }

        /// <summary>
        /// Scene loading state.
        /// </summary>
        LoadState State { get; set; }

        /// <summary>
        /// Priority lets you tweak in which order async operation calls will be performed.
        /// When multiple asynchronous operations are queued up, the operation with the higher priority will be executed first. Once an operation has been started on the background thread, changing the priority will have no effect anymore.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Allow scenes to be activated as soon as it is ready.
        /// </summary>
        bool AllowSceneActivation { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        void SetException(string error);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void SetException(Exception exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        void SetResult(TResult result);
    }


    public class SceneLoadingResult<TResult> : ISceneLoadingResult<TResult>, ISceneLoadingPromise<TResult>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SceneLoadingResult<TResult>));

        private const int DEFAULT_PRIORITY = 5;
        private int priority = DEFAULT_PRIORITY;
        private bool done = false;
        private TResult result;
        private Exception exception = null;
        private float progress = 0f;
        private LoadState state = LoadState.None;
        private bool allowSceneActivation = true;

        private Action<LoadState> callback;
        private Action<float> progressCallback;

        /// <summary>
        /// Exception
        /// </summary>
        public virtual Exception Exception
        {
            get { return this.exception; }
        }

        /// <summary>
        /// Returns  "true" if this task finished.
        /// </summary>
        public virtual bool IsDone
        {
            get { return this.done; }
        }

        /// <summary>
        /// The execution result
        /// </summary>
        public virtual TResult Result
        {
            get { return this.result; }
        }

        /// <summary>
        /// The task's progress.
        /// </summary>
        public virtual float Progress
        {
            get { return this.progress; }
            set
            {
                this.progress = value;
                this.RaiseOnProgressCallback(this.progress);
            }
        }

        public LoadState State
        {
            get { return this.state; }
            set
            {
                if (this.state == value)
                    return;

                this.state = value;
                this.RaiseOnStateChangedCallback(this.state);
            }
        }

        /// <summary>
        /// Priority lets you tweak in which order async operation calls will be performed.
        /// When multiple asynchronous operations are queued up, the operation with the higher priority will be executed first. Once an operation has been started on the background thread, changing the priority will have no effect anymore.
        /// </summary>
        public virtual int Priority
        {
            get { return this.priority; }
            set { this.priority = value; }
        }

        /// <summary>
        /// Allow scenes to be activated as soon as it is ready.
        /// </summary>
        public virtual bool AllowSceneActivation
        {
            get { return this.allowSceneActivation; }
            set { this.allowSceneActivation = value; }
        }

        public virtual void SetException(string error)
        {
            if (this.done)
                return;

            var exception = new Exception(string.IsNullOrEmpty(error) ? "unknown error!" : error);
            SetException(exception);
        }

        public virtual void SetException(Exception exception)
        {
            if (this.done)
                return;

            this.exception = exception;
            this.done = true;

            this.State = LoadState.Failed;
        }

        public virtual void SetResult(TResult result)
        {
            if (this.done)
                return;

            this.result = result;
            this.done = true;

            this.State = LoadState.Completed;
        }

        public virtual object WaitForDone()
        {
            return Executors.WaitWhile(() => !IsDone);
        }

        public virtual void OnStateChangedCallback(Action<LoadState> callback)
        {
            if (callback == null)
                return;

            if (this.IsDone)
            {
                try
                {
                    if (this.exception != null)
                        callback(LoadState.Failed);
                    else {
                        callback(LoadState.Completed);
                    }
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Class[{0}] progress callback exception.Error:{1}", this.GetType(), e);
                }
                return;
            }

            this.callback += callback;
        }

        public virtual void OnProgressCallback(Action<float> callback)
        {
            if (callback == null)
                return;

            if (this.IsDone)
            {
                try
                {
                    callback(this.Progress);
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Class[{0}] progress callback exception.Error:{1}", this.GetType(), e);
                }
                return;
            }

            this.progressCallback += callback;
        }

        protected virtual void RaiseOnStateChangedCallback(LoadState eventArgs)
        {
            try
            {
                if (this.callback == null)
                    return;

                var list = this.callback.GetInvocationList();
                foreach (Action<LoadState> action in list)
                {
                    try
                    {
                        action(eventArgs);
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("Class[{0}] callback exception.Error:{1}", this.GetType(), e);
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Class[{0}] callback exception.Error:{1}", this.GetType(), e);
            }
            finally
            {
                if (this.done)
                {
                    this.callback = null;
                    this.progressCallback = null;
                }
            }
        }

        protected virtual void RaiseOnProgressCallback(float progress)
        {
            try
            {
                if (this.progressCallback == null)
                    return;

                var list = this.progressCallback.GetInvocationList();
                foreach (Action<float> action in list)
                {
                    try
                    {
                        action(progress);
                    }
                    catch (Exception e)
                    {
                        if (log.IsWarnEnabled)
                            log.WarnFormat("Class[{0}] progress callback exception.Error:{1}", this.GetType(), e);
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Class[{0}] progress callback exception.Error:{1}", this.GetType(), e);
            }
        }
    }
}
