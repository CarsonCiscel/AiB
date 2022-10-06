using UnityEngine;
using UnityEngine.Events;

namespace BreathLib.Unity
{
	public class FollowAccuracyCC : UnityController<FollowAccuracyCM, FollowAccuracyCM.Config>
	{
		[Tooltip("Event for updating the progression.")]
		public UnityEvent<float> UpdateProgress;

		protected override FollowAccuracyCM CreateManager()
		{
			return new FollowAccuracyCM(config, CASManager);
		}

		protected override FollowAccuracyCM.Config DefaultConfig()
		{
			return new FollowAccuracyCM.Config(null, 0.05f, 1f, 0.0f);
		}

		private void Update()
		{
			UpdateProgress.Invoke(Manager.Accuracy);
		}
	}
}