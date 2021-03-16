﻿namespace RimEffect
{
    using System;
    using System.Collections.Generic;
    using RimWorld;
    using UnityEngine;
    using Verse;

    public class AbilityDef : Def
    {
        public Type    abilityClass;

        public HediffWithLevelCombination requiredHediff;

        public AbilityTargetingMode targetMode = AbilityTargetingMode.Self;

        public float              range             = 0f;
        public List<StatModifier> rangeStatFactors = new List<StatModifier>();

        public float              radius           = 0f;
        public List<StatModifier> radiusStatFactors = new List<StatModifier>();

        public float              power            = 0f;
        public List<StatModifier> powerStatFactors = new List<StatModifier>();

        public int                castTime            = 0;
        public List<StatModifier> castTimeStatFactors = new List<StatModifier>();

        public int              cooldownTime         = 0;
        public List<StatModifier> cooldownTimeStatFactors = new List<StatModifier>();

        public int                durationTime            = 0;
        public List<StatModifier> durationTimeStatFactors = new List<StatModifier>();

        [Unsaved(false)]
        public Texture2D icon = BaseContent.BadTex;
        public string iconPath;

        public SoundDef                 castSound;
        public ThingDef                 castMote;
        public HediffDef                casterHediff;

        public List<ThingDef>           targetMotes;

        public VerbProperties      verbProperties;
        public TargetingParameters targetingParameters;
        public float               chance = 1f;

        public float Chance => this.chance;

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string configError in base.ConfigErrors()) 
                yield return configError;

            if (!typeof(Ability).IsAssignableFrom(this.abilityClass))
                yield return $"{this.abilityClass} is not a valid ability type";

            if (this.GetModExtension<AbilityExtension_Projectile>() != null && (this.GetModExtension<AbilityExtension_Hediff>()?.applyAuto ?? false))
                yield return "Projectile and auto apply hediff present. Please check if that is intended.";
        }

        public override void PostLoad()
        {
            if (!this.iconPath.NullOrEmpty())
                LongEventHandler.ExecuteWhenFinished(delegate { this.icon = ContentFinder<Texture2D>.Get(this.iconPath); });

            if (this.targetingParameters == null)
            {
                this.targetingParameters = new TargetingParameters();

                switch (this.targetMode)
                {
                    case AbilityTargetingMode.Self:
                        this.targetingParameters = TargetingParameters.ForSelf(null);
                        break;
                    case AbilityTargetingMode.Location:
                        this.targetingParameters.canTargetLocations = true;
                        break;
                    case AbilityTargetingMode.Thing:
                        this.targetingParameters.canTargetItems  = true;
                        this.targetingParameters.canTargetBuildings = true;
                        break;
                    case AbilityTargetingMode.Pawn:
                        this.targetingParameters.canTargetPawns = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if(this.verbProperties == null)
                this.verbProperties = new VerbProperties
                                      {
                                          verbClass           = typeof(Verb_CastAbility),
                                          label               = this.label,
                                          category            = VerbCategory.Misc,
                                          range               = this.range,
                                          noiseRadius         = 3f,
                                          targetParams        = this.targetingParameters,
                                          warmupTime          = this.castTime / (float) GenTicks.TicksPerRealSecond,
                                          defaultCooldownTime = this.cooldownTime,
                                          meleeDamageBaseAmount = Mathf.RoundToInt(this.power),
                                          meleeDamageDef = DamageDefOf.Blunt
                                      };
        }

    }

    public class HediffWithLevelCombination
    {
        public HediffDef hediffDef;
        public int       minimumLevel;

        public bool Satisfied(Pawn p) => 
            this.Satisfied(p.health.hediffSet.GetFirstHediffOfDef(this.hediffDef) as Hediff_ImplantWithLevel);

        public bool Satisfied(Hediff_ImplantWithLevel hediff) => 
            hediff != null && hediff.level >= this.minimumLevel;
    }

    public enum AbilityTargetingMode : byte
    {
        Self,
        Location,
        Thing,
        Pawn
    }
}
