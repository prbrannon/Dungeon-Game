using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Sword : Melee
    {
        #region Constructors
        public Sword()
            : base()
        {
            Initialize();
        }

        // Create class with these parameters:
        private void Initialize()
        {
            ID = 31;
            MaxSpeed = 0;
            MaxHealth = 100000;
            MaxFrames = 20;
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

            // Melee Properies
            LungeDistance = 128;
        }
        #endregion

        #region Properties


        #endregion

        #region Methods
        public override void Update()
        {
            base.Update();
        }
        #endregion
    }
}
