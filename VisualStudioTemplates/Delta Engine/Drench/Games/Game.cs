using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes.UserInterfaces.Controls;
using DeltaEngine.ScreenSpaces;
using $safeprojectname$.Logics;

namespace $safeprojectname$.Games
{
	public abstract class Game
	{
		protected Game(Logic logic)
		{
			this.logic = logic;
			buttons = new InteractiveButton[logic.Board.Width, logic.Board.Height];
			ArrangeScene();
			ScreenSpace.Current.ViewportSizeChanged += ArrangeScene;
		}

		protected readonly Logic logic;
		private readonly InteractiveButton[,] buttons;

		protected void ArrangeScene()
		{
			upperText.DrawArea = new Rectangle(0.0f, ScreenSpace.Current.Top, 1.0f, Border);
			lowerText.DrawArea = new Rectangle(0.0f, ScreenSpace.Current.Bottom - Border, 1.0f, Border);
			buttonsLeft = ScreenSpace.Current.Left + Border;
			buttonsTop = ScreenSpace.Current.Top + Border;
			float width = ScreenSpace.Current.Right - ScreenSpace.Current.Left - 2 * Border;
			float height = ScreenSpace.Current.Bottom - ScreenSpace.Current.Top - 2 * Border;
			buttonWidth = width / logic.Board.Width;
			buttonHeight = height / logic.Board.Height;
			CreateButtons();
		}

		internal const float Border = 0.1f;
		internal readonly FontText upperText = new FontText(FontXml.Default, "", Rectangle.Zero);
		internal readonly FontText lowerText = new FontText(FontXml.Default, "", Rectangle.Zero);
		private float buttonsLeft;
		private float buttonsTop;
		private float buttonWidth;
		private float buttonHeight;

		private void CreateButtons()
		{
			for (int x = 0; x < logic.Board.Width; x++)
				for (int y = 0; y < logic.Board.Height; y++)
					CreateButton(x, y);
		}

		private void CreateButton(int x, int y)
		{
			if (buttons [x, y] != null)
				buttons [x, y].IsActive = false;

			var theme = CreateButtonTheme(x, y);
			var drawArea = new Rectangle(buttonsLeft + x * buttonWidth, buttonsTop + y * buttonHeight, 
				buttonWidth, buttonHeight);
			buttons [x, y] = new InteractiveButton(theme, drawArea);
			buttons [x, y].Clicked = () => ButtonClicked(x, y);
		}

		private Theme CreateButtonTheme(int x, int y)
		{
			var color = logic.Board.GetColor(x, y);
			var darkColor = new Color(color.RedValue * 0.7f, color.GreenValue * 0.7f, color.BlueValue * 
				0.7f);
			var lightColor = new Color(color.RedValue * 0.85f, color.GreenValue * 0.85f, 
				color.BlueValue * 0.85f);
			return new Theme {
				Button = new Theme.Appearance("DefaultLabel", darkColor),
				ButtonDisabled = new Theme.Appearance("DefaultLabel", Color.Grey),
				ButtonMouseover = new Theme.Appearance("DefaultLabel", lightColor),
				ButtonPressed = new Theme.Appearance("DefaultLabel", color)
			};
		}

		private void ButtonClicked(int x, int y)
		{
			if (logic.IsGameOver)
			{
				ExitGame();
				return;
			}
			if (!ProcessDesiredMove(x, y))
				return;

			ArrangeScene();
			if (logic.IsGameOver)
				GameOver();
		}

		private void ExitGame()
		{
			for (int x = 0; x < logic.Board.Width; x++)
				for (int y = 0; y < logic.Board.Height; y++)
					if (buttons [x, y] != null)
						buttons [x, y].IsActive = false;

			upperText.IsActive = false;
			lowerText.IsActive = false;
			if (Exited != null)
				Exited();
		}

		public event Action Exited;

		protected abstract bool ProcessDesiredMove(int x, int y);

		protected abstract void GameOver();
	}
}