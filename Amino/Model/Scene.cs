using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Amino
{
	/// <summary> A base 2D world, into which <see cref="Entity"/> instances can be placed. </summary>
	public class Scene : IGameServiceProvider
    {
		/// <summary> Provides services pertaining to the game's visual systems. May be left as null in the case of a model-only environment. </summary>
		private IViewServiceProvider? _view;

		/// <summary> The game <see cref="GraphicsDevice"/>. </summary>
		private GraphicsDevice? GraphicsDevice => _view?.GraphicsDevice;

		/// <summary>The <see cref="Renderer"/> used to draw visual components.	</summary>
		private RenderService? Renderer { get; init; }

		/// <summary> The game-application service provider. </summary>
		private IGameServiceProvider _game;
		public bool IsActive => _game.IsActive;

		/// <summary> The game <see cref="GameServiceContainer"/>, used to provide generic services. </summary>
		public GameServiceContainer Services => _game.Services;

		/// <summary> The game <see cref="ContentService"/>. </summary>
		public ContentService Content => _game.Content;

		/// <summary> The <see cref="KeyboardManager"/> used to track key states. </summary>
		public KeyboardManager Keyboard => _game.Keyboard;

		/// <summary> The <see cref="Camera"/> used to position the rendering of the game. </summary>
		public Camera Camera { get; private init; }

		/// <summary> All entities in the scene which have no parent entity. </summary>
		public IReadOnlyList<Entity> RootEntities => _rootEntities;
		private List<Entity> _rootEntities = new List<Entity>(Config.RootEntityMemoryReservation);

		/// <summary> The current number of entities in this scene. </summary>
		public int EntityCount { get; private set; }

		/// <summary> Fires each game update. </summary>
		public Action<GameTime> Updating { get; set; }

		/// <summary> Fires when creating the Imgui interface. </summary>
		public Action<GameTime> ImGuiUpdating { get; set; }

		public Scene(AminoGame game) : this(game, game)
		{

		}

		public Scene(IGameServiceProvider game) : this(game, null)
		{

		}

		private Scene(IGameServiceProvider game, IViewServiceProvider? view = null) : base()
        {
			_game = game;
			game.Updating += Update;
			game.ImGuiUpdating += ImGui;

			Entity cameraEntity = new Entity(this, "Camera");
			Camera = Camera.Create(cameraEntity);

			if (view != null)
			{
				_view = view;
				Renderer = new RenderService(game, view, Camera);
			}

			
		}

		public T GetService<T>() where T : class => Services.GetService<T>();

		protected void Update(GameTime gameTime)
		{
			Updating?.Invoke(gameTime);
		}

		private bool _checkbox = false;

		protected void ImGui(GameTime gameTime)
		{
			
		}

		

		public void OnEntityCreated(Entity newEntity, bool isRoot = false)
		{
			if(isRoot)
			{
				OnEntityRooted(newEntity);
			}
			EntityCount++;
		}

		public void OnEntityDestroyed(Entity destroyedEntity, bool isRoot = false)
		{
			if (isRoot && !_rootEntities.Remove(destroyedEntity))
			{
				throw new InvalidOperationException($"Entity '{destroyedEntity}' was reported as destroyed root-entity to the scene, but was not in the root-entities collection.");
			}
			if (EntityCount == 0)
			{
				throw new InvalidOperationException($"Entity '{destroyedEntity}' was reported as destroyed root-entity to the scene, yet the entity count is zero. Is the entity count incorrect?");
			}
			EntityCount--;
		}

		public void OnEntityRooted(Entity rootEntity)
		{
			_rootEntities.Add(rootEntity);
		}

		public void OnEntityUnrooted(Entity rootEntity)
		{
			if(!_rootEntities.Remove(rootEntity))
			{
				throw new InvalidOperationException($"Entity '{rootEntity}' was reported as no longer being a root-entity to the scene, but was not in the root-entities collection.");
			}
		}

		public void OnComponentCreated(Component newComponent)
		{
			if (newComponent is Sprite asSpriteComponent)
			{
				Renderer?.RegisterSprite(asSpriteComponent);
			}
		}

		public void OnComponentDestroyed(Component destroyedComponent)
		{
			if (destroyedComponent is Sprite asSpriteComponent)
			{
				Renderer?.UnregisterSprite(asSpriteComponent);
			}
		}

		/// <summary> Draw a debug geometry for a single frame. </summary>
		public void DebugDraw(IDebugRenderRequest order) => Renderer?.Debug.Order(order);
	}
}
