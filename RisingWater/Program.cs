using System;
using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

namespace RisingWater
{
    public class Program
    {
        /// <summary>
        /// Aspect Ratio
        /// </summary>
        public static readonly Vector2 RATIO = new(16, 9);
        /// <summary>
        /// Scale. Aspect Ratio * Scale = Screen Size
        /// </summary>
        static float scale = 60;
        /// <summary>
        /// Scale getter
        /// </summary>
        public static float SCALE => scale;
        /// <summary>
        /// Frames per second
        /// </summary>
        public const int FPS = 60;
        /// <summary>
        /// If true, we show debug information. If false, we don't. 
        /// When exporting public builds, this should be false.
        /// </summary>
        const bool DEBUG = true;

        /// <summary>
        /// The level we're on. May be delegated to a level builder class later.
        /// </summary>
        static int level = 0;
        /// <summary>
        /// The handler that ticks and renders all of our entities.
        /// </summary>
        static readonly Handler handler = new();

        const int MAX_PLAYERS = 1;

        static GameInput[] gameInputs = new GameInput[MAX_PLAYERS];

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            //init window
            Vector2 dimensions = RATIO * SCALE;
            Raylib.InitWindow((int)dimensions.X, (int)dimensions.Y, "Rising Water");
            Raylib.SetTargetFPS(FPS);

            //init game
            Init();

            //game runs while window not closed and esc not pressed
            //each frame, the game logic ticks everything, and then renders everything.
            while(!Raylib.WindowShouldClose())
            {
                Tick();

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);

                handler.Render();

                if(DEBUG)
                {
                    Raylib.DrawFPS(10, 10);
                }

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        /// <summary>
        /// Initializes everything in our game.
        /// </summary>
        static void Init()
        {
            gameInputs[0] = new(Raylib.IsGamepadAvailable(1) ? 1 : -1);

            //Load level
            handler.AddEntity(new Player(new Vector2(Utils.WIDTH_TILES / 2, Utils.HEIGHT_TILES / 2), gameInputs[0].Controls));
            handler.AddEntity(new Wall(
                new Vector2(0, 0),
                new Vector2(1, Utils.HEIGHT_TILES)));
            handler.AddEntity(new Wall(
                new Vector2(0, Utils.HEIGHT_TILES - 1),
                new Vector2(Utils.WIDTH_TILES, 1)));
            handler.AddEntity(new Wall(
                new Vector2(Utils.WIDTH_TILES - 1, 0),
                new Vector2(1, Utils.HEIGHT_TILES)));
        }

        /// <summary>
        /// Ticks everything in the game each frame.
        /// </summary>
        static void Tick()
        {
            //init gameInputs
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if (Raylib.IsGamepadAvailable(i))
                {
                    gameInputs[i].Gamepad = i;
                }
            }

            foreach (GameInput input in gameInputs)
            {
                input.Tick();
            }

            handler.Tick();
        }
    }

    
}
