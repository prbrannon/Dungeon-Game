using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    abstract class Enemy : Entity
    {
        #region Members
        bool active;                // Turned on when the enemy has seen a player
        int target_player_index;    // Index of the current player target
        Vector2 target;
        int sight_range;            // Units that the enemy can the players
        #endregion

        #region Constructors
        public Enemy() : base()
        {
            Initialize();
        }

        public Enemy(Vector3 location, Vector2 center, Texture2D pic, Vector3 i_hitbox) : base(location, center, pic, 0, i_hitbox)
        {
            Initialize();
        }

        // Create class with these parameters:
        private void Initialize()
        {
            active = false;
            target_player_index = -1;
            target = new Vector2(0,0);
            sight_range = 0;
            target_player_index = -1;
        }

        #endregion

        #region Properties
        public Vector2 Target
        {
            set { target = value; }
            get { return target; }
        }

        public bool Active
        {
            set { active = value; }
            get { return active; }
        }

        public int TargetPlayer
        {
            set { target_player_index = value; }
            get { return target_player_index; }
        }

        public int SightRange
        {
            set { sight_range = value; }
            get { return sight_range; }
        }
        #endregion

        #region Methods
        public override void Update()
        {
            base.Update();
        }

        // The enemy attacks
        // Pre: none
        // Post: If the attacking cooldown is over, reset the cooldown and attacking is true
        protected virtual bool AttemptAttack()
        {
            AttackDirection = new Vector2(target.X - this.CenterX, target.Y - this.CenterY);
            return base.AttemptAttack();
        }
        #endregion
    }
}
