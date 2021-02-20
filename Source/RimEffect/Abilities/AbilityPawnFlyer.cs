﻿namespace RimEffect
{
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class AbilityPawnFlyer : PawnFlyer
    {
        public Ability ability;

        protected Vector3 position;
        public    Vector3 target;

        public override void Tick()
        {
            this.position = Vector3.Lerp(this.startVec, this.target, (float) this.ticksFlying / (float) this.ticksFlightTime);
            base.Tick();
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            this.FlyingPawn.DrawAt(this.position, flip);
        }

        protected override void RespawnPawn()
        {
            Position = this.target.ToIntVec3();
            Pawn pawn = this.FlyingPawn;
            base.RespawnPawn();
            this.ability.ApplyHediffs(pawn);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref this.ability, nameof(this.ability));
        }
    }
}
