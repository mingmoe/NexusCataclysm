using FontStashSharp;
using FontStashSharp.Rasterizers.FreeType;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moe.TextEngine;

namespace NexusCataclysm.Client;

internal sealed class FNAGame : Game
{
	public required ILogger<FNAGame> Logger { get; init; }

	private SpriteBatch _spriteBatch = null!;
	private TextEngine _fontEngine = null!;

	public GraphicsDeviceManager DeviceManager { get; init; }

	public FNAGame()
	{
		DeviceManager = new(this)
		{
			PreferredBackBufferWidth = 1280,
			PreferredBackBufferHeight = 720,
			IsFullScreen = false,
			SynchronizeWithVerticalRetrace = true
		};

	}
	protected override void Initialize()
	{
		// FontSystemDefaults.FontLoader = new FreeTypeLoader();
		_spriteBatch = new SpriteBatch(GraphicsDevice);
		_fontEngine = new(GraphicsDevice)
		{
			RasterizerEngine = new FreeTypeEngine(),
			ShapeEngine = new HarfbuzzShapeEngine(),
		};
		base.Initialize();
	}

	protected override void LoadContent()
	{
		base.LoadContent();

		_fontEngine.AddFont(new(new FileFontSource(@"resources/fonts/NotoEmoji-Regular.ttf"), 0));
		_fontEngine.AddFont(new(new FileFontSource(@"resources/fonts/SarasaFixedSC-Regular.ttf"), 0));
	}

	protected override void UnloadContent()
	{
		base.UnloadContent();
	}

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);

		GraphicsDevice.Clear(Color.BurlyWood);

		_spriteBatch.Begin();

		var shapeRun = new ShapeRun()
		{
			FontOptions = new FontOptions()
			{
				PixelHeight = 32,
				PixelWidth = 32,
			},
			Text = "Hello World! jgyG 西郊有密林，助君出重围。😄",
			ShapeOptions = new()
			{
				Direction = TextDirection.LeftToRight,
				Language = "zh-CN",
				Script = "Hans"
			},
		};

		var size = _fontEngine.EstimateSize(shapeRun);

		_fontEngine.DrawString(
				shapeRun,
				new(0, 0),
			_spriteBatch);


		_fontEngine.DrawString(
				new ShapeRun(shapeRun.FontOptions, shapeRun.ShapeOptions)
				{
					Text = $"Estimated size:{size.X}x{size.Y}"
				},
				new(0, size.Y),
			_spriteBatch);

		_spriteBatch.End();
	}
}
