using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RisingWater
{
    /// <summary>
    /// Entity class. 
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Position in Tiles
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// Velocity in Tiles per second. Each frame, Velocity / FPS gets added to Position
        /// </summary>
        public Vector2 Velocity;
        /// <summary>
        /// The hitbox. Hurr durr lookatme imma hitbox.
        /// </summary>
        public Hitbox Hitbox;

        /// <summary>
        /// Empty entity constructor
        /// </summary>
        protected Entity()
        {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Hitbox = new();
        }
        /// <summary>
        /// Position entity constructor
        /// </summary>
        /// <param name="position"></param>
        protected Entity(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Hitbox = new();
        }
        /// <summary>
        /// Position velocity constructor
        /// </summary>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        protected Entity(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
            Hitbox = new();
        }

        /// <summary>
        /// Ticks each frame for game logic. When overriding this function,
        /// add base.Tick() to the end of your function
        /// </summary>
        public virtual void Tick()
        {
            Position += Velocity / Program.FPS;
            Hitbox.Box.x = Position.X;
            Hitbox.Box.y = Position.Y;
        }

        /// <summary>
        /// Override this function to render your entity
        /// </summary>
        public abstract void Render();

        /// <summary>
        /// Switch the various types your entity can interact with. 
        /// Then, do your code
        /// </summary>
        /// <param name="entity">The entity yours is intersecting</param>
        /// <param name="push">The X/Y amount your entity needs to be pushed out 
        /// by in order to no longer intersect <code>entity</code></param>
        public abstract void Interact(Entity entity, Vector2 push);
    }
}
