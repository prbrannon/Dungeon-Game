using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Sounds
    {
        #region Members
        private static Sounds instance;     // Used to play music and make it loop. Only one track at a time

        private SoundEffectInstance sfx_instance;

        private Dictionary<int, SoundEffect> musics;
        private Dictionary<int, SoundEffect> sound_effects;

        #endregion

        #region Constructors
        private Sounds()
        {
            musics = new Dictionary<int, SoundEffect>();
            sound_effects = new Dictionary<int, SoundEffect>();
        }

        #endregion

        #region Methods
        // Get the instance of the Graphics class. Creates a new one on the first access
        public static Sounds Instance()
        {
            if (instance == null)
                instance = new Sounds();
            return instance;
        }

        // Load textures from a file into the Textures dictionary
        // The int is the ID number of the sprite
        public void Load_Sounds(ContentManager Content)
        {
            // Music
            musics.Add(0, Content.Load<SoundEffect>(@"audio\Music\song"));
            musics.Add(1, Content.Load<SoundEffect>(@"audio\Music\boss"));
            musics.Add(2, Content.Load<SoundEffect>(@"audio\Music\game_over"));
            musics.Add(3, Content.Load<SoundEffect>(@"audio\Music\victory"));

            // Sound Effects
            sound_effects.Add(21, Content.Load<SoundEffect>(@"audio\SoundEffects\arrow"));
            sound_effects.Add(10, Content.Load<SoundEffect>(@"audio\SoundEffects\boss_wait"));
            sound_effects.Add(11, Content.Load<SoundEffect>(@"audio\SoundEffects\boss_charge"));
            sound_effects.Add(31, Content.Load<SoundEffect>(@"audio\SoundEffects\clang"));
            sound_effects.Add(41, Content.Load<SoundEffect>(@"audio\SoundEffects\fireSound"));
            sound_effects.Add(200, Content.Load<SoundEffect>(@"audio\SoundEffects\menu_sound"));
            sound_effects.Add(0, Content.Load<SoundEffect>(@"audio\SoundEffects\player_death"));
            sound_effects.Add(2, Content.Load<SoundEffect>(@"audio\SoundEffects\playerhit"));
            sound_effects.Add(1, Content.Load<SoundEffect>(@"audio\SoundEffects\enemy_death"));
            sound_effects.Add(3, Content.Load<SoundEffect>(@"audio\SoundEffects\thump"));

            sound_effects.Add(101, Content.Load<SoundEffect>(@"audio\SoundEffects\step1"));
            sound_effects.Add(102, Content.Load<SoundEffect>(@"audio\SoundEffects\step2"));
            sound_effects.Add(103, Content.Load<SoundEffect>(@"audio\SoundEffects\step3"));
            sound_effects.Add(104, Content.Load<SoundEffect>(@"audio\SoundEffects\step4"));
        }

        public void PlayMusic(int id, bool looped)
        {
            // Set the soundeffectsinstance to the music file and play it
            sfx_instance = musics[id].CreateInstance();
            sfx_instance.IsLooped = looped;
            sfx_instance.Play();
        }

        public void StopMusic()
        {
            sfx_instance.Stop(true);
        }

        public void PlaySoundEffect(int id)
        {
            sound_effects[id].Play();
        }

        #endregion
    }
}
