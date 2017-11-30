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
    class Asteroid
    {
        Vector2 pos;
        float rotation; //current rotated angle in radians
        sbyte rotationDir; //dir to rotate around
        float rotateSpeed; //how fast to rotate
        Texture2D texture;
        Vector2 origin;
        Color[] textureData;
        Vector2 velocity; //direction
        float speed; //speed
        float scale; 
        bool alive;
        int screenWidth, screenHeight;
        const int OutSideScreenBorder = 200; //the border past the screen which the asteroids will be reset if they hit into
        int hits; //hits this asteroid can take before it is destroyed
        bool destroyed; //to create particles and play sounds if destroyed by something other than leaving the screen

        const int MaxSpeed = 2;

        public Asteroid(Random rnd, Texture2D theTexture, Color[] theTextureData, int theScreenWidth, int theScreenHeight)
        {//give variables starting values
            texture = theTexture;
            textureData = theTextureData;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            alive = true;
            screenWidth = theScreenWidth;
            screenHeight = theScreenHeight;
            speed = MaxSpeed;
            destroyed = false;

            GetPosition(rnd); //ramdom pos
            velocity = new Vector2((float)(rnd.NextDouble() * 2 - 1), 1f * (float)(rnd.NextDouble() * 2 - 1)); //random velocity
            if (rnd.Next(2) == 1) //random rotation direction
                rotationDir = 1;
            else
                rotationDir = -1;

            rotation = (float)rnd.NextDouble() * MathHelper.TwoPi;
            rotateSpeed = (float)rnd.NextDouble() / 50;


            scale = (float)rnd.NextDouble() + 0.3F; //random scale between 0.3 and 1.3
            hits = (int)(4 * scale); //larger asteroid takes more hits
        }

        private void GetPosition(Random rnd)
        { //get a random position around the edges of the screen (out of the view of the player)
            if (rnd.NextDouble() > 0.75)
            {
                float x = (float)rnd.NextDouble() *
                    (screenWidth - texture.Width) + texture.Width / 2;//random x
                float y = -texture.Height; //above the screen for y
                pos = new Vector2(x, y); //random pos above the screen
            }
            else if (rnd.NextDouble() > 0.5)
            {
                float x = (float)rnd.NextDouble() *
                    (screenWidth - texture.Width) + texture.Width / 2;
                float y = screenHeight + texture.Height;
                pos = new Vector2(x, y);
            }
            else if (rnd.NextDouble() > 0.25)
            {
                float x = -texture.Width;
                float y = (float)rnd.NextDouble() *
                    (screenHeight - texture.Height) + texture.Height / 2;
                pos = new Vector2(x, y);
            }
            else 
            {
                float x = screenWidth + texture.Width;
                float y = (float)rnd.NextDouble() *
                    (screenHeight - texture.Height) + texture.Height / 2;
                pos = new Vector2(x, y);
            }
        }

        public void Update()
        { //update the asteroid
            if (pos.X < -OutSideScreenBorder || pos.X > screenWidth + OutSideScreenBorder || pos.Y < -OutSideScreenBorder || pos.Y > screenHeight + OutSideScreenBorder) 
                alive = false; //false if gone outside the screen
            if (hits == 0)
            {
                alive = false;
                destroyed = true;
            }

            pos += speed * velocity; //change asteroids position based on its velocity and speed
            rotation += rotateSpeed * rotationDir; //rotate around 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, pos, null, Color.White,
                rotation, origin, scale, SpriteEffects.None, 0.0f);
        }

        //properties
        public int Damage
        {
            get { return -5; } //5 damage to the player
        }
        public Texture2D Texture
        {
            get { return texture; }
        }

        public Color[] TextureData
        {
            get { return textureData; }
        }

        public Vector2 Position
        {
            get { return pos; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle(0, 0, texture.Width, texture.Height); }
        }

        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }

        public bool Destroyed
        {
            get { return destroyed; }
            set { destroyed = value; }
        }

        public int Hits
        {
            get { return hits; }
            set { hits = value; }
        }

        public Matrix Transform
        {
            get
            {
                return
                       Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                       Matrix.CreateRotationZ(rotation) *
                       Matrix.CreateScale(scale) *
                       Matrix.CreateTranslation(new Vector3(pos, 0.0f));
            }
        }
    }
}
