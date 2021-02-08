namespace RimEffect
{
    using System.Collections.Generic;
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class Hediff_BioticAmp : Hediff_Abilities
    {
        public float bioticEnergy;

        public void UseEnergy(float energyUsed) => 
            this.bioticEnergy -= energyUsed;

        public bool SufficientEnergyPresent(float energyWanted) => 
            this.bioticEnergy > energyWanted;

        public override void Tick()
        {
            base.Tick();
            this.bioticEnergy += this.pawn.GetStatValue(RE_DefOf.RE_BioticEnergyRecoveryRate) / GenTicks.TicksPerRealSecond;
            this.bioticEnergy =  Mathf.Min(this.bioticEnergy, this.pawn.GetStatValue(RE_DefOf.RE_BioticEnergyMax));
        }

        public override IEnumerable<Gizmo> DrawGizmos()
        {
            Gizmo_BioticEnergyStatus gizmoBioticEnergy = new Gizmo_BioticEnergyStatus {bioticHediff = this};
            yield return gizmoBioticEnergy;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.bioticEnergy, nameof(this.bioticEnergy));
        }
    }

    public class AbilityExtension_Biotic : DefModExtension
    {
        public float GetEnergyUsedByPawn(Pawn pawn) => this.energyUsed * pawn.GetStatValue(RE_DefOf.RE_BioticAbilityCostMultiplier);

        public float energyUsed = 0f;
    }

    [StaticConstructorOnStartup]
    public class Gizmo_BioticEnergyStatus : Gizmo
    {
        public Hediff_BioticAmp bioticHediff;

        private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        public Gizmo_BioticEnergyStatus() => this.order = -1500f;

        public override float GetWidth(float maxWidth) => 
            140f;

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            Rect rect  = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            Rect rect3 = rect2;
            rect3.height = rect.height / 2f;
            Text.Font    = GameFont.Tiny;
            Widgets.Label(rect3, this.bioticHediff.LabelCap);
            Rect rect4 = rect2;
            rect4.yMin = rect2.y + rect2.height / 2f;
            float fillPercent = this.bioticHediff.bioticEnergy / this.bioticHediff.pawn.GetStatValue(RE_DefOf.RE_BioticEnergyMax);
            Widgets.FillableBar(rect4, fillPercent, FullBarTex, EmptyBarTex, doBorder: false);
            Text.Font   = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect4, (this.bioticHediff.bioticEnergy).ToString("F0") + " / " + this.bioticHediff.pawn.GetStatValue(RE_DefOf.RE_BioticEnergyMax).ToString("F0"));
            Text.Anchor = TextAnchor.UpperLeft;
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
