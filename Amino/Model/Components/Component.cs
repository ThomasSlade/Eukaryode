namespace Amino
{
	/// <summary>The base class for all behaviour-driving components. Components define the behaviour of <see cref="Entity"/> instances.</summary>
	public class Component
    {
		/// <summary> The <see cref="Scene"/> in which this component exists. </summary>
		public Scene World => _owner.World;

		/// <summary>The <see cref="Entity"/> to which this component belongs.</summary>
		public Entity Owner => _owner;
        private Entity _owner { get; init; }

		/// <summary> A flag signifying that this component has been destroyed. Components may only be destroyed once. </summary>
		private bool _destroyed = false;

        public Component(Entity owner)
        {
            _owner = owner;
			_owner.RegisterComponent(this);
        }

		/// <summary> Destroy this component, removing it from its <see cref="Owner"/>. </summary>
		public void Destroy()
		{
			if(_destroyed)
			{
				return;
			}

			_owner.UnregisterComponent(this);

			_destroyed = true;
		}
    }
}
