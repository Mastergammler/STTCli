public abstract record Result<T>;
public record Success<T>(T value) : Result<T>;
public record Failure<T>(string Error) : Result<T>;

public static class Source
{
    public static Result<T> Of<T>(T value) => new Success<T>(value);
}

public static class ResultExtensions
{
    public static Result<TCur> Map<TPrev, TCur>(this Result<TPrev> result, Func<TPrev, TCur> func)
    {
        return result switch
        {
            Success<TPrev> s => new Success<TCur>(func(s.value)),
            Failure<TPrev> f => new Failure<TCur>(f.Error),
            _ => throw new NotImplementedException($"Handling not implemented for type: {result.GetType().Name}")
        };
    }

    public static Result<TCur> Map<TPrev, TCur>(this Result<TPrev> result, Func<TPrev, Result<TCur>> func)
    {
        return result switch
        {
            Success<TPrev> s => func(s.value),
            Failure<TPrev> f => new Failure<TCur>(f.Error),
            _ => throw new NotImplementedException($"Handling not implemented for type: {result.GetType().Name}")
        };
    }

    public static Result<TOut> IfOne<TIn, TOut>(this Result<IEnumerable<TIn>> result,
                                                Func<TIn, TOut> ifOne,
                                                Func<IEnumerable<TIn>, TOut> otherwise)
    {
        switch (result)
        {
            case Success<IEnumerable<TIn>> s:
                if (s.value.Count() == 1) return new Success<TOut>(ifOne(s.value.First()));
                else return new Success<TOut>(otherwise(s.value));
            case Failure<TIn> f:
                return new Failure<TOut>(f.Error);
            default:
                throw new NotImplementedException($"Handling not implemented for type: {result.GetType().Name}");
        }
    }

    public static Result<TOut> OneOrDefault<TIn, TOut>(this Result<IEnumerable<TIn>> result,
                                                       Func<TIn, TOut> ifOne,
                                                       TOut defaultValue)
    {
        switch (result)
        {
            case Success<IEnumerable<TIn>> s:
                if (s.value.Count() == 1) return new Success<TOut>(ifOne(s.value.First()));
                else return new Success<TOut>(defaultValue);
            case Failure<TIn> f:
                return new Failure<TOut>(f.Error);
            default:
                throw new NotImplementedException($"Handling not implemented for type: {result.GetType().Name}");
        }
    }

    public static Result<TOut> OneOrDefault<TIn, TOut>(this Result<IEnumerable<TIn>> result,
                                                       Func<TIn, Result<TOut>> ifOne,
                                                       TOut defaultValue)
    {
        switch (result)
        {
            case Success<IEnumerable<TIn>> s:
                if (s.value.Count() == 1) return ifOne(s.value.First());
                else return new Success<TOut>(defaultValue);
            case Failure<TIn> f:
                return new Failure<TOut>(f.Error);
            default:
                throw new NotImplementedException($"Handling not implemented for type: {result.GetType().Name}");
        }
    }


    public static Result<TOut> Conditional<TIn, TOut>(this Result<TIn> result,
                                                      Func<TIn, bool> condition,
                                                      Func<TIn, TOut> onTrue,
                                                      Func<TIn, TOut> onFalse)
    {
        switch (result)
        {
            case Success<TIn> s:
                if (condition(s.value)) return new Success<TOut>(onTrue(s.value));
                else return new Success<TOut>(onFalse(s.value));
            case Failure<TIn> f:
                return new Failure<TOut>(f.Error);
            default:
                throw new NotImplementedException($"Handling not implemented for type: {result.GetType().Name}");
        }
    }

    public static Result<T> Unwrap<T>(this Result<Result<T>> result)
    {
        return result switch
        {
            Success<Result<T>> s => s.value,
            Failure<Result<T>> f => new Failure<T>(f.Error),
            _ => throw new NotImplementedException($"Handling not implemented for type: {result.GetType().Name}")
        };
    }

    public static Result<TComb> Combine<T1, T2, TComb>(this Result<T1> r1,
                                                       Result<T2> r2,
                                                       Func<T1, T2, TComb> combinator)
    {
        string? e1 = null;
        string? e2 = null;

        if (r1 is Failure<T1> f1) e1 = f1.Error;
        if (r2 is Failure<T2> f2) e2 = f2.Error;

        string errorMsg;

        if (e1 is not null && e2 is not null) errorMsg = $"{e1}NL{e2}";
        else if (e1 is not null) errorMsg = e1;
        else if (e2 is not null) errorMsg = e2;
        else
        {
            if (r1 is Success<T1> s1 && r2 is Success<T2> s2)
                return new Success<TComb>(combinator(s1.value, s2.value));
            else
                throw new NotImplementedException($"Handling for either type {r1.GetType()} or {r2.GetType()} is not implemented!");
        }

        return new Failure<TComb>(errorMsg);
    }

    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> condition, Func<T, string> error)
    {
        return result switch
        {
            Success<T> s when !condition(s.value) => new Failure<T>(error(s.value)),
            _ => result
        };
    }

    public static Result<T> MapOrDefault<T>(this Result<T?> result, T defaultValue) where T : struct
    {
        switch (result)
        {
            case Success<T?> s:
                if (s.value.HasValue) return new Success<T>(s.value.Value);
                else return new Success<T>(defaultValue);
            case Failure<T?> f: return new Failure<T>(f.Error);
            default: throw new NotImplementedException($"Handling not implemented for type: {result.GetType().Name}");
        }
    }

    public static Result<TResult> MapNotNull<TSource, TResult>(this Result<TSource?> valueResult,
                                                               Func<TSource, TResult?> convert,
                                                               Func<TSource, string> error)
        where TResult : struct
        where TSource : struct
    {
        switch (valueResult)
        {
            case Success<TSource> s:
                var targetResult = convert(s.value);
                if (targetResult != null) return new Success<TResult>(targetResult.Value);
                return new Failure<TResult>(error(s.value));
            case Failure<TSource> f: return new Failure<TResult>(f.Error);
            default: throw new NotImplementedException($"Handling not implemented for type: {valueResult.GetType().Name}");
        }
    }




    //NOTE: Because the way nullables work in c# we need the same function twice effectively
    // once for classes and once for structs, because we access the value differently
    public static Result<TResult> MapNotNull<TSource, TResult>(this Result<TSource> valueResult,
                                                               Func<TSource, TResult?> convert,
                                                               Func<TSource, string> error) where TResult : struct
    {
        switch (valueResult)
        {
            case Success<TSource> s:
                var targetResult = convert(s.value);
                if (targetResult != null) return new Success<TResult>(targetResult.Value);
                return new Failure<TResult>(error(s.value));
            case Failure<TSource> f: return new Failure<TResult>(f.Error);
            default: throw new NotImplementedException($"Handling not implemented for type: {valueResult.GetType().Name}");
        }
    }

    public static Result<TResult> MapNotNull<TSource, TResult>(this Result<TSource> valueResult,
                                                               Func<TSource, TResult?> convert,
                                                               Func<TSource, string> error) where TResult : class
    {
        switch (valueResult)
        {
            case Success<TSource> s:
                var targetResult = convert(s.value);
                if (targetResult != null) return new Success<TResult>(targetResult);
                return new Failure<TResult>(error(s.value));
            case Failure<TSource> f: return new Failure<TResult>(f.Error);
            default: throw new NotImplementedException($"Handling not implemented for type: {valueResult.GetType().Name}");
        }
    }



    public static void Execute<T>(this Result<T> result, Action<T> action)
    {
        switch (result)
        {
            case Success<T> s:
                action(s.value);
                break;
            case Failure<T> f:
                Repl.Print(f.Error);
                break;
            default:
                throw new NotImplementedException($"Handling not implemented for type: {result.GetType().Name}");
        }
    }
}