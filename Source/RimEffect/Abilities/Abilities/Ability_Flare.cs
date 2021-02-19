namespace RimEffect
{
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class Ability_Flare : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);
            GenExplosion.DoExplosion(target.Cell, this.pawn.Map, this.GetRadiusForPawn(), DamageDefOf.Blunt, this.pawn, Mathf.RoundToInt(this.GetPowerForPawn()), 1f, SoundDefOf.PlanetkillerImpact);
            Flare flare = (Flare) GenSpawn.Spawn(RE_DefOf.RE_Biotic_FlareThing, target.Cell, this.pawn.Map);
            flare.radius    = this.GetRadiusForPawn();
            flare.startTick = Find.TickManager.TicksGame;
        }
    }

    public class Flare : Thing
    {
        public float radius;
        public float startTick;
        public float curRadius;

        public float curRotation = 0f;

        public static float rotSpeed = 5f;


        public override void Tick()
        {
            this.curRotation += Flare.rotSpeed % 360f;

            this.curRadius = (Find.TickManager.TicksGame - this.startTick) * 1.5f;

            if (this.radius <= this.curRadius)
                this.Destroy();
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            this.Graphic.drawSize = new Vector2(this.curRadius*2f, this.curRadius*2f);

            Mesh mesh = this.Graphic.MeshAt(this.Rotation);
            Quaternion quat = Quaternion.AngleAxis(this.curRotation, Vector3.up);

            Material mat = this.Graphic.MatAt(this.Rotation, this);

            Graphics.DrawMesh(mesh, drawLoc, quat, mat, 0);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.radius,      nameof(this.radius));
            Scribe_Values.Look(ref this.startTick,   nameof(this.startTick));
            Scribe_Values.Look(ref this.curRadius,   nameof(this.curRadius));
            Scribe_Values.Look(ref this.curRotation, nameof(this.curRotation));
        }
    }
}