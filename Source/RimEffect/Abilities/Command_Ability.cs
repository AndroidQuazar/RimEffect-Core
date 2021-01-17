namespace RimEffect
{
    using System;
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class Command_Ability : Command_Action
    {
        public Pawn    pawn;
        public Ability ability;

        public override void GizmoUpdateOnMouseover()
        {
            base.GizmoUpdateOnMouseover();

            float radius;

            switch (this.ability.def.targetMode)
            {
                case AbilityTargetingMode.Self:
                    radius = this.ability.GetRadiusForPawn();
                    break;
                case AbilityTargetingMode.Location:
                case AbilityTargetingMode.Thing:
                case AbilityTargetingMode.Pawn:
                    radius = this.ability.GetRangeForPawn();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (GenRadial.MaxRadialPatternRadius > radius && radius >= 1)
                GenDraw.DrawRadiusRing(this.pawn.Position, radius, Color.cyan);
        }
    }
}
