namespace BreathLib
{
	public abstract class ConfigurableManager<C> where C : struct
	{
		public C config;

		public ConfigurableManager(C config)
		{
			this.config = config;
		}
	}
}