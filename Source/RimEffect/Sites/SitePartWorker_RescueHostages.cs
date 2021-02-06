using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using Verse;

namespace RimEffect.Sites
{
    public class SitePartWorker_RescueHostages : SitePartWorker
    {
        public override void Notify_GeneratedByQuestGen(SitePart part, Slate slate, List<Verse.Grammar.Rule> outExtraDescriptionRules, Dictionary<string, string> outExtraDescriptionConstants)
        {
            base.Notify_GeneratedByQuestGen(part, slate, outExtraDescriptionRules, outExtraDescriptionConstants);
        }

        public override string GetPostProcessedThreatLabel(Site site, SitePart sitePart)
        {
            string text = base.GetPostProcessedThreatLabel(site, sitePart);
            if (sitePart.things != null && sitePart.things.Any)
            {
                text = text + ": " + sitePart.things[0].LabelShortCap;
            }
            if (site.HasWorldObjectTimeout)
            {
                text += " (" + "DurationLeft".Translate(site.WorldObjectTimeoutTicksLeft.ToStringTicksToPeriod(true, false, true, true)) + ")";
            }
            return text;
        }
    }
}