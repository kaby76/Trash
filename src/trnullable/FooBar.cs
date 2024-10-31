using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trash.trnullable;

public static class FooBar
{
    public static List<UnvParseTreeElement> GetChildren(this UnvParseTreeElement element, string name)
    {
        return element.Children
            .Where(c => c.GetType() == typeof(UnvParseTreeElement))
            .Select(c => c as UnvParseTreeElement)
            .Where(c => c.LocalName == name)
            .ToList();
    }

    public static string GetName(this UnvParseTreeElement element)
    {
        return element.LocalName;
    }
}

