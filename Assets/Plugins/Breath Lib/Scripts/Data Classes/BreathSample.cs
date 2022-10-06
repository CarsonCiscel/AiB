using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;

namespace BreathLib
{
	// TODO: Revert to a struct? Class might make data refs more clear, but slightly fudge the purpose.

	/// <summary>
	/// Data class for temporal breath data. Contains information of a breath at a specific time.
	/// </summary>
	[Serializable]
	[JsonObjectAttribute]
	public struct BreathSample: IEnumerable<float?>
	{
		/// <summary>
		/// BreathSample is an array of floats, but also uses field names to make it easier to serialize/parse/etc.
		/// This is the size of the BreathSample array. Mostly used for validation.
		/// </summary>
		private const int DATA_SIZE = 5;
		
		/// <summary>
		/// BreathSample is an array of floats, but also uses field names to make it easier to serialize/parse/etc.
		/// This is the breath sample stored as an array.
		/// </summary>
		private float? data_0;
		private float? data_1;
		private float? data_2;
		private float? data_3;
		private float? data_4;

#region Data Wrappers (Properties for Gets/Sets)
		
		/// <summary>
		/// 0.0f to 1.0f. Probability that the user is not breathing at a given time-step. A value of null describes that the No is not being detected.
		/// The sum of In, Out, and No must be 1.0f.
		/// </summary>
		public float? No { get => this[0]; set => this[0] = value; }
		/// <summary>
		/// 0.0f to 1.0f. Probability that the user is breathing at a given time-step. A value of null describes that the No is not being detected.
		/// The sum of In, Out, and No must be 1.0f.
		/// </summary>
		public float? Yes { get => 1 - No; set => No = 1 - value; }
		/// <summary>
		/// 0.0f to 1.0f. Probability that the user is breathing in [1] or out [0]  at a given time-step. A value of null describes that the In/Out is not being detected.
		/// </summary>
		public float? In { get => this[1]; set => this[1] = value; }
		/// <summary>
		/// 0.0f to 1.0f. Probability that the user is breathing in [0] or out [1]  at a given time-step. A value of null describes that the In/Out is not being detected.
		/// </summary>
		public float? Out { get => 1 - In; set => In = 1 - value; }

		/// <summary>
		/// 0.0f to 1.0f. Probability that a In/Out breath is coming through the nose (not mouth). A value of null describes that the Nasal is not being detected.
		/// Mid values like 0.5f show uncertainty or both nasal and mouth.
		/// </summary>
		public float? Nasal { get => this[2]; set => this[2] = value; }
		/// <summary>
		/// 0.0f to 1.0f. Probability that a In/Out breath is coming through the mouth (not nasal). A value of null describes that the Mouth is not being detected.
		/// Mid values like 0.5f show uncertainty.
		/// </summary>
		public float? Mouth { get => 1 - Nasal; set => Nasal = 1 - value; }

		/// <summary>
		/// In Hertz. The frequency of breath. A value of null describes that the pitch is not being detected.
		/// <para>Value should reasonably correspond to the following ranges:</para>
		/// <list type="bullet">
		/// 	<item>
		/// 		<term>null</term>
		/// 		<description>Invalid / Pitch is not being detected.</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>0.00 = 0hz to 60hz</term>
		/// 		<description>Not a breath detected.</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>0 to 0.25 = 60hz to 300hz</term>
		/// 		<description>Low (pitched) breath detected.</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>0.25 to 0.50 = 300hz to 600hz</term>
		/// 		<description>Middle (pitched) breath detected.</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>0.50 to 0.75 = 600hz to 1200hz</term>
		/// 		<description>High (pitched) breath detected.</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>0.75 to 1.00 = 1200hz to 2800hz</term>
		/// 		<description>Wheezing or Stridor</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>1.00 = 2800hz+</term>
		/// 		<description>Not a breath detected</description>
		/// 	</item>
		/// </list>
		/// </summary>
		public float? Pitch { get => this[3]; set => this[3] = value; }

		/// <summary>
		/// Volume is normalized to the range [0-1]. A value of null describes that the volume is not being detected.
		/// </summary>
		// TODO: Have a helper function that will normalize based on past volumes.
		public float? Volume { get => this[4]; set => this[4] = value; }

#endregion

#region Constructors

		public BreathSample(float? no = null, float? yes = null, float? _in = null, float? _out = null, float? nasal = null, float? mouth = null, float? pitch = null, float? volume = null):
			this(new float?[]{no ?? yes, _in ?? _out, nasal ?? mouth, pitch, volume}) {}
		
		public BreathSample(float?[] data)
		{
			Debug.Assert(data.Length == DATA_SIZE, "new BreathSample(): data.Length != DATA_SIZE");
			this.data_0 = data[0];
			this.data_1 = data[1];
			this.data_2 = data[2];
			this.data_3 = data[3];
			this.data_4 = data[4];
		}

#endregion

#region Elevated Variables/Operators

		/// <summary>
		/// Same as this.data[index] except with debug validation.
		/// </summary>
		/// <param name="index">Data index for Temporal Breath.</param>
		/// <returns>Float or null for the data at this index.</returns>
		public float? this[int index]
		{
			get
			{
				switch(index)
				{
					case 0: return data_0;
					case 1: return data_1;
					case 2: return data_2;
					case 3: return data_3;
					case 4: return data_4;
					default: throw new IndexOutOfRangeException("BreathSample: index out of range");
				}
			}

			set
			{
				Debug.Assert(value == null || value >= 0 || value <= 1, "BreathSample: data must be null or within range [0,1]: (setting index " + index + " to " + value + ")");
				switch(index)
				{
					case 0: data_0 = value; break;
					case 1: data_1 = value; break;
					case 2: data_2 = value; break;
					case 3: data_3 = value; break;
					case 4: data_4 = value; break;
					default: throw new IndexOutOfRangeException("BreathSample: index out of range");
				}
			}
		}

		public int Length { get => DATA_SIZE; }

#endregion

		// TODO Move out of this class?
		/// <summary> 
		/// Turns a frequency into a normalized value [0,1] for breath pitches.
		/// <list type="bullet">
		/// 	<item>
		/// 		<term>>null</term>
		/// 		<description>Invalid / Pitch is not being detected.</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>0.00 = 0hz to 60hz</term>
		/// 		<description>Not a breath detected.</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>0 to 0.25 = 60hz to 300hz</term>
		/// 		<description>Low (pitched) breath detected.</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>0.25 to 0.50 = 300hz to 600hz</term>
		/// 		<description>Middle (pitched) breath detected.</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>0.50 to 0.75 = 600hz to 1200hz</term>
		/// 		<description>High (pitched) breath detected.</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>0.75 to 1.00 = 1200hz to 2800hz</term>
		/// 		<description>Wheezing or Stridor</description>
		/// 	</item>
		/// 	<item>
		/// 		<term>1.00 = 2800hz+</term>
		/// 		<description>Not a breath detected</description>
		/// 	</item>
		/// </list>
		/// </summary>
		/// <param name="frequency">Should reasonably correspond to the ranges above.</param>
		/// <returns>The normalized value of the input frequency.</returns>
		/// <remarks> Function values were calculated thanks to https://mycurvefit.com/ </remarks>
		public static float NormalizePitch(float frequency)
		{
			// Clamp or use function to normalize the input frequency.
			if (frequency <= 60) return 0f;
			else if (frequency >= 2800) return 1f;
			else return 1.203619f + (-0.05506198f - 1.202529f)/(1 + (float)Math.Pow(frequency/741.1966f, 1.234942f));;
		}

		public override string ToString()
		{
			return string.Format("No: {0}, In: {1}, Nasal: {2}, Pitch: {3}, Volume: {4}", No.ToString() ?? "n/a", In.ToString() ?? "n/a", Nasal.ToString() ?? "n/a", Pitch.ToString() ?? "n/a", Volume.ToString() ?? "n/a");
		}

		public IEnumerator<float?> GetEnumerator()
		{
			return new float?[] { data_0, data_1, data_2, data_3, data_4 }.GetEnumerator() as IEnumerator<float?>;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}