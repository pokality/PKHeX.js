using PKHeX.Core;

class Program
{
    static void Main()
    {
        // Pre-initialize during _start (managed context) rather than
        // from WASI export entry points (unmanaged context). This avoids:
        // 1. Infinite exception recursion in NativeAOT-LLVM
        // 2. Stack overflow from deep static constructor chains
        _ = GameInfo.Strings;

        // Pre-initialize evolution trees so their static constructors
        // don't run during export calls where stack space is limited.
        foreach (var ctx in new[] {
            EntityContext.Gen1, EntityContext.Gen2, EntityContext.Gen3,
            EntityContext.Gen4, EntityContext.Gen5, EntityContext.Gen6,
            EntityContext.Gen7, EntityContext.Gen8, EntityContext.Gen9,
            EntityContext.Gen8a, EntityContext.Gen8b, EntityContext.Gen9a,
        })
        {
            try { _ = EvolutionTree.GetEvolutionTree(ctx); } catch { }
        }
    }
}
