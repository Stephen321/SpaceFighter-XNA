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
    class Bullet
    {
        Vector2 pos; //vector to hold position of image
        Vector2 velocity; //vector to hold the velocity 
        float rotation; //angle of rotation in radians
        bool alive;
        int speed;
        int screenWidth, screenHeight;
        Vector2 origin;
        const int BulletWidth = 9;
        const int BulletHeight = 20;

        public Bullet(int theScreenWidth, int theScreenHeight)
        { //set start values

            origin = new Vector2(BulletWidth / 2, BulletHeight / 2);
            alive = false;
            speed = 10;
            pos = new Vector2(-1000, -1000);
            screenWidth = theScreenWidth;
            screenHeight = theScreenHeight;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D bulletTex, Color colour)
        { //draw it self
            if (alive)
            {
                spriteBatch.Draw(bulletTex, pos, null, colour,
                        rotation, origin, 1.0F, SpriteEffects.None, 0.0f);
            }
        }

        public void Fire(Vector2 bulletPos, Vector2 shipOrigin, float shipRotation, Vector2 shipVelocity, bool playSounds, SoundEffect laser)
        { //get a starting position above the ship
            pos = bulletPos;
            pos = RotateAroundOrigin(pos, shipOrigin, shipRotation); //rotate the start position of the bullet about the origin of the ship (the centre)
            velocity = shipVelocity; //give it the same velocity of the ship 
            rotation = shipRotation; //and the same rotation
            alive = true; //it is now alive
            if (playSounds)
                laser.Play(); //play the laser sound
        }

        private Vector2 RotateAroundOrigin(Vector2 point, Vector2 origin, float rotation)
        { //rotate a given point about a certain origin by a certain amount
            return Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
        } 

        public void Update()
        {//update the bullet 
            if (alive)
            {
                pos += speed * velocity; //move 

                if (pos.X < 0 || pos.X > screenWidth || pos.Y < 0 || pos.Y > screenHeight) //left the screen
                {
                    Reset();
                }
            }
            else
                Reset();
        }

        public void Reset()
        { //reset pos of bullet after it left the screen or hit something
            alive = false;
            pos = new Vector2(-1000, -1000); //move bullet off the screen so it doesnt collide with any other objects
        }

        public bool Alive
        {
            set { alive = value; }
            get { return alive; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
        }

        public Matrix Transform
        {
            get
            {
                return
                Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(pos, 0.0f));
            }
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)(pos.X - origin.X), (int)(pos.Y - origin.Y), BulletWidth, BulletHeight); }
        }
    }
}
