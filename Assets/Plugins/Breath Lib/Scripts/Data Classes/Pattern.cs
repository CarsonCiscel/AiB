using System;

namespace BreathLib
{
	/// <summary>
	/// Data class for a breath pattern, including a list of keyframes for a breath.
	/// Class contains helper methods for calculating in-between keyframes.
	/// </summary>
	[Serializable]
	public struct Pattern
	{
		public string name;
		public float length;
		public Keyframe[] keyframes;

		/// <summary>
		/// Creates a Pattern based on the given data.
		/// </summary>
		/// <param name="name">Name of the Pattern.</param>
		/// <param name="totalTime">Time in seconds that the breath pattern will take.</param>
		/// <param name="keyframes">Array that defines how the breath should look.</param>
		public Pattern(string name, float length, Keyframe[] keyframes)
		{
			this.name = name;
			this.length = length;
			this.keyframes = keyframes;
		}

		/// <summary>
		/// Gets an in-between keyframe for the given normalized time [0,1].
		/// </summary>
		/// <param name="normalizedTime">Normalized position in the pattern.</param>
		/// <returns>A BreathSample object that contains the target state at the given time in the pattern.</returns>
		public BreathSample GetTargetAtNormalizedTime(float normalizedTime)
		{
			// Loop the time. Time 1 == 0, Time 1.34 == 0.34, Time 4.5 == 0.5, Time -1.66 == 0.66, etc.
			normalizedTime = normalizedTime - (float)Math.Truncate(normalizedTime);

			BreathSample result = new BreathSample();
			// Find the keyframe that is closest to the normalized time.
			for (int tempIndex = 0; tempIndex < result.Length; tempIndex++)
			{
				// (time: float, temporal value: float, transition type: Method)
				(float, float, Interpolators.Method)?
					begin = null,   // The 'a' of the interpolation.
					end = null,     // The 'b' of the interpolation.
					first = null;   // Reference to the first valid keyframe. Used for circular looping.
				for (int frameIndex = 0; frameIndex < keyframes.Length; frameIndex++)
				{
					if (keyframes[frameIndex].target[tempIndex] == null)
						continue;

					begin = end;
					end = (keyframes[frameIndex].time, keyframes[frameIndex].target[tempIndex].Value, keyframes[frameIndex].transition);

					if (first == null)
						first = end;

					if (keyframes[frameIndex].time > normalizedTime)
					{
						// If the normalized position is less than the first keyframe that appears, we need to also grab the
						// 	last keyframe (as long as the last doesn't match the first).
						if (begin == null)
						{
							for (int reverseIndex = keyframes.Length - 1; reverseIndex > frameIndex; reverseIndex--)
							{
								if (keyframes[reverseIndex].target[tempIndex] == null)
									continue;
								begin = (keyframes[reverseIndex].time - 1, keyframes[reverseIndex].target[tempIndex].Value, keyframes[reverseIndex].transition);
								break;
							}
						}

						break;
					}
				}

				//UnityEngine.Debug.LogFormat("i: {2} :: Begin: [{0}], end: [{1}], first: [{3}]", begin, end, tempIndex, first);

				// No keyframes with this value.
				if (end == null)
					continue;

				// If there was only one keyframe.
				else if (begin == null)
				{
					UnityEngine.Debug.Assert(first == end); // First is only different from end if there is more than one keyframe.
					result[tempIndex] = first.Value.Item2;
				}
				else
				{
					float pointInTransition;
					// Check for circular keyframe (ie, start time is after the end time due to looping)
					if (end.Value.Item1 < normalizedTime)
					{
						begin = end;
						end = first;

						pointInTransition = Interpolators.ILinear(
							// Cycle the end time.
							start: begin.Value.Item1,
							end: end.Value.Item1 + 1.0f,
							value: normalizedTime
						);
					}
					else
					{
						pointInTransition = Interpolators.ILinear(
							start: begin.Value.Item1,
							end: end.Value.Item1,
							value: normalizedTime
						);
					}



					result[tempIndex] = Interpolators.Interp(
						start: begin.Value.Item2,
						end: end.Value.Item2,
						time: pointInTransition,
						type: begin.Value.Item3
					);
				}
			}
			return result;
		}

		/// <summary>
		/// Helper for converting time in seconds to a normalized time [0,1] in the pattern.
		/// </summary>
		/// <param name="timeInSeconds">Time that will be converted</param>
		/// <returns>Normalized value [0,1] that equals the number of seconds into a pattern.</returns>
		public BreathSample GetTargetAtActualTime(float timeInSeconds)
		{
			return GetTargetAtNormalizedTime(timeInSeconds / length);
		}

		public void OrderKeyframes()
		{
			OrderKeyframes(keyframes);
		}

		private static void OrderKeyframes(Keyframe[] keyframes)
		{
			Array.Sort(keyframes, (a, b) => a.time.CompareTo(b.time));
		}

		public override string ToString()
		{
			return string.Format("BreathType: {0}, length: {1}, keyframes: [{2}]", name, length, string.Join(", ", keyframes));
		}
	}
}