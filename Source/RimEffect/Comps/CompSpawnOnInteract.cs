using Verse;
using RimWorld;

namespace RimEffect
{
    public class CompProperties_SpawnOnInteract : CompProperties
    {
        public CompProperties_SpawnOnInteract()
        {
            this.compClass = typeof(CompSpawnOnInteract);
        }

        public ThingDef thingToSpawn;

        public IntRange numToSpawnRange = new IntRange(1, 2);
    }

    public class CompSpawnOnInteract : ThingComp
    {
        public CompProperties_SpawnOnInteract Props
        {
            get
            {
                return (CompProperties_SpawnOnInteract)this.props;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.ShouldSpawn, "shouldSpawn");
            Scribe_Values.Look<bool>(ref this.alreadyUsed, "alreadyUsed");
        }

        public bool ShouldSpawn = false;
        public bool alreadyUsed = false;

        public override void CompTickLong()
        {
            if (!alreadyUsed) this.SpawnItems();
            base.CompTickLong();
        }

        public void SpawnItems()
        {
            MoteMaker.MakeStaticMote(this.parent.TrueCenter(), this.parent.Map, ThingDefOf.Mote_PsycastAreaEffect, 20);
            for (int i = 0; i <= this.Props.numToSpawnRange.RandomInRange; i++)
            {
                IntVec3 pos;
                CellFinder.TryFindRandomCellNear(this.parent.TrueCenter().ToIntVec3(), this.parent.Map, 4, c => c.Walkable(this.parent.Map) && !this.parent.Map.fogGrid.IsFogged(c), out pos);
                GenSpawn.Spawn(this.Props.thingToSpawn, pos, this.parent.Map, WipeMode.VanishOrMoveAside);
            }
            this.alreadyUsed = true;
        }
    }
}