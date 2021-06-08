﻿namespace RimEffect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class Ability_Singularity : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);

            Singularity singularity = (Singularity) GenSpawn.Spawn(RE_DefOf.RE_Biotic_SingularityThing, target.Cell, this.pawn.Map);
            singularity.caster        = this.pawn;
            singularity.casterFaction = this.pawn.Faction;
            singularity.radius        = this.GetRadiusForPawn();
            singularity.damage        = this.GetPowerForPawn();
            singularity.endTick       = Find.TickManager.TicksGame + this.GetDurationForPawn();

            if (this.def.targetMotes.Any()) 
                singularity.mote = this.def.targetMotes.First();
        }

        public override void CheckCastEffects(LocalTargetInfo targetInfo, out bool cast, out bool target, out bool hediffApply)
        {
            base.CheckCastEffects(targetInfo, out cast, out _, out hediffApply);
            target = false;
        }
    }

    public class Singularity : Thing
    {
        public Pawn     caster;
        public Faction  casterFaction;
        public ThingDef mote;
        public Mote     moteThing;

        public float radius;
        public float damage;
        public float endTick;

        [Unsaved]
        public List<Pawn> tmpPawns = new List<Pawn>();

        [Unsaved]
        public Graphic graphic;

        public float curRotation = 0f;

        public static float rotSpeed    = 5f;

        public static float lerpSpeed = 0.01f;

        public override Graphic Graphic
        {
            get
            {
                if (this.graphic == null || Math.Abs(this.graphic.drawSize.x - (this.radius * 2)) > float.Epsilon)
                    this.graphic = GraphicDatabase.Get(this.def.graphicData.graphicClass, this.def.graphicData.texPath, this.def.graphicData.shaderType.Shader,
                                                       new Vector2(this.radius * 2, this.radius * 2), this.def.graphicData.color, this.def.graphicData.colorTwo, this.def.graphicData,
                                                       this.def.graphicData.shaderParameters);
                return this.graphic;
            }
        }
        
        public override void Tick()
        {
            this.curRotation += rotSpeed % 360f;
            foreach (Pawn pawn in this.tmpPawns)
                if (!pawn.DestroyedOrNull() && pawn.Spawned)
                {
                    if (pawn.Position != this.Position)
                    {
                        Vector3 vector3 = Vector3.Lerp(pawn.Position.ToVector3(), this.Position.ToVector3(), lerpSpeed);
                        pawn.Position = vector3.ToIntVec3();
                        pawn.Notify_Teleported();
                        pawn.pather.nextCell = this.Position;
                    }

                    if (this.damage > 0)
                    {
                        DamageInfo dinfo = new DamageInfo(DamageDefOf.Blunt, this.damage / GenTicks.TicksPerRealSecond, float.MaxValue, instigator: this.caster);
                        pawn.TakeDamage(dinfo);
                    }
                }

            if (this.moteThing is null)
            {
                this.moteThing = MoteMaker.MakeStaticMote(this.DrawPos, this.Map, mote);
            }
            else
            {
                this.moteThing.Maintain();
            }

            if (this.tmpPawns.Any() ? this.IsHashIntervalTick(GenTicks.TickRareInterval) : this.IsHashIntervalTick(GenTicks.TickRareInterval/8))
            {
                this.tmpPawns.Clear();
                int     rangeForGrabbingPawns = Mathf.RoundToInt(this.radius * 2);
                IntVec2 rangeIntVec2          = new IntVec2(rangeForGrabbingPawns, rangeForGrabbingPawns);

                foreach (IntVec3 intVec3 in GenAdj.OccupiedRect(this.Position, this.Rotation, rangeIntVec2))
                {
                    if (!intVec3.InBounds(Find.CurrentMap)) 
                        continue;
                    List<Thing> cellThings = Find.CurrentMap.thingGrid.ThingsListAt(intVec3);
                    if (cellThings == null) 
                        continue;
                    
                    for (int i = 0; i < cellThings.Count; i++)
                    {
                        Thing t = cellThings[i];

                        if (t is Pawn p && (this.caster != null ? p.HostileTo(this.caster) : p.HostileTo(this.casterFaction)))
                            this.tmpPawns.Add(p);
                    }
                }
            }

            if(Find.TickManager.TicksGame > this.endTick)
                this.Destroy();
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            Mesh       mesh = this.Graphic.MeshAt(this.Rotation);
            Quaternion quat = Quaternion.AngleAxis(this.curRotation, Vector3.up);
            
            Material mat = this.Graphic.MatAt(this.Rotation, this);

            Graphics.DrawMesh(mesh, drawLoc, quat, mat, 0);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref this.caster,        nameof(this.caster));
            Scribe_References.Look(ref this.casterFaction, nameof(this.casterFaction));
            Scribe_Values.Look(ref this.radius,   nameof(this.radius));
            Scribe_Values.Look(ref this.damage,   nameof(this.damage));

            Scribe_Values.Look(ref this.curRotation, nameof(this.curRotation));
        }
    }
}