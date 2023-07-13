using Microsoft.Xna.Framework.Input;

namespace Amino
{
	/// <summary> Configurations driving the behaviour of an <see cref="AminoGame"/> and its entities. </summary>
	public static class Config
	{
		/// <summary> If set, this is the asset name of the default sprite used for all sprite-related components with an unset sprite. </summary>
		public static string DefaultSprite = null;
		/// <summary> The default pixels of a sprite rendered in one game-unit. e.g. if 32, a 64 x 32 sprite will appear 2 units long and 1 unit tall. </summary>
		public const float DefaultPixelsPerUnit = 32f;
		/// <summary> The default width of the camera's field of view, in game units. </summary>
		public const float DefaultCameraZoom = 32f;
		/// <summary>
		/// The furthest that a camera can zoom in.
		/// A value of zero risks the camera getting stuck at an infinitely small zoom.
		/// </summary>
		public const float MinimumCameraZoom = 0.01f;
		/// <summary> The size of the root entity list within a scene upon its initialisation. </summary>
		public const int RootEntityMemoryReservation = 64;
		/// <summary> The key used to toggle the display of immediate GUI. </summary>
		public const Keys ImGuiKey = Keys.F5;
		/// <summary> The speed at which ImGui scrolls with the scroll wheel. </summary>
		public const float ImGuiScrollWheelSpeed = 120;
	}
}
