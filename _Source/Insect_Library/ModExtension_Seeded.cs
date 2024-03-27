using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Insect_Library
{
    public class ModExtension_Seeded : DefModExtension
    {
        public int tickToSpawnInsect = 210;
        public List<PawnKindDef> insects = new List<PawnKindDef>();
        public List<HediffDef> postGenerateHediffs = new List<HediffDef>();
        public List<ThingDef> postGenerateThings = new List<ThingDef>();
        public ThoughtDef giveThought = null;
        public SoundDef soundSpawned;
    }
}
