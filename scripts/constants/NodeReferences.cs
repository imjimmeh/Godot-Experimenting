using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.constants
{
    public class NodeReferences
    {
        public const string NORMAL_LEVEL_ROOT = "/root/BaseLevel/Viewport";
        public const string MENU_BODY = "/root/BaseMenu";

        public class Characters
        {
            public const string BODY = "KinematicBody";
        }

        public class Systems
        {
            public const string SYSTEMS_ROOT = NORMAL_LEVEL_ROOT + "/Systems";

            public const string AIManager = SYSTEMS_ROOT + "/AIManager";   
            public const string ASTAR = SYSTEMS_ROOT + "/AStarNavigator";
            public const string GAMESTATE_MANAGER = SYSTEMS_ROOT + "/GameStateManager";
            public const string INPUT_MANAGER = SYSTEMS_ROOT + "/InputManager";
            public const string PERSISTENCE_MANAGER = SYSTEMS_ROOT + "/PersistenceManager";
            public const string SPAWN_MANAGER = SYSTEMS_ROOT + "/SpawnManager";
            public const string UI_MANAGER = SYSTEMS_ROOT + "/UIManager";
        }

        public class BaseLevel
        {
            public const string WORLD_MANAGER = NORMAL_LEVEL_ROOT + "/WorldManager";

            public const string CHARACTER_ROOT = NORMAL_LEVEL_ROOT + "/Characters";
            public const string PLAYER_ROOT = CHARACTER_ROOT + "/Player";
            public const string AI_ROOT = CHARACTER_ROOT + "/AI";

            public class Cameras
            {
                public const string CAMERAS_ROOT = NORMAL_LEVEL_ROOT + "/Cameras";

                public const string MAIN_CAMERA = CAMERAS_ROOT + "/MainCamera";
            }

            public class Effects
            {
                public const string ROOT = NORMAL_LEVEL_ROOT + "/Effects";

                public const string MOVEMENT_PATH = ROOT + "/CharacterMovementPath";
            }
            public const string UI = NORMAL_LEVEL_ROOT + "/UI";
        }

    }
}
