using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Insect_Library
{
    public class InsectPod : Building, IThingHolder, IOpenable
    {
		public InsectPod()
		{
			this.innerContainer = new ThingOwner<Pawn>(this, false, LookMode.Deep);
		}
        public bool HasPawn => !innerContainer.NullOrEmpty() && innerContainer.First() is Pawn;
		public Pawn Pawn => (Pawn)this.innerContainer?.First();
		public Hediff Seed 
		{
			get 
			{
				if (this.seed == null) 
				{
					this.seed = this.Pawn.health.hediffSet.GetFirstHediff<Hediff_Seeded>();
				}
				return this.seed;
			}
		}
        public ModExtension_InsectPod Extension => this.def.GetModExtension<ModExtension_InsectPod>();

        bool IOpenable.CanOpen => this.HasPawn;

        int IOpenable.OpenTicks => 500;

        public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
		{
			if (this.HasPawn)
			{
				Vector3 drawLoc2 = base.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.BuildingOnTop) + this.Extension.innerPawnDrawOffset;
				this.Pawn.Rotation = this.Rotation;
                this.Pawn.Drawer.renderer.DynamicDrawPhaseAt(phase, drawLoc2 + FloatingOffset(drawTick),null,false);
			}
			base.DynamicDrawPhaseAt(phase, drawLoc, flip);
		}
        public static Vector3 FloatingOffset(float tickOffset)
        {
            float num = tickOffset % 500f / 500f;
            float num2 = Mathf.Sin((float)Math.PI * num);
            float z = num2 * num2 * 0.02f;
            return new Vector3(z/2f, 0f, z);
        }
        private int damageTick;
        public void DamageApparel()
        {
            if (HasPawn)
            {
                if (!Pawn.apparel.AnyApparel) return;
                damageTick++;
                if (damageTick >= Extension.apparelDamageTick)
                {
                    for (int i = 0; i < Pawn.apparel.WornApparel.Count; i++)
                    {
                        Apparel item = Pawn.apparel.WornApparel[i];
                        if (item != null && item.def.useHitPoints)
                        {
                            item.HitPoints--;
                            if (item.HitPoints <= 0) item.Destroy();
                        }
                    }
                    damageTick = 0;
                }
            }
        }
        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
            this.Graphic.Draw(base.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Item), this.Rotation, this, 0f);
            if (this.bottomGraphic == null)
            {
                this.bottomGraphic = this.Extension.BottomGraphic.GraphicColoredFor(this);
            }
            this.bottomGraphic.Draw(base.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Item), this.Rotation, this, 0f);
        }
        public override void Tick()
        {
            base.Tick();
            if (this.HasPawn)
            {
                this.Seed?.Tick();
                this.Pawn.needs.NeedsTrackerTick();
                if (Pawn.needs?.food.CurLevel != null)
                {
                    Pawn.needs.food.CurLevel = 1f;
                }
                DamageApparel();
                drawTick++;
            }
            else if(!Destroyed)
            {
                Destroy();
            }
        }
		private int drawTick;
        public override void Destroy(DestroyMode mode = DestroyMode.KillFinalize)
		{
			if (this.HasPawn)
			{
				GenSpawn.Spawn(this.Pawn,this.Position,this.Map);
            }
            base.Destroy(mode);
        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
			foreach (Gizmo g in base.GetGizmos()) 
			{
				yield return g;	
			}
            if (this.HasPawn)
            {
				yield return ContainingSelectionUtility.SelectCarriedThingGizmo(this, this.Pawn) as Gizmo;
			}
            yield break;
		}
        public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
			drawTick = Rand.Range(-100, 100);
        }

        public ThingOwner GetDirectlyHeldThings()
		{
			return this.innerContainer;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
			{
                this
			});
		}

        void IOpenable.Open()
        {
            if (HasPawn)
            {
                EjectContents();
            }
        }
		public void EjectContents()
        {
			innerContainer.TryDrop(Pawn, ThingPlaceMode.Near, out Thing p);
        }
        public Hediff seed;
		private Graphic bottomGraphic;
		public ThingOwner innerContainer;
    }
}
