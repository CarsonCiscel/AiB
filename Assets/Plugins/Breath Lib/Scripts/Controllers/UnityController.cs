using UnityEngine;

namespace BreathLib.Unity
{
	[RequireComponent(typeof(CASController))]
	public abstract class UnityController<T, C> : MonoBehaviour 
		where T : ConfigurableManager<C>
		where C : struct
	{
		[Tooltip("Configuration for the controller.")]
		[SerializeField]
		public C config;

		public T Manager { get; private set; }

		protected CASManager CASManager => _casController.CASManager;
		protected CASController _casController;

		protected abstract T CreateManager();
		protected abstract C DefaultConfig();

		protected virtual void Awake()
		{
			_casController = GetComponent<CASController>();
		}

		protected virtual void Start()
		{
			Manager = CreateManager();
		}

		protected virtual void Reset()
		{
			config = DefaultConfig();
		}
	}
}