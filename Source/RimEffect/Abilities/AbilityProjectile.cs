namespace RimEffect
{
    using System.Collections.Generic;
    using RimWorld;
    using UnityEngine;
    using Verse;
    using Verse.Sound;

    public class AbilityProjectile : Projectile
    {
        public Ability ability;

        protected override void Impact(Thing hitThing)
        {
            Map     map      = this.Map;
            IntVec3 position = this.Position;
            base.Impact(hitThing);
            BattleLogEntry_RangedImpact battleLogEntryRangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntryRangedImpact);
            this.NotifyImpact(hitThing, map, position);


            this.ability.TargetEffects(new LocalTargetInfo(this.Position));

            float power = this.ability.GetPowerForPawn();

            if (hitThing != null)
            {
                DamageInfo dinfo = new DamageInfo(this.def.projectile.damageDef, power, this.ArmorPenetration, this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntryRangedImpact);

                if (hitThing is Pawn pawn)
                {
                    AbilityExtension_Hediff extensionHediff = this.ability.def.GetModExtension<AbilityExtension_Hediff>();
                    if (extensionHediff != null)
                    {
                        Hediff hediff = HediffMaker.MakeHediff(extensionHediff.hediff, pawn);
                        hediff.Severity = extensionHediff.severity;
                        pawn.health.AddHediff(hediff);
                    }

                    if (pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                    {
                        pawn.stances.StaggerFor(95);
                    }
                }
                if (this.def.projectile.extraDamages == null)
                {
                    return;
                }
                foreach (ExtraDamage extraDamage in this.def.projectile.extraDamages)
                {
                    if (Rand.Chance(extraDamage.chance))
                    {
                        DamageInfo dinfo2 = new DamageInfo(extraDamage.def, extraDamage.amount, extraDamage.AdjustedArmorPenetration(), this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                        hitThing.TakeDamage(dinfo2).AssociateWithLog(battleLogEntryRangedImpact);
                    }
                }
            }
            else
            {
                SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(this.Position, map));
                if (this.Position.GetTerrain(map).takeSplashes)
                {
                    MoteMaker.MakeWaterSplash(this.ExactPosition, map, Mathf.Sqrt(power) * 1f, 4f);
                }
                else
                {
                    MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt);
                }
            }
        }

        private void NotifyImpact(Thing hitThing, Map map, IntVec3 position)
        {
            return;
            
            int num = 9;
            for (int i = 0; i < num; i++)
            {
                IntVec3 c = position + GenRadial.RadialPattern[i];
                if (!c.InBounds(map))
                {
                    continue;
                }
                List<Thing> thingList = c.GetThingList(map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    if (thingList[j] != hitThing)
                    {
                        thingList[j].Notify_BulletImpactNearby(new BulletImpactData());
                    }
                }
            }
        }
    }
}