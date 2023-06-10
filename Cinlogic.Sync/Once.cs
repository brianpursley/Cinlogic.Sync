namespace Cinlogic.Sync;

/// <summary>
/// Once is a synchronization primitive that can be used to run a piece of code only once.
/// </summary>
public sealed class Once : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private volatile bool _done;
    private Exception? _exception;
    private volatile bool _disposed;
    private readonly object _disposeLock = new();

    ~Once()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        lock (_disposeLock)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                _semaphore.Dispose();
            }
        }
    }

    /// <summary>
    /// DoAsync runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    public Task DoAsync(Func<Task> action) => DoAsync(action, false);
    
    /// <summary>
    /// DoAsync runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="doneOnException">If true, the action is considered done even if it throws an exception.</param>
    public async Task DoAsync(Func<Task> action, bool doneOnException)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
        
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        
        if (_done)
        {
            if (_exception != null)
            {
                throw _exception;
            }
            
            return;
        }

        await _semaphore.WaitAsync();
        try
        {
            if (_done)
            {
                if (_exception != null)
                {
                    throw _exception;
                }
            
                return;
            }

            await action();
            _done = true;
        }
        catch (Exception ex)
        {
            if (doneOnException)
            {
                _exception = ex;
                _done = true;
            }
            
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Do runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    public void Do(Action action) => Do(action, false);
    
    /// <summary>
    /// Do runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="doneOnException">If true, the action is considered done even if it throws an exception.</param>
    public void Do(Action action, bool doneOnException)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
        
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        
        if (_done)
        {
            if (_exception != null)
            {
                throw _exception;
            }
            
            return;
        }

        _semaphore.Wait();
        try
        {
            if (_done)
            {
                if (_exception != null)
                {
                    throw _exception;
                }
            
                return;
            }

            action();
            _done = true;
        }
        catch (Exception ex)
        {
            if (doneOnException)
            {
                _exception = ex;
                _done = true;
            }
        
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

/// <summary>
/// Once is a synchronization primitive that can be used to run a piece of code only once.
/// </summary>
/// <typeparam name="T">The type of the result of the action.</typeparam>
public sealed class Once<T> : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private volatile bool _done;
    private T? _result;
    private Exception? _exception;
    private volatile bool _disposed;
    private readonly object _disposeLock = new();

    ~Once()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        lock (_disposeLock)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                _semaphore.Dispose();
            }
        }
    }

    /// <summary>
    /// DoAsync runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <returns>The result of the action.</returns>
    public Task<T?> DoAsync(Func<Task<T?>> action) => DoAsync(action, false);
    
    /// <summary>
    /// DoAsync runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="doneOnException">If true, the action is considered done even if it throws an exception.</param>
    /// <returns>The result of the action.</returns>
    public async Task<T?> DoAsync(Func<Task<T?>> action, bool doneOnException)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
        
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        
        if (_done)
        {
            if (_exception != null)
            {
                throw _exception;
            }
            
            return _result;
        }

        await _semaphore.WaitAsync();
        try
        {
            if (_done)
            {
                if (_exception != null)
                {
                    throw _exception;
                }
            
                return _result;
            }

            _result = await action();
            _done = true;

            return _result;
        }
        catch (Exception ex)
        {
            if (doneOnException)
            {
                _exception = ex; 
                _done = true;
            }

            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Do runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <returns>The result of the action.</returns>
    public T? Do(Func<T?> action) => Do(action, false);
    
    /// <summary>
    /// Do runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="doneOnException">If true, the action is considered done even if it throws an exception.</param>
    /// <returns>The result of the action.</returns>
    public T? Do(Func<T?> action, bool doneOnException)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
        
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        
        if (_done)
        {
            if (_exception != null)
            {
                throw _exception;
            }
            
            return _result;
        }

        _semaphore.Wait();
        try
        {
            if (_done)
            {
                if (_exception != null)
                {
                    throw _exception;
                }
                
                return _result;
            }

            _result = action();
            _done = true;

            return _result;
        }
        catch (Exception ex)
        {
            if (doneOnException)
            {
                _exception = ex;
                _done = true;
            }
        
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}