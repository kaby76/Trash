using System;
using System.Collections.Generic;

namespace Algorithms;

public struct SumType<T1, T2> : ISumType, IEquatable<SumType<T1, T2>>
{
    public SumType(T1 val)
    {
        this.Value = (object)val;
    }

    public SumType(T2 val)
    {
        this.Value = (object)val;
    }

    public object Value { get; }

    public override bool Equals(object obj)
    {
        return obj is SumType<T1, T2> other && this.Equals(other);
    }

    public bool Equals(SumType<T1, T2> other)
    {
        return EqualityComparer<object>.Default.Equals(this.Value, other.Value);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<object>.Default.GetHashCode(this.Value) - 1937169414;
    }

    public TResult Match<TResult>(Func<T1, TResult> firstMatch, Func<T2, TResult> secondMatch,
        Func<TResult> defaultMatch = null)
    {
        if (firstMatch == null)
            throw new ArgumentNullException(nameof(firstMatch));
        if (secondMatch == null)
            throw new ArgumentNullException(nameof(secondMatch));
        if (this.Value is T1 obj1)
            return firstMatch(obj1);
        if (this.Value is T2 obj2)
            return secondMatch(obj2);
        return defaultMatch != null ? defaultMatch() : default(TResult);
    }

    public static bool operator ==(SumType<T1, T2> left, SumType<T1, T2> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SumType<T1, T2> left, SumType<T1, T2> right)
    {
        return !(left == right);
    }

    public static implicit operator SumType<T1, T2>(T1 val)
    {
        return new SumType<T1, T2>(val);
    }

    public static implicit operator SumType<T1, T2>(T2 val)
    {
        return new SumType<T1, T2>(val);
    }

    public static explicit operator T1(SumType<T1, T2> sum)
    {
        if (sum.Value is T1 obj1)
            return obj1;
        throw new InvalidCastException();
    }

    public static explicit operator T2(SumType<T1, T2> sum)
    {
        if (sum.Value is T2 obj2)
            return obj2;
        throw new InvalidCastException();
    }
}

public struct SumType<T1, T2, T3> : ISumType, IEquatable<SumType<T1, T2, T3>>
{
    public SumType(T1 val)
    {
        this.Value = (object)val;
    }

    public SumType(T2 val)
    {
        this.Value = (object)val;
    }

    public SumType(T3 val)
    {
        this.Value = (object)val;
    }

    public object Value { get; }

    public override bool Equals(object obj)
    {
        return obj is SumType<T1, T2, T3> other && this.Equals(other);
    }

    public bool Equals(SumType<T1, T2, T3> other)
    {
        return EqualityComparer<object>.Default.Equals(this.Value, other.Value);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<object>.Default.GetHashCode(this.Value) - 1937169414;
    }

    public TResult Match<TResult>(Func<T1, TResult> firstMatch, Func<T2, TResult> secondMatch,
        Func<T3, TResult> thirdMatch, Func<TResult> defaultMatch = null)
    {
        if (firstMatch == null)
            throw new ArgumentNullException(nameof(firstMatch));
        if (secondMatch == null)
            throw new ArgumentNullException(nameof(secondMatch));
        if (thirdMatch == null)
            throw new ArgumentNullException(nameof(thirdMatch));
        if (this.Value is T1 obj1)
            return firstMatch(obj1);
        if (this.Value is T2 obj2)
            return secondMatch(obj2);
        if (this.Value is T3 obj3)
            return thirdMatch(obj3);
        return defaultMatch != null ? defaultMatch() : default(TResult);
    }

    public static bool operator ==(SumType<T1, T2, T3> left, SumType<T1, T2, T3> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SumType<T1, T2, T3> left, SumType<T1, T2, T3> right)
    {
        return !(left == right);
    }

    public static implicit operator SumType<T1, T2, T3>(T1 val)
    {
        return new SumType<T1, T2, T3>(val);
    }

    public static implicit operator SumType<T1, T2, T3>(T2 val)
    {
        return new SumType<T1, T2, T3>(val);
    }

    public static implicit operator SumType<T1, T2, T3>(T3 val)
    {
        return new SumType<T1, T2, T3>(val);
    }

    public static implicit operator SumType<T1, T2, T3>(SumType<T1, T2> sum)
    {
        throw new NotImplementedException();
    }

    public static explicit operator SumType<T1, T2>(SumType<T1, T2, T3> sum)
    {
        if (sum.Value is T1 obj1)
            return (SumType<T1, T2>)obj1;
        if (sum.Value is T2 obj2)
            return (SumType<T1, T2>)obj2;
        throw new InvalidCastException();
    }

    public static explicit operator T1(SumType<T1, T2, T3> sum)
    {
        if (sum.Value is T1 obj)
            return obj;
        throw new InvalidCastException();
    }

    public static explicit operator T2(SumType<T1, T2, T3> sum)
    {
        if (sum.Value is T2 obj)
            return obj;
        throw new InvalidCastException();
    }

    public static explicit operator T3(SumType<T1, T2, T3> sum)
    {
        if (sum.Value is T3 obj)
            return obj;
        throw new InvalidCastException();
    }
}

public struct SumType<T1, T2, T3, T4> : ISumType, IEquatable<SumType<T1, T2, T3, T4>>
{
    public SumType(T1 val)
    {
        this.Value = (object)val;
    }

    public SumType(T2 val)
    {
        this.Value = (object)val;
    }

    public SumType(T3 val)
    {
        this.Value = (object)val;
    }

    public SumType(T4 val)
    {
        this.Value = (object)val;
    }

    public object Value { get; }

    public override bool Equals(object obj)
    {
        return obj is SumType<T1, T2, T3, T4> other && this.Equals(other);
    }

    public bool Equals(SumType<T1, T2, T3, T4> other)
    {
        return EqualityComparer<object>.Default.Equals(this.Value, other.Value);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<object>.Default.GetHashCode(this.Value) - 1937169414;
    }

    public TResult Match<TResult>(Func<T1, TResult> firstMatch, Func<T2, TResult> secondMatch,
        Func<T3, TResult> thirdMatch, Func<T4, TResult> fourthMatch, Func<TResult> defaultMatch = null)
    {
        if (firstMatch == null)
            throw new ArgumentNullException(nameof(firstMatch));
        if (secondMatch == null)
            throw new ArgumentNullException(nameof(secondMatch));
        if (thirdMatch == null)
            throw new ArgumentNullException(nameof(thirdMatch));
        if (fourthMatch == null)
            throw new ArgumentNullException(nameof(fourthMatch));
        if (this.Value is T1 obj1)
            return firstMatch(obj1);
        if (this.Value is T2 obj2)
            return secondMatch(obj2);
        if (this.Value is T3 obj3)
            return thirdMatch(obj3);
        if (this.Value is T4 obj4)
            return fourthMatch(obj4);
        return defaultMatch != null ? defaultMatch() : default(TResult);
    }

    public static bool operator ==(SumType<T1, T2, T3, T4> left, SumType<T1, T2, T3, T4> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SumType<T1, T2, T3, T4> left, SumType<T1, T2, T3, T4> right)
    {
        return !(left == right);
    }

    public static implicit operator SumType<T1, T2, T3, T4>(T1 val)
    {
        return new SumType<T1, T2, T3, T4>(val);
    }

    public static implicit operator SumType<T1, T2, T3, T4>(T2 val)
    {
        return new SumType<T1, T2, T3, T4>(val);
    }

    public static implicit operator SumType<T1, T2, T3, T4>(T3 val)
    {
        return new SumType<T1, T2, T3, T4>(val);
    }

    public static implicit operator SumType<T1, T2, T3, T4>(SumType<T1, T2> sum)
    {
        throw new NotImplementedException();
    }

    public static implicit operator SumType<T1, T2, T3, T4>(SumType<T1, T2, T3> sum)
    {
        throw new NotImplementedException();
    }

    public static explicit operator SumType<T1, T2>(SumType<T1, T2, T3, T4> sum)
    {
        if (sum.Value is T1 obj1)
            return (SumType<T1, T2>)obj1;
        if (sum.Value is T2 obj2)
            return (SumType<T1, T2>)obj2;
        throw new InvalidCastException();
    }

    public static explicit operator SumType<T1, T2, T3>(SumType<T1, T2, T3, T4> sum)
    {
        if (sum.Value is T1 obj1)
            return (SumType<T1, T2, T3>)obj1;
        if (sum.Value is T2 obj2)
            return (SumType<T1, T2, T3>)obj2;
        if (sum.Value is T3 obj3)
            return (SumType<T1, T2, T3>)obj3;
        throw new InvalidCastException();
    }

    public static explicit operator T1(SumType<T1, T2, T3, T4> sum)
    {
        if (sum.Value is T1 obj)
            return obj;
        throw new InvalidCastException();
    }

    public static explicit operator T2(SumType<T1, T2, T3, T4> sum)
    {
        if (sum.Value is T2 obj)
            return obj;
        throw new InvalidCastException();
    }

    public static explicit operator T3(SumType<T1, T2, T3, T4> sum)
    {
        if (sum.Value is T3 obj)
            return obj;
        throw new InvalidCastException();
    }

    public static explicit operator T4(SumType<T1, T2, T3, T4> sum)
    {
        if (sum.Value is T4 obj)
            return obj;
        throw new InvalidCastException();
    }
}
