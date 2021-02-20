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

            if (target.Thing is Pawn)
            {
                AbilityPawnFlyer flyer = (AbilityPawnFlyer) PawnFlyer.MakeFlyer(RE_DefOf.RE_AbilityFlyer, target.Pawn, destination);
                flyer.ability = this;
                flyer.target  = destination.ToVector3();
                GenSpawn.Spawn(flyer, target.Cell, this.pawn.Map);
            }
            else
            {
                target.Thing.Position = destination;
            }

            base.Cast(target);
        }

        public override void CheckCastEffects(LocalTargetInfo targetInfo, out bool cast, out bool target, out bool hediffApply)
        {
            base.CheckCastEffects(targetInfo, out cast, out target, out _);
            hediffApply = false;
        }
    }
}