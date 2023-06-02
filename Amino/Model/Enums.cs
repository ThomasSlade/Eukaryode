using Microsoft.Xna.Framework;
using System;

namespace Amino
{
	/// <summary> An enum describing the positioning of something along any of 9 points within a rectangle, for example, the location of a pivot point in a sprite. </summary>
	public enum AnchorType
	{
		BottomLeft,
		Bottom,
		BottomRight,
		Left,
		Centre,
		Right,
		TopLeft,
		Top,
		TopRight,
		/// <summary> The alignment does not fit any of the predefined values of this enum. summary>
		Other
	}

	/// <summary> Contains extension functions for enums. </summary>
	public static class EnumExtensions
	{
		/// <summary> Convert this <see cref="AnchorType"/> to a <see cref="Vector2"/>. </summary>
		public static Vector2 ToVector(this AnchorType value)
		{
			return value switch
			{
				AnchorType.BottomLeft => new Vector2(0f, 1f),
				AnchorType.Bottom => new Vector2(0.5f, 1f),
				AnchorType.BottomRight => new Vector2(1f, 1f),
				AnchorType.Left => new Vector2(0f, 0.5f),
				AnchorType.Centre => new Vector2(0.5f, 0.5f),
				AnchorType.Right => new Vector2(1f, 0.5f),
				AnchorType.TopLeft => new Vector2(0f, 0f),
				AnchorType.Top => new Vector2(0.5f, 0f),
				AnchorType.TopRight => new Vector2(1f, 0f),
				AnchorType.Other => new Vector2(0f, 1f),	// Bottom Left by default.
				_ => throw new NotImplementedException($"A corresponding vector hasn't been defined for {nameof(AnchorType)} '{value}'")
			};
		}

		/// <summary> Convert this <see cref="Vector2"/> to an <see cref="AnchorType"/>. </summary>
		public static AnchorType ToAnchorType(this Vector2 value)
		{
			if(value.X == 0f)
			{
				if(value.Y == 0f)
				{
					return AnchorType.BottomLeft;
				}
				else if (value.Y == 0.5f)
				{
					return AnchorType.Bottom;
				}
				else if(value.Y == 1f)
				{
					return AnchorType.BottomRight;
				}
			}
			else if (value.X == 0.5f)
			{
				if (value.Y == 0f)
				{
					return AnchorType.Left;
				}
				else if (value.Y == 0.5f)
				{
					return AnchorType.Centre;
				}
				else if (value.Y == 1f)
				{
					return AnchorType.Right;
				}
			}
			else if (value.X == 1f)
			{
				if (value.Y == 0f)
				{
					return AnchorType.TopLeft;
				}
				else if (value.Y == 0.5f)
				{
					return AnchorType.Top;
				}
				else if (value.Y == 1f)
				{
					return AnchorType.TopRight;
				}
			}
			return AnchorType.Centre;
		}
	}
}
