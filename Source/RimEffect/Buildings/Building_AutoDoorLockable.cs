using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimEffect
{
    public enum DoorAccess
    {
        Default,
        Everyone,
        OnlyColonistsAndAnimals,
        OnlyColonistsAndAnimalsAndFriendlies,
        OnlyDrafted
    }
	public class Building_AutoDoorLockable : Building_Door
	{
        private DoorAccess curDoorAccess = DoorAccess.Default;

        private Dictionary<DoorAccess, string> doorStates = new Dictionary<DoorAccess, string> {
            {DoorAccess.Default, "RE.DoorLockDefault".Translate()},
            {DoorAccess.Everyone, "RE.DoorLockEveryone".Translate()},
            {DoorAccess.OnlyColonistsAndAnimals, "RE.DoorLockOnlyColonistsAndAnimals".Translate()},
            {DoorAccess.OnlyColonistsAndAnimalsAndFriendlies, "RE.DoorLockOnlyColonistsAndAnimalsAndFriendlies".Translate()},
            {DoorAccess.OnlyDrafted, "RE.DoorLockOnlyDrafted".Translate()},
        };
        public override bool PawnCanOpen(Pawn p)
        {
            switch (curDoorAccess)
            {
                case DoorAccess.Default: return this.powerComp.PowerOn && base.PawnCanOpen(p);
                case DoorAccess.Everyone: return this.powerComp.PowerOn && true;
                case DoorAccess.OnlyColonistsAndAnimals: return OnlyColonistsAndAnimals(p);
                case DoorAccess.OnlyColonistsAndAnimalsAndFriendlies: return OnlyColonistsAndAnimalsAndFriendlies(p);
                case DoorAccess.OnlyDrafted: return OnlyDrafted(p);
                default: return true;
            }
        }

        public Material DoorStateMaterial
        {
            get
            {
                switch (curDoorAccess)
                {
                    case DoorAccess.Everyone: return MaterialPool.MatFrom("UI/Overlays/DoorStateOverlay_Green");
                    case DoorAccess.OnlyColonistsAndAnimals: return MaterialPool.MatFrom("UI/Overlays/DoorStateOverlay_Orange");
                    case DoorAccess.OnlyColonistsAndAnimalsAndFriendlies: return MaterialPool.MatFrom("UI/Overlays/DoorStateOverlay_Blue");
                    case DoorAccess.OnlyDrafted: return MaterialPool.MatFrom("UI/Overlays/DoorStateOverlay_Red");
                    default: return MaterialPool.MatFrom("UI/Overlays/DoorStateOverlay_Green");
                }
            }
        }
        public override void Draw()
        {
            base.Draw();
            if (!this.Open)
            {
                Vector3 drawPos = DrawPos;
                drawPos.y = AltitudeLayer.DoorMoveable.AltitudeFor() + 1;
                Graphics.DrawMesh(MeshPool.plane10, drawPos, base.Rotation.AsQuat, DoorStateMaterial, 0);
            }

        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            var command = new Command_Action();
            command.action = delegate ()
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (var doorState in doorStates)
                {
                    list.Add(new FloatMenuOption(doorState.Value, delegate
                    {
                        Log.Message("doorState.Key: " + doorState.Key);
                        curDoorAccess = doorState.Key;
                    }, MenuOptionPriority.Default));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            };
            command.defaultLabel = "RE.DoorLockState".Translate(doorStates[curDoorAccess]);
            command.defaultDesc = "RE.DoorLockStateDesc".Translate();
            command.disabled = !this.powerComp.PowerOn;
            command.disabledReason = "RE.DoorLockStatePowerOff".Translate();
            command.icon = Textures.DoorStateButton;
            yield return command;
        }
        private bool OnlyColonistsAndAnimals(Pawn p)
        {
            if (this.powerComp.PowerOn && p.Faction == this.Faction)
            {
                return true;
            }
            return false;
        }

        private bool OnlyColonistsAndAnimalsAndFriendlies(Pawn p)
        {
            if (this.powerComp.PowerOn && p.Faction != null && (p.Faction == this.Faction || !p.Faction.HostileTo(this.Faction)))
            {
                return true;
            }
            return false;
        }
        private bool OnlyDrafted(Pawn p)
        {
            if (this.powerComp.PowerOn && p.Faction == this.Faction && p.Drafted)
            {
                return true;
            }
            return false;
        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref curDoorAccess, "curDoorAccess");
        }
    }
}
