using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Projectile : Attack
    {
        #region Constructors
        public Projectile() : base()
        {
            Initialize();
        }

        // Create class with these parameters:
        private void Initialize()
        {
            return;
        }
        #endregion

        #region Properties

        public override float Radius
        {
            get { return (Height + Width) / 8; }
        }

        #endregion

        #region Methods
        public override void Update()
        {
            base.Update();
        }

        // Override drawing to not draw a shadow
        public override void Draw(SpriteBatch batch, Camera camera)
        {
            // Draw the actual entity over the shadow
            DrawEntity(batch, camera);
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
                       Scale,
                       SpriteEffects.None,
                       Center.Y / 100000);
        }
        #endregion
    }
}
