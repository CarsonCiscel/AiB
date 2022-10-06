namespace BreathLib
{
	/// <summary>
	/// Interface between Breath Detection scripts and the Aggregate Breath script.
	/// </summary>
	public interface IBreathDetection
	{
		/// <summary>
		/// Gets the supported platforms. This is used to determine which detection features are available, as well as for warnings if the platform is not supported.
		/// </summary>
		public SupportedPlatforms SupportedPlatforms { get; }

        // TODO : Needed hardware


        /// <summary>
        /// Returns the data values for the last detected breath (breath at this specific time).
        /// </summary>
        /// <returns></returns>
        public BreathSample GetBreathSample();
	}
}

namespace BreathLib.Unity
{
	// TODO Figure this stupid thing out. No type alias that has class and interface???
	// public abstract class BreathDetectionObject : UnityEngine.Object, IBreathDetection 
	// {
	// 	public abstract SupportedPlatforms SupportedPlatforms { get; }
	// 	public abstract BreathSample GetBreathSample();
	// }

	public abstract class BreathDetectionMono : UnityEngine.MonoBehaviour, IBreathDetection
	{
		public abstract SupportedPlatforms SupportedPlatforms { get; }
		public abstract BreathSample GetBreathSample();
	}
}
