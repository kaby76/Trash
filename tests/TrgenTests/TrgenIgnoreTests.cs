using Xunit;

namespace TrgenTests;

public class TrgenIgnoreTests
{
    [Fact]
    public void FindsClosestIgnoreFileInAncestorDirectory()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var parent = Path.Combine(root, "parent");
        var child = Path.Combine(parent, "child");
        Directory.CreateDirectory(child);
        var rootIgnore = Path.Combine(root, ".trgen-ignore");
        var parentIgnore = Path.Combine(parent, ".trgen-ignore");

        try
        {
            File.WriteAllText(rootIgnore, "root-pattern");
            File.WriteAllText(parentIgnore, "parent-pattern");

            var actual = Trash.Test.FindIgnoreFile(".trgen-ignore", child);

            Assert.Equal(Path.GetFullPath(parentIgnore), actual);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ReturnsNullWhenNoAncestorContainsIgnoreFile()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        try
        {
            Assert.Null(Trash.Test.FindIgnoreFile("missing.trgen-ignore", root));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
