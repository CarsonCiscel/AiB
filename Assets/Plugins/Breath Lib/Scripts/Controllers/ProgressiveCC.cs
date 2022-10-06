using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BreathLib.Unity
{
	public class ProgressiveCC : UnityController<ProgressiveCM, ProgressiveCM.Config>
	{
		[Tooltip("Event for updating the progression.")]
		public UnityEvent<float> UpdateProgress;

		protected override ProgressiveCM CreateManager()
		{
			return new ProgressiveCM(config, CASManager);
		}

		protected override ProgressiveCM.Config DefaultConfig()
		{
			return new ProgressiveCM.Config(null, 0.5f, 1f, 0.0f);
		}

		private void Update()
		{
			UpdateProgress.Invoke(Manager.NormalizedBreathPosition);
		}
	}
}
