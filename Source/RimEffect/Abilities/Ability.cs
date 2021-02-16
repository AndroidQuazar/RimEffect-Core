namespace RimEffect
{
    using System;
    using System.Linq;
    using System.Text;
    using RimWorld;
    using UnityEngine;
    using Verse;
    using Verse.AI;
    using Verse.Sound;

    public abstract class Ability : IExposable, ILoadReferenceable, ITargetingSource
    {
        public Pawn       pawn;
        public Thing      holder;
        public AbilityDef def;
        public int        cooldown;

        public Hediff_Abilities hediff;

        public Hediff_Abilities Hediff => this.hediff == null && this.def.requiredHediff != null
                                              ? (this.hediff = (Hediff_Abilities) this.pawn?.health.hediffSet.GetFirstHediffOfDef(this.def.requiredHediff.hediffDef))
                                              : this.hediff;

        public virtual bool IsEnabledForPawn(out string reason)
        {
            if (this.cooldown > Find.TickManager.TicksGame)
            {
                reason = "RE.AbilityDisableReasonCooldown".Translate(this.def.LabelCap, (this.cooldown - Find.TickManager.TicksGame).ToStringTicksToPeriod());
                return false;
            }

            AbilityExtension_Biotic biotic = this.def.GetModExtension<AbilityExtension_Biotic>();
            if (biotic != null)
            {

                reason = "RE.AbilityDisableReasonBioticEnergyLack".Translate();
                Log.ResetMessageCount();
                if (this.Hediff == null || !((Hediff_BioticAmp) this.Hediff).SufficientEnergyPresent(biotic.GetEnergyUsedByPawn(this.pawn)))
                    return false;
            }

            reason = "RE.AbilityDisableReasonGeneral".Translate(this.pawn?.NameShortColored ?? this.holder.LabelCap);

            return this.def.requiredHediff?.Satisfied(this.Hediff) ?? true;
        }

        public virtual float GetRangeForPawn() =>
            this.def.rangeStatFactors.Aggregate(this.def.range, (current, statFactor) => current * (this.pawn.GetStatValue(statFactor.stat) * statFactor.value));

        public virtual float GetRadiusForPawn() =>
            this.def.targetMode == AbilityTargetingMode.Self
                ? 0f
                : this.def.radiusStatFactors.Aggregate(this.def.radius, (current, statFactor) => current * (this.pawn.GetStatValue(statFactor.stat) * statFactor.value));

        public virtual float GetPowerForPawn() =>
            this.def.powerStatFactors.Aggregate(this.def.power, (current, statFactor) => current * (this.pawn.GetStatValue(statFactor.stat) * statFactor.value));

        public virtual int GetCastTimeForPawn() =>
            Mathf.RoundToInt(this.def.castTimeStatFactors.Aggregate((float) this.def.castTime, (current, statFactor) => current * (this.pawn.GetStatValue(statFactor.stat) * statFactor.value)));

        public virtual int GetCooldownForPawn() =>
            Mathf.RoundToInt(this.def.cooldownTimeStatFactors.Aggregate((float) this.def.cooldownTime, (current, statFactor) => current * (this.pawn.GetStatValue(statFactor.stat) * statFactor.value)));

        public virtual int GetDurationForPawn() =>
            Mathf.RoundToInt(this.def.durationTimeStatFactors.Aggregate((float)this.def.durationTime, (current, statFactor) => current * (this.pawn.GetStatValue(statFactor.stat) * statFactor.value)));

        public virtual string GetDescriptionForPawn()
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
            int durationForPawn = this.GetDurationForPawn();
            if (durationForPawn > 0)
                sb.AppendLine($"{"RE.AbilityStatsDuration".Translate()}: {durationForPawn.ToStringTicksToPeriod(shortForm: true)}".Colorize(Color.cyan));

            AbilityExtension_Biotic biotic = this.def.GetModExtension<AbilityExtension_Biotic>();
            if (biotic != null) 
                sb.AppendLine($"{"RE.AbilityStatsBioticEnergy".Translate()}: {biotic.GetEnergyUsedByPawn(this.pawn)}".Colorize(Color.cyan));


            return sb.ToString();
        }

        public virtual Command_Action GetGizmo()
        {
            Command_Ability action = new Command_Ability
                                     {
                                         defaultLabel   = this.def.LabelCap,
                                         defaultDesc    = this.GetDescriptionForPawn(),
                                         icon           = this.def.icon,
                                         disabled       = !this.IsEnabledForPawn(out string reason),
                                         disabledReason = reason.Colorize(Color.red),
                                         action         = this.DoAction,
                                         ability        = this,
                                         pawn           = this.pawn
                                     };

            return action;
        }

        public virtual void DoAction()
        {
            if (this.def.targetMode == AbilityTargetingMode.Self)
                this.CreateCastJob(this.pawn);
            else
                Find.Targeter.BeginTargeting(this);
        }

        public virtual void CreateCastJob(LocalTargetInfo target)
        {
            this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, false);

            Job job = JobMaker.MakeJob(RE_DefOf.RE_UseAbility, target);
            this.pawn.GetComp<CompAbilities>().currentlyCasting = this;
            this.pawn.jobs.StartJob(job, JobCondition.InterruptForced);
        }

        public virtual void Cast(LocalTargetInfo target)
        {
            this.cooldown = Find.TickManager.TicksGame + this.GetCooldownForPawn();

            AbilityExtension_Biotic biotic = this.def.GetModExtension<AbilityExtension_Biotic>();
            if (biotic != null)
            {
                Hediff_BioticAmp bioticAmp = (Hediff_BioticAmp) this.pawn.health.hediffSet.GetFirstHediffOfDef(RE_DefOf.RE_BioticAmpHediff);
                bioticAmp.UseEnergy(biotic.GetEnergyUsedByPawn(this.pawn));
            }

            if (target.HasThing)
            {
                AbilityExtension_Hediff hediffExtension = this.def.GetModExtension<AbilityExtension_Hediff>();
                if (hediffExtension?.applyAuto ?? false)
                {
                    Hediff localHediff = HediffMaker.MakeHediff(hediffExtension.hediff, this.pawn);
                    if (Math.Abs(hediffExtension.severity - -1f) > float.Epsilon)
                        localHediff.Severity = hediffExtension.severity;
                    target.Pawn.health.AddHediff(localHediff);
                }
            }

            this.CheckCastEffects(target, out bool cast, out bool targetMote, out bool sound);

            if (cast)
            {
                if (this.def.castMote != null)
                    MoteMaker.MakeStaticMote(this.pawn.DrawPos, this.pawn.Map, this.def.castMote);
                if (this.def.casterHediff != null)
                    this.pawn.health.AddHediff(this.def.casterHediff);
            }

            if (!this.def.targetMotes.NullOrEmpty() && targetMote)
                foreach (ThingDef mote in this.def.targetMotes)
                    MoteMaker.MakeStaticMote(target.Cell, this.pawn.Map, mote);

            if (sound)
                this.def.castSound?.PlayOneShot(new TargetInfo(this.pawn.Position, this.pawn.Map));
        }

        public virtual void CheckCastEffects(LocalTargetInfo targetInfo, out bool cast, out bool target, out bool sound) => 
            cast = target = sound = true;

        public void ExposeData()
        {
            Scribe_References.Look(ref this.pawn,   nameof(this.pawn));
            Scribe_Values.Look(ref this.cooldown, nameof(this.cooldown));
            Scribe_Defs.Look(ref this.def, nameof(this.def));
        }

        public string GetUniqueLoadID() => 
            $"Ability_{this.def.defName}_{this.holder.GetUniqueLoadID()}";

        public virtual bool CanHitTarget(LocalTargetInfo target) => 
            target.Cell.DistanceTo(this.pawn.Position) < this.GetRangeForPawn() && GenSight.LineOfSight(this.pawn.Position, target.Cell, this.pawn.Map);

        public virtual bool ValidateTarget(LocalTargetInfo target) => 
            this.targetParams.CanTarget(target.ToTargetInfo(this.pawn.Map)) && this.CanHitTarget(target);

        public virtual void DrawHighlight(LocalTargetInfo target)
        {
            float range = this.GetRangeForPawn();
            if (GenRadial.MaxRadialPatternRadius > range && range >= 1)
                GenDraw.DrawRadiusRing(this.pawn.Position, range, Color.cyan);

            if (target.IsValid)
            {
                GenDraw.DrawTargetHighlight(target);

                float radius = this.GetRadiusForPawn();
                if (GenRadial.MaxRadialPatternRadius > radius && radius >= 1)
                    GenDraw.DrawRadiusRing(target.Cell, radius, Color.red);
            }
        }

        public virtual void OrderForceTarget(LocalTargetInfo target) => 
            this.CreateCastJob(target);

        public virtual void OnGUI(LocalTargetInfo target)
        {
            Texture2D icon = (!target.IsValid) ? TexCommand.CannotShoot : ((!(this.UIIcon != BaseContent.BadTex)) ? TexCommand.Attack : this.UIIcon);
            GenUI.DrawMouseAttachment(icon);
        }

        public bool      CasterIsPawn  => this.CasterPawn           != null;
        public bool      IsMeleeAttack => this.GetRangeForPawn() < 6;
        public bool      Targetable    => this.def.targetMode    != AbilityTargetingMode.Self;
        public bool      MultiSelect   { get; }
        public Thing     Caster        => this.pawn ?? this.holder;
        public Pawn      CasterPawn    => this.pawn;
        public Verb      GetVerb       { get; }
        public Texture2D UIIcon        => this.def.icon;

        public virtual TargetingParameters targetParams
        {
            get
            {
                TargetingParameters parameters = new TargetingParameters();

                switch (this.def.targetMode)
                {
                    case AbilityTargetingMode.Self:
                        parameters = TargetingParameters.ForSelf(this.pawn);
                        break;
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
                return parameters;
            }
        }

        public ITargetingSource DestinationSelector { get; }
    }

    public class Ability_Blank : Ability
    {

    }

    public class AbilityExtension_Hediff : DefModExtension
    {
        public HediffDef hediff;
        public float     severity  = -1f;
        public bool      applyAuto = true;
    }
}