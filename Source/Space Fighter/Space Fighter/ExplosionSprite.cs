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
    class ExplosionSprite
    {
        int curFrame; //current frame of the animation
        Rectangle sourceRect; //rectangle of the sprite on the spritesheet
        Rectangle locationRect; //rectangle of the sprite on the screen
        Texture2D spriteSheet;
        bool alive; //if the explosion is alive and should be drawn
        int timer; //timer for how often to change frame
        const int FrameWidth = 100;//frame dimensions
        const int FrameHeight = 100;
        int framesPerRow;
        int framesPerCol;

        public ExplosionSprite(Texture2D _spriteSheet, Rectangle location)
        {
            //set values
            locationRect = location;
            alive = true;
            spriteSheet = _spriteSheet;
            curFrame = 0;

            framesPerRow = spriteSheet.Width / FrameWidth;
            framesPerCol = spriteSheet.Height / FrameHeight;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.Milliseconds;
            if (timer > 100) //every 50 milliseconds
            {
                curFrame++; //change frame
            }


            int row = curFrame / framesPerCol; //gets the row for the current frame
            int col = curFrame % framesPerRow; //gets the col for the current frame

            //uses these to get the rectangle of the curFrame on the sprite sheet
            sourceRect = new Rectangle(col * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight);
            spriteBatch.Draw(spriteSheet, locationRect, sourceRect, Color.White);  //draw the sprite

            if (row == framesPerRow && col == framesPerCol)
                alive = false;  //finished exploding
        }

        public bool Alive
        {
            get { return alive; }
        }
    }  
}
