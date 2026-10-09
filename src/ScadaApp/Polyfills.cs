// .NET Framework 4.8 缺少的编译器支撑类型：
// record / init / required / 索引范围（[..]、[^n]）等 C# 语法依赖这些类型，
// 这里按 BCL 等价语义补齐，仅为编译通过，不参与业务逻辑。
using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
        {
            FeatureName = featureName;
        }

        public string FeatureName { get; }
        public bool IsOptional { get; set; }
        public const string RefStructs = nameof(RefStructs);
        public const string RequiredMembers = nameof(RequiredMembers);
    }

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface |
                    AttributeTargets.Delegate | AttributeTargets.Enum, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Constructor, Inherited = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute { }
}

namespace System
{
    public readonly struct Index
    {
        private readonly int _value;

        public Index(int value, bool fromEnd = false)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");

            _value = fromEnd ? ~value : value;
        }

        public static Index FromStart(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            return new Index(value);
        }

        public static Index FromEnd(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            return new Index(value, fromEnd: true);
        }

        public int Value => _value < 0 ? ~_value : _value;
        public bool IsFromEnd => _value < 0;

        public int GetOffset(int length) => _value < 0 ? length + _value : _value;

        public override bool Equals(object value) => value is Index other && _value == other._value;
        public override int GetHashCode() => _value;
        public override string ToString() => IsFromEnd ? "^" + Value.ToString() : Value.ToString();

        public static implicit operator Index(int value) => new Index(value, fromEnd: false);
    }

    public readonly struct Range
    {
        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

        public Index Start { get; }
        public Index End { get; }

        public static Range StartAt(Index start) => new Range(start, Index.FromEnd(0));
        public static Range EndAt(Index end) => new Range(Index.FromStart(0), end);
        public static Range All => new Range(Index.FromStart(0), Index.FromEnd(0));

        public (int Offset, int Length) GetOffsetAndLength(int length)
        {
            var start = Start.GetOffset(length);
            var end = End.GetOffset(length);
            if ((uint)end > (uint)length || (uint)start > (uint)end)
                throw new ArgumentOutOfRangeException(nameof(length));
            return (start, end - start);
        }

        public override bool Equals(object value) => value is Range other && Start.Equals(other.Start) && End.Equals(other.End);
        public override int GetHashCode() => Start.GetHashCode() * 31 + End.GetHashCode();
        public override string ToString() => $"{Start}..{End}";

        public static bool operator ==(Range left, Range right) => left.Equals(right);
        public static bool operator !=(Range left, Range right) => !(left == right);
    }

    /// <summary>BCL HashCode 的最小等价实现，供 record 合成的 GetHashCode 使用。</summary>
    public struct HashCode
    {
        private uint _hash;

        public void Add<T>(T value) => Add(value, null);

        public void Add<T>(T value, IEqualityComparer<T>? comparer)
        {
            var hashCode = value is null ? 0 : comparer?.GetHashCode(value) ?? value.GetHashCode();
            _hash = unchecked(_hash * 16777619 + (uint)hashCode);
        }

        public int ToHashCode() => unchecked((int)_hash);

        public override int GetHashCode() => ToHashCode();

        public override bool Equals(object obj) => obj is HashCode other && ToHashCode() == other.ToHashCode();

        public static bool operator ==(HashCode left, HashCode right) => left.Equals(right);
        public static bool operator !=(HashCode left, HashCode right) => !left.Equals(right);

        public static int Combine<T1>(T1 value1)
        {
            var hash = default(HashCode);
            hash.Add(value1);
            return hash.ToHashCode();
        }

        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            var hash = default(HashCode);
            hash.Add(value1);
            hash.Add(value2);
            return hash.ToHashCode();
        }

        public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            var hash = default(HashCode);
            hash.Add(value1);
            hash.Add(value2);
            hash.Add(value3);
            return hash.ToHashCode();
        }

        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            var hash = default(HashCode);
            hash.Add(value1);
            hash.Add(value2);
            hash.Add(value3);
            hash.Add(value4);
            return hash.ToHashCode();
        }

        public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            var hash = default(HashCode);
            hash.Add(value1);
            hash.Add(value2);
            hash.Add(value3);
            hash.Add(value4);
            hash.Add(value5);
            return hash.ToHashCode();
        }

        public static int Combine<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        {
            var hash = default(HashCode);
            hash.Add(value1);
            hash.Add(value2);
            hash.Add(value3);
            hash.Add(value4);
            hash.Add(value5);
            hash.Add(value6);
            return hash.ToHashCode();
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
        {
            var hash = default(HashCode);
            hash.Add(value1);
            hash.Add(value2);
            hash.Add(value3);
            hash.Add(value4);
            hash.Add(value5);
            hash.Add(value6);
            hash.Add(value7);
            return hash.ToHashCode();
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        {
            var hash = default(HashCode);
            hash.Add(value1);
            hash.Add(value2);
            hash.Add(value3);
            hash.Add(value4);
            hash.Add(value5);
            hash.Add(value6);
            hash.Add(value7);
            hash.Add(value8);
            return hash.ToHashCode();
        }
    }
}
