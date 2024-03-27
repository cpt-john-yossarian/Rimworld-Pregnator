using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Insect_Library
{
    public class ModExtension_InsectPod : DefModExtension
    {
        public Vector3 innerPawnDrawOffset;
        public GraphicData BottomGraphic;
        public AnimationDef animationDef;
        public int apparelDamageTick = 1000;
    }
}
