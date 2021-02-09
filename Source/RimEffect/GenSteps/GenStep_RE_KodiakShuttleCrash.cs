using RimWorld;
using System.Linq;
using Verse;

namespace RimEffect.GenSteps
{
    public class GenStep_RE_KodiakShuttleCrash : GenStep
    {
        public override int SeedPart => 69513569;

        public override void Generate(Map map, GenStepParams parms)
        {
            IntVec3 nearCenterWalkable = IntVec3.Invalid;

            int radiusSearch = 2;
            while (nearCenterWalkable == IntVec3.Invalid)
            {
                CellFinder.TryFindRandomCellNear(map.Center, map, radiusSearch, c => c.Walkable(map) && !map.fogGrid.IsFogged(c), out nearCenterWalkable);
                radiusSearch++;
            }

            Thing beacon = ThingMaker.MakeThing(RE_DefOf.RE_CrashedKodiakShuttle);
            beacon.SetFactionDirect(map.ParentFaction);
            GenSpawn.Spawn(beacon, nearCenterWalkable, map);

            for (int i = 0; i < Rand.RangeInclusive(5, 15); i++)
            {
                IntVec3 sPos;
                CellFinder.TryFindRandomCellNear(map.Center, map, 15, c => c.Walkable(map) && !map.fogGrid.IsFogged(c), out sPos);

                Thing thingToSpawn = ThingMaker.MakeThing(ThingDefOf.ChunkSlagSteel);

                GenSpawn.Spawn(thingToSpawn, sPos, map);
            }

            for (int i = 0; i < 40; i++)
            {
                this.StartRandomFire(map, nearCenterWalkable);
            }
        }

        private void StartRandomFire(Map map, IntVec3 position)
        {
            FireUtility.TryStartFireIn((from x in GenRadial.RadialCellsAround(position, 10f, true)
                                        where x.InBounds(map)
                                        select x).RandomElement(), map, Rand.Range(0.1f, 0.925f));
        }
    }
}