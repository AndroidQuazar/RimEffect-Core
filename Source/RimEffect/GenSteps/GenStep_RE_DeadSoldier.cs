using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimEffect.GenSteps
{
    class GenStep_RE_DeadSoldier : GenStep
    {
        public override int SeedPart => 369725981;

        public override void Generate(Map map, GenStepParams parms)
        {
            Faction allianceFac = Find.QuestManager.QuestsListForReading.Find(q => q.QuestLookTargets.Any(look => look.HasWorldObject && Find.World.worldObjects.ObjectsAt(map.Tile).Contains(look.WorldObject))).InvolvedFactions.ToList().Find(f => f != map.ParentFaction && f != Faction.OfPlayer);

            for (int i = 0; i < Rand.RangeInclusive(2, 5); i++)
            {
                IntVec3 nearCenterWalkable;
                CellFinder.TryFindRandomCellNear(map.Center, map, 8, c => c.Walkable(map) && !map.fogGrid.IsFogged(c), out nearCenterWalkable);

                Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(RE_DefOf.RE_Alliance_Marine, allianceFac));
                HealthUtility.DamageUntilDead(pawn);

                GenSpawn.Spawn(pawn.Corpse, nearCenterWalkable, map);
            }

            for (int i = 0; i < Rand.RangeInclusive(5, 15); i++)
            {
                IntVec3 nearCenterWalkable;
                CellFinder.TryFindRandomCellNear(map.Center, map, 15, c => c.Walkable(map) && !map.fogGrid.IsFogged(c), out nearCenterWalkable);

                IEnumerable<ThingDef> chooseFrom = new ThingDef[] { ThingDefOf.ComponentIndustrial, ThingDefOf.Steel, ThingDefOf.Plasteel, ThingDefOf.MealSurvivalPack, ThingDefOf.Gold };
                Thing thingToSpawn = ThingMaker.MakeThing(chooseFrom.RandomElement());
                thingToSpawn.stackCount = Mathf.Clamp(Rand.RangeInclusive(1, 20), 1, thingToSpawn.def.stackLimit);
                thingToSpawn.SetForbidden(true);

                GenSpawn.Spawn(thingToSpawn, nearCenterWalkable, map);
            }
        }
    }
}
