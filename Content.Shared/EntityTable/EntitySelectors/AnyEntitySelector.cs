using System.Linq;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.EntityTable.EntitySelectors;

public sealed partial class AnyEntitySelector : EntityTableSelector
{
    private List<EntProtoId>? _cachedPrototypes;

    private List<EntProtoId> GetValidPrototypes(IPrototypeManager protoMan)
    {
        if (_cachedPrototypes != null)
            return _cachedPrototypes;

        _cachedPrototypes = protoMan.EnumeratePrototypes<EntityPrototype>()
            .Where(p => !p.Abstract)
            .Select(p => (EntProtoId) p.ID)
            .ToList();

        return _cachedPrototypes;
    }

    protected override IEnumerable<EntProtoId> GetSpawnsImplementation(
        IRobustRandom rand,
        IEntityManager entMan,
        IPrototypeManager protoMan,
        EntityTableContext ctx)
    {
        var allPrototypes = GetValidPrototypes(protoMan);
        if (allPrototypes.Count == 0)
            yield break;

        // Base class already handles the 'rolls' loop, we only need to pick 1 per roll
        yield return rand.Pick(allPrototypes);
    }

    protected override IEnumerable<(EntProtoId spawn, double)> ListSpawnsImplementation(
        IEntityManager entMan,
        IPrototypeManager protoMan,
        EntityTableContext ctx)
    {
        var allPrototypes = GetValidPrototypes(protoMan);
        return allPrototypes.Select(p => (p, 1.0));
    }

    protected override IEnumerable<(EntProtoId spawn, double)> AverageSpawnsImplementation(
        IEntityManager entMan,
        IPrototypeManager protoMan,
        EntityTableContext ctx)
    {
        var allPrototypes = GetValidPrototypes(protoMan);
        var avgPerItem = allPrototypes.Count > 0 ? 1.0 / allPrototypes.Count : 0.0;

        return allPrototypes.Select(p => (p, avgPerItem));
    }
}
