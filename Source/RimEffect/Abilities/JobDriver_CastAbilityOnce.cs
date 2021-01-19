namespace RimEffect
{
    using System.Collections.Generic;
    using Verse;
    using Verse.AI;

    public class JobDriver_CastAbilityOnce : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);



            //yield return Toils_Combat.GotoCastPosition(TargetIndex.A);

            CompAbilities comp = this.pawn.GetComp<CompAbilities>();
            Toil          toil = Toils_General.Wait(comp.currentlyCasting.GetCastTimeForPawn(), TargetIndex.A);
            toil.WithProgressBarToilDelay(TargetIndex.C);
            toil.AddFinishAction(newAct: () =>
                                         {
                                             LocalTargetInfo target        = this.pawn.jobs.curJob.GetTarget(TargetIndex.A);
                                             comp.currentlyCasting.Cast(target);
                                         });
            yield return toil;
        }
    }
}
