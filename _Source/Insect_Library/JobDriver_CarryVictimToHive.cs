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
    public class JobDriver_CarryVictimToHive : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.TargetThingA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            ModExtension_PodJob extension = this.job.def.GetModExtension<ModExtension_PodJob>();
            yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.Touch);
            yield return Toils_General.Wait(extension.tickToBuildPod);
            yield return new Toil()
            {
                initAction = () =>
{
    InsectPod pod = (InsectPod)GenSpawn.Spawn(ThingMaker.MakeThing(extension.pod), this.TargetB.Cell, pawn.Map, Rot4.Random);
    pod.SetFaction(this.pawn.Faction);
    if (this.pawn.GetLord() is Lord lord)
    {
        lord.AddBuilding(pod);
    }
    Pawn p = (Pawn)this.TargetThingA;
    p.health.hediffSet.GetHediffsTendable().ToList().ListFullCopy().ForEach(h => h.Tended(10, 50));
    pod.innerContainer.TryAddOrTransfer(this.TargetThingA);
}
            };
            yield break;
        }
    }
}