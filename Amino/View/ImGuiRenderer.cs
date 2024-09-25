using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Amino
{
	/// <summary>
	/// Renders Immediate GUI, which can be used for debugging. Also handles input for ImGui.
	/// Mostly taken from ImGUI's example project for monogame.
	/// https://github.com/ImGuiNET/ImGui.NET/tree/master/src/ImGui.NET.SampleProgram.XNA
	/// </summary>
	public class ImGuiRenderer
	{
		private IGameServiceProvider _game;
		private IViewServiceProvider _view;
		private GraphicsDevice GraphicsDevice => _view.GraphicsDevice;
		private BasicEffect _effect;
		private RasterizerState _rasterizerState;

		private byte[] _vertexData;
		private VertexBuffer _vertexBuffer;
		private int _vertexBufferSize;

		private byte[] _indexData;
		private IndexBuffer _indexBuffer;
		private int _indexBufferSize;

		/// <summary> Any texture loaded for rendering in ImGui. </summary>
		private Dictionary<IntPtr, Texture2D> _loadedTextures = new Dictionary<IntPtr, Texture2D>();
		private int _textureId;
		private IntPtr? _fontTextureId;

		/// <summary> Tracks the current scroll-wheel value. </summary>
		private int _scrollWheelValue;
		/// <summary> Tracks the current horizontal scroll-wheel value. </summary>
		private int _horizontalScrollWheelValue;
		private Keys[] _allKeys = Enum.GetValues<Keys>();

		public ImGuiRenderer(IGameServiceProvider game, IViewServiceProvider view)
		{
			IntPtr context = ImGui.CreateContext();
			ImGui.SetCurrentContext(context);

			_game = game;
			_view = view;

			_rasterizerState = new RasterizerState()
			{
				CullMode = CullMode.None,
				DepthBias = 0,
				FillMode = FillMode.Solid,
				MultiSampleAntiAlias = false,
				ScissorTestEnable = true,
				SlopeScaleDepthBias = 0
			};
		}

		private void OnTextInput(object? sender, Microsoft.Xna.Framework.TextInputEventArgs e)
		{
			if (e.Character == '\t')
			{
				return;
			}
			ImGui.GetIO().AddInputCharacter(e.Character);
		}

		public virtual unsafe void RebuildFontAtlas()
		{
			ImGuiIOPtr io = ImGui.GetIO();
			io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int bytesPerPixel);

			// Copy the data to a managed array
			var pixels = new byte[width * height * bytesPerPixel];
			unsafe { Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length); }

			// Create and register the texture as an XNA texture
			var tex2d = new Texture2D(GraphicsDevice, width, height, false, SurfaceFormat.Color);
			tex2d.SetData(pixels);

			// Should a texture already have been build previously, unbind it first so it can be deallocated
			if (_fontTextureId.HasValue)
			{
				UnbindTexture(_fontTextureId.Value);
			}

			// Bind the new texture to an ImGui-friendly id
			_fontTextureId = BindTexture(tex2d);

			// Let ImGui know where to find the texture
			io.Fonts.SetTexID(_fontTextureId.Value);
			io.Fonts.ClearTexData(); // Clears CPU side texture data
		}

		public virtual IntPtr BindTexture(Texture2D texture)
		{
			var id = new IntPtr(_textureId++);

			_loadedTextures.Add(id, texture);

			return id;
		}

		public virtual void UnbindTexture(IntPtr textureId)
		{
			_loadedTextures.Remove(textureId);
		}

		public void BeforeImGui(GameTime gameTime)
		{
			ImGui.GetIO().DeltaTime = gameTime.Delta();
			UpdateInput();
			ImGui.NewFrame();
		}

		public void AfterImGui()
		{
			ImGui.Render();
			unsafe { RenderDrawData(ImGui.GetDrawData()); }
		}

		/// <summary> Send the current XNA input state to ImGui. </summary>
		private void UpdateInput()
		{
			if (!_game.IsActive)
			{
				return;
			}

			ImGuiIOPtr io = ImGui.GetIO();
			MouseState mouse = Mouse.GetState();
			KeyboardState keyboard = Keyboard.GetState();
			io.AddMousePosEvent(mouse.X, mouse.Y);
			io.AddMouseButtonEvent(0, mouse.LeftButton == ButtonState.Pressed);
			io.AddMouseButtonEvent(1, mouse.RightButton == ButtonState.Pressed);
			io.AddMouseButtonEvent(2, mouse.MiddleButton == ButtonState.Pressed);
			io.AddMouseButtonEvent(3, mouse.XButton1 == ButtonState.Pressed);
			io.AddMouseButtonEvent(4, mouse.XButton2 == ButtonState.Pressed);

			io.AddMouseWheelEvent(
				(mouse.HorizontalScrollWheelValue - _horizontalScrollWheelValue) / Config.ImGuiScrollWheelSpeed,
				(mouse.ScrollWheelValue - _scrollWheelValue) / Config.ImGuiScrollWheelSpeed);
			_scrollWheelValue = mouse.ScrollWheelValue;
			_horizontalScrollWheelValue = mouse.HorizontalScrollWheelValue;

			foreach (Keys key in _allKeys)
			{
				if (TryGetImGuiKey(key, out ImGuiKey asImGuiKey))
				{
					io.AddKeyEvent(asImGuiKey, keyboard.IsKeyDown(key));
				}
			}

			io.DisplaySize = new System.Numerics.Vector2(_view.ViewportDimensions.X, _view.ViewportDimensions.Y);
			io.DisplayFramebufferScale = System.Numerics.Vector2.One;
		}

		/// <summary> Attempt to convert the provided <paramref name="xnaKey"/> to an <see cref="ImGuiKey"/>. </summary>
		private bool TryGetImGuiKey(Keys xnaKey, out ImGuiKey imGuiKey)
		{
			imGuiKey = xnaKey switch
			{
				Keys.Back => ImGuiKey.Backspace,
				Keys.Tab => ImGuiKey.Tab,
				Keys.Enter => ImGuiKey.Enter,
				Keys.CapsLock => ImGuiKey.CapsLock,
				Keys.Escape => ImGuiKey.Escape,
				Keys.Space => ImGuiKey.Space,
				Keys.PageUp => ImGuiKey.PageUp,
				Keys.PageDown => ImGuiKey.PageDown,
				Keys.End => ImGuiKey.End,
				Keys.Home => ImGuiKey.Home,
				Keys.Left => ImGuiKey.LeftArrow,
				Keys.Right => ImGuiKey.RightArrow,
				Keys.Up => ImGuiKey.UpArrow,
				Keys.Down => ImGuiKey.DownArrow,
				Keys.PrintScreen => ImGuiKey.PrintScreen,
				Keys.Insert => ImGuiKey.Insert,
				Keys.Delete => ImGuiKey.Delete,
				>= Keys.D0 and <= Keys.D9 => ImGuiKey._0 + (xnaKey - Keys.D0),
				>= Keys.A and <= Keys.Z => ImGuiKey.A + (xnaKey - Keys.A),
				>= Keys.NumPad0 and <= Keys.NumPad9 => ImGuiKey.Keypad0 + (xnaKey - Keys.NumPad0),
				Keys.Multiply => ImGuiKey.KeypadMultiply,
				Keys.Add => ImGuiKey.KeypadAdd,
				Keys.Subtract => ImGuiKey.KeypadSubtract,
				Keys.Decimal => ImGuiKey.KeypadDecimal,
				Keys.Divide => ImGuiKey.KeypadDivide,
				>= Keys.F1 and <= Keys.F12 => ImGuiKey.F1 + (xnaKey - Keys.F1),
				Keys.NumLock => ImGuiKey.NumLock,
				Keys.Scroll => ImGuiKey.ScrollLock,
				Keys.LeftShift or Keys.RightShift => ImGuiKey.ModShift,
				Keys.LeftControl or Keys.RightControl => ImGuiKey.ModCtrl,
				Keys.LeftAlt or Keys.RightAlt => ImGuiKey.ModAlt,
				Keys.OemSemicolon => ImGuiKey.Semicolon,
				Keys.OemPlus => ImGuiKey.Equal,
				Keys.OemComma => ImGuiKey.Comma,
				Keys.OemMinus => ImGuiKey.Minus,
				Keys.OemPeriod => ImGuiKey.Period,
				Keys.OemQuestion => ImGuiKey.Slash,
				Keys.OemTilde => ImGuiKey.GraveAccent,
				Keys.OemOpenBrackets => ImGuiKey.LeftBracket,
				Keys.OemCloseBrackets => ImGuiKey.RightBracket,
				Keys.OemPipe => ImGuiKey.Backslash,
				Keys.OemQuotes => ImGuiKey.Apostrophe,
				_ => ImGuiKey.None,
			};
			return imGuiKey != ImGuiKey.None;
		}

		private void RenderDrawData(ImDrawDataPtr drawData)
		{
			// Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers
			GraphicsDevice.BlendFactor = Color.White;
			GraphicsDevice.BlendState = BlendState.NonPremultiplied;
			GraphicsDevice.RasterizerState = _rasterizerState;
			GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

			// Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
			drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

			GraphicsDevice.Viewport = new Viewport(0, 0, _view.ViewportDimensions.X, _view.ViewportDimensions.Y);

			UpdateBuffers(drawData);
			RenderCommandLists(drawData);
		}

		private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
		{
			if (drawData.TotalVtxCount == 0)
			{
				return;
			}

			// Expand buffers if we need more room.
			if (drawData.TotalVtxCount > _vertexBufferSize)
			{
				_vertexBuffer?.Dispose();

				_vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
				_vertexBuffer = new VertexBuffer(GraphicsDevice, ImGuiVertDeclaration.Declaration, _vertexBufferSize, BufferUsage.None);
				_vertexData = new byte[_vertexBufferSize * ImGuiVertDeclaration.Size];
			}

			if (drawData.TotalIdxCount > _indexBufferSize)
			{
				_indexBuffer?.Dispose();

				_indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
				_indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, _indexBufferSize, BufferUsage.None);
				_indexData = new byte[_indexBufferSize * sizeof(ushort)];
			}

			// Copy ImGui's vertices and indices to a set of managed byte arrays.
			int vtxOffset = 0;
			int idxOffset = 0;

			for (int cl = 0; cl < drawData.CmdListsCount; cl++)
			{
				ImDrawListPtr cmdList = drawData.CmdListsRange[cl];

				fixed (void* vtxDstPtr = &_vertexData[vtxOffset * ImGuiVertDeclaration.Size])
				fixed (void* idxDstPtr = &_indexData[idxOffset * sizeof(ushort)])
				{
					Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, vtxDstPtr, _vertexData.Length, cmdList.VtxBuffer.Size * ImGuiVertDeclaration.Size);
					Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, idxDstPtr, _indexData.Length, cmdList.IdxBuffer.Size * sizeof(ushort));
				}

				vtxOffset += cmdList.VtxBuffer.Size;
				idxOffset = cmdList.IdxBuffer.Size;
			}

			// Copy the managed byte arrays to the gpu vertex and index buffers.
			_vertexBuffer.SetData(_vertexData, 0, drawData.TotalVtxCount * ImGuiVertDeclaration.Size);
			_indexBuffer.SetData(_indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
		}

		public void RenderCommandLists(ImDrawDataPtr drawData)
		{
			GraphicsDevice.SetVertexBuffer(_vertexBuffer);
			GraphicsDevice.Indices = _indexBuffer;

			int vtxOffset = 0;
			int idxOffset = 0;

			for (int cl = 0; cl < drawData.CmdListsCount; cl++)
			{
				ImDrawListPtr cmdList = drawData.CmdListsRange[cl];

				for (int c = 0; c < cmdList.CmdBuffer.Size; c++)
				{
					ImDrawCmdPtr drawCmd = cmdList.CmdBuffer[c];

					if (drawCmd.ElemCount == 0)
					{
						continue;
					}

					if (!_loadedTextures.ContainsKey(drawCmd.TextureId))
					{
						throw new InvalidOperationException($"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings.");
					}

					GraphicsDevice.ScissorRectangle = new Rectangle(
						(int)drawCmd.ClipRect.X,
						(int)drawCmd.ClipRect.Y,
						(int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
						(int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y)
					);

					var effect = UpdateEffect(_loadedTextures[drawCmd.TextureId]);

					foreach (var pass in effect.CurrentTechnique.Passes)
					{
						pass.Apply();

						GraphicsDevice.DrawIndexedPrimitives(
							primitiveType: PrimitiveType.TriangleList,
							baseVertex: (int)drawCmd.VtxOffset + vtxOffset,
							startIndex: (int)drawCmd.IdxOffset + idxOffset,
							primitiveCount: (int)drawCmd.ElemCount / 3
						);
					}
				}

				vtxOffset += cmdList.VtxBuffer.Size;
				idxOffset += cmdList.IdxBuffer.Size;
			}
		}

		private Effect UpdateEffect(Texture2D texture)
		{
			_effect = _effect ?? new BasicEffect(GraphicsDevice);

			var io = ImGui.GetIO();

			_effect.World = Matrix.Identity;
			_effect.View = Matrix.Identity;
			_effect.Projection = Matrix.CreateOrthographicOffCenter(0f, io.DisplaySize.X, io.DisplaySize.Y, 0f, -1f, 1f);
			_effect.TextureEnabled = true;
			_effect.Texture = texture;
			_effect.VertexColorEnabled = true;

			return _effect;
		}
	}
}
