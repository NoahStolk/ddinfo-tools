#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable S1104 // Fields should not have public accessibility
#pragma warning disable S2328 // "GetHashCode" should not reference mutable fields

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DevilDaggersInfo.App.Engine.Maths.Numerics;

public struct Vector2i<T> : IEquatable<Vector2i<T>>, IFormattable
	where T : IBinaryInteger<T>, IMinMaxValue<T>
{
	public T X;

	public T Y;

	public Vector2i(T value)
		: this(value, value)
	{
	}

	public Vector2i(T x, T y)
	{
		X = x;
		Y = y;
	}

	public static Vector2i<T> Zero => default;

	public static Vector2i<T> One => new(T.One);

	public static Vector2i<T> UnitX => new(T.One, T.Zero);

	public static Vector2i<T> UnitY => new(T.Zero, T.One);

	#region Equality

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Vector2i<T> left, Vector2i<T> right)
		=> left.X == right.X && left.Y == right.Y;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Vector2i<T> left, Vector2i<T> right)
		=> !(left == right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
		=> obj is Vector2i<T> other && Equals(other);

	public readonly bool Equals(Vector2i<T> other)
		=> this == other;

	public override readonly int GetHashCode()
		=> HashCode.Combine(X, Y);

	#endregion Equality

	#region Addition

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i<T> operator +(Vector2i<T> left, Vector2i<T> right)
		=> new(left.X + right.X, left.Y + right.Y);

	#endregion Addition

	#region Subtraction

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i<T> operator -(Vector2i<T> left, Vector2i<T> right)
		=> new(left.X - right.X, left.Y - right.Y);

	#endregion Subtraction

	#region Multiplication

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i<T> operator *(Vector2i<T> left, Vector2i<T> right)
		=> new(left.X * right.X, left.Y * right.Y);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i<T> operator *(Vector2i<T> left, T right)
		=> left * new Vector2i<T>(right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i<T> operator *(T left, Vector2i<T> right)
		=> right * left;

	#endregion Multiplication

	#region Division

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i<T> operator /(Vector2i<T> left, Vector2i<T> right)
		=> new(left.X / right.X, left.Y / right.Y);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i<T> operator /(Vector2i<T> value1, T value2)
		=> value1 / new Vector2i<T>(value2);

	#endregion Division

	#region Negation

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i<T> operator -(Vector2i<T> value)
		=> Zero - value;

	#endregion Negation

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i<T> Max(Vector2i<T> value1, Vector2i<T> value2)
	{
		return new(
			value1.X > value2.X ? value1.X : value2.X,
			value1.Y > value2.Y ? value1.Y : value2.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i<T> Min(Vector2i<T> value1, Vector2i<T> value2)
	{
		return new(
			value1.X < value2.X ? value1.X : value2.X,
			value1.Y < value2.Y ? value1.Y : value2.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i<T> Abs(Vector2i<T> value)
		=> new(T.Abs(value.X), T.Abs(value.Y));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i<T> Clamp(Vector2i<T> value1, Vector2i<T> min, Vector2i<T> max)
		=> Min(Max(value1, min), max);

	public override readonly string ToString()
		=> ToString("G", CultureInfo.CurrentCulture);

	public readonly string ToString(string? format)
		=> ToString(format, CultureInfo.CurrentCulture);

	public readonly string ToString(string? format, IFormatProvider? formatProvider)
	{
		StringBuilder sb = new();
		string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
		sb.Append('<');
		sb.Append(X.ToString(format, formatProvider));
		sb.Append(separator);
		sb.Append(' ');
		sb.Append(Y.ToString(format, formatProvider));
		sb.Append('>');
		return sb.ToString();
	}
}
