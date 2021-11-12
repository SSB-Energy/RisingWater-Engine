using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RisingWater
{
    public class Player : Entity
    {
        /// <summary>
        /// The dimensions of the player's hitbox.
        /// </summary>
        public Vector2 Dimensions = new(1, 1);
        /// <summary>
        /// The controls object.
        /// </summary>
        private Controls controls = new();
        /// <summary>
        /// A boolean for grounded. 
        /// Gets set to true upon landing on a platform. 
        /// Gets set to false when ySpeed falls outside of the range (-2G, 2G). 
        /// Gets set to false upon jumping. 
        /// While true, the player can jump.
        /// </summary>
        public bool Grounded = false;
        /// <summary>
        /// A boolean for in water. 
        /// Gets set to false at the end of each tick. 
        /// Gets set to true upon collision with Water. 
        /// While true, the player's Breath is decremented by BREATH_SPEED. 
        /// While true, the player can jump. 
        /// While true, the player moves more slowly.
        /// </summary>
        public bool InWater = false;
        /// <summary>
        /// Gets decremented each frame the user spends in water.
        /// </summary>
        public float Breath = 1;
        /// <summary>
        /// If true, the player can jump.
        /// </summary>
        public bool CanJump => InWater || Grounded;

        /// <summary>
        /// The player's horizontal move speed in tiles per second
        /// </summary>
        public const float X_SPEED = 6;
        /// <summary>
        /// The player's initial jump speed in tiles per second.
        /// </summary>
        public const float JUMP_SPEED = -16;
        /// <summary>
        /// The player's compound jump deceleration per frame.
        /// </summary>
        public const float JUMP_RELEASE_DECEL = 2 / 3;
        /// <summary>
        /// The player's gravity in tiles per seconed per frame.
        /// </summary>
        public const float GRAVITY = 0.5f;
        /// <summary>
        /// The player's breathing speed in lung capacity per frame.
        /// </summary>
        public const float BREATH_SPEED = 0.2f / Program.FPS;
        /// <summary>
        /// The player's horizontal speed underwater.
        /// </summary>
        public const float WATER_SPEED = 4;
        public float HSpeed => InWater ? WATER_SPEED : X_SPEED;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="position"></param>
        public Player(Vector2 position, Controls controls) : base(position)
        {
            Hitbox = new Hitbox(Position, Dimensions);
            this.controls = controls;
        }

        public override void Tick()
        {
            //Check for jump
            if (CanJump && controls.Pressed(Controls.UP))
            {
                Velocity.Y = JUMP_SPEED;
                Grounded = false;
            }
            //Check for short jump
            else if(Velocity.Y < 0 && !controls.Held(Controls.UP))
            {
                Velocity.Y *= JUMP_RELEASE_DECEL;
            }

            //Do horizontal movement
            bool right = controls.Held(Controls.RIGHT);
            bool left = controls.Held(Controls.LEFT);
            if(right && !left)
            {
                Velocity.X = HSpeed;
            }
            else if(!right && left)
            {
                Velocity.X = -HSpeed;
            }
            else if(!right && !left)
            {
                Velocity.X = 0;
            }

            //Add gravity
            Velocity.Y += GRAVITY;

            //Check left ground
            if(Math.Abs(Velocity.Y) > 2 * GRAVITY)
            {
                Grounded = false;
            }

            //Handle water state
            if(InWater)
            {
                InWater = false;
            }
            else
            {
                Breath = 1;
            }

            //super tick
            base.Tick();
        }

        /// <summary>
        /// Can interact with Walls.
        /// </summary>
        /// <param name="entity">The other entity the player's touching. 
        /// Can be a Wall.</param>
        /// <param name="push">The amount by which to push the player out</param>
        public override void Interact(Entity entity, Vector2 push)
        {
            //check type of entity
            switch(entity)
            {
                //do wall collision
                case Wall:
                    //simple push out code with up push allowance of 0.2 tiles
                    Vector2 abs = Vector2.Abs(push);
                    if (abs.Y < 0.2 && push.Y < 0 || abs.Y < abs.X)
                    {
                        Position.Y += push.Y;
                        Velocity.Y = 0;
                        if (push.Y < 0)
                        {
                            Grounded = true;
                        }
                    }
                    else
                    {
                        Position.X += push.X;
                        Velocity.X = 0;
                    }
                    break;
            }
        }

        public override void Render()
        {
            Vector2 renderPos = Utils.Renderable(Position);
            Vector2 renderDims = Utils.Renderable(Dimensions);
            Raylib.DrawRectangle(
                (int)renderPos.X, (int)renderPos.Y, 
                (int)renderDims.X, (int)renderDims.Y, 
                Color.GREEN);
        }
    }
}
