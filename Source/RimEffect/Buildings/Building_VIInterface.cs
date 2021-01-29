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
	public class Building_VIInterface : Building
	{
		private CompPowerTrader powerComp;
        private CompGlower compGlower;

        public bool CanUse => user == null && powerComp.PowerOn;

        private bool inUse;
		public bool InUse
        {
            get
            {
                var active = user != null && powerComp.PowerOn && user.Spawned && !user.Dead && user.CurJob.targetA.Thing == this;
                if (inUse != active && !active)
                {
                    inUse = active;
                    this.Disconnect();
                }
                return active;
            }
        }

		private Pawn user;
        private Pawn terminalAsPawn;

        public static Dictionary<Pawn, Building_VIInterface> terminals = new Dictionary<Pawn, Building_VIInterface>();
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			powerComp = GetComp<CompPowerTrader>();
            if (!respawningAfterLoad)
            {
                terminalAsPawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, this.Faction);
                terminalAsPawn.Name = new NameSingle(this.Label);
            }
            PawnComponentsUtility.AddComponentsForSpawn(terminalAsPawn);
            terminals[terminalAsPawn] = this;
            Log.Message("terminalAsPawn: " + terminalAsPawn);
        }

        public void Connect(Pawn user)
        {
            this.user = user;
            this.inUse = true;
            AddBlueGlower();
        }
        public void Disconnect()
        {
            this.user = null;
            this.inUse = false;
            RemoveBlueGlower();
        }

        public void ChatWithTerminal(Pawn user)
        {
            var interaction = Rand.Bool ? InteractionDefOf.Chitchat : InteractionDefOf.DeepTalk;
            if (!user.interactions.InteractedTooRecentlyToInteract())
            {
                var result = user.interactions.TryInteractWith(terminalAsPawn, interaction);
                Log.Message(terminalAsPawn + " - " + interaction + " - " + user + " - " + result);
            }
        }

        public void RemoveBlueGlower()
        {
            if (this.compGlower != null)
            {
                base.Map.glowGrid.DeRegisterGlower(this.compGlower);
                this.compGlower = null;
            }
        }
        public void AddBlueGlower()
        {
            RemoveBlueGlower();
            this.compGlower = new CompGlower();
            this.compGlower.parent = this;
            this.compGlower.Initialize(new CompProperties_Glower()
            {
                glowColor = new ColorInt(30, 144, 255),
                glowRadius = 3,
                overlightRadius = 0
            });

            base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things);
            base.Map.glowGrid.RegisterGlower(this.compGlower);
        }

        public override void SetFaction(Faction newFaction, Pawn recruiter = null)
        {
            base.SetFaction(newFaction, recruiter);
            terminalAsPawn.SetFaction(newFaction);
        }

        private Material interfaceMat;
        public Material InterfaceMat
        {
            get
            {
                if (interfaceMat is null)
                {
                    interfaceMat = MaterialPool.MatFrom("Things/Building/Joy/VITerminal/VIInterface");
                }
                if (compGlower is null)
                {
                    AddBlueGlower();
                }
                return interfaceMat;
            }
        }
        public override void Draw()
        {
            base.Draw();
            if (InUse)
            {
                Vector3 drawPos = DrawPos;
                drawPos.z += 0.35f;
                drawPos.y += 1;
                Graphics.DrawMesh(MeshPool.plane10, drawPos, base.Rotation.AsQuat, InterfaceMat, 0);
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_References.Look(ref user, "user");
            Scribe_Deep.Look(ref terminalAsPawn, "terminalAsPawn");
            Scribe_Values.Look(ref inUse, "inUse");
        }
    }
}
