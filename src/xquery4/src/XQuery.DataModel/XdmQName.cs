namespace XQuery.DataModel;

/// <summary>
/// Represents a qualified name (QName) in the XDM 4.0 data model.
/// A QName consists of a namespace URI, local name, and optional prefix.
/// </summary>
public sealed class XdmQName : IEquatable<XdmQName>
{
    public static readonly string XsNamespace = "http://www.w3.org/2001/XMLSchema";
    public static readonly string FnNamespace = "http://www.w3.org/2005/xpath-functions";
    public static readonly string XmlNamespace = "http://www.w3.org/XML/1998/namespace";
    public static readonly string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";
    public static readonly string LocalNamespace = "http://www.w3.org/2005/xquery-local-functions";
    public static readonly string MapNamespace = "http://www.w3.org/2005/xpath-functions/map";
    public static readonly string ArrayNamespace = "http://www.w3.org/2005/xpath-functions/array";
    public static readonly string MathNamespace = "http://www.w3.org/2005/xpath-functions/math";

    /// <summary>
    /// The namespace URI, or empty string if no namespace.
    /// </summary>
    public string NamespaceUri { get; }

    /// <summary>
    /// The local name part of the QName.
    /// </summary>
    public string LocalName { get; }

    /// <summary>
    /// The prefix, or empty string if no prefix.
    /// </summary>
    public string Prefix { get; }

    public XdmQName(string localName) : this(string.Empty, localName, string.Empty)
    {
    }

    public XdmQName(string namespaceUri, string localName) : this(namespaceUri, localName, string.Empty)
    {
    }

    public XdmQName(string namespaceUri, string localName, string prefix)
    {
        NamespaceUri = namespaceUri ?? string.Empty;
        LocalName = localName ?? throw new ArgumentNullException(nameof(localName));
        Prefix = prefix ?? string.Empty;
    }

    /// <summary>
    /// Returns true if this QName has a namespace.
    /// </summary>
    public bool HasNamespace => !string.IsNullOrEmpty(NamespaceUri);

    /// <summary>
    /// Returns true if this QName has a prefix.
    /// </summary>
    public bool HasPrefix => !string.IsNullOrEmpty(Prefix);

    /// <summary>
    /// Gets the Clark notation representation {namespace}localname.
    /// </summary>
    public string ClarkNotation =>
        HasNamespace ? $"{{{NamespaceUri}}}{LocalName}" : LocalName;

    /// <summary>
    /// Gets the EQName representation Q{namespace}localname.
    /// </summary>
    public string EQName =>
        HasNamespace ? $"Q{{{NamespaceUri}}}{LocalName}" : LocalName;

    /// <summary>
    /// Gets the prefixed representation prefix:localname or just localname.
    /// </summary>
    public string PrefixedName =>
        HasPrefix ? $"{Prefix}:{LocalName}" : LocalName;

    public bool Equals(XdmQName? other)
    {
        if (other is null) return false;
        return NamespaceUri == other.NamespaceUri && LocalName == other.LocalName;
    }

    public override bool Equals(object? obj) => obj is XdmQName qname && Equals(qname);

    public override int GetHashCode() => HashCode.Combine(NamespaceUri, LocalName);

    public override string ToString() => HasPrefix ? PrefixedName : ClarkNotation;

    public static bool operator ==(XdmQName? left, XdmQName? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(XdmQName? left, XdmQName? right) => !(left == right);

    /// <summary>
    /// Creates a QName in the XML Schema namespace.
    /// </summary>
    public static XdmQName Xs(string localName) => new(XsNamespace, localName, "xs");

    /// <summary>
    /// Creates a QName in the XPath functions namespace.
    /// </summary>
    public static XdmQName Fn(string localName) => new(FnNamespace, localName, "fn");

    // Common XSD type names
    public static readonly XdmQName XsString = Xs("string");
    public static readonly XdmQName XsBoolean = Xs("boolean");
    public static readonly XdmQName XsDecimal = Xs("decimal");
    public static readonly XdmQName XsInteger = Xs("integer");
    public static readonly XdmQName XsDouble = Xs("double");
    public static readonly XdmQName XsFloat = Xs("float");
    public static readonly XdmQName XsDate = Xs("date");
    public static readonly XdmQName XsDateTime = Xs("dateTime");
    public static readonly XdmQName XsTime = Xs("time");
    public static readonly XdmQName XsDuration = Xs("duration");
    public static readonly XdmQName XsYearMonthDuration = Xs("yearMonthDuration");
    public static readonly XdmQName XsDayTimeDuration = Xs("dayTimeDuration");
    public static readonly XdmQName XsAnyURI = Xs("anyURI");
    public static readonly XdmQName XsQName = Xs("QName");
    public static readonly XdmQName XsNCName = Xs("NCName");
    public static readonly XdmQName XsUntypedAtomic = Xs("untypedAtomic");
    public static readonly XdmQName XsAnyAtomicType = Xs("anyAtomicType");
    public static readonly XdmQName XsHexBinary = Xs("hexBinary");
    public static readonly XdmQName XsBase64Binary = Xs("base64Binary");
    public static readonly XdmQName XsNormalizedString = Xs("normalizedString");
    public static readonly XdmQName XsToken = Xs("token");
    public static readonly XdmQName XsLanguage = Xs("language");
    public static readonly XdmQName XsNMTOKEN = Xs("NMTOKEN");
    public static readonly XdmQName XsName = Xs("Name");
    public static readonly XdmQName XsID = Xs("ID");
    public static readonly XdmQName XsIDREF = Xs("IDREF");
    public static readonly XdmQName XsENTITY = Xs("ENTITY");
    public static readonly XdmQName XsLong = Xs("long");
    public static readonly XdmQName XsInt = Xs("int");
    public static readonly XdmQName XsShort = Xs("short");
    public static readonly XdmQName XsByte = Xs("byte");
    public static readonly XdmQName XsNonNegativeInteger = Xs("nonNegativeInteger");
    public static readonly XdmQName XsPositiveInteger = Xs("positiveInteger");
    public static readonly XdmQName XsUnsignedLong = Xs("unsignedLong");
    public static readonly XdmQName XsUnsignedInt = Xs("unsignedInt");
    public static readonly XdmQName XsUnsignedShort = Xs("unsignedShort");
    public static readonly XdmQName XsUnsignedByte = Xs("unsignedByte");
    public static readonly XdmQName XsNonPositiveInteger = Xs("nonPositiveInteger");
    public static readonly XdmQName XsNegativeInteger = Xs("negativeInteger");
    public static readonly XdmQName XsGYear = Xs("gYear");
    public static readonly XdmQName XsGYearMonth = Xs("gYearMonth");
    public static readonly XdmQName XsGMonth = Xs("gMonth");
    public static readonly XdmQName XsGMonthDay = Xs("gMonthDay");
    public static readonly XdmQName XsGDay = Xs("gDay");
    public static readonly XdmQName XsUntyped = Xs("untyped");
}
