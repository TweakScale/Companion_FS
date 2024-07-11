/*
	This file is part of TweakScalerFSbuoyancyIntegrator, a component of TweakScaleCompanion_FS
		© 2020-2024 LisiasT : http://lisias.net <support@lisias.net>

	TweakScaleCompanion_FS is double licensed, as follows:
		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	And you are allowed to choose the License that better suit your needs.

	TweakScaleCompanion_Frameworks is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the SKL Standard License 1.0
	along with TweakScaleCompanion_Frameworks. If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

	You should have received a copy of the GNU General Public License 2.0
	along with TweakScaleCompanion_Frameworks. If not, see <https://www.gnu.org/licenses/>.

*/
using UnityEngine;
using KSPe.Annotations;

namespace TweakScaleCompanion.FS.Buoyancy
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public class Startup : MonoBehaviour
	{
		internal static bool OK_TO_GO = false;	// If we can't load the Integrator, there's no point on dry running the PartModule...

		[UsedImplicitly]
		private void Awake()
		{
			if (KSPe.Util.SystemTools.Type.Exists.ByQualifiedName(".FSbuoyancy")) // Ugh... Classes without Namespace is a Use Case that I missed on KSPe...
				// Pegadinha do malandro: If I use <Version>, the path will be resolved to "./GameData/TweakScaleCompanion/FS/Buoyancy", what's
				// not exactly what we are aiming for! :)
				using(KSPe.Util.SystemTools.Assembly.Loader a = new KSPe.Util.SystemTools.Assembly.Loader<FS.Startup>())
				{ 
					a.LoadAndStartup("TweakScalerFSBuoyancyIntegrator");
					OK_TO_GO = true;
				}
		}
	}
}
