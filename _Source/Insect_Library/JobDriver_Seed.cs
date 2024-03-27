using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace Insect_Library
{
    public class JobDriver_Seed : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.TargetThingA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            ModExtension_PodJob extension = this.job.def.GetModExtension<ModExtension_PodJob>();
            yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.ClosestTouch);
            Toil toil = Toils_General.WaitWith(TargetIndex.A, extension.tickToSeed, true);
            toil.AddPreTickAction(() => this.pawn.rotationTracker.FaceTarget(TargetThingA));
            yield return toil;
            yield return new Toil()
            {
                initAction = () =>
                {
                    InsectPod pod = (InsectPod)this.TargetThingA;
                    Hediff_Seeded seed = (Hediff_Seeded)pod.Pawn.health.AddHediff(extension.seedHediff);
                }
            };
            yield break;
        }
    }
}
