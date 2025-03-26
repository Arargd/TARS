using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace TARS
{
    [BepInPlugin("org.bepinex.plugins.TARS", "TARS", "0.0.0.1")]
    class Loader : BaseUnityPlugin
    {
        void Awake()
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
