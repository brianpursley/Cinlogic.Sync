using System.Runtime.ExceptionServices;

namespace Cinlogic.Sync;

/// <summary>
/// Once is a synchronization primitive that can be used to run a piece of code only once.
/// </summary>
public sealed class Once
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private volatile bool _done;
    private ExceptionDispatchInfo? _exception;

    /// <summary>
    /// DoAsync runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    public Task DoAsync(Func<Task> action, CancellationToken cancellationToken = default) => DoAsync(WrapAction(action), false, cancellationToken);

    /// <summary>
    /// DoAsync runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="doneOnException">If true, the action is considered done even if it throws an exception.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    public Task DoAsync(Func<Task> action, bool doneOnException, CancellationToken cancellationToken = default) => DoAsync(WrapAction(action), doneOnException, cancellationToken);

    /// <summary>
    /// DoAsync runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    public Task DoAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default) => DoAsync(action, false, cancellationToken);

    /// <summary>
    /// DoAsync runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="doneOnException">If true, the action is considered done even if it throws an exception.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    public async Task DoAsync(Func<CancellationToken, Task> action, bool doneOnException, CancellationToken cancellationToken = default)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(action);
#else
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
#endif

        if (_done)
        {
            _exception?.Throw();
            return;
        }

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_done)
            {
                _exception?.Throw();
                return;
            }

            await action(cancellationToken).ConfigureAwait(false);
            _done = true;
        }
        catch (Exception ex)
        {
            if (doneOnException)
            {
                _exception = ExceptionDispatchInfo.Capture(ex);
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
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(action);
#else
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
#endif

        if (_done)
        {
            _exception?.Throw();
            return;
        }

        _semaphore.Wait();
        try
        {
            if (_done)
            {
                _exception?.Throw();
                return;
            }

            action();
            _done = true;
        }
        catch (Exception ex)
        {
            if (doneOnException)
            {
                _exception = ExceptionDispatchInfo.Capture(ex);
                _done = true;
            }

            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static Func<CancellationToken, Task> WrapAction(Func<Task> action)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(action);
#else
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
#endif
        return _ => action();
    }
}

/// <summary>
/// Once is a synchronization primitive that can be used to run a piece of code only once.
/// </summary>
/// <typeparam name="T">The type of the result of the action.</typeparam>
public sealed class Once<T>
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private volatile bool _done;
    private T? _result;
    private ExceptionDispatchInfo? _exception;

    /// <summary>
    /// DoAsync runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>The result of the action.</returns>
    public Task<T?> DoAsync(Func<Task<T?>> action, CancellationToken cancellationToken = default) => DoAsync(WrapAction(action), false, cancellationToken);

    /// <summary>
    /// DoAsync runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="doneOnException">If true, the action is considered done even if it throws an exception.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>The result of the action.</returns>
    public Task<T?> DoAsync(Func<Task<T?>> action, bool doneOnException, CancellationToken cancellationToken = default) => DoAsync(WrapAction(action), doneOnException, cancellationToken);

    /// <summary>
    /// DoAsync runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>The result of the action.</returns>
    public Task<T?> DoAsync(Func<CancellationToken, Task<T?>> action, CancellationToken cancellationToken = default) => DoAsync(action, false, cancellationToken);

    /// <summary>
    /// DoAsync runs the given action only once.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="doneOnException">If true, the action is considered done even if it throws an exception.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>The result of the action.</returns>
    public async Task<T?> DoAsync(Func<CancellationToken, Task<T?>> action, bool doneOnException, CancellationToken cancellationToken = default)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(action);
#else
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
#endif

        if (_done)
        {
            _exception?.Throw();
            return _result;
        }

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_done)
            {
                _exception?.Throw();
                return _result;
            }

            _result = await action(cancellationToken).ConfigureAwait(false);
            _done = true;

            return _result;
        }
        catch (Exception ex)
        {
            if (doneOnException)
            {
                _exception = ExceptionDispatchInfo.Capture(ex);
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
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(action);
#else
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
#endif

        if (_done)
        {
            _exception?.Throw();
            return _result;
        }

        _semaphore.Wait();
        try
        {
            if (_done)
            {
                _exception?.Throw();
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
                _exception = ExceptionDispatchInfo.Capture(ex);
                _done = true;
            }

            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static Func<CancellationToken, Task<T?>> WrapAction(Func<Task<T?>> action)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(action);
#else
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }
#endif
        return _ => action();
    }
}