using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace Insect_Library
{
    public class Hediff_Seeded : HediffWithComps
    {
        public ModExtension_Seeded Extension => def.GetModExtension<ModExtension_Seeded>();
        public float Progress => (float)progress / (float)Extension.tickToSpawnInsect;
        public override void Tick()
        {
            base.Tick();
            progress++;
            Severity = Progress;
            if (this.progress >= Extension.tickToSpawnInsect)
            {
                if (pawn.holdingOwner.Owner is InsectPod pod)
                {
                    Pawn pawn = (Pawn)GenSpawn.Spawn(PawnGenerator.GeneratePawn(Extension.insects.RandomElement(), pod.Faction), pod.Position, pod.MapHeld);
                    if (pod.GetLord() is Lord lord)
                    {
                        lord.AddPawn(pawn);
                    }
                    pod.seed = null;
                    FilthMaker.TryMakeFilth(pod.Position, pod.MapHeld, ThingDefOf.Filth_AmnioticFluid, this.pawn.LabelIndefinite(), 5);
                    Extension.soundSpawned.PlayOneShot(SoundInfo.InMap(new TargetInfo(pod.Position, pod.MapHeld)));
                }
                else //在Pod外生育
                {
                    Pawn pawn = (Pawn)GenSpawn.Spawn(PawnGenerator.GeneratePawn(Extension.insects.RandomElement(),Find.FactionManager.OfInsects), this.pawn.Position, this.pawn.MapHeld);
                    FilthMaker.TryMakeFilth(this.pawn.Position, this.pawn.MapHeld, ThingDefOf.Filth_AmnioticFluid, this.pawn.LabelIndefinite(), 5);
                    Extension.soundSpawned.PlayOneShot(SoundInfo.InMap(new TargetInfo(pawn.Position, pawn.MapHeld)));
                }
                if (Extension.postGenerateHediffs?.Count != 0)
                {
                    foreach (var item in Extension.postGenerateHediffs)
                    {
                        pawn.health.AddHediff(item);
                    }
                }            
                    pawn.health.RemoveHediff(this);
            }
        }
        private void TickMiscarry()
        {
            if ((!pawn.RaceProps.Humanlike || !Find.Storyteller.difficulty.babiesAreHealthy) && pawn.IsHashIntervalTick(1000))
            {
                if (pawn.needs.food != null && pawn.needs.food.CurCategory == HungerCategory.Starving)
                {
                    Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition);
                    if (firstHediffOfDef != null && firstHediffOfDef.Severity > 0.1f && Rand.MTBEventOccurs(2f, 60000f, 1000f))
                    {
                        if (PawnUtility.ShouldSendNotificationAbout(pawn))
                        {
                            string text = (pawn.Name.Numerical ? pawn.LabelShort : (pawn.LabelShort + " (" + pawn.kindDef.label + ")"));
                            Messages.Message("MessageMiscarriedStarvation".Translate(text, pawn), pawn, MessageTypeDefOf.NegativeHealthEvent);
                        }

                        Miscarry();
                        return;
                    }
                }

                if (IsSeverelyWounded && Rand.MTBEventOccurs(2f, 60000f, 1000f))
                {
                    if (PawnUtility.ShouldSendNotificationAbout(pawn))
                    {
                        string text2 = (pawn.Name.Numerical ? pawn.LabelShort : (pawn.LabelShort + " (" + pawn.kindDef.label + ")"));
                        Messages.Message("MessageMiscarriedPoorHealth".Translate(text2, pawn), pawn, MessageTypeDefOf.NegativeHealthEvent);
                    }

                    Miscarry();
                    return;
                }
            }
            void Miscarry()
            {
                //base.pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(DNP_DefOf.DNP_MiscarryAlien);
                base.pawn.health.RemoveHediff(this);
            }
        }
        private bool IsSeverelyWounded
        {
            get
            {
                float num = 0f;
                List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
                for (int i = 0; i < hediffs.Count; i++)
                {
                    if (hediffs[i] is Hediff_Injury && !hediffs[i].IsPermanent())
                    {
                        num += hediffs[i].Severity;
                    }
                }

                List<Hediff_MissingPart> missingPartsCommonAncestors = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
                for (int j = 0; j < missingPartsCommonAncestors.Count; j++)
                {
                    if (missingPartsCommonAncestors[j].IsFreshNonSolidExtremity)
                    {
                        num += missingPartsCommonAncestors[j].Part.def.GetMaxHealth(pawn);
                    }
                }

                return num > 38f * pawn.RaceProps.baseHealthScale;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.progress, "progress");
        }

        public int progress = 0;
    }
}
