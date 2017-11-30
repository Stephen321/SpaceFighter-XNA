using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace JointProject2
{
    public class Particle
    {// http://rbwhitaker.wikidot.com/2d-particle-engine-2
        public Texture2D Texture { get; set; }//texture of the particle that is seen
        public Vector2 Position { get; set; }//current position of the particle        
        public Vector2 Velocity { get; set; }//speed of the particle 
        public float Angle { get; set; }//rotation of the particle
        public float AngularVelocity { get; set; }//how fast the particle is rotating around
        public int TTL { get; set; }  //time to live of the particle ( how long until it is removed)
        public Color Colour { get; set; }// current colour of this particle
        Color targetColour;
        int MaxTTL;

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,int ttl, Color theColour, Color theTargetColour)
        { //create a particle with the values passed in
            Texture = texture;
            Position = position;
            Velocity = velocity;
            TTL = ttl;
            MaxTTL = ttl;
            Colour = theColour;
            targetColour = theTargetColour;
        }

        public void Update()
        {
            TTL--; //reduce the time this particle has left before it is removed
            Position += Velocity; //move the particle
            Angle += AngularVelocity; //rotate the particle
            Colour = Color.Lerp(Colour, targetColour, MaxTTL/500f ); //change the colour
        }

        public void Draw(SpriteBatch spriteBatch)
        { //draw particle
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);//if using spritesheets
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2); //centre of the origin

            spriteBatch.Draw(Texture, Position, sourceRectangle, Colour,
                0, origin, 1, SpriteEffects.None, 0f); //draw itself
        }
    }
}
