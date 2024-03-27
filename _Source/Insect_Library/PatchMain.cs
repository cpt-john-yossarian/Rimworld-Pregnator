using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Insect_Library
{
	[StaticConstructorOnStartup]
	public static class PatchMain
	{
		static PatchMain()
		{
			Harmony harmony = new Harmony("Insect_Patch");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
