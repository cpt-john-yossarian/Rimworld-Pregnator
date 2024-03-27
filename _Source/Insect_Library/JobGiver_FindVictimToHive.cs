using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Insect_Library
{
    public class JobGiver_FindVictimToHive : ThinkNode_JobGiver
    {
            
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn != null && pawn.Spawned && GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.Touch, TraverseParms.For(pawn), 30, t => t is Pawn p
                 && p.Downed && !p.Dead && p.gender == Gender.Female && p.RaceProps.intelligence == Intelligence.Humanlike && pawn.CanReserve(p)) is Pawn target && this.FindClosestHive(pawn) is Hive hive)
            {
                ModExtension_PodJob extension = this.buildPodJob.GetModExtension<ModExtension_PodJob>();
                List<IntVec3> targetPoss = GenRadial.RadialCellsAround(hive.Position, extension.buildPodRadius, false).ToList().FindAll(p => pawn.CanReach(p, PathEndMode.Touch, Danger.Deadly) && p.GetFirstBuilding(pawn.Map) == null);
                if (targetPoss.Any())
                {
                    Job job = JobMaker.MakeJob(this.buildPodJob, target, targetPoss.RandomElement());
                    job.count = 1;
                    return job;
                }
            }
            return null;
        }

        private Hive FindClosestHive(Pawn pawn)
        {
            return (Hive)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.Hive), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn));
        }

        public JobDef buildPodJob;
    }
}
