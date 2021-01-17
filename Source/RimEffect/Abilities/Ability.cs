namespace RimEffect
{
    using System;
    using System.Linq;
    using System.Text;
    using RimWorld;
    using UnityEngine;
    using Verse;
    using Verse.AI;

    public abstract class Ability : IExposable, ILoadReferenceable
    {
        public Pawn       pawn;
        public Thing      holder;
        public AbilityDef def;
        public int        cooldown;

        public virtual bool IsEnabledForPawn(out string reason)
        {
            if (this.cooldown > Find.TickManager.TicksGame)
            {
                reason = "RE.AbilityDisableReasonCooldown".Translate(this.def.LabelCap, (this.cooldown - Find.TickManager.TicksGame).ToStringTicksToPeriod());
                return false;
            }

            reason = "RE.AbilityDisableReasonGeneral".Translate(this.pawn?.NameShortColored ?? this.holder.LabelCap);

            HediffWithLevelCombination requirement = this.def.requiredHediff;
            return requirement == null || this.pawn?.health.hediffSet.GetFirstHediffOfDef(requirement.hediffDef) is Hediff_Abilities hediffAbility && hediffAbility.level >= requirement.minimumLevel;
        }

        public virtual float GetRangeForPawn() =>
            this.def.rangeStatFactors.Aggregate(this.def.range, (current, statFactor) => current * (pawn.GetStatValue(statFactor.stat) * statFactor.value));

        public virtual float GetRadiusForPawn() =>
            this.def.targetMode == AbilityTargetingMode.Self ? 0f : this.def.radiusStatFactors.Aggregate(this.def.radius, (current, statFactor) => current * (pawn.GetStatValue(statFactor.stat) * statFactor.value));

        public virtual float GetPowerForPawn() =>
            this.def.powerStatFactors.Aggregate(this.def.power, (current, statFactor) => current * (pawn.GetStatValue(statFactor.stat) * statFactor.value));

        public virtual int GetCastTimeForPawn() =>
            Mathf.RoundToInt(this.def.castTimeStatFactors.Aggregate((float) this.def.castTime, (current, statFactor) => current * (pawn.GetStatValue(statFactor.stat) * statFactor.value)));

        public virtual int GetCooldownForPawn() =>
            Mathf.RoundToInt(this.def.cooldownTimeStatFactors.Aggregate((float) this.def.cooldownTime, (current, statFactor) => current * (pawn.GetStatValue(statFactor.stat) * statFactor.value)));

        public string GetDescriptionForPawn()
        {
            StringBuilder sb = new StringBuilder(this.def.description);

            sb.AppendLine();

            float rangeForPawn = this.GetRangeForPawn();
            if (rangeForPawn > 0f)
                sb.AppendLine($"{"Range".Translate()}: {rangeForPawn}".Colorize(Color.cyan));
            float radiusForPawn = this.GetRadiusForPawn();
            if (radiusForPawn > 0f)
                sb.AppendLine($"{"radius".Translate()}: {radiusForPawn}".Colorize(Color.cyan));
            float powerForPawn = this.GetPowerForPawn();
            if (powerForPawn > 0f)
                sb.AppendLine($"{"RE.AbilityStatsPower".Translate()}: {powerForPawn}".Colorize(Color.cyan));
            int castTimeForPawn = this.GetCastTimeForPawn();
            if (castTimeForPawn > 0)
                sb.AppendLine($"{"AbilityCastingTime".Translate()}: {castTimeForPawn.ToStringTicksToPeriod(shortForm: true)}".Colorize(Color.cyan));
            int cooldownForPawn = this.GetCooldownForPawn();
            if (cooldownForPawn > 0)
                sb.AppendLine($"{"CooldownTime".Translate()}: {cooldownForPawn.ToStringTicksToPeriod(shortForm: true)}".Colorize(Color.cyan));

            return sb.ToString();
        }

        public virtual Command_Action GetGizmo()
        {
            Command_Ability action = new Command_Ability
                                    {
                                        defaultLabel = this.def.LabelCap,
                                        defaultDesc = this.GetDescriptionForPawn(), 
                                        icon = this.def.icon,
                                        disabled = !this.IsEnabledForPawn(out string reason),
                                        disabledReason = reason.Colorize(Color.red),
                                        action = this.DoAction,
                                        ability = this
                                    };
            
            return action;
        }

        public virtual void DoAction()
        {
            TargetingParameters parameters = new TargetingParameters();

            switch (this.def.targetMode)
            {
                case AbilityTargetingMode.Self:
                    parameters = TargetingParameters.ForSelf(this.pawn);
                    this.CreateCastJob(this.pawn);
                    return;
                case AbilityTargetingMode.Location:
                    parameters.canTargetLocations = true;
                    break;
                case AbilityTargetingMode.Thing:
                    parameters.canTargetItems     = true;
                    parameters.canTargetBuildings = true;
                    break;
                case AbilityTargetingMode.Pawn:
                    parameters.canTargetPawns = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Find.Targeter.BeginTargeting(parameters, this.CreateCastJob, this.pawn);
        }

        public virtual void CreateCastJob(LocalTargetInfo target)
        {
            Job job = JobMaker.MakeJob(RE_DefOf.RE_UseAbility, target);
            this.pawn.GetComp<CompAbilities>().currentlyCasting = this;
            this.pawn.jobs.StartJob(job, JobCondition.InterruptForced);
        }

        public virtual void Cast(LocalTargetInfo target) => 
            this.cooldown = Find.TickManager.TicksGame + this.GetCooldownForPawn();

        public void ExposeData()
        {
            Scribe_References.Look(ref this.pawn, nameof(this.pawn));
            Scribe_References.Look(ref this.holder, nameof(this.holder));
            Scribe_Values.Look(ref this.cooldown, nameof(this.cooldown));
            Scribe_Defs.Look(ref this.def, nameof(this.def));
        }

        public string GetUniqueLoadID() => 
            $"Ability_{this.def.defName}_{this.holder.GetUniqueLoadID()}";
    }
}