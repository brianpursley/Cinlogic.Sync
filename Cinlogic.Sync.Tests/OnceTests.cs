namespace Cinlogic.Sync.Tests;

public class OnceTests
{
    [Fact]
    public void Do_Executes_Only_Once_Even_In_Parallel_Calls()
    {
        var once = new Once();
        var counter = 0;

        Parallel.For(0, 1000, _ => once.Do(() => counter++));

        Assert.Equal(1, counter);
    }

    [Fact]
    public async Task DoAsync_Executes_Only_Once_Even_In_Parallel_Calls()
    {
        var once = new Once();
        var counter = 0;

        var tasks = Enumerable.Range(0, 1000)
            .Select(_ => once.DoAsync(() =>
            {
                Interlocked.Increment(ref counter);
                return Task.CompletedTask;
            }))
            .ToArray();

        await Task.WhenAll(tasks);

        Assert.Equal(1, counter);
    }

        
    [Fact]
    public void Once_Generic_Returns_Result()
    {
        var once = new Once<int>();
        var result = once.Do(() => 10);

        Assert.Equal(10, result);
    }

    [Fact]
    public async Task Once_Generic_Returns_Result_Async()
    {
        var once = new Once<int>();
        var result = await once.DoAsync(() => Task.FromResult(10));

        Assert.Equal(10, result);
    }

    [Fact]
    public void Once_Generic_Returns_Same_Result_On_Subsequent_Calls()
    {
        var once = new Once<int>();
        var result1 = once.Do(() => 10);
        var result2 = once.Do(() => 20);

        Assert.Equal(10, result1);
        Assert.Equal(10, result2);
    }

    [Fact]
    public async Task Once_Generic_Returns_Same_Result_On_Subsequent_Calls_Async()
    {
        var once = new Once<int>();
        var result1 = await once.DoAsync(() => Task.FromResult(10));
        var result2 = await once.DoAsync(() => Task.FromResult(20));

        Assert.Equal(10, result1);
        Assert.Equal(10, result2);
    }
        
        
    [Fact]
    public void Do_Executes_Immediately()
    {
        var once = new Once();
        var counter = 0;
        once.Do(() => counter++);
        Assert.Equal(1, counter);
    }

    [Fact]
    public async Task DoAsync_Executes_Immediately()
    {
        var once = new Once();
        var counter = 0;
        await once.DoAsync(() =>
        {
            Interlocked.Increment(ref counter);
            return Task.CompletedTask;
        });
        Assert.Equal(1, counter);
    }

    [Fact]
    public void Dispose_Throws_On_Subsequent_Do_Calls()
    {
        var once = new Once();
        once.Dispose();
        Assert.Throws<ObjectDisposedException>(() => once.Do(() => {}));
    }

    [Fact]
    public async Task Dispose_Throws_On_Subsequent_DoAsync_Calls()
    {
        var once = new Once();
        once.Dispose();
        await Assert.ThrowsAsync<ObjectDisposedException>(() => once.DoAsync(() => Task.CompletedTask));
    }

    [Fact]
    public void Do_Executes_Once_And_Throws_Exception_Again_If_DoneOnException_True()
    {
        var once = new Once();
        var counter = 0;
        Assert.Throws<ApplicationException>(() => once.Do(() => { counter++; throw new ApplicationException("Something happened"); }, true));
        var ex = Assert.Throws<ApplicationException>(() => once.Do(() => { counter++; }));
        Assert.Equal("Something happened", ex.Message);
        Assert.Equal(1, counter);
    }

    [Fact]
    public async Task DoAsync_Executes_Once_And_Throws_Exception_Again_If_DoneOnException_True()
    {
        var once = new Once();
        var counter = 0;
        await Assert.ThrowsAsync<ApplicationException>(() => once.DoAsync(() => { counter++; throw new ApplicationException("Something happened"); }, true));
        var ex = await Assert.ThrowsAsync<ApplicationException>(() => once.DoAsync(() => { counter++; return Task.CompletedTask; }));
        Assert.Equal("Something happened", ex.Message);
        Assert.Equal(1, counter);
    }

    [Fact]
    public void Do_Reexecutes_If_DoneOnException_False()
    {
        var once = new Once();
        var counter = 0;
        Assert.Throws<Exception>(() => once.Do(() => { counter++; throw new Exception(); }, false));
        once.Do(() => counter++);
        Assert.Equal(2, counter);
    }

    [Fact]
    public async Task DoAsync_Reexecutes_If_DoneOnException_False()
    {
        var once = new Once();
        var counter = 0;
        await Assert.ThrowsAsync<Exception>(() => once.DoAsync(() => { counter++; throw new Exception(); }, false));
        await once.DoAsync(() => { counter++; return Task.CompletedTask; });
        Assert.Equal(2, counter);
    }
    
    [Fact]
    public void Do_Throws_When_Action_Is_Null()
    {
        var once = new Once();
        Assert.Throws<ArgumentNullException>(() => once.Do(null!));
    }

    [Fact]
    public async Task DoAsync_Throws_When_Action_Is_Null()
    {
        var once = new Once();
        await Assert.ThrowsAsync<ArgumentNullException>(() => once.DoAsync(null!));
    }
    
    [Fact]
    public void Do_Returns_Result_After_Exception()
    {
        var once = new Once<int>();
        Assert.Throws<Exception>(() => once.Do(() => throw new Exception()));
        var result = once.Do(() => 10);
        Assert.Equal(10, result);
    }

    [Fact]
    public async Task DoAsync_Returns_Result_After_Exception()
    {
        var once = new Once<int>();
        await Assert.ThrowsAsync<Exception>(() => once.DoAsync(() => throw new Exception()));
        var result = await once.DoAsync(() => Task.FromResult(10));
        Assert.Equal(10, result);
    }
    
    [Fact]
    public void Do_Throws_After_Exception_If_DoneOnException_True()
    {
        var once = new Once<int>();
        Assert.Throws<Exception>(() => once.Do(() => throw new Exception(), true));
        Assert.Throws<Exception>(() => once.Do(() => 10));
    }

    [Fact]
    public async Task DoAsync_Throws_After_Exception_If_DoneOnException_True()
    {
        var once = new Once<int>();
        await Assert.ThrowsAsync<Exception>(() => once.DoAsync(() => throw new Exception(), true));
        await Assert.ThrowsAsync<Exception>(() => once.DoAsync(() => Task.FromResult(10)));
    }
    
    [Fact]
    public async Task Mixed_Do_And_DoAsync_Executes_Only_Once()
    {
        var once = new Once();
        var counter = 0;

        once.Do(() => counter++);
        await once.DoAsync(() =>
        {
            Interlocked.Increment(ref counter);
            return Task.CompletedTask;
        });

        Assert.Equal(1, counter);
    }

    [Fact]
    public async Task Mixed_DoAsync_And_Do_Executes_Only_Once()
    {
        var once = new Once();
        var counter = 0;

        await once.DoAsync(() =>
        {
            Interlocked.Increment(ref counter);
            return Task.CompletedTask;
        });
        once.Do(() => counter++);

        Assert.Equal(1, counter);
    }

    [Fact]
    public async Task Mixed_Generic_Do_And_DoAsync_Returns_Same_Result()
    {
        var once = new Once<int>();

        var result1 = once.Do(() => 10);
        var result2 = await once.DoAsync(() => Task.FromResult(20));

        Assert.Equal(10, result1);
        Assert.Equal(10, result2);
    }

    [Fact]
    public async Task Mixed_Generic_DoAsync_And_Do_Returns_Same_Result()
    {
        var once = new Once<int>();

        var result1 = await once.DoAsync(() => Task.FromResult(10));
        var result2 = once.Do(() => 20);

        Assert.Equal(10, result1);
        Assert.Equal(10, result2);
    }
}