using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimEffect
{
	public class IncidentWorker_LocalBattlefield : IncidentWorker
	{
        Predicate<Faction> baseValidator = delegate (Faction x)
        {
            if (x.hidden ?? false)
            {
                return false;
            }
            if (x.IsPlayer)
            {
                return false;
            }
            if (x.defeated)
            {
                return false;
            }
            if (!x.def.humanlikeFaction)
            {
                return false;
            }
            if (x.def.pawnGroupMakers == null || !x.def.pawnGroupMakers.Where(y => y.kindDef == PawnGroupKindDefOf.Combat).Any())
            {
                return false;
            }
            return true;
        };
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            var friendlyFactions = Find.FactionManager.AllFactions.Where(x => baseValidator(x)
                && x.RelationKindWith(Faction.OfPlayer) != FactionRelationKind.Hostile);
            
            var hostileFactions = Find.FactionManager.AllFactions.Where(x => baseValidator(x)
                && x.RelationKindWith(Faction.OfPlayer) == FactionRelationKind.Hostile);
            if (!friendlyFactions.Any())
            {
                Log.Message("No friendly faction");
                return false;
            }
            if (!hostileFactions.Any())
            {
                Log.Message("No hostile faction");

                return false;
            }
            if (!friendlyFactions.Where(x => hostileFactions.Where(y => x.HostileTo(y)).Any()).Any())
            {
                Log.Message("No hostile and friendly factions");
                return false;
            }
            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            var friendlyFactions = Find.FactionManager.AllFactions.Where(x => baseValidator(x) && x.RelationKindWith(Faction.OfPlayer) != FactionRelationKind.Hostile);
            var hostileFactions = Find.FactionManager.AllFactions.Where(x => baseValidator(x) && x.RelationKindWith(Faction.OfPlayer) == FactionRelationKind.Hostile);

            var friendlyFactionsWithHostiles = friendlyFactions.Where(x => hostileFactions.Where(y => x.HostileTo(y)).Any());
            var friendlyFaction = friendlyFactionsWithHostiles.RandomElement();
            var enemyFaction = hostileFactions.Where(x => x.HostileTo(friendlyFaction)).RandomElement();

            var points = StorytellerUtility.DefaultThreatPointsNow(parms.target) * 5;
            var raidStrategy = RaidStrategyDefOf.ImmediateAttackFriendly;
            var raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            
            var friendlyParms = new IncidentParms();
            friendlyParms.target = parms.target;
            friendlyParms.faction = friendlyFaction;
            friendlyParms.points = points;
            friendlyParms.raidStrategy = raidStrategy;
            friendlyParms.raidArrivalMode = raidArrivalMode;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out friendlyParms.spawnCenter, map, CellFinder.EdgeRoadChance_Friendly))
            {
                return false;
            }
            var enemyParms = new IncidentParms();
            enemyParms.target = parms.target;
            enemyParms.faction = enemyFaction;
            enemyParms.points = points;
            enemyParms.raidStrategy = raidStrategy;
            enemyParms.raidArrivalMode = raidArrivalMode;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out enemyParms.spawnCenter, map, CellFinder.EdgeRoadChance_Hostile, allowFogged: true, 
                (IntVec3 x) => x.DistanceTo(friendlyParms.spawnCenter) > map.Size.x))
            {
                return false;
            }

            var enemies = SpawnRaid(enemyParms, out List<TargetInfo> targetInfosEnemies);
            if (!enemies.Any() || !targetInfosEnemies.Any())
            {
                return false;
            }
            var friendlies = SpawnRaid(friendlyParms, out List<TargetInfo> targetInfosFriendlies);
            if (!friendlies.Any() || !targetInfosFriendlies.Any())
            {
                return false;
            }
            foreach (var enemy in enemies)
            {
                Log.Message(enemy.mindState.duty + " - " + enemy.mindState.enemyTarget);
                //enemy.mindState.duty = new PawnDuty(RE_DefOf.RE_AssaultEnemies, friendlies.RandomElement());
                enemy.mindState.enemyTarget = friendlies.RandomElement();
                var jbg = new JobGiver_AttackOtherHostiles();
                jbg.ResolveReferences();
                jbg.enemies = friendlies;
                var result = jbg.TryIssueJobPackage(enemy, default(JobIssueParams));
                if (result.Job != null)
                {
                    Log.Message(enemy + " - " + result.Job);
                    enemy.jobs.TryTakeOrderedJob(result.Job);
                }
                Log.Message(enemy.mindState.duty + " - " + enemy.mindState.enemyTarget);
            }
            var totalTargets = targetInfosEnemies.ListFullCopy();
            totalTargets.AddRange(targetInfosFriendlies.ListFullCopy());
            Find.LetterStack.ReceiveLetter("RE.LocalBattlefield".Translate(), "RE.LocalBattlefieldDesc".Translate(friendlyFaction.Named("friendlyFACTION"), 
                enemyFaction.Named("enemyFACTION")), LetterDefOf.ThreatBig, totalTargets);
			return true;
        }

        private List<Pawn> SpawnRaid(IncidentParms parms, out List<TargetInfo> targetInfos)
        {
            PawnGroupKindDef combat = PawnGroupKindDefOf.Combat;
            parms.raidStrategy.Worker.TryGenerateThreats(parms);
            List<Pawn> list = parms.raidStrategy.Worker.SpawnThreats(parms);
            if (list == null)
            {
                list = PawnGroupMakerUtility.GeneratePawns(IncidentParmsUtility.GetDefaultPawnGroupMakerParms(combat, parms)).ToList();
                if (list.Count == 0)
                {
                    Log.Error("Got no pawns spawning raid from parms " + parms);
                    targetInfos = null;
                    return list;
                }
                parms.raidArrivalMode.Worker.Arrive(list, parms);
            }

            List<TargetInfo> list2 = new List<TargetInfo>();
            if (parms.pawnGroups != null)
            {
                List<List<Pawn>> list3 = IncidentParmsUtility.SplitIntoGroups(list, parms.pawnGroups);
                List<Pawn> list4 = list3.MaxBy((List<Pawn> x) => x.Count);
                if (list4.Any())
                {
                    list2.Add(list4[0]);
                }
                for (int i = 0; i < list3.Count; i++)
                {
                    if (list3[i] != list4 && list3[i].Any())
                    {
                        list2.Add(list3[i][0]);
                    }
                }
            }
            else if (list.Any())
            {
                foreach (Pawn item in list)
                {
                    list2.Add(item);
                }
            }
            parms.raidStrategy.Worker.MakeLords(parms, list);
            targetInfos = list2;
            return list;
        }
    }
}
