using System;

namespace BreathLib
{
    /// <summary>
	/// Utility Class for calculating interpolations (blending) between float values.
	/// All interp functions clamp the input Time to be within the range of 0-1, and all inverse functions clamp the input to be within the range of start-end.
	/// All inverse functions are safe, returning "a" if a == b.
	/// </summary>
	public static class Interpolators
	{
		[Serializable]
		public enum Method
		{
			Constant = 0,
			Linear = 1,
			Cosine = 2,
			Square = 3,
			Cubic = 4,
			Root = 5,
		}

		public static float Interp(float start, float end, float time, Interpolators.Method type)
		{
			switch (type)
			{
				case Interpolators.Method.Constant:
					return Constant(start, end, time);
				case Interpolators.Method.Linear:
					return Linear(start, end, time);
				case Interpolators.Method.Cosine:
					return Cosine(start, end, time);
				case Interpolators.Method.Square:
					return Square(start, end, time);
				case Interpolators.Method.Cubic:
					return Cubic(start, end, time);
				case Interpolators.Method.Root:
					return Root(start, end, time);
				default:
					throw new Exception("Unknown transition type: " + type);
			}
		}

        public static float Interp(float time, Interpolators.Method type)
		{
            return Interp(0, 1, time, type);
		}

        // TODO Add inverse functions.
        // /// <summary>
		// /// 
		// /// </summary>
		// /// <param name="start"></param>
		// /// <param name="end"></param>
		// /// <param name="value"></param>
		// /// <param name="type"></param>
		// /// <returns></returns>
        // public static float InverseInterp(float start, float end, float value, Interpolators.Method type)
        // {
        //     value = Math.Clamp(value, start, end);
        //     float time = (value - start) / (end - start);
        // }

		public static float Constant(float start, float end, float time)
		{
			if (time < 1.0f)
				return start;
			else return end;
		}

		public static float IConstant(float start, float end, float value)
		{
			if (value < end)
				return 0.0f;
			else return 1.0f;
		}

		public static float Linear(float start, float end, float time)
		{
			float t = Math.Clamp(time, 0, 1);
			return start + (end - start) * t;
		}

		/// <summary>
		/// If start == end, returns 1f.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="value"></param>
		/// <returns></returns>
        public static float ILinear(float start, float end, float value)
		{
			if (value >= end)
				return 1.0f;
			else if (value <= start)
				return 0.0f;
			else return (value - start) / (end - start);
		}

		public static float Cosine(float start, float end, float time)
		{
			float t = Math.Clamp(time, 0, 1);
			float t2 =  (1f -(float)Math.Cos(t * Math.PI)) / 2.0f;
			return start + (end - start) * t2;
		}

		public static float Square(float start, float end, float time)
		{
			float t = Math.Clamp(time, 0, 1);
			float t2 = t * t;
			return start + (end - start) * t2;
		}

		public static float Cubic(float start, float end, float time)
		{
			float t = Math.Clamp(time, 0, 1);
			float t2 = t * t;
			float t3 = t2 * t;
			return start + (end - start) * (t3 * (t - 2f) + t2 * (2f - t));
		}

		public static float Root(float start, float end, float time)
		{
			float t = Math.Clamp(time, 0, 1);
			float root_t = (float)Math.Pow(t, 0.5);
			return start * (end - start) * root_t;
		}

		/// <summary>
		/// Helper for setting the previous value in a low pass.
		/// <returns>New value of the lowpass.</returns>
		public static float Lowpass(ref float previous, float current, float alpha, Method method = Method.Linear)
		{
			return previous = Interp(previous, current, alpha, method);
		}
	}
}