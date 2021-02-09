using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimEffect
{
    public class JobDriver_FreeHostages : JobDriver
    {
        public Pawn OtherPawn
        {
            get
            {
                return (Pawn)this.pawn.CurJob.GetTarget(TargetIndex.A).Thing;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOn(() => !this.OtherPawn.health.hediffSet.HasHediff(RE_DefOf.RE_TurnBackToFormerFaction));

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil finalize = new Toil();
            finalize.initAction = delegate ()
            {
                IntVec3 leavePoint;
                CellFinder.TryFindRandomPawnExitCell(OtherPawn, out leavePoint);
                // Turn back to former faction and villager + exit the map
                Faction fac = Find.QuestManager.QuestsListForReading.Find(q => q.QuestLookTargets.Any(look => look.HasWorldObject && Find.World.worldObjects.ObjectsAt(pawn.Map.Tile).Contains(look.WorldObject))).InvolvedFactions.ToList().Find(f => f != pawn.Map.ParentFaction && f != Faction.OfPlayer);
                if (fac.HostileTo(Faction.OfPlayer)) fac = Find.FactionManager.RandomNonHostileFaction();
                foreach (Pawn pawn in Map.mapPawns.AllPawnsSpawned.FindAll(p => p.Position.GetRoom(Map) == this.OtherPawn.Position.GetRoom(Map) && p.health.hediffSet.HasHediff(RE_DefOf.RE_TurnBackToFormerFaction)))
                {
                    pawn.SetFactionDirect(fac);
                    pawn.ChangeKind(PawnKindDefOf.Villager);
                    pawn.health.RemoveHediff(pawn.health.hediffSet.hediffs.Find(h => h.def == RE_DefOf.RE_TurnBackToFormerFaction));
                    pawn.jobs.TryTakeOrderedJob(new Job(RE_DefOf.RE_LeaveMap, leavePoint));
                }
            };
            finalize.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return finalize;
            yield break;
        }
    }
}