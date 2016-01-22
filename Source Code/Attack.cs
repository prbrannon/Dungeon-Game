using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Attack : Entity
    {
        #region Members

        Vector2 source_center;
        int sourceID;           // The unique ID of the attacker. Attacker shouldn't hit themselves
        int total_frames;       // The amount of time that the attack exists
        int remaining_frames;     // The amount of frames that an attack exists
        bool multi_hit;         // False if the attack gets deleted after hitting one thing

        #endregion

        #region Constructors
        public Attack() : base()
        {
            Initialize(-1);
        }

        // Create class with these parameters:
        private void Initialize(int i_sourceID)
        {
            source_center = new Vector2();
            sourceID = i_sourceID;
            total_frames = 0;
            remaining_frames = 0;
            multi_hit = false;
        }
        #endregion

        #region Properties

        public float SourceCenterX
        {
            set { source_center.X = value; }
            get { return source_center.X; }
        }

        public float SourceCenterY
        {
            set { source_center.Y = value; }
            get { return source_center.Y; }
        }

        public int SourceID
        {
            set { sourceID = value; }
            get { return sourceID; }
        }

        public int RemainingFrames
        {
            set { remaining_frames = value; }
            get { return remaining_frames; }
        }

        public int MaxFrames
        {
            set 
            { 
                total_frames = value;
                remaining_frames = value;
            }
            get { return total_frames; }
        }

        public bool Multihit
        {
            set { multi_hit = value; }
            get { return multi_hit; }
        }

        #endregion

        #region Methods

        public override void Update()
        {
            base.Update();

            remaining_frames--;
        }

        // See if the attack is finished
        public virtual bool Is_Dead()
        {
            if (remaining_frames <= 0 || CurrentHealth <= 0)
                return true;
            return false;
        }
        #endregion
    }
}
