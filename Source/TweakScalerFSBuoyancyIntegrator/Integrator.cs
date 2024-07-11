/*
	This file is part of TweakScalerFSbuoyancyIntegrator, a component of TweakScaleCompanion_FS
		© 2020-2024 LisiasT : http://lisias.net <support@lisias.net>

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

namespace TweakScaleCompanion.FS.Buoyancy.Integrator
{
	public class Implementation : IRescalable, Notifier
	{
		private const string TARGETFIELDNAME = "buoyancyForce";

		private readonly Listener listener;
		private readonly Part part;
		private readonly TweakScale.TweakScale tweakscale;
		private readonly FSbuoyancy targetPartModule;

		private readonly BaseField myField;
		private readonly UI_FloatRange myUiControl;

		private float buoyancyPercent = 100f;
		private float buoyancyForceMax = -1f;
		private float buoyancyForceDefault = -1f;

		public Implementation(Part part, BaseField myField, Listener listener)
		{
			this.listener = listener;
			this.part = part;

			this.tweakscale = this.part.Modules.GetModule<TweakScale.TweakScale>();
			if (null == this.tweakscale) throw new System.NullReferenceException("TweakScale not found!");

			this.targetPartModule = this.part.Modules.GetModule<FSbuoyancy>();
			if (null == this.targetPartModule)  throw new System.NullReferenceException("FSbuoyancy not found!");

			this.myField = myField;
			this.myUiControl = (this.myField.uiControlEditor as UI_FloatRange);
			this.myUiControl.onFieldChanged += this.OnMyBuyoancyFieldChange;
		}

		void Notifier.Destroy()
		{
			{
				BaseField bf = this.targetPartModule.Fields[TARGETFIELDNAME];
				bf.guiActive = false;
				bf.guiActiveEditor = true;
			}
		}

		bool Notifier.IsEnabled()
		{
			return this.IsEnabled();
		}

		void Notifier.Init()
		{
			Log.dbg("InitInternalData {0}", this.InstanceID);

			{	// KSP insists on overwritting this info at Scene changing...
				// And I don't think it's a good idea to overwrite the datum on the prefab - but I can be convinced otherwise :)
				BaseField bf = this.targetPartModule.Fields[TARGETFIELDNAME];
				bf.guiActive = false;
				bf.guiActiveEditor = false;
			}

			this.buoyancyForceDefault = (
					this.part.partInfo.partPrefab.Modules.GetModule<FSbuoyancy>().Fields[TARGETFIELDNAME].uiControlEditor as UI_FloatRange
				).maxValue;

			this.RefreshInternalData();
			this.RefreshUI();
		}

		void Notifier.Update()
		{
			ScalingFactor factor = this.tweakscale.ScalingFactor;
			Log.dbg("UpdateTarget {0} by {1}", this.InstanceID, factor.absolute.linear);
			this.RescaleMaxBuoyancy();
			this.UpdateTarget();
			this.RefreshUI();
		}


		#region Part Events Handlers

		private void OnMyBuyoancyFieldChange(BaseField field, object what)
		{
			if (!this.IsEnabled()) return;

			Log.dbg("OnMyBuyoancyFieldChange {0}:{1:X}", field.name, this.part.GetInstanceID());

			this.listener.NotifyRestoreNeeded();
		}

		void IRescalable.OnRescale(ScalingFactor factor)
		{
			if (!this.IsEnabled()) return;

			Log.dbg("OnRescale {0} to {1}", this.InstanceID, factor.ToString());

			this.listener.NotifyRestoreNeeded();
		}

		#endregion


		private bool IsEnabled()
		{
			bool enabled = this.targetPartModule.enabled;
			return enabled;
		}

		private void RescaleMaxBuoyancy()
		{
			this.listener.NotifyNewBuoyancyForceMax(this.buoyancyForceMax=
				(float)Math.Truncate(this.buoyancyForceDefault * this.tweakscale.CurrentScaleFactor)
			);
		}

		private void UpdateTarget()
		{
			this.targetPartModule.buoyancyForce = (float)Math.Truncate(this.buoyancyForceMax * this.buoyancyPercent / 100f);
			Log.dbg("this.targetPartModule.buoyancyForce {0} = {1}", this.InstanceID, this.targetPartModule.buoyancyForce);
		}

		private void RefreshUI()
		{
			this.listener.NotifyNewRawBuoyancyData(this.targetPartModule.buoyancyForce);
		}

		private void RefreshInternalData()
		{
			this.RescaleMaxBuoyancy();
			this.targetPartModule.buoyancyForce = (float)Math.Truncate(this.buoyancyPercent * this.buoyancyForceMax / 100f);
			Log.dbg("RefreshInternalData {0} to {1} / {2}", this.InstanceID, this.buoyancyForceDefault, this.buoyancyPercent);
		}

		private string InstanceID => string.Format("{0}:{1:X}", this.part.name, (null == this.part ? 0 : this.part.GetInstanceID()));
	}
}
