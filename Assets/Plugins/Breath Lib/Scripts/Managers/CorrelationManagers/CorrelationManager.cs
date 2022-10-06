using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

namespace BreathLib
{
	/// <summary>
	/// Extendable class that can be used for interpreting aggregate data. This contains a lot of Util functions for comparing breath sample.
	/// Intended for making comparisons between different breath patterns.
	/// TODO: Add abstract features. What needs to be shared between all correlators?
	/// </summary>
	public abstract class CorrelationManager<C> : ConfigurableManager<C> where C : struct
	{
		protected CASManager CASManager;

		protected CorrelationManager(C config, CASManager casManager)
			: base(config)
		{
			CASManager = casManager;
		}
	}
}
