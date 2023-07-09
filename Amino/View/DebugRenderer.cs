using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Amino
{
	/// <summary> Renders debug geometry on a sprite batch. </summary>
	/// <remarks> Orders to render a shape are stored in the <see cref="_drawOrders"/> collection, then rendered at draw time before, being cleared. </remarks>
	public class DebugRenderer : EntityRenderer
	{
		private List<IDebugRenderRequest> _drawOrders = new List<IDebugRenderRequest>(256);

		public DebugRenderer(RenderService renderer) : base(renderer)
		{

		}

		public void Order(IDebugRenderRequest order)
		{
			_drawOrders.Add(order);
		}

		public override void Draw(GameTime gameTime, ref SpriteBatch batch, Matrix3x3 cameraMatrix)
		{
			foreach (IDebugRenderRequest order in _drawOrders)
			{
				order.Draw(batch, cameraMatrix);
			}

			_drawOrders.Clear();
		}
	}
}
