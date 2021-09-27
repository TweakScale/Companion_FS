/*
	This file is part of TweakScaleCompanion_FS
	© 2020-21 LisiasT : http://lisias.net <support@lisias.net>

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
using UnityEngine;
using KSPe.Annotations;

namespace TweakScaleCompanion.FS
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public class Startup : MonoBehaviour
	{
		[UsedImplicitly]
		private void Start()
		{
			Log.force("Version {0}", Version.Text);

			try
			{
				KSPe.Util.Installation.Check<Startup>("TweakScaleCompanion_FS", "FS", Version.Vendor);
			}
			catch (KSPe.Util.InstallmentException e)
			{
				Log.error(e.ToShortMessage());
				KSPe.Common.Dialogs.ShowStopperAlertBox.Show(e);
			}

			this.checkDependencies();
		}

		private void checkDependencies()
		{
			if (KSPe.Util.SystemTools.Assembly.Finder.ExistsByName("Scale"))
			{
				System.Reflection.Assembly assembly = KSPe.Util.SystemTools.Assembly.Finder.FindByName("Scale");
				Log.detail("Found {0}", assembly.FullName);
				if (assembly.GetName().Version.CompareTo(new System.Version(2, 4, 6)) >= 0) return;
			}
			GUI.UnmetRequirementsShowStopperAlertBox.Show("TweakScale v2.4.6 or superior");
		}
	}
}
