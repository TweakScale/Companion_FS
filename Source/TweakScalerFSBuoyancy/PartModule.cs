/*
	This file is part of TweakScalerFSbuoyancy, a component of TweakScaleCompanion_FS
	© 2020 LisiasT : http://lisias.net <support@lisias.net>

	TweakScaleCompanion_FS is double licensed, as follows:

	* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
	* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	And you are allowed to choose the License that better suit your needs.

	TweakScaleCompanion_FS is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the SKL Standard License 1.0
	along with TweakScaleCompanion_FS. If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

	You should have received a copy of the GNU General Public License 2.0
	along with TweakScaleCompanion_FS. If not, see <https://www.gnu.org/licenses/>.

*/
using System;

using TweakScale;
using TweakScale.Annotations;

namespace TweakScaleCompanion.FS.Buoyancy
{
	public class TweakScalerFSbuoyancy : PartModule
	{
		private const string TARGETFIELDNAME = "buoyancyForce";

		#region KSP UI

		[KSPField(isPersistant=true, guiName="Buoyancy (%)", guiActive = false, guiActiveEditor = true, guiUnits = "%"), UI_FloatRange(minValue = 0f, maxValue = 100f, stepIncrement = 1f)]
		public float buoyancyPercent = 100f;

		[KSPField(isPersistant=false, guiName="Raw Buoyancy", guiActive = false, guiActiveEditor = true)]
		public string rawBuoyancyData = "xxx / xxx";

		#endregion


		#region Part Module Fields

        /// <summary>
        /// Whether the Helper was deativated by some reason (usually the Sanity Checks)
        /// </summary>
        [KSPField(isPersistant = false)]
        public bool isActive = true;

		[KSPField(isPersistant=false)]
		public float buoyancyForceMax = -1f;

		#endregion

		public float buoyancyForceDefault = -1f;

		private TweakScale.TweakScale tweakscale;
		private FSbuoyancy targetPartModule;

		private BaseField myField;
		private UI_FloatRange myUiControl;

		private bool IsRestoreNeeded = false;
		private bool IsInitNeeded = true;

		#region KSP Life Cycle

		public override void OnAwake()
		{
			Log.dbg("OnAwake {0}:{1:X}", this.name, this.part.GetInstanceID());
			base.OnAwake();
		}

		public override void OnStart(StartState state)
		{
			Log.dbg("OnStart {0}:{1:X} {2}", this.name, this.part.GetInstanceID(), state);
			base.OnStart(state);

			// Needed because I can't intialize this on OnAwake as this module can be awaken before FSbuoyancy or TweakScale,
			// and OnRescale can be fired before OnLoad.
			if (null == this.targetPartModule) this.InitModule();

			this.IsInitNeeded = true;
			this.IsRestoreNeeded = true;
		}

		public override void OnCopy(PartModule fromModule)
		{
			Log.dbg("OnCopy {0}:{1:X} from {2:X}", this.name, this.part.GetInstanceID(), fromModule.part.GetInstanceID());
			base.OnCopy(fromModule);

			// Needed because I can't intialize this on OnAwake as this module can be awaken before FSbuoyancy,
			// and OnRescale can be fired before OnLoad.
			if (null == this.targetPartModule) this.InitModule();

			this.IsRestoreNeeded = true;
		}

		public override void OnLoad(ConfigNode node)
		{
			Log.dbg("OnLoad {0}:{1:X} {2}", this.name, this.part.GetInstanceID(), null == node ? "prefab" : node.name);
			base.OnLoad(node);
			if (null == node) return;   // Load from Prefab - not interesting.

			// Needed because I can't intialize this on OnAwake as this module can be awaken before FSbuoyancy,
			// and OnRescale can be fired before OnLoad.
			if (null == this.targetPartModule)
			{
				this.InitModule();
				this.IsInitNeeded = true;
			}
			this.IsRestoreNeeded = true;
		}

		public override void OnSave(ConfigNode node)
		{
			Log.dbg("OnSave {0}:{1:X} {2}", this.name, this.part.GetInstanceID(), null != node);
			base.OnSave(node);
		}

		#endregion


		#region Unity Life Cycle
		 
		[UsedImplicitly]
		private void Update()
		{
			if (this.IsInitNeeded)
			{
				this.InitInternalData();
				this.IsInitNeeded = false;
				Log.dbg("OnInitNeeded");
			}

			if (this.IsRestoreNeeded)
			{
				this.RescaleMaxBuoyancy();
				this.UpdateTarget();
				this.RefreshUI();
				this.IsRestoreNeeded = false;
				Log.dbg("OnRestoreNeeded");
			}
		}

		[UsedImplicitly]
		private void OnDestroy()
		{
			Log.dbg("OnDestroy {0}:{1:X}", this.name, this.part.GetInstanceID());

			// The object can be destroyed before the full initialization cycle while KSP is booting, so we need to check first.
			if (null == this.targetPartModule) return;

			this.DeInitUiControl();
		}

		#endregion


		#region Part Events Handlers

		internal void OnRescale(ScalingFactor factor)
		{
			Log.dbg("OnRescale {0}:{1:X} to {2}", this.name, this.part.GetInstanceID(), factor.ToString());

			// Needed because I can't intialize this on OnAwake as this module can be awaken before FSbuoyancy,
			// and OnRescale can be fired before OnLoad.
			if (null == this.targetPartModule)
			{
				this.InitInternalData();
			}

			this.RescaleMaxBuoyancy();
			this.UpdateTarget();
			this.RefreshUI();
		}

		private void OnMyBuyoancyFieldChange(BaseField field, object what)
		{
			Log.dbg("OnMyBuyoancyFieldChange {0}:{1:X}", field.name, this.part.GetInstanceID());

			this.RescaleMaxBuoyancy();
			this.UpdateTarget();
			this.RefreshUI();
		}

		#endregion

		private void InitModule()
		{
			this.tweakscale = this.part.Modules.GetModule<TweakScale.TweakScale>();
			this.targetPartModule = this.part.Modules.GetModule<FSbuoyancy>();
			if (null == this.targetPartModule || !this.targetPartModule.enabled)
			{
				this.enabled = false;
				return;
			}

			this.myField = this.Fields["buoyancyPercent"];
			this.myUiControl = (this.myField.uiControlEditor as UI_FloatRange);
			this.myUiControl.onFieldChanged += this.OnMyBuyoancyFieldChange;
		}

		private void InitInternalData()
		{
			{	// KSP insists on overwritting this info at Scene changing...
				// And I don't think it's a good idea to overwrite the datum on the prefab - but I can be convinced otherwise :)
				BaseField bf = this.targetPartModule.Fields[TARGETFIELDNAME];
				bf.guiActive = false;
				bf.guiActiveEditor = false;
			}

			this.buoyancyForceDefault = (this.part.partInfo.partPrefab.Modules.GetModule<FSbuoyancy>().Fields[TARGETFIELDNAME].uiControlEditor as UI_FloatRange).maxValue;

			this.RefreshInternalData();
			this.RefreshUI();
		}

		private void DeInitUiControl()
		{
			{
				BaseField bf = this.targetPartModule.Fields[TARGETFIELDNAME];
				bf.guiActive = false;
				bf.guiActiveEditor = true;
			}
		}

		private void RefreshInternalData()
		{
			this.RescaleMaxBuoyancy();
			this.targetPartModule.buoyancyForce = (float)Math.Truncate(this.buoyancyPercent * this.buoyancyForceMax / 100f);
			Log.dbg("RefreshInternalData {0}:{1:X} to {2} / {3}", this.name, this.part.GetInstanceID(), this.buoyancyForceDefault, this.buoyancyPercent);
		}

		private void RescaleMaxBuoyancy()
		{
			this.buoyancyForceMax = (float)Math.Truncate(this.buoyancyForceDefault * this.tweakscale.CurrentScaleFactor);
		}

		private void UpdateTarget()
		{
			this.targetPartModule.buoyancyForce = (float)Math.Truncate(this.buoyancyForceMax * this.buoyancyPercent / 100f);
			Log.dbg("this.targetPartModule.buoyancyForce {0}:{1:X} = {2}", this.name, this.part.GetInstanceID(), this.targetPartModule.buoyancyForce);
		}

		private void RefreshUI()
		{
			this.rawBuoyancyData = string.Format("{0} / {1}", this.targetPartModule.buoyancyForce, this.buoyancyForceMax);
		}

		private static KSPe.Util.Log.Logger Log = KSPe.Util.Log.Logger.CreateForType<TweakScalerFSbuoyancy>("TweakScaleCompanion_FS", "TweakScalerFSbuoyancy");

		static TweakScalerFSbuoyancy()
		{
			Log.level =
#if DEBUG
				KSPe.Util.Log.Level.TRACE
#else
				KSPe.Util.Log.Level.INFO
#endif
				;
		}
	}

	public class Scaler : TweakScale.IRescalable<TweakScalerFSbuoyancy>
	{
		private readonly TweakScalerFSbuoyancy pm;

		public Scaler(TweakScalerFSbuoyancy pm)
		{
			this.pm = pm;
		}

		public void OnRescale(ScalingFactor factor)
		{
			this.pm.OnRescale(factor);
		}
	}
}
