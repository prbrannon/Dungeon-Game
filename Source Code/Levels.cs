using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class Levels
    {
        #region Levels
        private static Levels instance;

        private Dictionary<int, Room> rooms;

        int num_rooms;

        #endregion

        #region Constructors
        private Levels()
        {
            rooms = new Dictionary<int, Room>();
            num_rooms = 0;
        }

        #endregion

        #region Methods
        // Get the instance of the Graphics class. Creates a new one on the first access
        public static Levels Instance()
        {
            if (instance == null)
                instance = new Levels();
            return instance;
        }

        // Load textures from a file into the Textures dictionary
        // The int is the ID number of the sprite
        public void Load_Rooms()
        {
            // TODO FIX ONE CHARACTER READING per line plszs C# fo' life

            // create reader & open file
            StreamReader reader = new StreamReader("levels.txt");

            
            num_rooms = ReadNextInt(reader);

            int width = -1;
            width = ReadNextInt(reader);

            int height = -1;
            height = ReadNextInt(reader);

            Room temp_room;

            for(int mason = 0; mason < num_rooms; mason++)
            {
                int room_type = ReadNextInt(reader);

                int number_enemies = ReadNextInt(reader);

                temp_room = new Room(-1);

                // Fill the room tiles
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        temp_room.Set_Tile_Type(x, y, ReadNextInt(reader));
                    }
                }

                // Add enemies to the dictionary at the key index
                for (int i = 0; i < number_enemies; i++)
                {
                    temp_room.Add_Entity(ReadNextInt(reader), new Microsoft.Xna.Framework.Vector3(ReadNextInt(reader), ReadNextInt(reader), 0));
                }

                // Set the room type
                temp_room.Type = room_type;

                rooms.Add(mason, temp_room);
            }

            // close the stream
            reader.Close();
        }

        private bool EndOfLine(ref StreamReader reader)
        {
            // Is the char an #
            if (reader.Peek() == 35)
            {
                reader.ReadLine();
                return true;
            }
            return false;
        }

        private int ReadNextInt(StreamReader reader)
        {
            while (!(reader.Peek() >= 48 && reader.Peek() <= 57))
            {
                reader.Read();
            }

            int temp = 0;
            int read_digit = 0;

            while (reader.Peek() >= 48 && reader.Peek() <= 57)
            {
                int ascii_num = reader.Read();
                switch (ascii_num)
                {
                    case 48: read_digit = 0; break;
                    case 49: read_digit = 1; break;
                    case 50: read_digit = 2; break;
                    case 51: read_digit = 3; break;
                    case 52: read_digit = 4; break;
                    case 53: read_digit = 5; break;
                    case 54: read_digit = 6; break;
                    case 55: read_digit = 7; break;
                    case 56: read_digit = 8; break;
                    case 57: read_digit = 9; break;
                }
                temp = temp * 10 + read_digit;
            }

            reader.Read();

            return temp;
        }

        public Room GetRandomRoom(int type)
        {
            if (num_rooms == 0)
                return new Room();
            while(true)
            {
                int random_index = Generators.Instance().NextRandom(0 , num_rooms - 1);
                if(rooms[random_index].Type == type)
                {
                    return rooms[random_index];
                }
            }
        }

        #endregion
    }
}
