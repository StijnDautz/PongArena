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
     * An Object is a sprite that spawns at a specific location
     */
    public class Object
    {
        protected Texture2D texture;
        protected Vector2 location;
        protected Vector2 destination;
        protected Vector2 direction;
        protected Vector2 origin;
        protected Rectangle sourceRectangle;
        protected List<Object> listBounceObjects = new List<Object>();
        protected int elapsedTimeAfterBounce = 0;
        protected double rotation;
        protected float speed;
        protected int height;
        protected int width;
        protected int totalFrames;
        protected int displayedFrame;
        protected int frameWidth;
        protected string name;

        //topLeft, topRight, bottomRight, bottomLeft
        private Vector2[] corners = new Vector2[4];
        private struct border
        {
            public Vector2 vec1, vec2, axis, normal;
            public border(Vector2 v1, Vector2 v2)
            {
                vec1 = v1;
                vec2 = v2;
                axis = v2 - v1;
                normal = new Vector2(axis.Y, -axis.X);
                normal.Normalize();
            }
        };

        /*
         * Object Constructer -- textureheight and -width
         */
        public Object(Vector2 loc, int h, int w)
        {
            location = loc;
            height = h;
            width = w;
            direction = Vector2.Zero;
            sourceRectangle = new Rectangle(1, 0, w, h);
            origin = new Vector2(width / 2, height / 2);
            corners = new Vector2[] {
            new Vector2(loc.X, loc.Y),
            new Vector2(loc.X + width, loc.Y),
            new Vector2(loc.X + width, loc.Y + height),
            new Vector2(loc.X, loc.Y + height),
            };
        }


        /*
         * Object Constructer -- textureheight and -width
         */
        public Object(string n, Vector2 loc, Vector2 destination, int h, int w, float s)
        {
            if (name == null)
            {
                Console.Write("Object.name is not initialized");
            }
            name = n;
            location = loc;
            height = h;
            width = w;
            speed = s;
            direction = destination - loc;
            if(direction != Vector2.Zero)
            {
                direction.Normalize();
            }
            sourceRectangle = new Rectangle(1, 0, w, h);
            origin = new Vector2(width / 2, height / 2);
            corners = new Vector2[] {
            new Vector2(loc.X, loc.Y),
            new Vector2(loc.X + width, loc.Y),
            new Vector2(loc.X + width, loc.Y + height),
            new Vector2(loc.X, loc.Y + height),
            };
        }

        /*
         * Object Constructer -- Use for static framebased Objects
         */
        public Object(string n, Vector2 loc, Vector2 destination, int h, int w, float s, int totalframes, int displayedframe)
        {
            if (name == null)
            {
                Console.Write("Object.name is not initialized");
            }
            name = n;
            location = loc;
            height = h;
            width = w;
            speed = s;
            direction = destination - loc;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }
            totalFrames = totalframes;
            displayedFrame = displayedframe;
            frameWidth = width / totalframes;
            sourceRectangle = new Rectangle(displayedframe * frameWidth, 0, w, h);
            origin = new Vector2(width / 2, height / 2);
            corners = new Vector2[] {
            new Vector2(loc.X, loc.Y),
            new Vector2(loc.X + width, loc.Y),
            new Vector2(loc.X + width, loc.Y + height),
            new Vector2(loc.X, loc.Y + height),
            };
        }

        /*
         * Called every frame
         */
        public virtual void Update(GameTime gameTime)
        {
            //check for collision and check if this should bounce on this Object, if so Bounce
            int bounceInterval = 50;
            elapsedTimeAfterBounce += gameTime.ElapsedGameTime.Milliseconds;

            for (int i = 0; i < listBounceObjects.Count; i++)
            {
                if (CollidesWith(listBounceObjects[i]) && elapsedTimeAfterBounce > bounceInterval)
                {
                    Bounce(listBounceObjects[i]);
                    elapsedTimeAfterBounce = 0;
                }
            }

            //
            
            //Move this Object
            Move();
        }

        /*
         * Checks if this Object collides with the tested Object
         * As collision is less commmon it is not based on detecting collision, but on detecting if there's no collision to get maximum performance
         */
        public bool CollidesWith(Object collider)
        {
            //setting up borders to check, 8 in total, 4 for each object
            border[] borders = { new border(corners[0], corners[1]), new border(corners[1], corners[2]), new border(corners[2], corners[3]), new border(corners[3], corners[0]),
                                 new border(collider.corners[0], collider.corners[1]), new border(collider.corners[1], collider.corners[2]), new border(collider.corners[2], collider.corners[3]), new border(collider.corners[3], collider.corners[0]) };
            //Declaring lists as the List class contains the useful functions max() and min(), which are used later in the function
            List<float> thisScalarProjections = new List<float>();
            List<float> colliderScalarProjections = new List<float>();
            
            //collision is always true, except when there's a gap found, then collision is false and we break from the loop to improve performance
            bool collision = true;

            //loop through borders and store the min and max scalarprojection of this and collider allong border
            for (int b = 0; b < borders.Length; b++)
            {
                //get min and max scalarprojection of this.corners[t] along border[b]
                for (int t = 0; t < corners.Length; t++)
                {
                    thisScalarProjections.Add(Vector2.Dot(corners[t] - borders[b].vec1, borders[b].axis) / borders[b].axis.Length());
                }
                //get min and max scalarprojection of collider.corners[t] along border[b]
                for (int c = 0; c < collider.corners.Length; c++)
                {
                    colliderScalarProjections.Add(Vector2.Dot(collider.corners[c] - borders[b].vec1, borders[b].axis) / borders[b].axis.Length());
                }
                //if there's a gap between this and collider found on border[b], set collision to false, exit the loop and return collision
                if (thisScalarProjections.Max() <= colliderScalarProjections.Min() || thisScalarProjections.Min() >= colliderScalarProjections.Max())
                {
                    collision = false; break;
                }
                //clear lists to prevent from giving false min() and max() values
                thisScalarProjections.Clear();
                colliderScalarProjections.Clear();
            }
            return collision;
        }

        /*
         * Rotate a certain angle in radials
         */
        public void Rotate(double angle)
        {
            for (int i = 0; i < corners.Length; i++)
            {
                //transform origin of Object to Vector2.Zero, so corners rotate around Vector2.Zero
                Vector2 p = corners[i] - (location + origin);

                //rotate object
                float x = p.X;
                float y = p.Y;
                p.X = (float)(x * Math.Cos(angle) - y * Math.Sin(angle));
                p.Y = (float)(x * Math.Sin(angle) + y * Math.Cos(angle));

                //transform origin back to old location
                corners[i] = location + origin + p;
            }
            //set rotation to angle, so the Objects plane corresponds to the texture that's drawn
            rotation += angle;
        }

        /*
         * Move a certain distance
         */
        protected void Move()
        {
            location += direction * speed;
            for (int i = 0; i < corners.Length; i++) { corners[i] += direction * speed; }
        }

        public void Bounce(Object collider)
        {
            //init borders to check
            border[] borders = { new border(collider.corners[0], collider.corners[1]), new border(collider.corners[1], collider.corners[2]), new border(collider.corners[2], collider.corners[3]), new border(collider.corners[3], collider.corners[0])};

            //set originlocation, as origin itself does not represent the origin at the current location of the object, but is used for rotating the object instead
            Vector2 originloc = location + origin;
            Vector2 collideroriginloc = collider.location + collider.origin;

            //calculate distances to borders
            for (int i = 0; i < borders.Length; i++)
            {
                //if the origin is on the other side of the border to bounce on then check1 or -2 is negative and the other positive
                //if this is the case the bounce on this side(border) of the plane
                float check1 = (borders[i].vec2.X - borders[i].vec1.X) * (originloc.Y - borders[i].vec2.Y) - (borders[i].vec2.Y - borders[i].vec1.Y) * (originloc.X - borders[i].vec2.X);
                float check2 = (borders[i].vec2.X - borders[i].vec1.X) * (collideroriginloc.Y - borders[i].vec2.Y) - (borders[i].vec2.Y - borders[i].vec1.Y) * (collideroriginloc.X - borders[i].vec2.X);
              
                if ((check1 > 0 && check2 < 0) || (check1 < 0 && check2 > 0))
                {
                    //calculate new direction after bounce and set it equal to the direction the object should move in
                    direction = (-2 * borders[i].normal * Vector2.Dot(borders[i].normal, direction)) + direction;
                    //make sure length of direction is still 1
                    direction.Normalize();
                    break;
                }
            }    
        }

        /*
         * Get
         */
        public string getName() { return name; }
        public double getRotation() { return rotation; }
        public Texture2D getTexture() { return texture; }
        public Rectangle getSourceRectangle() { return sourceRectangle; }
        public Vector2 getOrigin() { return origin; }
        public Vector2 getLocation() { return location; }

        public void setDirection(Vector2 dir) { this.direction = dir; }

        public void setSpeed(float s) { speed = s; }

        /*
         * Set
         */
        public void setTexture(Texture2D tex) { texture = tex; }
        public void setBounceObjects(List<Object> bounceObjects) { listBounceObjects = bounceObjects; }
    }
} 