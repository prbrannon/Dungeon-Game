using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Arrow : Projectile
    {
        #region Constructors
        public Arrow() : base()
        {
            Initialize();
        }

        // Create class with these parameters:
        private void Initialize()
        {
            ID = 21;
            MaxSpeed = 5;
            MaxHealth = 1;
            MaxFrames = 100000;
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

            // Get the tip of the arrow to travel in the correct direction
            float arrow_direction = (float) Math.Atan2(Direction.Y, Direction.X);
            Angle = arrow_direction;
        }
        #endregion
    }
}
