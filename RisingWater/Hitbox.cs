using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RisingWater
{
    public struct Hitbox
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="push">The max x/y vector by which a hitbox must be pushed
        /// to no longer intersect the pusher hitbox</param>
        public delegate void CallbackFunction(Vector2 push);
        public Rectangle Box;

        public Hitbox(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            Box = new Rectangle(x, y, width, height);
        }

        public Hitbox(Vector2 position, Vector2 dimensions)
        {
            Box = new Rectangle(position.X, position.Y, dimensions.X, dimensions.Y);
        }

        public Hitbox(Rectangle box)
        {
            Box = box;
        }

        /// <summary>
        /// if the hitboxes intersect, it calls the callback function, then returns true
        /// </summary>
        /// <param name="hitbox">the pusher hitbox</param>
        /// <param name="func">the callback function</param>
        /// <returns>whether or not the hitboxes intersect</returns>
        public bool Intersects(Hitbox hitbox, CallbackFunction func = null)
        {
            
            if (Raylib.CheckCollisionRecs(Box, hitbox.Box))
            {
                
                if (func != null)
                {
                    
                    Rectangle pushee = Box;
                    Rectangle pusher = hitbox.Box;

                    float x;
                    float y;
                    //if pushee to left
                    if(pushee.x + pushee.width / 2 < pusher.x + pusher.width / 2)
                    {
                        //push pushee left
                        x = -(pushee.x + pushee.width - pusher.x);
                    }
                    else
                    {
                        //push pushee right
                        x = pusher.x + pusher.width - pushee.x;
                    }
                    //if pushee below
                    if(pushee.y + pushee.height / 2 < pusher.y + pusher.height / 2)
                    {
                        //push pushee down
                        y = -(pushee.y + pushee.height - pusher.y);
                    }
                    else
                    {
                        //push pushee up
                        y = pusher.y + pusher.height - pushee.y;
                    }
                    //find amounts by which to push out
                    Vector2 push = new Vector2(x, y);
                    func.Invoke(push);
                }

                return true;
            }

            return false;
        }
    }
}
