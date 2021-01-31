using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimEffect
{
    public class GenStep_Beacon : GenStep
    {
        public override int SeedPart => 398632941;

        public override void Generate(Map map, GenStepParams parms)
        {
            IntVec3 nearCenterWalkable = IntVec3.Invalid;

            int radiusSearch = 2;
            while (nearCenterWalkable == IntVec3.Invalid)
            {
                CellFinder.TryFindRandomCellNear(map.Center, map, radiusSearch, c => c.Walkable(map) && !map.fogGrid.IsFogged(c), out nearCenterWalkable);
                radiusSearch++;
            }

            Thing beacon = ThingMaker.MakeThing(RE_DefOf.RE_ProtheanBeacon);
            beacon.SetFactionDirect(map.ParentFaction);
            GenSpawn.Spawn(beacon, nearCenterWalkable, map);
        }
    }
}
