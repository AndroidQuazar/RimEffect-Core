namespace RimEffect
{
    using RimWorld;
    using Verse;

    public class Ability_Pull : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            IntVec3 destination = this.pawn.Position + ((target.Cell - this.pawn.Position).ToVector3().normalized * 2).ToIntVec3();

            /*
            if (!parent.def.HasAreaOfEffect)
            {
                parent.AddEffecterToMaintain(EffecterDefOf.Skip_Entry.Spawn(target.Thing, pawn.Map), target.Thing.Position, 60);
            }

            if (Props.destination == AbilityEffectDestination.Selected)
            {
                parent.AddEffecterToMaintain(EffecterDefOf.Skip_Exit.Spawn(target.Cell, pawn.Map), destination, 60);
            }
            */

            target.Thing.TryGetComp<CompCanBeDormant>()?.WakeUp();
            target.Thing.Position = destination;

            if (target.Thing is Pawn pawn2)
            {
                //pawn2.stances.stunner.StunFor_NewTmp(GenDate.TicksPerHour, pawn, addBattleLog: false, showMote: false);
                pawn2.Notify_Teleported();
            }

            base.Cast(target);
        }
    }
}