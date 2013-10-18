using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$
{
	public class PlayerShip : Sprite, Updateable
	{
		public PlayerShip() : base(ContentLoader.Load<Material>("Ship2D"), new 
			Rectangle(Vector2D.Half, new Size(.05f)))
		{
			Add(new Velocity2D(Vector2D.Zero, MaximumPlayerVelocity));
			RenderLayer = (int)AsteroidsRenderLayer.Player;
			projectileMaterial = ContentLoader.Load<Material>("Missile");
			timeLastShot = GlobalTime.Current.Milliseconds;
		}

		private const float MaximumPlayerVelocity = .5f;
		private readonly Material projectileMaterial;
		private float timeLastShot;

		public void ShipAccelerate(float amount = 1)
		{
			Get<Velocity2D>().Accelerate(PlayerAcceleration * amount * Time.Delta, Rotation);
		}

		private const float PlayerAcceleration = 1;

		public void SteerLeft(float amount = 1)
		{
			Rotation -= PlayerTurnSpeed * amount * Time.Delta;
		}

		public void SteerRight(float amount = 1)
		{
			Rotation += PlayerTurnSpeed * amount * Time.Delta;
		}

		private const float PlayerTurnSpeed = 160;
		private const float PlayerDecelFactor = 0.7f;

		public void Update()
		{
			MoveUpdate();
			if (!IsFiring || !(GlobalTime.Current.Milliseconds - 1 / CadenceShotsPerMilliSec > 
				timeLastShot))
				return;

			new Projectile(projectileMaterial, DrawArea.Center, Rotation);
			timeLastShot = GlobalTime.Current.Milliseconds;
			if (ProjectileFired != null)
				ProjectileFired.Invoke();
		}

		private const float CadenceShotsPerMilliSec = 0.003f;

		public bool IsFiring
		{
			get;
			set;
		}

		public event Action ProjectileFired;

		private void MoveUpdate()
		{
			var velocity2D = Get<Velocity2D>();
			velocity2D.velocity -= velocity2D.velocity * PlayerDecelFactor * Time.Delta;
			var nextRect = CalculateRectAfterMove(Time.Delta);
			MoveEntity(nextRect);
			Set(velocity2D);
		}

		private Rectangle CalculateRectAfterMove(float deltaT)
		{
			return new Rectangle(DrawArea.TopLeft + Get<Velocity2D>().velocity * deltaT, Size);
		}

		private void MoveEntity(Rectangle rect)
		{
			StopAtBorder(rect);
			Set(rect);
		}

		private void StopAtBorder(Rectangle rect)
		{
			var vel = Get<Velocity2D>();
			var borders = ScreenSpace.Current.Viewport;
			CheckStopRightBorder(rect, vel, borders);
			CheckStopLeftBorder(rect, vel, borders);
			CheckStopTopBorder(rect, vel, borders);
			CheckStopBottomBorder(rect, vel, borders);
			Set(vel);
		}

		private static void CheckStopRightBorder(Rectangle rect, Velocity2D vel, Rectangle borders)
		{
			if (rect.Right <= borders.Right)
				return;

			vel.velocity.X = -0.02f;
			rect.Right = borders.Right;
		}

		private static void CheckStopLeftBorder(Rectangle rect, Velocity2D vel, Rectangle borders)
		{
			if (rect.Left >= borders.Left)
				return;

			vel.velocity.X = 0.02f;
			rect.Left = borders.Left;
		}

		private static void CheckStopTopBorder(Rectangle rect, Velocity2D vel, Rectangle borders)
		{
			if (rect.Top >= borders.Top)
				return;

			vel.velocity.Y = 0.02f;
			rect.Top = borders.Top;
		}

		private static void CheckStopBottomBorder(Rectangle rect, Velocity2D vel, Rectangle borders)
		{
			if (rect.Bottom <= borders.Bottom)
				return;

			vel.velocity.Y = -0.02f;
			rect.Bottom = borders.Bottom;
		}

		public bool IsPauseable
		{
			get
			{
				return true;
			}
		}
	}
}