using Verse;
using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using System.Text;

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

        public bool ShouldSpawn = false;
        public bool alreadyUsed = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.ShouldSpawn, "shouldSpawn");
            Scribe_Values.Look<bool>(ref this.alreadyUsed, "alreadyUsed");
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (this.alreadyUsed)
            {
                yield return new FloatMenuOption("RE.OpenBeacon".Translate() + " (" + "RE.AlreadyOpened".Translate() + ")", null);
            }
            else if (selPawn.Faction != this.parent.Faction)
            {
                yield return new FloatMenuOption("RE.OpenBeacon".Translate() + " (" + "RE.WrongFaction".Translate() + ")", null);
            }
            else if (!selPawn.CanReach(this.parent, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                yield return new FloatMenuOption("RE.OpenBeacon".Translate() + " (" + "NoPath".Translate() + ")", null);
            }
            else if (!selPawn.CanReserve(this.parent, 1, -1, null, false))
            {
                yield return new FloatMenuOption("RE.OpenBeacon".Translate() + " (" + "Reserved".Translate() + ")", null);
            }
            else
            {
                FloatMenuOption floatMenuOption = new FloatMenuOption("RE.OpenBeacon".Translate(), delegate ()
                {
                    if (selPawn.CanReserveAndReach(this.parent, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false))
                    {
                        selPawn.jobs.TryTakeOrderedJob(new Job(RE_DefOf.RE_OpenBeacon, this.parent));
                    }
                }, MenuOptionPriority.Default, null, null, 0f, null, null);
                yield return floatMenuOption;
            }
        }

        public override void CompTickLong()
        {
            if (!alreadyUsed && ShouldSpawn) this.SpawnItems();
            base.CompTickLong();
        }

        public void SpawnItems()
        {
            MoteMaker.MakeStaticMote(this.parent.TrueCenter(), this.parent.Map, ThingDefOf.Mote_PsycastAreaEffect, 8);
            for (int i = 0; i <= this.Props.numToSpawnRange.RandomInRange; i++)
            {
                IntVec3 pos;
                CellFinder.TryFindRandomCellNear(this.parent.TrueCenter().ToIntVec3(), this.parent.Map, 4, c => c.Walkable(this.parent.Map) && !this.parent.Map.fogGrid.IsFogged(c), out pos);
                GenSpawn.Spawn(this.Props.thingToSpawn, pos, this.parent.Map, WipeMode.VanishOrMoveAside);
            }
            this.parent.TryGetComp<CompGlower>().ReceiveCompSignal();
            this.alreadyUsed = true;
            this.ShouldSpawn = false;
        }
    }
}