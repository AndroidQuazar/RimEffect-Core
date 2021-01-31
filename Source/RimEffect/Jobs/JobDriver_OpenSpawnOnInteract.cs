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
            this.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            
            base.AddFinishAction(delegate
            {
                if (this.Openable.TryGetComp<CompSpawnOnInteract>() is CompSpawnOnInteract comp && comp != null)
                {
                    comp.ShouldSpawn = true;
                }
            });

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_General.Wait(15, TargetIndex.None).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield break;
        }
    }
}