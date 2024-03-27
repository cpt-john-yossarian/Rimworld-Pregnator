using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace Insect_Library
{
    public class JobGiver_Seed : ThinkNode_JobGiver
    {
            
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn != null && pawn.Spawned && pawn.GetLord() is Lord lord && lord.ownedBuildings.Find(b => b is InsectPod pod && pod.HasPawn && IsValidTarget(pod.Pawn)) is InsectPod target 
                && pawn.CanReserveAndReach(target,PathEndMode.Touch,Danger.Deadly))
            {
                return JobMaker.MakeJob(this.seekJob,target);
            }
            return null;
        }
        protected bool IsValidTarget(Pawn pawn)
        {
            return !pawn.health.hediffSet.HasHediff(this.seedHediff) && !pawn.apparel.AnyApparel;
        }

        public HediffDef seedHediff;
        public JobDef seekJob;
    }
}
