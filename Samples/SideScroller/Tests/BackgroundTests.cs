﻿using DeltaEngine.Platforms;
using NUnit.Framework;

namespace SideScroller.Tests
{
	internal class BackgroundTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateParallaxBackground()
		{
			var parallaxBackground = new ParallaxBackground(4, layerImageNames, layerScrollFactors);
			parallaxBackground.BaseSpeed = 0.3f;
			parallaxBackground.RenderLayer = 1;
		}

		private readonly string[] layerImageNames = new[]
		{ "SkyBackground", "Mountains_Back", "Mountains_Middle", "Mountains_Front" };
		private readonly float[] layerScrollFactors = new[] { 0.4f, 0.6f, 1.0f, 1.4f };
	}
}