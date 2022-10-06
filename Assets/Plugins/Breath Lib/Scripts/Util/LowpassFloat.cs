namespace BreathLib.Util
{
	public struct LowpassFloat
	{
		private float _value;
		public float Value
		{
			get => _value;
			set
			{
				_value = value * Alpha + _value * (1 - Alpha);
			}
		}
		public float Alpha;

		public LowpassFloat(float value, float alpha)
		{
			_value = value;
			Alpha = alpha;
		}
	}
}