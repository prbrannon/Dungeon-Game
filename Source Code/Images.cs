using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Images
    {
        #region Members
        private static Images instance;

        private Dictionary<int, Texture2D> entity_textures;
        private Dictionary<int, Texture2D> room_textures;
        private Dictionary<int, Texture2D> HUD_textures;
        private Dictionary<int, Texture2D> Menu_textures;

        #endregion

        #region Constructors
        private Images()
        {
            entity_textures = new Dictionary<int, Texture2D>();
            room_textures = new Dictionary<int, Texture2D>();
            HUD_textures = new Dictionary<int, Texture2D>();
            Menu_textures = new Dictionary<int, Texture2D>();
        }

        #endregion

        #region Methods
        // Get the instance of the Graphics class. Creates a new one on the first access
        public static Images Instance()
        {
            if (instance == null)
                instance = new Images();
            return instance;
        }

        // Load textures from a file into the Textures dictionary
        // The int is the ID number of the sprite
        public void Load_Textures(ContentManager Content)
        {
            // Entities
            entity_textures.Add(0, Content.Load<Texture2D>(@"graphics\Entities\playerPic"));
            entity_textures.Add(1, Content.Load<Texture2D>(@"graphics\Entities\chaser"));
            entity_textures.Add(2, Content.Load<Texture2D>(@"graphics\Entities\shooter"));
            entity_textures.Add(3, Content.Load<Texture2D>(@"graphics\Entities\bat"));
            entity_textures.Add(10, Content.Load<Texture2D>(@"graphics\Entities\tankMidget"));
            entity_textures.Add(21, Content.Load<Texture2D>(@"graphics\Entities\arrow"));
            entity_textures.Add(31, Content.Load<Texture2D>(@"graphics\Entities\sword"));
            entity_textures.Add(41, Content.Load<Texture2D>(@"graphics\Entities\fireball"));
            entity_textures.Add(100, Content.Load<Texture2D>(@"graphics\Entities\shadow"));

            // Room
            room_textures.Add(0, Content.Load<Texture2D>(@"graphics\Room\floor"));
            room_textures.Add(1, Content.Load<Texture2D>(@"graphics\Room\wallTile"));
            room_textures.Add(50, Content.Load<Texture2D>(@"graphics\Room\wallColumn"));
            room_textures.Add(100, Content.Load<Texture2D>(@"graphics\Room\edge"));
            room_textures.Add(101, Content.Load<Texture2D>(@"graphics\Room\closeddoor"));
            room_textures.Add(102, Content.Load<Texture2D>(@"graphics\Room\blocked"));

            // HUD
            HUD_textures.Add(10, Content.Load<Texture2D>(@"graphics\HUD\status_bar"));
            HUD_textures.Add(11, Content.Load<Texture2D>(@"graphics\HUD\health_overlay"));
            HUD_textures.Add(12, Content.Load<Texture2D>(@"graphics\HUD\lost_health"));
            HUD_textures.Add(13, Content.Load<Texture2D>(@"graphics\HUD\stamina_overlay"));

            // Map
            HUD_textures.Add(-4, Content.Load<Texture2D>(@"graphics\HUD\Map\visibleRoom"));
            HUD_textures.Add(-3, Content.Load<Texture2D>(@"graphics\HUD\Map\connector"));
            HUD_textures.Add(-2, Content.Load<Texture2D>(@"graphics\HUD\Map\currentRoom"));
            HUD_textures.Add(-1, Content.Load<Texture2D>(@"graphics\HUD\Map\emptyRoom"));
            HUD_textures.Add(0, Content.Load<Texture2D>(@"graphics\HUD\Map\spawnRoom"));
            HUD_textures.Add(1, Content.Load<Texture2D>(@"graphics\HUD\Map\normalRoom"));
            HUD_textures.Add(2, Content.Load<Texture2D>(@"graphics\HUD\Map\bossRoom"));

            // Main Menu (must be from index 0 in order to select options properly)
            Menu_textures.Add(0, Content.Load<Texture2D>(@"graphics\Menu\start_button"));
            Menu_textures.Add(1, Content.Load<Texture2D>(@"graphics\Menu\quit_button"));
            Menu_textures.Add(100, Content.Load<Texture2D>(@"graphics\Menu\title"));
            Menu_textures.Add(200, Content.Load<Texture2D>(@"graphics\Menu\character_screen"));
            Menu_textures.Add(300, Content.Load<Texture2D>(@"graphics\Menu\characters"));
            Menu_textures.Add(400, Content.Load<Texture2D>(@"graphics\Menu\arrow"));
            Menu_textures.Add(500, Content.Load<Texture2D>(@"graphics\Menu\game_over"));
            Menu_textures.Add(600, Content.Load<Texture2D>(@"graphics\Menu\level_complete"));
            Menu_textures.Add(700, Content.Load<Texture2D>(@"graphics\Menu\controller_type"));
        }

        public Texture2D GetEntityTexture(int id)
        {
            return entity_textures[id];
        }

        public Texture2D GetRoomTexture(int id)
        {
            return room_textures[id];
        }

        public Texture2D GetHUDTexture(int id)
        {
            return HUD_textures[id];
        }

        public Texture2D GetMenuTexture(int id)
        {
            return Menu_textures[id];
        }

        #endregion
    }
}
