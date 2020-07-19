﻿/*
	This file is part of TweakScaleCompanion_FS
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
using System.Collections;

using UnityEngine;

namespace TweakScaleCompanion.FS
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	internal class SanityCheck : MonoBehaviour
	{
		private static readonly int WAIT_ROUNDS = 120; // @60fps, would render 2 secs.

		private const string FSBUOYANCY_MODULE_NAME = "FSbuoyancy";
		private const string SCALERFSBUOYANCY_MODULE_NAME = "TweakScalerFSbuoyancy";

		internal static bool isConcluded = false;

		private void Start()
		{
			StartCoroutine("SanityCheckCoroutine");
		}

		private IEnumerator SanityCheckCoroutine()
		{
			SanityCheck.isConcluded = false;
			Log.info("SanityCheck: Started");

			{  // Toe Stomping Fest prevention
				for (int i = WAIT_ROUNDS; i >= 0 && null == PartLoader.LoadedPartsList; --i)
				{
					yield return null;
					if (0 == i) Log.warn("Timeout waiting for PartLoader.LoadedPartsList!!");
				}
	
				 // I Don't know if this is needed, but since I don't know that this is not needed,
				 // I choose to be safe than sorry!
				{
					int last_count = int.MinValue;
					for (int i = WAIT_ROUNDS; i >= 0; --i)
					{
						if (last_count == PartLoader.LoadedPartsList.Count) break;
						last_count = PartLoader.LoadedPartsList.Count;
						yield return null;
						if (0 == i) Log.warn("Timeout waiting for PartLoader.LoadedPartsList.Count!!");
					}
				}
			}

			int total_count = 0;
			int parts_with_buoyancy_count = 0;
			int showstoppers_count = 0;

			foreach (AvailablePart p in PartLoader.LoadedPartsList)
			{
				for (int i = WAIT_ROUNDS; i >= 0 && (null == p.partPrefab || null == p.partPrefab.Modules); --i)
				{
					yield return null;
					if (0 == i) Log.error("Timeout waiting for {0}.prefab.Modules!!", p.name);
				}
			  
				Part prefab;
				{ 
					// Historically, we had problems here.
					// However, that co-routine stunt appears to have solved it.
					// But we will keep this as a ghinea-pig in the case the problem happens again.
					int retries = WAIT_ROUNDS;
					bool containsBuoyancy = false;
					Exception culprit = null;
					
					prefab = p.partPrefab; // Reaching the prefab here in the case another Mod recreates it from zero. If such hypothecical mod recreates the whole part, we're doomed no matter what.
					
					while (retries > 0)
					{ 
						bool should_yield = false;
						try 
						{
							containsBuoyancy = prefab.Modules.Contains(FSBUOYANCY_MODULE_NAME) || prefab.Modules.Contains(SCALERFSBUOYANCY_MODULE_NAME);
							++total_count;
							break;  // Yeah. This while stunt was done just to be able to do this. All the rest is plain clutter! :D 
						}
						catch (Exception e)
						{
							culprit = e;
							--retries;
							should_yield = true;
						}
						if (should_yield) // This stunt is needed as we can't yield from inside a try-catch!
							yield return null;
					}

					if (0 == retries)
					{
						Log.error("Exception on {0}.prefab.Modules.Contains: {1}", p.name, culprit);
						Log.detail("{0}", prefab.Modules);
						continue;
					}

					try
					{
						string due = null;

						if (containsBuoyancy)
						{
							if (null != (due = this.checkForInventory(prefab)))
							{
								Log.info("Removing scaling support for {0} ({1}) due {2}.", p.name, p.title, due);
								if (prefab.Modules.Contains(FSBUOYANCY_MODULE_NAME))
									prefab.RemoveModule(prefab.Modules[FSBUOYANCY_MODULE_NAME]);
								if (prefab.Modules.Contains(SCALERFSBUOYANCY_MODULE_NAME))
									prefab.RemoveModule(prefab.Modules[SCALERFSBUOYANCY_MODULE_NAME]);
							}
							else ++parts_with_buoyancy_count;
						}

					}
					catch (Exception e)
					{
						++showstoppers_count;
						Log.error("part={0} ({1}) Exception on Sanity Checks: {2}", p.name, p.title, e);
					}
					// End of hack. Ugly, uh? :P
				}
#if DEBUG
				{
					Log.dbg("Found part named {0} ; title {1}:", p.name, p.title);
					foreach (PartModule m in prefab.Modules)
						Log.dbg("\tPart {0} has module {1}", p.name, m.moduleName);
				}
#endif
			}

			Log.info("SanityCheck Concluded : {0} parts found ; {1} parts using {2} ; {3} show stoppers detected .", total_count, parts_with_buoyancy_count, FSBUOYANCY_MODULE_NAME, showstoppers_count);
			SanityCheck.isConcluded = true;

			if (showstoppers_count > 0)
			{
				GUI.ShowStopperAlertBox.Show(showstoppers_count);
			}
		}

		private const string MSG_KSP_NO_SUPPORTED = "your KSP version doesn't need it.";
		private const string MSG_PART_DOES_NOT_NEED = "this part doesn't need it.";
		private const string MSG_PART_NOT_SUPPORTED = "this part is not supported.";
		private const string MSG_PART_IMPROPERLY_PATCHED = "this part is improperly patched.";
		private string checkForInventory(Part p)
		{
			Log.dbg("Checking {0} Sanity for {1} at {2}", SCALERFSBUOYANCY_MODULE_NAME, p.name, (null != p.partInfo ? p.partInfo.partUrl : "<no partInfo>"));

			if (!p.Modules.Contains(FSBUOYANCY_MODULE_NAME)) return MSG_PART_DOES_NOT_NEED;
			if (!p.Modules.Contains(SCALERFSBUOYANCY_MODULE_NAME)) return MSG_PART_IMPROPERLY_PATCHED;
			if (p.name.StartsWith("kerbalEVA")) return MSG_PART_NOT_SUPPORTED;
			if (p.name.StartsWith("maleEVA")) return MSG_PART_NOT_SUPPORTED;
			if (p.name.StartsWith("femaleEVA")) return MSG_PART_NOT_SUPPORTED;

			return null;
		}

	}
}
