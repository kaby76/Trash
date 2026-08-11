namespace XQuery.DataModel;

/// <summary>
/// Exception thrown for XQuery/XPath errors with standard error codes.
/// </summary>
public class XdmException : Exception
{
    /// <summary>
    /// The error code (e.g., "XPTY0004", "FORG0001").
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// The module URI where the error occurred, if available.
    /// </summary>
    public string? ModuleUri { get; }

    /// <summary>
    /// The line number where the error occurred, if available.
    /// </summary>
    public int? LineNumber { get; }

    /// <summary>
    /// The column number where the error occurred, if available.
    /// </summary>
    public int? ColumnNumber { get; }

    public XdmException(string errorCode, string message)
        : base($"[{errorCode}] {message}")
    {
        ErrorCode = errorCode;
    }

    public XdmException(string errorCode, string message, Exception innerException)
        : base($"[{errorCode}] {message}", innerException)
    {
        ErrorCode = errorCode;
    }

    public XdmException(string errorCode, string message, string? moduleUri, int? lineNumber, int? columnNumber)
        : base($"[{errorCode}] {message} at {moduleUri}:{lineNumber}:{columnNumber}")
    {
        ErrorCode = errorCode;
        ModuleUri = moduleUri;
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
    }

    // Common error codes as static methods for convenience
    public static XdmException TypeError(string message) => new("XPTY0004", message);
    public static XdmException DynamicError(string message) => new("XPDY0002", message);
    public static XdmException StaticError(string message) => new("XPST0003", message);
    public static XdmException CardinalityError(string message) => new("XPTY0004", message);
    public static XdmException InvalidCast(string message) => new("FORG0001", message);
    public static XdmException DivisionByZero() => new("FOAR0001", "Division by zero");
    public static XdmException Overflow(string message) => new("FOAR0002", message);
    public static XdmException InvalidRegex(string message) => new("FORX0002", message);
    public static XdmException NoContextItem() => new("XPDY0002", "Context item is absent");
    public static XdmException UndefinedVariable(string name) => new("XPST0008", $"Variable ${name} is not defined");
    public static XdmException UndefinedFunction(string name, int arity) => new("XPST0017", $"Function {name}#{arity} is not defined");
}
