namespace RimEffect
{
    using System;
    using System.Reflection;
    using HarmonyLib;
    using RimWorld;
    using UnityEngine;
    using Verse;
    using Object = UnityEngine.Object;

    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipment")]
    public static class MeleeAttack_Patch
    {
        private static readonly Graphic graphic = GraphicDatabase.Get(typeof(Graphic_Single), @"Things/AbilityEffects/Omniblade/Omniblade", ShaderTypeDefOf.Cutout.Shader,
                                                                      new Vector2(1, 1), Color.white, Color.white);

        delegate bool CarryWeaponOpenly(PawnRenderer instance);

        static CarryWeaponOpenly carryOpenlyDelegate = AccessTools.MethodDelegate<CarryWeaponOpenly>(AccessTools.Method(typeof(PawnRenderer), "CarryWeaponOpenly"));

        [HarmonyPostfix]
        public static void Postfix(PawnRenderer __instance, Pawn ___pawn)
        {
            Pawn pawn = ___pawn;

            if (pawn.equipment?.PrimaryEq is null)
            {
                if ((pawn.stances.curStance is Stance_Busy stanceBusy && !stanceBusy.neverAimWeapon && stanceBusy.focusTarg.IsValid) || carryOpenlyDelegate(__instance))
                {
                    if (pawn?.health.hediffSet.HasHediff(RE_DefOf.RE_OmniToolHediff) ?? false)
                    {
                        Vector3 drawLoc  = pawn.DrawPos;
                        float   aimAngle = 0f;

                        if (pawn.Rotation == Rot4.South)
                        {
                            drawLoc   += new Vector3(0f, 0f, -0.22f);
                            drawLoc.y += 9f / 245f;
                            aimAngle  =  143f;
                        }
                        else if (pawn.Rotation == Rot4.North)
                        {
                            drawLoc   += new Vector3(0f, 0f, -0.11f);
                            drawLoc.y += 0f;
                            aimAngle  =  143f;
                        }
                        else if (pawn.Rotation == Rot4.East)
                        {
                            drawLoc   += new Vector3(0.2f, 0f, -0.22f);
                            drawLoc.y += 9f / 245f;
                            aimAngle  =  143f;
                        }
                        else if (pawn.Rotation == Rot4.West)
                        {
                            drawLoc   += new Vector3(-0.2f, 0f, -0.22f);
                            drawLoc.y += 9f / 245f;
                            aimAngle  =  217f;
                        }



                        Mesh  mesh;
                        float num                 = aimAngle - 90f;
                        float equippedAngleOffset = -65f;

                        if (aimAngle > 20f && aimAngle < 160f)
                        {
                            mesh =  MeshPool.plane10;
                            num  += equippedAngleOffset;
                        }
                        else if (aimAngle > 200f && aimAngle < 340f)
                        {
                            mesh =  MeshPool.plane10Flip;
                            num  -= 180f;
                            num  -= equippedAngleOffset;
                        }
                        else
                        {
                            mesh =  MeshPool.plane10;
                            num  += equippedAngleOffset;
                        }

                        num %= 360f;
                        Graphics.DrawMesh(material: graphic.MatSingle, mesh: mesh, position: drawLoc, rotation: Quaternion.AngleAxis(num, Vector3.up), layer: 0);
                    }
                }
            }
        }
    }
}