using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Fireball : Projectile
    {
        #region Constructors
        public Fireball()
            : base()
        {
            Initialize();
        }

        // Create class with these parameters:
        private void Initialize()
        {
            ID = 41;
            MaxSpeed = 2;
            MaxHealth = 1;
            MaxFrames = 180;
            CurrentHealth = MaxHealth;
            Damage = 1;
            HitboxWidth = 64;
            HitboxHeight = 64;
            OriginX = HitboxWidth / 2.0f;
            OriginY = HitboxHeight / 2.0f;

            // Animation
            SpriteSheetColumns = 1;
            SpriteSheetRows = 1;
            Width = 64;
            Height = 64;
        }
        #endregion

        #region Properties


        #endregion

        #region Methods
        public override void Update()
        {
            base.Update();

            Angle = Angle + 0.3f;
        }

        // Draw the entity in 2.5D over the shadow
        protected override void DrawEntity(SpriteBatch batch, Camera camera)
        {
            // Parameters:
            // Texture2D
            // Position - modified to draw above the shadow
            // Sprite frame position - position on the Texture2D to be drawn
            // Tint
            // Angle (radians)
            // Origin (center from 0,0 / position)
            // Scale (percentage-wise)
            // SpriteEffects - flipped or not
            // Depth order (percentage of screen from 0-1f) (sufficiently large to draw sprites in right order)
            batch.Draw(Images.Instance().GetEntityTexture(this.ID),
                       new Vector2(this.Position.X - camera.X, this.Position.Y - camera.Y),
                       new Rectangle(0, 0, Images.Instance().GetEntityTexture(this.ID).Width, Images.Instance().GetEntityTexture(this.ID).Height),
                       Color.White,
                       Angle,
                       Origin,
                       (float)RemainingFrames / MaxFrames,
                       SpriteEffects.None,
                       Center.Y / 100000);
        }
        #endregion
    }
}
