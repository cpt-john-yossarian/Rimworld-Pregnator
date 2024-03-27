using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Insect_Library
{
    [HarmonyPatch(typeof(PawnUtility), "GetPosture")]
    public class Patch_Posture
    {
        [HarmonyPostfix]
        public static void postfix(Pawn p, ref PawnPosture __result)
        {
            if (p.holdingOwner?.Owner is InsectPod)
            {
                __result = PawnPosture.Standing;
            }
        }
    }
}
