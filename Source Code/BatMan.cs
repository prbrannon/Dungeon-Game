using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    // (NOT ACTUALLY A MAN)
    class BatMan : Enemy
    {
        const int CHANGE_LIMIT = 15;

        int change_direction;

        #region Constructors
        public BatMan()
            : base()
        {
            Initialize();
        }

        public BatMan(Vector3 location, Vector2 center, Texture2D pic, Vector3 i_hitbox)
            : base(location, center, pic, i_hitbox)
        {
            Initialize();
        }

        // Create class with these parameters:
        private void Initialize()
        {
            change_direction = 0;
            FrameCounterX = Generators.Instance().NextRandom(0, 3);

            ID = 3;
            MaxSpeed = 3;
            SightRange = 0;
            HitboxWidth = 64;
            HitboxHeight = 64;
            AttackCooldown = 1;
            AttackTypeID = -1;
            MaxHealth = 2;
            CurrentHealth = 2;
            Damage = 1;
            AttackDamage = 0;
            OriginX = HitboxWidth / 2.0f;
            OriginY = HitboxHeight / 2.0f;

            // Animation
            SpriteSheetColumns = 5;
            SpriteSheetRows = 1;
            Width = 64;
            Height = 64;
            FrameTimer = 6;
            FrameCounterY = 0;
        }
        #endregion

        #region Properties

        #endregion

        #region Methods
        public override void Update()
        {
            base.Update();

            // Change the bat's direction once every CHANGE_LIMIT frames
            change_direction++;
            if(change_direction > CHANGE_LIMIT)
            {
                DirectionX =  Generators.Instance().NextRandom(-1, 1);
                DirectionY =  Generators.Instance().NextRandom(-1, 1);
                change_direction = 0;
            }
        }
        #endregion
    }
}
