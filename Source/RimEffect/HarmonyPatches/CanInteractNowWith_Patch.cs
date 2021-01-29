namespace RimEffect
{
    using HarmonyLib;
    using RimWorld;
    using System.Collections.Generic;
    using Verse;

    [HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.CanInteractNowWith))]
    public static class CanInteractNowWith_Patch
    {
        public static bool Prefix(ref bool __result, Pawn recipient, InteractionDef interactionDef = null)
        {
            if (Building_VIInterface.terminals.TryGetValue(recipient, out Building_VIInterface vIInterface) && vIInterface.Spawned)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }

    //[HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.TryInteractWith))]
    //public static class TryInteractWith_Patch
    //{
    //    public static void Prefix(ref bool __result, Pawn_InteractionsTracker __instance, int ___lastInteractionTime, Pawn ___pawn, Pawn recipient, InteractionDef intDef)
    //    {
    //        Log.Message(" - Prefix - if (DebugSettings.alwaysSocialFight) - 1", true);
    //        if (DebugSettings.alwaysSocialFight)
    //        {
    //            Log.Message(" - Prefix - intDef = InteractionDefOf.Insult; - 2", true);
    //            intDef = InteractionDefOf.Insult;
    //        }
    //        Log.Message(" - Prefix - if (___pawn == recipient) - 3", true);
    //        if (___pawn == recipient)
    //        {
    //            Log.Message(" - Prefix - Log.Warning(string.Concat(___pawn, \" tried to interact with self, interaction=\", intDef.defName)); - 4", true);
    //            Log.Warning(string.Concat(___pawn, " tried to interact with self, interaction=", intDef.defName));
    //            Log.Message(" - Prefix - return; - 5", true);
    //            return;
    //        }
    //        Log.Message(" - Prefix - if (!__instance.CanInteractNowWith(recipient, intDef)) - 6", true);
    //        if (!__instance.CanInteractNowWith(recipient, intDef))
    //        {
    //            Log.Message(" - Prefix - return; - 7", true);
    //            return;
    //        }
    //        Log.Message(" - Prefix - if (!intDef.ignoreTimeSinceLastInteraction && __instance.InteractedTooRecentlyToInteract()) - 8", true);
    //        if (!intDef.ignoreTimeSinceLastInteraction && __instance.InteractedTooRecentlyToInteract())
    //        {
    //            Log.Error(string.Concat(___pawn, " tried to do interaction ", intDef, " to ", recipient, " only ", Find.TickManager.TicksGame - ___lastInteractionTime, " ticks since last interaction (min is ", 120, ")."));
    //            Log.Message(" - Prefix - return; - 10", true);
    //            return;
    //        }
    //        List<RulePackDef> list = new List<RulePackDef>();
    //        Log.Message(" - Prefix - if (intDef.initiatorThought != null) - 12", true);
    //        if (intDef.initiatorThought != null)
    //        {
    //
    //        }
    //        Log.Message(" - Prefix - if (intDef.recipientThought != null && recipient.needs.mood != null) - 13", true);
    //        if (intDef.recipientThought != null && recipient.needs.mood != null)
    //        {
    //
    //        }
    //        Log.Message(" - Prefix - if (intDef.initiatorXpGainSkill != null) - 14", true);
    //        if (intDef.initiatorXpGainSkill != null)
    //        {
    //            Log.Message(" - Prefix - ___pawn.skills.Learn(intDef.initiatorXpGainSkill, intDef.initiatorXpGainAmount); - 15", true);
    //            ___pawn.skills.Learn(intDef.initiatorXpGainSkill, intDef.initiatorXpGainAmount);
    //        }
    //        Log.Message(" - Prefix - if (intDef.recipientXpGainSkill != null && recipient.RaceProps.Humanlike) - 16", true);
    //        if (intDef.recipientXpGainSkill != null && recipient.RaceProps.Humanlike)
    //        {
    //            Log.Message(" - Prefix - recipient.skills.Learn(intDef.recipientXpGainSkill, intDef.recipientXpGainAmount); - 17", true);
    //            recipient.skills.Learn(intDef.recipientXpGainSkill, intDef.recipientXpGainAmount);
    //        }
    //        bool flag = false;
    //        Log.Message(" - Prefix - if (recipient.RaceProps.Humanlike) - 19", true);
    //        if (recipient.RaceProps.Humanlike)
    //        {
    //            Log.Message(" - Prefix - flag = recipient.interactions.CheckSocialFightStart(intDef, ___pawn); - 20", true);
    //            flag = recipient.interactions.CheckSocialFightStart(intDef, ___pawn);
    //        }
    //        string letterText;
    //        Log.Message(" - Prefix - string letterLabel; - 22", true);
    //        string letterLabel;
    //        Log.Message(" - Prefix - LetterDef letterDef; - 23", true);
    //        LetterDef letterDef;
    //        Log.Message(" - Prefix - LookTargets lookTargets; - 24", true);
    //        LookTargets lookTargets;
    //        Log.Message(" - Prefix - if (!flag) - 25", true);
    //        if (!flag)
    //        {
    //            Log.Message(" - Prefix - intDef.Worker.Interacted(___pawn, recipient, list, out letterText, out letterLabel, out letterDef, out lookTargets); - 26", true);
    //            intDef.Worker.Interacted(___pawn, recipient, list, out letterText, out letterLabel, out letterDef, out lookTargets);
    //        }
    //        else
    //        {
    //            Log.Message(" - Prefix - letterText = null; - 27", true);
    //            letterText = null;
    //            Log.Message(" - Prefix - letterLabel = null; - 28", true);
    //            letterLabel = null;
    //            Log.Message(" - Prefix - letterDef = null; - 29", true);
    //            letterDef = null;
    //            Log.Message(" - Prefix - lookTargets = null; - 30", true);
    //            lookTargets = null;
    //        }
    //        MoteMaker.MakeInteractionBubble(___pawn, recipient, intDef.interactionMote, intDef.Symbol);
    //        ___lastInteractionTime = Find.TickManager.TicksGame;
    //        Log.Message(" - Prefix - if (flag) - 33", true);
    //        if (flag)
    //        {
    //            Log.Message(" - Prefix - list.Add(RulePackDefOf.Sentence_SocialFightStarted); - 34", true);
    //            list.Add(RulePackDefOf.Sentence_SocialFightStarted);
    //        }
    //        PlayLogEntry_Interaction playLogEntry_Interaction = new PlayLogEntry_Interaction(intDef, ___pawn, recipient, list);
    //        Log.Message(" - Prefix - Find.PlayLog.Add(playLogEntry_Interaction); - 36", true);
    //        Find.PlayLog.Add(playLogEntry_Interaction);
    //        Log.Message(" - Prefix - if (letterDef != null) - 37", true);
    //        if (letterDef != null)
    //        {
    //            Log.Message(" - Prefix - string text = playLogEntry_Interaction.ToGameStringFromPOV(___pawn); - 38", true);
    //            string text = playLogEntry_Interaction.ToGameStringFromPOV(___pawn);
    //            Log.Message(" - Prefix - if (!letterText.NullOrEmpty()) - 39", true);
    //            if (!letterText.NullOrEmpty())
    //            {
    //                text = text + "" + letterText;
    //            }
    //            Find.LetterStack.ReceiveLetter(letterLabel, text, letterDef, lookTargets ?? ((LookTargets)___pawn));
    //        }
    //        return;
    //    }
    //}
}
