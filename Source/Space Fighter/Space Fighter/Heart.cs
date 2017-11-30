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
    class Heart
    {

        Vector2 pos;
        static Texture2D texture;
        bool alive;
        int timer; //timer until heart is no longer alive
        const int MaxTime = 300; 

        public Heart()
        { //default
            alive = false;
            pos = new Vector2(-32, -32);
            timer = MaxTime;
        }

        public Heart(float x, float y)
        {
            alive = true;
            pos = new Vector2(x, y);
            timer = MaxTime;
        }

        public void LoadContent(ContentManager theContentManager, string assetName)
        {
            texture = theContentManager.Load<Texture2D>(assetName); //load own content
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            if (alive)
            {
                theSpriteBatch.Draw(texture, pos, Color.White);
            }
        }
        public void Update()
        {
            timer--;
            if (timer < 0)
                Reset(); //heart moves off the screen
        }           

        public void Reset()
        {
            alive = false;
            pos = new Vector2(-10, -10); //move heart off the screen
        }

        public bool Alive { get { return alive; } }
        public int Health { get { return 20; } } //gives the player 50 health

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)pos.X, (int)pos.Y, texture.Width, texture.Height); }
        }

        public int halfWidth 
        {
            get { return texture.Width / 2; }
        }

        public int halfHeight
        {
            get { return texture.Height / 2; }
        }
    }
}
