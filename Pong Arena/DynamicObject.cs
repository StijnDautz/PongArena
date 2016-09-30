using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pong_Arena
{
    /*
     * A DynamicObject is an Object that is able to move and/or be animated
     */
    public class DynamicObject : Object
    {
        double previousTime;
        int currentFrame;
        int frameTime;
        bool loop;

        /*
         * DynamicObject constructor
         * Use for animated DynamicObjects that move
         * totalframes 0 - x -- frametime in milliseconds -- speed in distance/milleseconds
         */
        public DynamicObject(string name, Vector2 location, Vector2 destination, int height, int width, float speed, int totalframes, int displayedframe, int frametime, float movespeed, bool shouldloop)
        : base(name, location, destination, height, width, speed, totalframes, displayedframe)
        {
            frameTime = frametime;
            loop = shouldloop;
        }

        /*
         * Is called every frame, plays animation + code in Object.Update
         */
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;

            ///Move object based on speed and direction
            if (direction != null) { location = direction * (float)currentTime * speed; }

            ///Check if enough time has passed to go to the next frame of the Animation/DynamicObject
            if (totalFrames != 0)
            {
                if (currentTime - previousTime > frameTime)
                {
                    ///Update frame
                    sourceRectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, height);

                    if (currentFrame >= totalFrames - 1)
                    {
                        if (loop) { currentFrame = 0; }                       
                        else { /*-- TODO -- delete from animlist*/ }
                    }
                    else { currentFrame++; }

                    previousTime = currentTime;
                }
            }
        }
        
        /*
         * Get
         */
        public Rectangle GetSourceRectangle() { return sourceRectangle; }

        /*
         * Set
         */
        public void setDestination(Vector2 destination)
        {
            direction = destination - location;
        }
    }
}