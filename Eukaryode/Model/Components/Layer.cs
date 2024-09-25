using Amino;
using Microsoft.Xna.Framework;
using System;

namespace Eukaryode
{
	/// <summary>
	/// Represents a single layer within a <see cref="LayerCell"/>. Entities occupy these layers.
	/// </summary>
	public class Layer : Component
	{
		/// <summary> The type of biolayer this layer is - whether it's a surface, subterranean, air environment, etc. </summary>
		private Biolayer _biolayer;
		/// <summary> The sprite component used to display this layer. </summary>
		private Sprite _terrainSprite;

		public static Layer Create(Entity owner, LayerCell ownerCell, string biolayerKey)
			=> Create(owner, ownerCell, owner.World.Content.Load<Biolayer>(biolayerKey));

		public static Layer Create(Entity owner, LayerCell ownerCell, Biolayer biolayer)
		{
			Layer l = new Layer(owner, ownerCell, biolayer);
			l.World.OnComponentCreated(l);
			return l;
		}

		protected Layer(Entity owner, LayerCell ownerCell, Biolayer biolayer) : base(owner)
		{
			_biolayer = biolayer;

			if (!string.IsNullOrEmpty(_biolayer.SpriteKey))
			{
				ownerCell.SurfaceAltitudeChanged += OnSurfaceAltitudeChanged;
				_terrainSprite = Sprite.Create(owner, biolayer.SpriteKey);
				_terrainSprite.PixelsPerUnit = EukaryodeConfig.TerrainPixelsPerUnit;
				_terrainSprite.OffsetType = AnchorType.BottomLeft;
				OnSurfaceAltitudeChanged(ownerCell, EventArgs.Empty);
			}
		}

		private void OnSurfaceAltitudeChanged(object sender, EventArgs e)
		{
			float surfaceAlt = ((LayerCell)sender).SurfaceAltitude;
			float lerpAmount = (surfaceAlt < 0 ? -surfaceAlt : surfaceAlt) / 100f;
			_terrainSprite.Color = Color.Lerp(_biolayer.SpriteColor, surfaceAlt < 0f ? Color.Black : Color.White, lerpAmount);
		}
	}
}
