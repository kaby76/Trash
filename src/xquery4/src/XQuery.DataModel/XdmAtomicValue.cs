using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace XQuery.DataModel;

/// <summary>
/// Represents an atomic value in the XDM 4.0 data model.
/// Atomic values are pairs of a type and a value.
/// </summary>
public class XdmAtomicValue : XdmItem, IComparable<XdmAtomicValue>
{
    private readonly object _value;
    private readonly XdmQName _typeName;

    public override bool IsAtomicValue => true;

    public override XdmQName TypeName => _typeName;

    /// <summary>
    /// Gets the underlying CLR value.
    /// </summary>
    public object Value => _value;

    #region Constructors

    public XdmAtomicValue(string value)
    {
        _value = value ?? string.Empty;
        _typeName = XdmQName.XsString;
    }

    public XdmAtomicValue(bool value)
    {
        _value = value;
        _typeName = XdmQName.XsBoolean;
    }

    public XdmAtomicValue(long value)
    {
        _value = value;
        _typeName = XdmQName.XsInteger;
    }

    public XdmAtomicValue(int value)
    {
        _value = (long)value;
        _typeName = XdmQName.XsInteger;
    }

    public XdmAtomicValue(decimal value)
    {
        _value = value;
        _typeName = XdmQName.XsDecimal;
    }

    public XdmAtomicValue(double value)
    {
        _value = value;
        _typeName = XdmQName.XsDouble;
    }

    public XdmAtomicValue(float value)
    {
        _value = value;
        _typeName = XdmQName.XsFloat;
    }

    public XdmAtomicValue(DateTime value)
    {
        _value = value;
        _typeName = XdmQName.XsDateTime;
    }

    public XdmAtomicValue(DateOnly value)
    {
        _value = value;
        _typeName = XdmQName.XsDate;
    }

    public XdmAtomicValue(TimeOnly value)
    {
        _value = value;
        _typeName = XdmQName.XsTime;
    }

    public XdmAtomicValue(TimeSpan value, bool isDayTimeDuration = true)
    {
        _value = value;
        _typeName = isDayTimeDuration ? XdmQName.XsDayTimeDuration : XdmQName.XsDuration;
    }

    public XdmAtomicValue(Uri value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
        _typeName = XdmQName.XsAnyURI;
    }

    public XdmAtomicValue(XdmQName value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
        _typeName = XdmQName.XsQName;
    }

    public XdmAtomicValue(byte[] value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
        _typeName = XdmQName.XsBase64Binary;
    }

    public XdmAtomicValue(object value, XdmQName typeName)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
        _typeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
    }

    #endregion

    #region Type Accessors

    public bool IsString => _typeName == XdmQName.XsString || _typeName == XdmQName.XsUntypedAtomic;
    public bool IsBoolean => _typeName == XdmQName.XsBoolean;
    public bool IsInteger => _typeName == XdmQName.XsInteger || IsIntegerSubtype;
    public bool IsDecimal => _typeName == XdmQName.XsDecimal || IsInteger;
    public bool IsDouble => _typeName == XdmQName.XsDouble;
    public bool IsFloat => _typeName == XdmQName.XsFloat;
    public bool IsNumeric => IsInteger || IsDecimal || IsDouble || IsFloat;
    public bool IsDateTime => _typeName == XdmQName.XsDateTime;
    public bool IsDate => _typeName == XdmQName.XsDate;
    public bool IsTime => _typeName == XdmQName.XsTime;
    public bool IsDuration => _typeName == XdmQName.XsDuration ||
                              _typeName == XdmQName.XsDayTimeDuration ||
                              _typeName == XdmQName.XsYearMonthDuration;
    public bool IsUri => _typeName == XdmQName.XsAnyURI;
    public bool IsQName => _typeName == XdmQName.XsQName;
    public bool IsBinary => _typeName == XdmQName.XsBase64Binary || _typeName == XdmQName.XsHexBinary;
    public bool IsUntypedAtomic => _typeName == XdmQName.XsUntypedAtomic;

    private bool IsIntegerSubtype =>
        _typeName == XdmQName.XsLong || _typeName == XdmQName.XsInt ||
        _typeName == XdmQName.XsShort || _typeName == XdmQName.XsByte ||
        _typeName == XdmQName.XsNonNegativeInteger || _typeName == XdmQName.XsPositiveInteger ||
        _typeName == XdmQName.XsNonPositiveInteger || _typeName == XdmQName.XsNegativeInteger ||
        _typeName == XdmQName.XsUnsignedLong || _typeName == XdmQName.XsUnsignedInt ||
        _typeName == XdmQName.XsUnsignedShort || _typeName == XdmQName.XsUnsignedByte;

    #endregion

    #region Value Accessors

    public override string StringValue
    {
        get
        {
            return _value switch
            {
                string s => s,
                bool b => b ? "true" : "false",
                long l => l.ToString(CultureInfo.InvariantCulture),
                decimal d => FormatDecimal(d),
                double dbl => FormatDouble(dbl),
                float f => FormatFloat(f),
                DateTime dt => FormatDateTime(dt),
                DateOnly date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                TimeOnly time => time.ToString("HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture),
                TimeSpan ts => FormatDuration(ts),
                Uri uri => uri.ToString(),
                XdmQName qname => qname.PrefixedName,
                byte[] bytes => Convert.ToBase64String(bytes),
                _ => _value.ToString() ?? string.Empty
            };
        }
    }

    private static string FormatDecimal(decimal d)
    {
        string s = d.ToString("G", CultureInfo.InvariantCulture);
        if (!s.Contains('.'))
            return s;
        return s.TrimEnd('0').TrimEnd('.');
    }

    private static string FormatDouble(double d)
    {
        if (double.IsNaN(d)) return "NaN";
        if (double.IsPositiveInfinity(d)) return "INF";
        if (double.IsNegativeInfinity(d)) return "-INF";
        if (d == 0.0) return d.ToString("0.0E0", CultureInfo.InvariantCulture);

        string s = d.ToString("R", CultureInfo.InvariantCulture);
        if (!s.Contains('.') && !s.Contains('E'))
            s += ".0";
        return s;
    }

    private static string FormatFloat(float f)
    {
        if (float.IsNaN(f)) return "NaN";
        if (float.IsPositiveInfinity(f)) return "INF";
        if (float.IsNegativeInfinity(f)) return "-INF";
        return f.ToString("R", CultureInfo.InvariantCulture);
    }

    private static string FormatDateTime(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Utc)
            return dt.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFZ", CultureInfo.InvariantCulture);
        return dt.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
    }

    private static string FormatDuration(TimeSpan ts)
    {
        var sb = new System.Text.StringBuilder();
        if (ts < TimeSpan.Zero)
        {
            sb.Append('-');
            ts = ts.Negate();
        }
        sb.Append('P');

        if (ts.Days > 0)
            sb.Append($"{ts.Days}D");

        if (ts.Hours > 0 || ts.Minutes > 0 || ts.Seconds > 0 || ts.Milliseconds > 0)
        {
            sb.Append('T');
            if (ts.Hours > 0)
                sb.Append($"{ts.Hours}H");
            if (ts.Minutes > 0)
                sb.Append($"{ts.Minutes}M");
            if (ts.Seconds > 0 || ts.Milliseconds > 0)
            {
                if (ts.Milliseconds > 0)
                    sb.Append($"{ts.Seconds}.{ts.Milliseconds:D3}".TrimEnd('0') + "S");
                else
                    sb.Append($"{ts.Seconds}S");
            }
        }

        if (sb.Length == 1 || (sb.Length == 2 && sb[0] == '-'))
            sb.Append("T0S");

        return sb.ToString();
    }

    public bool AsBoolean()
    {
        if (_value is bool b) return b;
        throw XdmException.TypeError($"Cannot convert {_typeName} to xs:boolean");
    }

    public long AsInteger()
    {
        return _value switch
        {
            long l => l,
            int i => i,
            decimal d => (long)d,
            double dbl => (long)dbl,
            float f => (long)f,
            _ => throw XdmException.TypeError($"Cannot convert {_typeName} to xs:integer")
        };
    }

    public decimal AsDecimal()
    {
        return _value switch
        {
            decimal d => d,
            long l => l,
            int i => i,
            double dbl => (decimal)dbl,
            float f => (decimal)f,
            _ => throw XdmException.TypeError($"Cannot convert {_typeName} to xs:decimal")
        };
    }

    public double AsDouble()
    {
        return _value switch
        {
            double d => d,
            float f => f,
            decimal dec => (double)dec,
            long l => l,
            int i => i,
            _ => throw XdmException.TypeError($"Cannot convert {_typeName} to xs:double")
        };
    }

    public DateTime AsDateTime()
    {
        if (_value is DateTime dt) return dt;
        throw XdmException.TypeError($"Cannot convert {_typeName} to xs:dateTime");
    }

    public DateOnly AsDate()
    {
        return _value switch
        {
            DateOnly d => d,
            DateTime dt => DateOnly.FromDateTime(dt),
            _ => throw XdmException.TypeError($"Cannot convert {_typeName} to xs:date")
        };
    }

    public TimeSpan AsDuration()
    {
        if (_value is TimeSpan ts) return ts;
        throw XdmException.TypeError($"Cannot convert {_typeName} to duration");
    }

    public Uri AsUri()
    {
        if (_value is Uri uri) return uri;
        if (_value is string s) return new Uri(s, UriKind.RelativeOrAbsolute);
        throw XdmException.TypeError($"Cannot convert {_typeName} to xs:anyURI");
    }

    public XdmQName AsQName()
    {
        if (_value is XdmQName qname) return qname;
        throw XdmException.TypeError($"Cannot convert {_typeName} to xs:QName");
    }

    #endregion

    public override XdmSequence TypedValue => new XdmSequence(this);

    public override bool EffectiveBooleanValue
    {
        get
        {
            if (IsBoolean)
                return (bool)_value;
            if (IsString || IsUri || IsUntypedAtomic)
                return !string.IsNullOrEmpty(StringValue);
            if (IsNumeric)
            {
                var d = AsDouble();
                return d != 0 && !double.IsNaN(d);
            }
            throw new XdmException("FORG0006", $"Cannot compute effective boolean value for {_typeName}");
        }
    }

    public override bool Equals(XdmItem? other)
    {
        if (other is not XdmAtomicValue av) return false;
        return ValueEquals(av);
    }

    public bool ValueEquals(XdmAtomicValue other)
    {
        if (other is null) return false;

        // NaN special case
        if (IsDouble && other.IsDouble)
        {
            var d1 = AsDouble();
            var d2 = other.AsDouble();
            if (double.IsNaN(d1) && double.IsNaN(d2)) return true;
            if (double.IsNaN(d1) || double.IsNaN(d2)) return false;
        }

        // Numeric comparison
        if (IsNumeric && other.IsNumeric)
        {
            if (IsDouble || other.IsDouble || IsFloat || other.IsFloat)
                return AsDouble() == other.AsDouble();
            if (IsDecimal || other.IsDecimal)
                return AsDecimal() == other.AsDecimal();
            return AsInteger() == other.AsInteger();
        }

        // String comparison
        if (IsString && other.IsString)
            return StringValue == other.StringValue;

        // Boolean comparison
        if (IsBoolean && other.IsBoolean)
            return AsBoolean() == other.AsBoolean();

        // Date/time comparison
        if (IsDateTime && other.IsDateTime)
            return AsDateTime() == other.AsDateTime();

        if (IsDate && other.IsDate)
            return AsDate() == other.AsDate();

        // Duration comparison
        if (IsDuration && other.IsDuration)
            return AsDuration() == other.AsDuration();

        // QName comparison
        if (IsQName && other.IsQName)
            return AsQName().Equals(other.AsQName());

        // URI comparison (as strings)
        if (IsUri && other.IsUri)
            return StringValue == other.StringValue;

        // Untyped atomic - promote to string
        if (IsUntypedAtomic || other.IsUntypedAtomic)
            return StringValue == other.StringValue;

        return false;
    }

    public int CompareTo(XdmAtomicValue? other)
    {
        if (other is null) return 1;

        // Numeric comparison
        if (IsNumeric && other.IsNumeric)
        {
            if (IsDouble || other.IsDouble || IsFloat || other.IsFloat)
                return AsDouble().CompareTo(other.AsDouble());
            if (IsDecimal || other.IsDecimal)
                return AsDecimal().CompareTo(other.AsDecimal());
            return AsInteger().CompareTo(other.AsInteger());
        }

        // String comparison
        if (IsString && other.IsString)
            return string.Compare(StringValue, other.StringValue, StringComparison.Ordinal);

        // Boolean comparison
        if (IsBoolean && other.IsBoolean)
            return AsBoolean().CompareTo(other.AsBoolean());

        // Date/time comparison
        if (IsDateTime && other.IsDateTime)
            return AsDateTime().CompareTo(other.AsDateTime());

        if (IsDate && other.IsDate)
            return AsDate().CompareTo(other.AsDate());

        // Duration comparison
        if (IsDuration && other.IsDuration)
            return AsDuration().CompareTo(other.AsDuration());

        // URI comparison (as strings)
        if (IsUri && other.IsUri)
            return string.Compare(StringValue, other.StringValue, StringComparison.Ordinal);

        // Untyped atomic - compare as strings
        if (IsUntypedAtomic || other.IsUntypedAtomic)
            return string.Compare(StringValue, other.StringValue, StringComparison.Ordinal);

        throw new XdmException("XPTY0004", $"Cannot compare {_typeName} with {other._typeName}");
    }

    public override int GetHashCode()
    {
        if (IsNumeric)
            return AsDouble().GetHashCode();
        return _value.GetHashCode();
    }

    public override string ToString() => StringValue;

    #region Casting

    /// <summary>
    /// Casts this atomic value to the specified type.
    /// </summary>
    public XdmAtomicValue CastAs(XdmQName targetType)
    {
        if (_typeName == targetType)
            return this;

        // String to various types
        if (targetType == XdmQName.XsString)
            return new XdmAtomicValue(StringValue);

        if (targetType == XdmQName.XsUntypedAtomic)
            return new XdmAtomicValue(StringValue, XdmQName.XsUntypedAtomic);

        if (targetType == XdmQName.XsBoolean)
            return new XdmAtomicValue(ParseBoolean(StringValue));

        if (targetType == XdmQName.XsInteger)
            return new XdmAtomicValue(ParseInteger(StringValue));

        if (targetType == XdmQName.XsDecimal)
            return new XdmAtomicValue(ParseDecimal(StringValue));

        if (targetType == XdmQName.XsDouble)
            return new XdmAtomicValue(ParseDouble(StringValue));

        if (targetType == XdmQName.XsFloat)
            return new XdmAtomicValue(ParseFloat(StringValue));

        if (targetType == XdmQName.XsDateTime)
            return new XdmAtomicValue(ParseDateTime(StringValue));

        if (targetType == XdmQName.XsDate)
            return new XdmAtomicValue(ParseDate(StringValue));

        if (targetType == XdmQName.XsTime)
            return new XdmAtomicValue(ParseTime(StringValue));

        if (targetType == XdmQName.XsAnyURI)
            return new XdmAtomicValue(new Uri(StringValue, UriKind.RelativeOrAbsolute));

        throw XdmException.InvalidCast($"Cannot cast {_typeName} to {targetType}");
    }

    private static bool ParseBoolean(string s)
    {
        s = s.Trim();
        return s switch
        {
            "true" or "1" => true,
            "false" or "0" => false,
            _ => throw XdmException.InvalidCast($"Invalid boolean value: {s}")
        };
    }

    private static long ParseInteger(string s)
    {
        s = s.Trim();
        if (long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            return result;
        throw XdmException.InvalidCast($"Invalid integer value: {s}");
    }

    private static decimal ParseDecimal(string s)
    {
        s = s.Trim();
        if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
            return result;
        throw XdmException.InvalidCast($"Invalid decimal value: {s}");
    }

    private static double ParseDouble(string s)
    {
        s = s.Trim();
        if (s == "INF") return double.PositiveInfinity;
        if (s == "-INF") return double.NegativeInfinity;
        if (s == "NaN") return double.NaN;
        if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            return result;
        throw XdmException.InvalidCast($"Invalid double value: {s}");
    }

    private static float ParseFloat(string s)
    {
        s = s.Trim();
        if (s == "INF") return float.PositiveInfinity;
        if (s == "-INF") return float.NegativeInfinity;
        if (s == "NaN") return float.NaN;
        if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            return result;
        throw XdmException.InvalidCast($"Invalid float value: {s}");
    }

    private static DateTime ParseDateTime(string s)
    {
        s = s.Trim();
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result))
            return result;
        throw XdmException.InvalidCast($"Invalid dateTime value: {s}");
    }

    private static DateOnly ParseDate(string s)
    {
        s = s.Trim();
        if (DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            return result;
        throw XdmException.InvalidCast($"Invalid date value: {s}");
    }

    private static TimeOnly ParseTime(string s)
    {
        s = s.Trim();
        if (TimeOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            return result;
        throw XdmException.InvalidCast($"Invalid time value: {s}");
    }

    #endregion

    #region Static Factory Methods

    public static XdmAtomicValue FromString(string value) => new(value);
    public static XdmAtomicValue FromBoolean(bool value) => new(value);
    public static XdmAtomicValue FromInteger(long value) => new(value);
    public static XdmAtomicValue FromDecimal(decimal value) => new(value);
    public static XdmAtomicValue FromDouble(double value) => new(value);
    public static XdmAtomicValue FromFloat(float value) => new(value);
    public static XdmAtomicValue FromDateTime(DateTime value) => new(value);
    public static XdmAtomicValue FromDate(DateOnly value) => new(value);
    public static XdmAtomicValue FromUri(string uri) => new(new Uri(uri, UriKind.RelativeOrAbsolute));

    public static XdmAtomicValue UntypedAtomic(string value) =>
        new(value, XdmQName.XsUntypedAtomic);

    /// <summary>
    /// The xs:boolean true value.
    /// </summary>
    public static readonly XdmAtomicValue True = new(true);

    /// <summary>
    /// The xs:boolean false value.
    /// </summary>
    public static readonly XdmAtomicValue False = new(false);

    #endregion
}
