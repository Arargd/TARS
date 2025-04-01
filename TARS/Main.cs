using UnityEngine;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace TARS
{
	public class Main : MonoBehaviour
	{
		private bool setAnim;
		private PlayerCharacter WobPlayer;
		private Animator WobAnim;
		private GameObject wobblerobj;
		public static VocalBank wobblervocal;

		public bool loadedwobbler = false;

		public static Main instance;

		public void Awake()
		{
			instance = this;
			DontDestroyOnLoad(this);
		}

		public void Update()
		{
			if (setAnim)
			{
				{
					if (WobAnim != null)
					{
						WobAnim.SetBool("IsGrounded", WobPlayer.data.mostlyGrounded);
						WobAnim.SetFloat("Speed", WobPlayer.refs.rig.velocity.magnitude / 100f);
					}
				}
			}
		}

		public void WobblerTime()
		{
			if (!loadedwobbler)
			{
				string wobblepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "wobbler");
				AssetBundle ab = AssetBundle.LoadFromFile(wobblepath);
				if (ab == null)
				{
					Debug.LogError("Failed to load asset bundle.");
					return;
				}
				wobblerobj = ab.LoadAllAssets<GameObject>().FirstOrDefault();
				wobblervocal = ab.LoadAllAssets<VocalBank>().FirstOrDefault();
				loadedwobbler = true;
				Debug.Log("Wobbler assetbundle loaded!"); 
			}
			var wobbler = Instantiate(wobblerobj);

			GameObject playerVisual = GameObject.Find("/Player/Visual");
			if (playerVisual == null)
			{
				Debug.LogError("Player/Visual not found!");
				return;
			}

			wobbler.transform.SetParent(playerVisual.transform);
			wobbler.transform.localPosition = Vector3.zero; 
			wobbler.transform.rotation = new Quaternion(0, 0, 0, 0);

			GameObject courierRetake = GameObject.Find("/Player/Visual/Courier_Retake");
			if (courierRetake != null)
			{
				courierRetake.SetActive(false);
			}

			GameObject player = GameObject.Find("/Player");
			if (player != null)
			{
				PlayerAnimationHandler animationHandler = player.GetComponent<PlayerAnimationHandler>();
				if (animationHandler != null)
				{
					Animator wobblerAnimator = wobbler.GetComponent<Animator>();
					if (wobblerAnimator != null)
					{
						animationHandler.animator = wobblerAnimator;
						WobAnim = animationHandler.animator;
						WobPlayer = player.GetComponentInChildren<PlayerCharacter>();
						if (PlayerVocalSFX.Instance != null)
						{
							PlayerVocalSFX.Instance.vocalBank = wobblervocal;
						}
						else
						{
							Debug.LogError("Player Vocals Null.");
						}
					}
					else
					{
						Debug.LogError("wobbler does not have an Animator component!");
					}
				}
				else
				{
					Debug.LogError("PlayerAnimationHandler script not found on Player!");
				}
			}
			else
			{
				Debug.LogError("Player object not found!");
			}
			setAnim = true;
		}

		[HarmonyPatch(typeof(PlayerVocalSFX), "Start")]
		public class LoadVocals
		{

			public static void Postfix()
			{
				PlayerVocalSFX.Instance.vocalBank = wobblervocal;
			}

		}

		[HarmonyPatch(typeof(PlayerAnimationHandler), "Awake")]
		public class LoadAfterLevel
		{

			public static void Postfix()
			{
				if (instance == null)
				{
					Debug.LogError("Null instance");
				}
				else
				{
					instance.WobblerTime();
				}
			}

		}
	}
}
