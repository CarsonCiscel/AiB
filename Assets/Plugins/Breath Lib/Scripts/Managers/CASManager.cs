using System;
using System.Collections.Generic;
using System.Linq;

namespace BreathLib
{
	/// <summary>
	/// Configuration for a subscribed detector instance. This is used to help manage and merge the detector information.
	/// Notably, this contains the weight of the detector, which is used to determine the weight of the data when merging.
	/// TODO: Probably more here based on the detector own frequency or even per value weighting.
	/// </summary>
	[Serializable]
	public struct DetectorConfig
	{
		/// <summary>
		/// Detector associated with this configuration.
		/// </summary>
		[UnityEngine.Tooltip("Detector associated with this configuration.")]
		public Unity.BreathDetectionMono detector;

		/// <summary>
		/// Weight of the detector for merging breath sample into the stream.
		/// </summary>
		[UnityEngine.Tooltip("Weight of this detector when merging breath sample into the stream.")]
		[UnityEngine.Min(0.001f)]
		public float weight;

		public DetectorConfig(Unity.BreathDetectionMono detector, float weight = 1)
		{
			UnityEngine.Debug.Assert(detector != null, "new DetectorConfig(): Detector cannot be null.");
			UnityEngine.Debug.Assert(weight > 0, "new DetectorConfig(): Weight must be greater than 0");

			this.detector = detector;
			this.weight = weight;
		}
	}

	/// <summary>
	/// Post Processor for aggregating the Temporal Breath data into timelines, stored in an AggregateData object.
	/// Notable functions include:
	/// <list>
	/// 	<item>
	/// 		<term>Sample Frequency Synchronization</term>
	/// 		<description>Synchronizes the sample frequency of the detectors with a fixed sampling rate (much better for AI processing and usage).</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Data Correction</term>
	/// 		<description>Corrects the data for any errors, abnormalities, or outliers.</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Data Aggregation</term>
	/// 		<description>Aggregates the data into a timeline for the most recent in-the-moment data.</description>
	/// 	</item>
	/// </list>
	/// </summary>
	public class CASManager: ConfigurableManager<CASManager.Config>
	{
		[Serializable]
		public struct Config
		{
			/// <summary>
			/// The time in seconds between gathering breath samples into the breath stream.
			/// </summary>
			[UnityEngine.Tooltip("The time in seconds between gathering breath samples into the breath stream.")]
			[UnityEngine.Min(0.001f)]
			public float SampleDeltaTime;

			/// <summary>
			/// The number of samples to keep in the breath stream.
			/// </summary>
			[UnityEngine.Tooltip("The number of samples to keep in the breath stream.")]
			[UnityEngine.Min(1)]
			public int SampleCount;

			public Config(float sampleDeltaTime, int sampleCount)
			{
				UnityEngine.Debug.Assert(sampleDeltaTime >= 0.001f, "new CASConfig(): Sample delta time must be greater than 0.001f. Anything lower is more than 1000 samples per second!");
				UnityEngine.Debug.Assert(sampleCount >= 1, "new CASConfig(): Sample count must be greater than 1.");
				SampleDeltaTime = sampleDeltaTime;
				SampleCount = sampleCount;
			}
		}

		public readonly List<DetectorConfig> detectors;
		public readonly BreathStream breathStream;

		/// <summary>
		/// Sets up the Breath CAS manager.
		/// </summary>
		/// <param name="config">The configuration for this CAS</param>
		/// <param name="detectors">List of Detectors that are running on this CAS</param>
		public CASManager(Config config, List<DetectorConfig> detectors = null)
			: base(config)
		{
			this.detectors = detectors ?? new List<DetectorConfig>();
			breathStream = new BreathStream(config.SampleCount);
		}

		public void AddDetector(DetectorConfig config)
		{
			detectors.Add(config);
		}

		public void RemoveDetector(IBreathDetection detector)
		{
			detectors.RemoveAll(d => (d.detector as IBreathDetection) == detector);
		}

		/// <summary>
		/// Updates the AggregateData object with the new data.
		/// TODO: Manage the synchronization of the sample frequency.
		/// </summary>
		internal void GatherSamples()
		{
			// If statements are used for slight optimization / validation.
			if (detectors == null || detectors.Count == 0)
			{
				UnityEngine.Debug.LogWarning("BreathAggregation is not configured properly. Please add BreathDetection objects to the Detectors.");
				breathStream.Enqueue(new BreathSample());
			}
			else if (detectors.Count == 1)
			{
				breathStream.Enqueue(detectors[0].detector.GetBreathSample());
			}
			else
			{
				breathStream.Enqueue(
					MergeSamples(
						detectors.Select(conf =>
							(conf.detector.GetBreathSample(), conf.weight)
						).ToArray()
					)
				);
			}
		}

		/// <summary>
		/// Merges an array of weights and BreathSample objects into a single BreathSample object.
		/// </summary>
		/// <param name="data">A pair containing a BreathSample and its corresponding weight</param>
		/// <returns>One breath sample object that is the combination of all data in input.</returns>
		public static BreathSample MergeSamples(params (BreathSample, float)[] data)
		{
			UnityEngine.Debug.Assert(data.Aggregate(true, (a, b) => a && b.Item2 >= 0), "Correlator.TemporalMerge(): All data must have a non-negative weight.");

			BreathSample result = new BreathSample();

			for (int i = 0; i < result.Length; i++)
			{
				float? value = 0;
				float weights = 0;
				foreach (var (temp, weight) in data)
				{
					if (temp[i] != null)
					{
						weights += weight;
						value += temp[i].Value * weight;
					}
				}

				if (weights > 0)
				{
					result[i] = value / weights;
				}
				else result[i] = null;
			}

			return result;
		}
	}
}