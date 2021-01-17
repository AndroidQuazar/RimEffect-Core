namespace RimEffect
{
    using System.Collections.Generic;
    using System.Runtime.Remoting.Messaging;
    using Verse;
    using Verse.AI;

    public class JobDriver_CastAbilityOnce : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);



            //yield return Toils_Combat.GotoCastPosition(TargetIndex.A);

            Toil          toil = new Toil();
            CompAbilities comp = this.pawn.GetComp<CompAbilities>();
            toil.defaultDuration     = comp.currentlyCasting.GetCastTimeForPawn();
            toil.WithProgressBarToilDelay(TargetIndex.A);
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.AddFinishAction(newAct: () =>
                                         {
                                             LocalTargetInfo target        = this.pawn.jobs.curJob.GetTarget(TargetIndex.A);
                                             comp.currentlyCasting.Cast(target);
                                         });
            yield return toil;
        }
    }
}
