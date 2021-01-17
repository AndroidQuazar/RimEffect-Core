namespace RimEffect
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

        [Unsaved(false)]
        public Texture2D icon = BaseContent.BadTex;
        public string iconPath;

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string configError in base.ConfigErrors()) 
                yield return configError;

            if (!typeof(Ability).IsAssignableFrom(this.abilityClass))
                yield return $"{this.abilityClass} is not a valid ability type";
        }

        public override void PostLoad()
        {
            if (!this.iconPath.NullOrEmpty())
                LongEventHandler.ExecuteWhenFinished(delegate
                                                     {
                                                         this.icon = ContentFinder<Texture2D>.Get(this.iconPath);
                                                     });
        }

    }

    public class HediffWithLevelCombination
    {
        public HediffDef hediffDef;
        public int       minimumLevel = 0;
    }

    public enum AbilityTargetingMode : byte
    {
        Self,
        Location,
        Thing,
        Pawn
    }
}
