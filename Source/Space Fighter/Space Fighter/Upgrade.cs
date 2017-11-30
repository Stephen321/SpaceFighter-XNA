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
    class Upgrade
    {
        Vector2 pos;
        static Texture2D texture ;
        bool alive;
        int speed = 2;
        Vector2 velocity;
        const int OutSideScreenBorder = 100;
        int screenWidth, screenHeight;


        public Upgrade()
        { //default
            alive = false;
            pos = new Vector2(-32, -32);
        }

        public Upgrade(Random rnd, int theScreenWidth, int theScreenHeight)
        {//initalize values
            alive = true;
            screenWidth = theScreenWidth; screenHeight = theScreenHeight;
            GetPosition(rnd);
            velocity = new Vector2((float)(rnd.NextDouble() * 2 - 1), 1f * (float)(rnd.NextDouble() * 2 - 1)); //random velocity
        }

        public void LoadContent(ContentManager theContentManager, string assetName)
        {
            texture = theContentManager.Load<Texture2D>(assetName); //load own content
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            if (alive)
            {
                theSpriteBatch.Draw(texture, pos, Color.White); //draw if alive
            }
        }

        public void Update()
        {//move down the sreen 
            if (pos.X < -OutSideScreenBorder || pos.X > screenWidth + OutSideScreenBorder || pos.Y < -OutSideScreenBorder || pos.Y > screenHeight + OutSideScreenBorder)
            {
                Reset(); //reset if left the screen
            }

            pos += speed * velocity; //move depending on speed and velocity
        }

        private void GetPosition(Random rnd)
        {//get random position outside the screen on one of the 4 sides
            if (rnd.NextDouble() > 0.75)
            {
                float x = (float)rnd.NextDouble() *
                    (screenWidth - texture.Width) + texture.Width / 2;
                float y = -texture.Height;
                pos = new Vector2(x, y);
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

        public void Reset()
        {
            alive = false;
            pos = new Vector2(-100, -100); //move upgrade off the screen
        }

        //properties
        public bool Alive { get { return alive; } }

        public Texture2D Texture
        {
            get { return texture; }
        }

        public Matrix Transform
        {
            get
            {
                return
                Matrix.CreateTranslation(new Vector3(pos, 0.0f));
            }
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)pos.X, (int)pos.Y, texture.Width, texture.Height); }
        }
    }
}
