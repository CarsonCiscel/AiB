using System.Collections;
using System.Collections.Generic;
using BreathLib;
using BreathLib.Unity;
using UnityEngine;

public class ArrowKeyBreathDetect : BreathDetectionMono
{
	public override SupportedPlatforms SupportedPlatforms => SupportedPlatforms.Windows | SupportedPlatforms.MacOS | SupportedPlatforms.Linux;

	public override BreathSample GetBreathSample()
	{
		return detected;
	}

	private BreathSample detected = new BreathSample();

	// Update is called once per frame
	void Update()
	{
		bool in_ = Input.GetKey(KeyCode.LeftArrow);
		bool out_ = Input.GetKey(KeyCode.RightArrow);

		if (in_ && out_)
		{
			detected.In = 0.5f;
			detected.Yes = 1f;
		}
		else if (in_ && !out_)
		{
			detected.In = 1f;
			detected.Yes = 1f;
		}
		else if (!in_ && out_)
		{
			detected.Out = 1f;
			detected.Yes = 1f;
		}
		else
		{
			detected.In = 0.5f;
			detected.No = 1f;
		}
	}
}
