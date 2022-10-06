using System;
using System.Linq;
using UnityEngine;

namespace BreathLib.Unity
{
	public class CASController : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("If true, this object and its manager will be destroyed when the scene is changed.")]
		private bool DestroyOnLoad = false;

		[Tooltip("The Breath Detection scripts to aggregate data from. Only one is needed, but you can used multiple with weights to influence the confidence of the aggregate data.")]
		public DetectorConfig[] InitialDetectors;

		[Tooltip("The configuration for the CAS manager.")]
		public CASManager.Config Config;

		public CASManager CASManager { get; private set; }

		/// <summary>
		/// This is the counter for counting the number of frames that have passed since the last update.
		/// </summary>
		private int FixedFrameCount = 0;
		/// <summary>
		/// This is the number of frames that have to pass before the buffer is updated. This is fixed after initialization.
		/// </summary>
		private int FixedUpdateFrameCount = 0;
		
		/// <summary>
		/// Called once this GameObject is loaded into the scene.
		/// Initializes the BreathAggregation system.
		/// </summary>
		private void Awake()
		{
			if (!DestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}

			FixedUpdateFrameCount = Math.Max((int)Math.Round(Config.SampleDeltaTime / Time.fixedDeltaTime), 1);
			Config = new CASManager.Config(FixedUpdateFrameCount * Time.fixedDeltaTime, Config.SampleCount);

			CASManager = new CASManager(Config, InitialDetectors.ToList());
		}

		/// <summary>
		/// Preforms an update on the Aggregator, but only at the desired rate.
		/// </summary>
		private void FixedUpdate()
		{
			FixedFrameCount++;
			if (FixedFrameCount % FixedUpdateFrameCount != 0) return;
			else FixedFrameCount = 0;

			CASManager.GatherSamples();
		}

#if UNITY_EDITOR
		/// <summary>
		/// Throw a warning if there are multiple CAS controllers in a scene.
		/// </summary>
		private void Reset()
		{
			CASController[] controllers = FindObjectsOfType<CASController>();
			if (controllers.Length > 0 && (controllers.Length > 1 || controllers[0] != this))
			{
				Debug.LogWarning("There is more than one CASController in the scene. This can lead to unexpected behavior and is only recommended when comparing internal breath scripts.");
			}

			Config = new CASManager.Config(2 * Time.fixedDeltaTime, 2048);
		}

		/// <summary>
		/// Throws a warning if the sample count rate is not a multiple of the fixed delta time.
		/// </summary>
		private void OnValidate()
		{
			int frameCount = Math.Max((int)Math.Round(Config.SampleDeltaTime / Time.fixedDeltaTime), 1);

			if (Math.Abs(Config.SampleDeltaTime - frameCount * Time.fixedDeltaTime) > 0.001f)
			{
				Debug.LogWarning("SampleDeltaTime is not a multiple of FixedDeltaTime. Rounded to " + frameCount * Time.fixedDeltaTime);
			}
		}
#endif
	}
}
