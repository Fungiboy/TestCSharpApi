namespace Dyndata;

// Arr: Join method
public partial class Arr
{
    public string Join(string glue = ",")
    {
        Utils.SetInvariantCulture();
        var result = Reduce(
            (a, c, i) => a + c + (i == Length - 1 ? "" : glue),
            ""
        );
        Utils.SetOriginalCulture();
        return result;
    }
}