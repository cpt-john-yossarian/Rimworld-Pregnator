using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Insect_Library
{
    public class ModExtension_PodJob : DefModExtension
    {
        public int tickToSeed = 210;
        public HediffDef seedHediff;

        public int tickToBuildPod = 2100;
        public int buildPodRadius = 7;
        public ThingDef pod;
    }
}
