using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimEffect
{
    public class JobDriver_LeaveMap : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            this.pawn.Map.pawnDestinationReservationManager.Reserve(this.pawn, this.job, this.job.targetA.Cell);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil finalize = new Toil();
            finalize.initAction = delegate ()
             {
                 // End related quest if no other pawn of same faction are still on the map
                 if (this.Map.mapPawns.AllPawnsSpawned.FindAll(predicate => predicate.Faction == pawn.Faction).Count <= 1)
                 {
                     foreach (var quest in Find.QuestManager.QuestsListForReading)
                     {
                         var signal = new Signal("Quest" + quest.id + ".AllHostagesAreFree");
                         Find.SignalManager.SendSignal(signal);
                     }
                 }

                 this.pawn.ExitMap(false, Rot4.Random);
             };
            finalize.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return finalize;
            yield break;
        }
    }
}