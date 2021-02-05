using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimEffect
{
    public class JobDriver_OpenSpawnOnInteract : JobDriver
    {
        private Thing Openable
        {
            get
            {
                return this.job.GetTarget(TargetIndex.A).Thing;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(this.Openable, job, errorOnFailed: errorOnFailed);
        } 

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_General.Wait(15, TargetIndex.None).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            Toil finalize = new Toil();
            finalize.initAction = delegate ()
            {
                if (this.Openable.TryGetComp<CompSpawnOnInteract>() is CompSpawnOnInteract comp && comp != null)
                {
                    comp.ShouldSpawn = true;
                }
            };
            finalize.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return finalize;
            yield break;
        }
    }
}