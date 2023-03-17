/*
	This file is part of TweakScalerFSbuoyancy, a component of TweakScaleCompanion_FS
	© 2020-2023 LisiasT : http://lisias.net <support@lisias.net>

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
using KSPe.Annotations;
using TweakScaleCompanion.FS.Buoyancy.Integrator;

namespace TweakScaleCompanion.FS.Buoyancy
{
	public class TweakScalerFSbuoyancy : PartModule, Integrator.Listener
	{
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

		private Integrator.Notifier notifier = null;
		private bool IsRestoreNeeded = false;
		private bool IsInitNeeded = true;

		#region KSP Life Cycle

		public override void OnAwake()
		{
			Log.dbg("OnAwake {0}", this.InstanceID);
			base.OnAwake();
		}

		public override void OnStart(StartState state)
		{
			Log.dbg("OnStart {0} {1}", this.InstanceID, state);
			base.OnStart(state);

			// If the Integrator's DLL was not loaded, we are dead in the water.
			if (!(this.enabled = Startup.OK_TO_GO)) return;

			// Needed because I can't intialize this on OnAwake as this module can be awaken before ModuleWaterfallFX,
			// and OnRescale can be fired before OnLoad.
			if (null == this.notifier) this.IsInitNeeded = true;

			this.IsInitNeeded = true;
			this.IsRestoreNeeded = true;
			this.enabled = true;
		}

		public override void OnCopy(PartModule fromModule)
		{
			Log.dbg("OnCopy {0} {1:X}", this.InstanceID, fromModule.part.GetInstanceID());
			base.OnCopy(fromModule);

			// Needed because I can't intialize this on OnAwake as this module can be awaken before ModuleWaterfallFX,
			// and OnRescale can be fired before OnLoad.
			if (null == this.notifier) this.IsInitNeeded = true;

			this.IsRestoreNeeded = true;
			this.enabled = true;
		}

		public override void OnLoad(ConfigNode node)
		{
			Log.dbg("OnLoad {0} {1}", this.InstanceID, null == node ? "prefab" : node.name);
			base.OnLoad(node);
			if (null == node) return;   // Load from Prefab - not interesting.

			// Needed because I can't intialize this on OnAwake as this module can be awaken before ModuleWaterfallFX,
			// and OnRescale can be fired before OnLoad.
			if (null == this.notifier)
				this.IsInitNeeded = true;
			this.IsRestoreNeeded = true;
			this.enabled = true;
		}

		public override void OnSave(ConfigNode node)
		{
			Log.dbg("OnSave {0} {1}", this.InstanceID, null != node);
			base.OnSave(node);
		}

		#endregion


		#region Unity Life Cycle
		 
		[UsedImplicitly]
		private void Update()
		{
			if (this.IsInitNeeded)
			{
				if (this.InitModule()) return;	// Perhaps the Target Module is not initialized yet? Let's try again next round.

				this.notifier.Init();
				this.IsInitNeeded = false;
				Log.dbg("OnInitNeeded");
			}

			if (this.IsRestoreNeeded)
			{
				this.notifier.Update();
				this.IsRestoreNeeded = false;
				Log.dbg("OnRestoreNeeded");
			}

			this.enabled = false;
		}

		[UsedImplicitly]
		private void OnDestroy()
		{
			Log.dbg("OnDestroy {0}", this.InstanceID);

			// The object can be destroyed before the full initialization cycle while KSP is booting, so we need to check first.
			if (null == this.notifier) return;

			this.notifier.Destroy();
			this.notifier = null;
		}

		#endregion


		string Integrator.Listener.GetName()
		{
			return this.name;
		}

		void Integrator.Listener.NotifyRestoreNeeded()
		{
			this.enabled = this.IsRestoreNeeded = true;
		}

		void Listener.NotifyNewBuoyancyForceMax(float buoyancyForce)
		{
			this.buoyancyForceMax = buoyancyForce;
		}

		void Listener.NotifyNewRawBuoyancyData(float buoyancyForce)
		{
			this.rawBuoyancyData = string.Format("{0} / {1}", buoyancyForce, this.buoyancyForceMax);
			this.IsRestoreNeeded = true;
			this.enabled = true;
		}

		private bool InitModule()
		{	// Returns if the caller should call us again.
			try
			{
				System.Type type = KSPe.Util.SystemTools.Type.Find.ByInterfaceName("TweakScaleCompanion.FS.Buoyancy.Integrator.Notifier");
				System.Reflection.ConstructorInfo ctor = type.GetConstructor(new[] { typeof(Part), typeof(BaseField), typeof(Integrator.Listener) });
				this.notifier = (Integrator.Notifier) ctor.Invoke(new object[] { this.part, this.Fields["buoyancyPercent"], (Integrator.Listener)this });
			}
			catch (System.NullReferenceException e)
			{
				Log.error(this, e);
				return true;
			}
			return false;
		}

		private string InstanceID => string.Format("{0}:{1:X}", this.part.name, (null == this.part ? 0 : this.part.GetInstanceID()));
	}
}
