namespace XQuery.DataModel;

/// <summary>
/// The evaluation context for XPath/XQuery expressions.
/// </summary>
public class EvaluationContext
{
    private readonly Dictionary<XdmQName, XdmSequence> _variables = new();
    private readonly Dictionary<(XdmQName name, int arity), XdmFunction> _functions = new();
    private readonly Dictionary<string, string> _namespaces = new();
    private XdmItem? _contextItem;
    private int _contextPosition = 1;
    private int _contextSize = 1;

    /// <summary>
    /// Gets or sets the context item.
    /// </summary>
    public XdmItem? ContextItem
    {
        get => _contextItem;
        set => _contextItem = value;
    }

    /// <summary>
    /// Gets or sets the context position (1-based).
    /// </summary>
    public int ContextPosition
    {
        get => _contextPosition;
        set => _contextPosition = value;
    }

    /// <summary>
    /// Gets or sets the context size.
    /// </summary>
    public int ContextSize
    {
        get => _contextSize;
        set => _contextSize = value;
    }

    /// <summary>
    /// Gets the default element namespace.
    /// </summary>
    public string DefaultElementNamespace { get; set; } = string.Empty;

    /// <summary>
    /// Gets the default function namespace.
    /// </summary>
    public string DefaultFunctionNamespace { get; set; } = XdmQName.FnNamespace;

    /// <summary>
    /// Gets or sets the base URI.
    /// </summary>
    public Uri? BaseUri { get; set; }

    /// <summary>
    /// Gets or sets the default collation.
    /// </summary>
    public string DefaultCollation { get; set; } = "http://www.w3.org/2005/xpath-functions/collation/codepoint";

    /// <summary>
    /// Creates a copy of this context with a new context item.
    /// </summary>
    public EvaluationContext WithContextItem(XdmItem? item, int position = 1, int size = 1)
    {
        var copy = Clone();
        copy._contextItem = item;
        copy._contextPosition = position;
        copy._contextSize = size;
        return copy;
    }

    /// <summary>
    /// Creates a copy of this context with a variable binding.
    /// </summary>
    public EvaluationContext WithVariable(string name, XdmSequence value)
    {
        return WithVariable(new XdmQName(name), value);
    }

    /// <summary>
    /// Creates a copy of this context with a variable binding.
    /// </summary>
    public EvaluationContext WithVariable(XdmQName name, XdmSequence value)
    {
        var copy = Clone();
        copy._variables[name] = value;
        return copy;
    }

    /// <summary>
    /// Sets a variable in this context (mutating).
    /// </summary>
    public void SetVariable(string name, XdmSequence value)
    {
        SetVariable(new XdmQName(name), value);
    }

    /// <summary>
    /// Sets a variable in this context (mutating).
    /// </summary>
    public void SetVariable(XdmQName name, XdmSequence value)
    {
        _variables[name] = value;
    }

    /// <summary>
    /// Gets a variable value.
    /// </summary>
    public XdmSequence GetVariable(XdmQName name)
    {
        if (_variables.TryGetValue(name, out var value))
            return value;

        // Try without namespace
        if (name.HasNamespace)
        {
            var simpleName = new XdmQName(name.LocalName);
            if (_variables.TryGetValue(simpleName, out value))
                return value;
        }

        throw XdmException.UndefinedVariable(name.ToString());
    }

    /// <summary>
    /// Gets a variable value by name.
    /// </summary>
    public XdmSequence GetVariable(string name)
    {
        return GetVariable(new XdmQName(name));
    }

    /// <summary>
    /// Checks if a variable is defined.
    /// </summary>
    public bool HasVariable(XdmQName name)
    {
        if (_variables.ContainsKey(name))
            return true;

        if (name.HasNamespace)
        {
            var simpleName = new XdmQName(name.LocalName);
            return _variables.ContainsKey(simpleName);
        }

        return false;
    }

    /// <summary>
    /// Registers a function.
    /// </summary>
    public void RegisterFunction(XdmQName name, int arity, XdmFunction function)
    {
        _functions[(name, arity)] = function;
    }

    /// <summary>
    /// Registers a function (uses arity from function).
    /// </summary>
    public void RegisterFunction(XdmQName name, XdmFunction function)
    {
        _functions[(name, function.Arity)] = function;
    }

    /// <summary>
    /// Gets a function by name and arity.
    /// </summary>
    public XdmFunction? GetFunction(XdmQName name, int arity)
    {
        if (_functions.TryGetValue((name, arity), out var func))
            return func;

        // Try with default function namespace
        if (!name.HasNamespace)
        {
            var fnName = new XdmQName(DefaultFunctionNamespace, name.LocalName, "fn");
            if (_functions.TryGetValue((fnName, arity), out func))
                return func;
        }

        // Try without namespace (for user-defined functions that may be called with fn: namespace)
        if (name.HasNamespace && name.NamespaceUri == DefaultFunctionNamespace)
        {
            var simpleName = new XdmQName(name.LocalName);
            if (_functions.TryGetValue((simpleName, arity), out func))
                return func;
        }

        return null;
    }

    /// <summary>
    /// Declares a namespace.
    /// </summary>
    public void DeclareNamespace(string prefix, string uri)
    {
        _namespaces[prefix] = uri;
    }

    /// <summary>
    /// Registers a namespace (alias for DeclareNamespace).
    /// </summary>
    public void RegisterNamespace(string prefix, string uri)
    {
        _namespaces[prefix] = uri;
    }

    /// <summary>
    /// Resolves a namespace prefix to a URI.
    /// </summary>
    public string? ResolveNamespace(string prefix)
    {
        if (_namespaces.TryGetValue(prefix, out var uri))
            return uri;

        return prefix switch
        {
            "xs" => XdmQName.XsNamespace,
            "fn" => XdmQName.FnNamespace,
            "map" => XdmQName.MapNamespace,
            "array" => XdmQName.ArrayNamespace,
            "math" => XdmQName.MathNamespace,
            "xml" => XdmQName.XmlNamespace,
            "local" => XdmQName.LocalNamespace,
            _ => null
        };
    }

    /// <summary>
    /// Gets the namespace URI for a prefix.
    /// </summary>
    public string? GetNamespace(string prefix)
    {
        return ResolveNamespace(prefix);
    }

    /// <summary>
    /// Sets the default element namespace.
    /// </summary>
    public void SetDefaultElementNamespace(string uri)
    {
        DefaultElementNamespace = uri;
    }

    /// <summary>
    /// Sets the default function namespace.
    /// </summary>
    public void SetDefaultFunctionNamespace(string uri)
    {
        DefaultFunctionNamespace = uri;
    }

    /// <summary>
    /// Creates a copy of this context.
    /// </summary>
    public EvaluationContext Clone()
    {
        var copy = new EvaluationContext
        {
            _contextItem = _contextItem,
            _contextPosition = _contextPosition,
            _contextSize = _contextSize,
            DefaultElementNamespace = DefaultElementNamespace,
            DefaultFunctionNamespace = DefaultFunctionNamespace,
            BaseUri = BaseUri,
            DefaultCollation = DefaultCollation
        };

        foreach (var kvp in _variables)
            copy._variables[kvp.Key] = kvp.Value;

        foreach (var kvp in _functions)
            copy._functions[kvp.Key] = kvp.Value;

        foreach (var kvp in _namespaces)
            copy._namespaces[kvp.Key] = kvp.Value;

        return copy;
    }

    /// <summary>
    /// Creates a default context with standard namespaces.
    /// </summary>
    public static EvaluationContext CreateDefault()
    {
        var ctx = new EvaluationContext();
        ctx.DeclareNamespace("xs", XdmQName.XsNamespace);
        ctx.DeclareNamespace("fn", XdmQName.FnNamespace);
        ctx.DeclareNamespace("map", XdmQName.MapNamespace);
        ctx.DeclareNamespace("array", XdmQName.ArrayNamespace);
        ctx.DeclareNamespace("math", XdmQName.MathNamespace);
        ctx.DeclareNamespace("xml", XdmQName.XmlNamespace);
        ctx.DeclareNamespace("local", XdmQName.LocalNamespace);
        return ctx;
    }
}
