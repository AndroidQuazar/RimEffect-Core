using RimWorld;
using RimWorld.BaseGen;
using System.Linq;
using Verse;

namespace RimEffect.GenSteps
{
    public class GenStep_RE_HostagesSite : GenStep_Scatterer
    {
        public override int SeedPart
        {
            get
            {
                return 69356999;
            }
        }

        protected override bool CanScatterAt(IntVec3 c, Map map)
        {
            if (!base.CanScatterAt(c, map))
            {
                return false;
            }
            if (!c.SupportsStructureType(map, TerrainAffordanceDefOf.Heavy))
            {
                return false;
            }
            if (!map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false)))
            {
                return false;
            }
            foreach (IntVec3 c2 in CellRect.CenteredOn(c, 8, 8))
            {
                if (!c2.InBounds(map) || c2.GetEdifice(map) != null)
                {
                    return false;
                }
            }
            return true;
        }

        protected override void ScatterAt(IntVec3 loc, Map map, GenStepParams parms, int count = 1)
        {
            Faction faction;
            if (map.ParentFaction == null || map.ParentFaction == Faction.OfPlayer)
            {
                faction = Find.FactionManager.RandomEnemyFaction(false, false, true, TechLevel.Undefined);
            }
            else
            {
                faction = map.ParentFaction;
            }

            /** Spawn bandit camp **/
            int num = (Rand.Bool ? 2 : 4);
            ResolveParams rp = default(ResolveParams);
            rp.rect = CellRect.CenteredOn(map.Center, 30, 30);
            rp.faction = faction;
            rp.edgeDefenseTurretsCount = Rand.RangeInclusive(0, 1);
            rp.edgeDefenseMortarsCount = 0;

            ResolveParams resolveParams6 = rp;
            resolveParams6.floorDef = TerrainDefOf.Bridge;
            resolveParams6.floorOnlyIfTerrainSupports = true;
            resolveParams6.allowBridgeOnAnyImpassableTerrain = true;
            BaseGen.symbolStack.Push("floor", resolveParams6, null);
            BaseGen.globalSettings.map = map;
            BaseGen.Generate();

            ResolveParams resolveParams5 = rp;
            resolveParams5.rect = rp.rect.ContractedBy(num);
            resolveParams5.faction = faction;
            resolveParams5.floorOnlyIfTerrainSupports = true;
            BaseGen.symbolStack.Push("basePart_outdoors", resolveParams5, null);
            BaseGen.globalSettings.map = map;
            BaseGen.globalSettings.minBuildings = 3;
            BaseGen.globalSettings.minBarracks = 2;
            BaseGen.Generate();

            ResolveParams resolveParams4 = rp;
            resolveParams4.rect = rp.rect.ContractedBy(num);
            resolveParams4.faction = faction;
            BaseGen.symbolStack.Push("ensureCanReachMapEdge", resolveParams4, null);
            BaseGen.globalSettings.map = map;
            BaseGen.Generate();

            ResolveParams resolveParams3 = rp;
            resolveParams3.faction = faction;
            resolveParams3.edgeDefenseWidth = num;
            resolveParams3.edgeThingMustReachMapEdge = false;
            BaseGen.symbolStack.Push("edgeDefense", resolveParams3, null);
            BaseGen.globalSettings.map = map;
            BaseGen.Generate();

            if (faction.def.techLevel >= TechLevel.Industrial)
            {
                int num3 = Rand.Chance(0.75f) ? GenMath.RoundRandom((float)rp.rect.Area / 400f) : 0;
                for (int i = 0; i < num3; i++)
                {
                    ResolveParams resolveParams2 = rp;
                    resolveParams2.faction = faction;
                    BaseGen.symbolStack.Push("firefoamPopper", resolveParams2, null);
                    BaseGen.globalSettings.map = map;
                    BaseGen.Generate();
                }
            }

            BaseGen.symbolStack.Push("outdoorLighting", rp, null);
            BaseGen.globalSettings.map = map;
            BaseGen.Generate();

            if (faction != null && faction == Faction.Empire)
            {
                BaseGen.globalSettings.minThroneRooms = 1;
                BaseGen.globalSettings.minLandingPads = 1;
            }
            if (faction != null && faction == Faction.Empire && BaseGen.globalSettings.landingPadsGenerated == 0)
            {
                CellRect item;
                GenStep_Settlement.GenerateLandingPadNearby(rp.rect, map, faction, out item);
            }

            /** Spawn cell block **/
            foreach (IntVec3 item in rp.rect.Cells.InRandomOrder())
            {
                if (item.GetRoom(map) is Room room && room != null && room.ContainedBeds.EnumerableCount() > 0)
                {
                    IntVec3 spwWait = room.Cells.ToList().FindAll(c => c.Standable(map) && c.GetDoor(map) == null).RandomElement();
                    for (int i = 0; i < Rand.RangeInclusive(2, 8); i++)
                    {
                        Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Slave, map.ParentFaction, PawnGenerationContext.NonPlayer, map.Tile, false, false, false, false, false, false));
                        GenSpawn.Spawn(pawn, spwWait, map, WipeMode.VanishOrMoveAside);
                        pawn.health.AddHediff(RE_DefOf.RE_TurnBackToFormerFaction);
                        pawn.guest.SetGuestStatus(map.ParentFaction, true);
                    }
                    foreach (Building_Bed bed in room.ContainedBeds)
                    {
                        bed.ForPrisoners = true;
                    }
                    MapGenerator.rootsToUnfog.Add(room.Cells.ToList().Find(c => c.Standable(map) && c.GetDoor(map) == null));
                    break;
                }
            }
        }
    }
}