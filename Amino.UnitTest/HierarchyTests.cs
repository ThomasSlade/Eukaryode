using Xunit;

namespace Amino.UnitTest
{
	/// <summary> Tests relating to the 2D entity hierarchy in <see cref="Scene"/>. </summary>
	public class HierarchyTests
	{
		[Fact]
		public void RootEntityCanBeDestroyed()
		{
			DummyGame game = new DummyGame();
			Scene scene = new Scene(game);
			// A scene may contain some default objects (e.g. Camera) so check the initial object count.
			int rawCount = scene.RootEntities.Count;

			Entity e = new Entity(scene);
			Assert.Equal(rawCount + 1, scene.RootEntities.Count);

			e.Destroy();
			Assert.Equal(rawCount, scene.RootEntities.Count);

			// Destroying again should change nothing.
			e.Destroy();
			Assert.Equal(rawCount, scene.RootEntities.Count);
		}

		[Fact]
		public void ChildEntityCanBeDestroyed()
		{
			DummyGame game = new DummyGame();
			Scene scene = new Scene(game);
			int rawCount = scene.RootEntities.Count;

			Entity parent = new Entity(scene);
			Entity child = new Entity(parent);

			Assert.Equal(rawCount + 1, scene.RootEntities.Count);
			Assert.Equal(rawCount + 2, scene.EntityCount);

			child.Destroy();
			Assert.Equal(rawCount + 1, scene.RootEntities.Count);
			Assert.Equal(rawCount + 1, scene.EntityCount);
		}

		[Fact]
		public void EntityDestructionClearsChildren()
		{
			DummyGame game = new DummyGame();
			Scene scene = new Scene(game);
			int rawCount = scene.RootEntities.Count;

			Entity parent = new Entity(scene);
			Entity child = new Entity(parent);
			Entity grandChild1 = new Entity(child);
			Entity grandChild2 = new Entity(child);
			Entity greatGrandChild = new Entity(grandChild1);

			Assert.Equal(rawCount + 1, scene.RootEntities.Count);
			Assert.Equal(rawCount + 5, scene.EntityCount);

			child.Destroy();
			Assert.Equal(rawCount + 1, scene.RootEntities.Count);
			Assert.Equal(rawCount + 1, scene.EntityCount);
		}
	}
}
