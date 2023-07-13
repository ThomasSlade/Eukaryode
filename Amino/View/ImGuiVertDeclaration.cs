using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace Amino
{
	/// <summary>
	/// Provides a <see cref="VertexDeclaration"/> for use by <see cref="ImGuiRenderer"/>.
	/// </summary>
	public static class ImGuiVertDeclaration
	{
		public static readonly VertexDeclaration Declaration;
		public static readonly int Size;

		static ImGuiVertDeclaration()
		{
			unsafe { Size = sizeof(ImDrawVert); }

			Declaration = new VertexDeclaration(
				Size,
				// Position
				new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
				// UV
				new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
				// Color
				new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
			);
		}
	}
}
