namespace MatrixWeb.TankerKoenig;
internal struct MinMax : IEquatable<MinMax> {
    public static bool operator ==(MinMax left, MinMax right) => left.Equals(right);
    public static bool operator !=(MinMax left, MinMax right) => !(left == right);

    public MinMax() { }

    public DateOnly Date { get; init; }
    public double Min { get; set; }
    public double Max { get; set; }

    public readonly override bool Equals(object? obj) => obj is MinMax max && Min == max.Min && Max == max.Max;
    public readonly bool Equals(MinMax other) => Date.Equals(other.Date) && Min == other.Min && Max == other.Max;
    public readonly override int GetHashCode() => HashCode.Combine(Date, Min, Max);
}