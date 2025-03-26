using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trash;

class Reformat
{
    public Reformat()
    {
    }

    void reformat(UnvParseTreeElement node)
    {
        var type = node.GetChildren("RULE_REF")
            .First()
            .GetChildrenText()
            .First();
    }
}
