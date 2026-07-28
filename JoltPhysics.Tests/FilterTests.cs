using JoltPhysics;

namespace JoltPhysics.Test;

[TestFixture]
public class FilterTests : JoltTestBase
{
    private class RejectAllBroadPhaseFilter : BroadPhaseLayerFilter
    {
        protected override bool ShouldCollide(BroadPhaseLayer layer) => false;
    }

    private class RejectAllObjectLayerFilter : ObjectLayerFilter
    {
        protected override bool ShouldCollide(ObjectLayer layer) => false;
    }

    private class RejectAllBodyFilter : BodyFilter
    {
        protected override bool ShouldCollide(BodyID bodyID) => false;
        protected override bool ShouldCollideLocked(BodyID bodyID) => false;
    }

    [Test]
    public void BroadPhaseLayerFilter_RejectAll_CastRayMisses()
    {
        var boxId = CreateStaticBox(Float3.Zero, new Float3(5f, 5f, 5f));
        Step();

        var npq = PhysicsSystem.GetNarrowPhaseQuery();
        using var filter = new RejectAllBroadPhaseFilter();

        bool hit = npq.CastRay(
            new Float3(0, 20f, 0), new Float3(0, -40f, 0),
            out _,
            broadPhaseLayerFilter: filter);

        Assert.That(hit, Is.False, "RejectAll filter should cause miss");

        BodyInterface.RemoveAndDestroyBody(boxId);
    }

    [Test]
    public void ObjectLayerFilter_RejectAll_CastRayMisses()
    {
        var boxId = CreateStaticBox(Float3.Zero, new Float3(5f, 5f, 5f));
        Step();

        var npq = PhysicsSystem.GetNarrowPhaseQuery();
        using var filter = new RejectAllObjectLayerFilter();

        bool hit = npq.CastRay(
            new Float3(0, 20f, 0), new Float3(0, -40f, 0),
            out _,
            objectLayerFilter: filter);

        Assert.That(hit, Is.False, "RejectAll filter should cause miss");

        BodyInterface.RemoveAndDestroyBody(boxId);
    }

    [Test]
    public void BodyFilter_RejectAll_CastRayMisses()
    {
        var boxId = CreateStaticBox(Float3.Zero, new Float3(5f, 5f, 5f));
        Step();

        var npq = PhysicsSystem.GetNarrowPhaseQuery();
        using var filter = new RejectAllBodyFilter();

        bool hit = npq.CastRay(
            new Float3(0, 20f, 0), new Float3(0, -40f, 0),
            out _,
            bodyFilter: filter);

        Assert.That(hit, Is.False, "RejectAll filter should cause miss");

        BodyInterface.RemoveAndDestroyBody(boxId);
    }

    [Test]
    public void Filter_NullFilters_CastRayHits()
    {
        var boxId = CreateStaticBox(Float3.Zero, new Float3(5f, 5f, 5f));
        Step();

        var npq = PhysicsSystem.GetNarrowPhaseQuery();

        bool hit = npq.CastRay(
            new Float3(0, 20f, 0), new Float3(0, -40f, 0),
            out _);

        Assert.That(hit, Is.True, "Null filters should not block ray");

        BodyInterface.RemoveAndDestroyBody(boxId);
    }
}
