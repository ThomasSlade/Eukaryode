using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Amino
{
	/// <summary> A positionable game object, to which <see cref="Component"/>s can be attached to provide behaviour. </summary>
	public class Entity
    {
		/// <summary> A legible name for the entity. This doesn't have to be unique. </summary>
		public string Name
		{
			get => _name;
			set
			{
				if(value == null)
				{
					value = string.Empty;
				}
				_name = value;
			}
		}
		private string _name = "Entity";

		/// <summary>The <see cref="Scene"/> to which this entity belongs.</summary>
		public Scene World { get => _world; }
        private Scene _world;

        /// <summary>The <see cref="Entity"/> under which this entity exists in the hierarchy.</summary>
        public Entity? Parent
        {
            get => _parent;
            set
            {
                if(value == _parent)
                {
                    return;
                }

                if(value != null && value.World != World)
                {
                    throw new ArgumentException($"Cannot set parent of '{this}' to '{value}' because they are not in the same scene.");
                }

                if(_parent != null)
                {
					_parent.UnregisterChild(this);
                }

                _parent = value;

                if(_parent != null)
                {
					_parent.RegisterChild(this);
                }
            }
        }
        private Entity? _parent;

		/// <summary>The child <see cref="Entity"/> objects of this entity.</summary>
		public ReadOnlyCollection<Entity> Children { get; private init; }
		protected List<Entity> _children = new List<Entity>();

		/// <summary>The <see cref="Component"/>s of this entity.</summary>
		protected ReadOnlyDictionary<Type, Component> Components { get; private init; }
		protected Dictionary<Type, Component> _components = new Dictionary<Type, Component>();

		/// <summary> The global transform of this entity, containing its translation, rotation, and scale in world-space. </summary>
		public Matrix3x3 Transform => Parent != null ? Parent.Transform * LocalTransform : LocalTransform;

		/// <summary>
		/// The local transform of this entity, containing its translation, rotation, and scale relative to its parent,
		/// or in world-space if it has no parent.
		/// </summary>
		private Matrix3x3 LocalTransform
		{
			get => _localTransform;
			set => _localTransform = value;
		}
		private Matrix3x3 _localTransform = new Matrix3x3();

		/// <summary>
		/// The <see cref="Matrix3x3.RelativeTranslation"/> of this entity: note that this translation is relative
		/// to the entity's own rotation, rather than to its parent.
		/// </summary>
		public Vector2 LocalTranslation
		{
			get => _localTransform.RelativeTranslation;
			set
			{
				if(value == _localTransform.RelativeTranslation)
				{
					return;
				}
				_localTransform.RelativeTranslation = value;
				_transformChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary> The scale of this entity relative to its parent's transform. </summary>
		public Vector2 LocalScale
		{
			get => _localTransform.Scale;
			set
			{
				if (value == _localTransform.Scale)
				{
					return;
				}
				_localTransform.Scale = value;
				_transformChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary> The rotation of this entity relative to its parent's transform, in degrees. </summary>
		public float LocalRotation
		{
			get => MathHelper.ToDegrees(_localTransform.Rotation);
			set
			{
				value = MathHelper.ToRadians(value);
				// When setting rotation, in order to rotate the entity on its pivot, place it at the origin, rotate it, then move it back.
				Matrix3x3 transformAtOrigin = _localTransform;
				transformAtOrigin.Translation = Vector2.Zero;
				if (value == transformAtOrigin.Rotation)
				{
					return;
				}
				transformAtOrigin.Rotation = value;
				transformAtOrigin.Translation = _localTransform.Translation;
				_localTransform = transformAtOrigin;
				_transformChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary> Fires when this entity's transform changes. </summary>
		public event EventHandler TransformChanged
		{
			add
			{
				// Fire this event when it has a new subscriber.
				_transformChanged += value;
				value?.Invoke(this, EventArgs.Empty);
			}
			remove
			{
				_transformChanged -= value;
			}
		}
		private event EventHandler _transformChanged;

        protected Entity(Scene world, Entity? parent, string? name = null) : base()
        {
            _world = world;
            Parent = parent;
			Name = name == null ? "Entity" : name;

			Children = new ReadOnlyCollection<Entity>(_children);
			Components = new ReadOnlyDictionary<Type, Component>(_components);
        }

		public Entity(Scene scene, string? name = null) : this(scene, null, name)
        {

        }

        public Entity(Entity parent, string? name = null) : this(parent.World, parent, name)
        {

        }

		/// <summary> Add a child to this entity, presuming that the child already has this entity as its parent. </summary>
		internal void RegisterChild(Entity child)
		{
			if(child.Parent != this)
			{
				throw new InvalidOperationException($"Cannot register '{child}' as a child of '{this}' because the child's parent ('{child.Parent}') does not equal this entity.");
			}
			_children.Add(child);
		}

		/// <summary> Remove a child from this entity, presuming that the child actually belongs to this entity. </summary>
		internal void UnregisterChild(Entity child)
		{
			if(!_children.Remove(child))
			{
				throw new InvalidOperationException($"Cannot unregister child entity '{child}' from parent '{this}': That child was not present on this entity.");
			}
			_children.Remove(child);
		}

		/// <summary> Add a component to this entity, presuming that the component already has this entity as its owner. </summary>
		internal void RegisterComponent(Component component)
		{
			if (component.Owner != this)
			{
				throw new InvalidOperationException($"Cannot register '{component}' as a component of '{this}' because the component's owner ('{component.Owner}') does not equal this entity.");
			}
			Type type = component.GetType();
			if(!_components.TryAdd(type, component))
			{
				throw new InvalidOperationException($"Cannot register component '{component}' of type '{type}': a component of this type ({_components[type]}) is already present on this entity.");
			}
		}

		/// <summary> Remove a component from this entity, presuming that the component actually belongs to this entity. </summary>
		internal void UnregisterComponent(Component component)
		{
			Type type = component.GetType();
			if (!_components.TryGetValue(type, out Component existingComponent))
			{
				throw new InvalidOperationException($"Cannot unregister component '{component}' of type '{type}': no component of this type was on this entity.");
			}
			if (existingComponent != component)
			{
				throw new InvalidOperationException($"Cannot unregister component '{component}' of type '{type}': The component of that type on this entity is different to this component ({existingComponent}).");
			}
			_components.Remove(type);
		}

		/// <summary> Try getting a component of the specified type. </summary>
		public bool TryGetComponent(Type type, out Component component)
		{
			component = null;
			if (!type.IsAssignableTo(typeof(Component)))
			{
				throw new ArgumentException($"Must get component using a type that is assignable to {nameof(Component)}: Parameter type was '{type}'", nameof(type));
			}
			return Components.TryGetValue(type, out component);
		}

		/// <summary> Try getting a component of type <typeparamref name="T"/>. </summary>
		public bool TryGetComponent<T>(out T component) where T : Component
		{
			component = null;
			bool success = TryGetComponent(typeof(T), out Component uncasted);
			if(success)
			{
				component = (T)uncasted;
			}
			return success;
		}

		/// <summary> Get a component of the specified type. </summary>
		public Component GetComponent(Type type)
		{
			if(!TryGetComponent(type, out Component component))
			{
				throw new ArgumentException($"Entity '{this}' did not have a component of type '{type}'.");
			}
			return component;
		}

		/// <summary> Get a component of type <typeparamref name="T"/>. </summary>
		public T GetComponent<T>() where T : Component => (T)GetComponent(typeof(T));
    }
}
