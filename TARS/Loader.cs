using HarmonyLib;
using UnityEngine;
using Landfall.Modding;

namespace TARS
{
    [LandfallPlugin]
    class Loader : MonoBehaviour
    {
        static Loader()
        {
			Harmony harmony = new Harmony("TARS");
			harmony.PatchAll();
			Debug.Log("======Loaded TARS Patches======");
		}
    }

	[HarmonyPatch(typeof(GameHandler), "Awake")]
	public class InitPatch
	{

		public static void Postfix()
		{
			var go = new GameObject("TARS");
			UnityEngine.Object.DontDestroyOnLoad(go);
			go.AddComponent<Main>();
			Debug.Log("======Loaded TARS Successfully======");
		}

	}
}
