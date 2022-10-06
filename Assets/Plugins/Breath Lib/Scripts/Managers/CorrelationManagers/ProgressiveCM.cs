using System;
using Debug = UnityEngine.Debug;

namespace BreathLib
{
	/// <summary>
	/// A Correlator that is used to track the users progress in a specific breath. This breath is static through the lifetime of this class, but new Progressive Breaths can be created.
	/// </summary>
	public class ProgressiveCM : CorrelationManager<ProgressiveCM.Config>
	{
		[Serializable]
		public struct Config
		{
			[UnityEngine.Tooltip("The JSON file which should be used to create the breath pattern.")]
			public UnityEngine.TextAsset BreathPattern;

			[UnityEngine.Tooltip("Cutoff threshold for progression. If likelihood of breath correlation is below this value, the progression will not continue.")]
			[UnityEngine.Range(0, 1)]
			public float likenessThreshold;

			[UnityEngine.Tooltip("This scales the correlation value to make it more or less sensitive. 1 = normal, 0.5 = half sensitive, 2 = twice sensitive, etc.")]
			[UnityEngine.Min(0.01f)]
			public float scalePower;

			[UnityEngine.Tooltip("Starting time of the progression on play.")]
			[UnityEngine.Range(0, 1)]
			public float normStartPosition;

			public Config(UnityEngine.TextAsset breathPattern, float likenessThreshold, float scalePower, float normStartPosition)
			{
				UnityEngine.Debug.Assert(breathPattern != null, "Breath pattern must be specified.");
				UnityEngine.Debug.Assert(likenessThreshold >= 0 && likenessThreshold <= 1, "likenessThreshold must be between 0 and 1.");
				UnityEngine.Debug.Assert(scalePower >= 0.01f, "scalePower must be greater than 0.01f.");
				UnityEngine.Debug.Assert(normStartPosition >= 0 && normStartPosition <= 1, "normStartPosition must be between 0 and 1.");
	
				this.BreathPattern = breathPattern;
				this.likenessThreshold = likenessThreshold;
				this.scalePower = scalePower;
				this.normStartPosition = normStartPosition;
			}
		}

		public ProgressiveCM(Config config, CASManager casManager)
			: base(config, casManager)
		{
			if (config.BreathPattern == null)
			{
				UnityEngine.Debug.LogError("new ProgressiveCM(): Breath pattern inside config is null. This object will not function properly.");
				return;
			}

			breath_type = PatternLibrary.GetBreathPattern(config.BreathPattern);

			m_currentBreathPosition = config.normStartPosition;
			// if (breath_type == null)
			// {
			// 	UnityEngine.Debug.LogError("new ProgressiveCM(): Breath pattern inside config could not be loaded! This object will not function properly.");
			// }
		}

		public readonly Pattern breath_type;
		private float m_currentBreathPosition;
		private ulong lastUpdateTime;

		public float NormalizedBreathPosition
		{
			get
			{
				Update();
				return m_currentBreathPosition;
			}
			set => m_currentBreathPosition = value;
		}

		private void Update()
		{
			Debug.Assert(CASManager != null, "CASManager is null!");
			Debug.Assert(CASManager.breathStream != null, "Breath type is null!");

			if (lastUpdateTime + (ulong)CASManager.config.SampleCount < CASManager.breathStream.TotalSamples)
			{
				ulong latestTime = CASManager.breathStream.TotalSamples - (ulong)CASManager.config.SampleCount;
				Debug.LogWarning("ProgressiveCM.Update: last update is too old. Skipping '" + (latestTime - lastUpdateTime) + "' samples.");
				lastUpdateTime = latestTime;
			}

			while (CASManager.breathStream.TotalSamples > lastUpdateTime)
			{
				BreathSample detected = CASManager.breathStream[lastUpdateTime++].Value;
				BreathSample target = breath_type.GetTargetAtNormalizedTime(m_currentBreathPosition);

				Debug.Log("Detected: " + detected);
				Debug.Log("Target: " + target);

				float likeness = detected.SampleLikeness(target);
				float cutoffLikeness = likeness < config.likenessThreshold ? 0 : likeness;
				float scaledLikeness = (float)Math.Pow(cutoffLikeness, config.scalePower);
				

				Debug.Log("Likeness: " + likeness + ", cutoff: " + cutoffLikeness + ", scaled: " + scaledLikeness);

				m_currentBreathPosition += scaledLikeness * CASManager.config.SampleDeltaTime * (1 / breath_type.length);

				while (m_currentBreathPosition >= 1)
					m_currentBreathPosition -= 1;
			}
		}
	}
}