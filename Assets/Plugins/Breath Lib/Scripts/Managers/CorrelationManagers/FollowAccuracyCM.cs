using System;
using BreathLib.Util;

namespace BreathLib
{
	public class FollowAccuracyCM : CorrelationManager<FollowAccuracyCM.Config>
	{
		[Serializable]
		public struct Config
		{
			[UnityEngine.Tooltip("The JSON file which should be used to create the breath pattern.")]
			public UnityEngine.TextAsset BreathPattern;

			[UnityEngine.Tooltip("Speed at which the accuracy changes to match user performance.")]
			[UnityEngine.Range(0, 1)]
			public float lowpassAlpha;

			[UnityEngine.Tooltip("This scales the correlation value to make it more or less sensitive. 1 = normal, 0.5 = half sensitive, 2 = twice sensitive, etc.")]
			[UnityEngine.Min(0.01f)]
			public float scalePower;

			[UnityEngine.Tooltip("Starting time of the progression on play.")]
			[UnityEngine.Range(0, 1)]
			public float normStartPosition;

			public Config(UnityEngine.TextAsset breathPattern, float alpha, float scalePower, float normStartPosition)
			{
				UnityEngine.Debug.Assert(breathPattern != null, "Breath pattern must be specified.");
				UnityEngine.Debug.Assert(alpha >= 0 && alpha <= 1, "likenessThreshold must be between 0 and 1.");
				UnityEngine.Debug.Assert(scalePower >= 0.01f, "scalePower must be greater than 0.01f.");
				UnityEngine.Debug.Assert(normStartPosition >= 0 && normStartPosition <= 1, "normStartPosition must be between 0 and 1.");
	
				this.BreathPattern = breathPattern;
				this.lowpassAlpha = alpha;
				this.scalePower = scalePower;
				this.normStartPosition = normStartPosition;
			}
		}

		public FollowAccuracyCM(Config config, CASManager casManager)
			: base(config, casManager)
		{
			if (config.BreathPattern == null)
			{
				UnityEngine.Debug.LogError("new FollowAccuracyCM(): Breath pattern inside config is null. This object will not function properly.");
				return;
			}

			breath_type = PatternLibrary.GetBreathPattern(config.BreathPattern);
			m_accuracy = new LowpassFloat(0, config.lowpassAlpha);

			m_currentBreathPosition = config.normStartPosition;
			// if (breath_type == null)
			// {
			// 	UnityEngine.Debug.LogError("new FollowAccuracyCM(): Breath pattern inside config could not be loaded! This object will not function properly.");
			// }
		}

		public readonly Pattern breath_type;
		private float m_currentBreathPosition;
		private LowpassFloat m_accuracy;
		private ulong lastUpdateTime;

		public float Accuracy
		{
			get
			{
				Update();
				return m_accuracy.Value;
			}
			set => m_accuracy.Value = value;
		}

		public float CurrentBreathPosition
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
			UnityEngine.Debug.Assert(CASManager != null, "CASManager is null!");
			UnityEngine.Debug.Assert(CASManager.breathStream != null, "Breath type is null!");

			if (lastUpdateTime + (ulong)CASManager.config.SampleCount < CASManager.breathStream.TotalSamples)
			{
				ulong latestTime = CASManager.breathStream.TotalSamples - (ulong)CASManager.config.SampleCount;
				UnityEngine.Debug.LogWarning("ProgressiveCM.Update: last update is too old. Skipping '" + (latestTime - lastUpdateTime) + "' samples.");
				lastUpdateTime = latestTime;
			}

			while (CASManager.breathStream.TotalSamples > lastUpdateTime)
			{
				BreathSample detected = CASManager.breathStream[lastUpdateTime++].Value;
				BreathSample target = breath_type.GetTargetAtNormalizedTime(CurrentBreathPosition);

				UnityEngine.Debug.Log("Detected: " + detected);
				UnityEngine.Debug.Log("Target: " + target);

				float likeness = detected.SampleLikeness(target);
				float scaledLikeness = (float)Math.Pow(likeness, config.scalePower);

				Accuracy = likeness;

				CurrentBreathPosition += CASManager.config.SampleDeltaTime / breath_type.length;

				while (CurrentBreathPosition >= 1)
					CurrentBreathPosition -= 1;
			}
		}
	}
}