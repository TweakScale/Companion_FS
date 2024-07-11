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

namespace TweakScaleCompanion.FS.Buoyancy.Integrator
{
	public interface Listener
	{
		string GetName();
		void NotifyRestoreNeeded();
		void NotifyNewBuoyancyForceMax(float buoyancyForce);
        void NotifyNewRawBuoyancyData(float buoyancyForce);
    }

	public interface Notifier
	{
		bool IsEnabled();
		void Init();
		void Update();
        void Destroy();
    }
}
