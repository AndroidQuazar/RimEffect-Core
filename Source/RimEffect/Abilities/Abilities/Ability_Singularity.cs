namespace RimEffect
{
    using System.Collections.Generic;
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class Ability_Singularity : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);

            Singularity singularity = (Singularity) GenSpawn.Spawn(ThingDef.Named("RE_Biotic_Singularity"), target.Cell, this.pawn.Map);
            singularity.caster = this.pawn;
        }
    }

    public class Singularity : Thing
    {
        public Pawn caster;

        [Unsaved]
        public List<Pawn> tmpPawns = new List<Pawn>();

        public override void Tick()
        {
            base.Tick();

            foreach (Pawn pawn in this.tmpPawns)
                if (!pawn.DestroyedOrNull() && pawn.Spawned) 
                    pawn.Position = Vector3.Lerp(pawn.Position.ToVector3(), pawn.Position.ToVector3(), 0.25f).ToIntVec3();


            if (this.IsHashIntervalTick(GenTicks.TickRareInterval))
            {
                this.tmpPawns.Clear();

                foreach (IntVec3 intVec3 in this.OccupiedRect())
                {
                    if (!intVec3.InBounds(Find.CurrentMap)) 
                        continue;
                    List<Thing> cellThings = Find.CurrentMap.thingGrid.ThingsListAt(intVec3);
                    if (cellThings == null) 
                        continue;
                    for (int i = 0; i < cellThings.Count; i++)
                    {
                        Thing t = cellThings[i];

                        if (t is Pawn p && p.HostileTo(this.caster)) 
                            this.tmpPawns.Add(p);
                    }
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref this.caster, nameof(caster));
        }
    }
}