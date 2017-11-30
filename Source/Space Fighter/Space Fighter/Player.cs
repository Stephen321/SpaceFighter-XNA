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
using System.Xml;

namespace JointProject2
{
    class Player
    { 
        Texture2D texture; //texture to display
        Vector2 pos; //vector to hold position of image
        Vector2 velocity; //vector to hold the velocity
        Random rnd = new Random();
        Vector2 origin;
        MouseState previousMouseState = Mouse.GetState();
        ParticleEngine particleEngine;
        const int MaxBullets = 20;
        Bullet[] bullets = new Bullet[MaxBullets];
        Color[] textureData;

        float rotation;

        int screenWidth, screenHeight;
        float speed;
        const float Acceleration = 0.08f; //float to hold the acceleration 
        const int MaxSpeed = 3;
        SoundEffect laser;
        bool bulletUpgraded;

        public Player(int _screenWidth, int _screenHeight)
        { //set all values
            screenWidth = _screenWidth; //set the screen height and width
            screenHeight = _screenHeight;
            pos = new Vector2(screenWidth / 2, screenHeight/2); //starting position
            speed = 0;
            bulletUpgraded = false;
            for (int i = 0; i < MaxBullets; i++)
            {
                bullets[i] = new Bullet(screenWidth, screenHeight); //make all new bullets
            }
               
        }

        public void LoadContent(ContentManager theContentManager, string theAssetName, List<Texture2D> textures, SoundEffect theLaserSound)
        {//load in textures and sounds
            laser = theLaserSound;
            texture = theContentManager.Load<Texture2D>(theAssetName);
            origin.X = texture.Width / 2; //get origin after getting texture
            origin.Y = texture.Height / 2;

            textureData = new Color[texture.Width * texture.Height]; //get data for the player texture
            texture.GetData(textureData);

            particleEngine = new ParticleEngine(textures, new Vector2(400, 240), new Color(253, 162, 5), new Color(84, 202, 248)); //create new particle texture 
        }
        public void Update(bool playSounds)
        //Move towards the mouse when the left button is clicked
        {
            //stop the player moving off the screen
            pos.X = MathHelper.Clamp(pos.X,
            0 + origin.X, screenWidth - texture.Width + origin.X);
            pos.Y = MathHelper.Clamp(pos.Y,
            0 + origin.Y, screenHeight - texture.Height + origin.Y);

            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y); //position of the mouse on the screen
            double targetX = mousePos.X - pos.X;
            double targetY = mousePos.Y - pos.Y;
            rotation = (float)Math.Atan2(targetY, targetX) + (float)(Math.PI * 0.5);//get rotation to face the mouse

            pos += speed * velocity;// update the position of the player

            

            //rotationMatrix = Matrix.CreateRotationZ(rotation);

            if (previousMouseState.RightButton == ButtonState.Pressed && Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                if (DetectCircleCol(mousePos)) //keep updating the velocity if the player isnt too close to the mouse
                {
                    velocity = mousePos - pos; //get vector to the mouse
                    velocity.Normalize(); //normalise this 
                }
                if (speed < MaxSpeed)
                {
                    speed += Acceleration; //increase the speed of the player
                }

            }
            if (previousMouseState.RightButton == ButtonState.Released && Mouse.GetState().RightButton == ButtonState.Released)
            {
                if (speed > 0)
                {
                    speed -= Acceleration; //decrease the speed of the player when they release the mouse button
                }
                else
                    speed = 0; //set to 0 when speed gets less than 0
            }

            if ((previousMouseState.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released))
            {
                velocity = mousePos - pos; //get vector to the mouse
                velocity.Normalize(); //normalise this 
                byte bulletsFired = 0; //how many bullets fired this time
                for (int i = 0; i < MaxBullets; i++)
                {
                    if (bulletUpgraded)
                    {
                        if (bullets[i].Alive == false && bulletsFired == 0) //if no bullets fired yet
                        {
                            bullets[i].Fire(new Vector2(pos.X - 28, pos.Y - 15), pos, rotation, velocity, playSounds, laser);
                            bulletsFired++;
                        }
                        if (bullets[i].Alive == false)
                        {
                            bullets[i].Fire(new Vector2(pos.X + 28, pos.Y - 15), pos, rotation, velocity, playSounds, laser);
                            bulletsFired++;
                        }
                        if (bulletsFired == 2) //break when 2 fired
                            break;
                    }
                    else if (bullets[i].Alive == false) //only fire one when bullets arent upgraded
                    {
                        bullets[i].Fire(new Vector2(pos.X, pos.Y - 35), pos, rotation, velocity, playSounds, laser);
                        break;
                    }
                }
            }

            UpdateParticles();
            for (int i = 0; i < MaxBullets; i++)
            {
                bullets[i].Update();
            }
            previousMouseState = Mouse.GetState();
        }

        private bool DetectCircleCol(Vector2 mousePos)
        //Checks if there is no collision between player and the mouse circle
        { //Circle Collision checking
            int mouseCircleRadius = 75; //distance around the mouse the players velocity wont change

            //Check distance between two points
            double distance = (mousePos - pos).Length();

            //Check if distance between the circles is greater than the two radius
            if (distance > mouseCircleRadius)
                return true;  //return true if there is no collision
            else
                return false;
        }

        public void Reset()
        {
            pos = new Vector2(screenWidth / 2, screenHeight / 2); //starting position
            speed = 0;
            particleEngine.Reset();
            bulletUpgraded = false;
            foreach (Bullet b in bullets)
                b.Reset();
        }

        private void UpdateParticles()
        { //update particles to do with the player
            particleEngine.EmitterLocation = new Vector2(pos.X, pos.Y + origin.Y); //reset position
            particleEngine.EmitterLocation = RotateAroundOrigin(particleEngine.EmitterLocation, pos, rotation); //rotate it around the centre of the player
            particleEngine.Update((int)(speed), 30);
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D bulletTex)
        {
            for (int i = 0; i < MaxBullets; i++)
            {
                bullets[i].Draw(spriteBatch, bulletTex, Color.GreenYellow);
            }

            particleEngine.Draw(spriteBatch);
            spriteBatch.Draw(texture, pos, null, Color.White,
                        rotation, origin, 1.0F, SpriteEffects.None, 0.0f);
        }

        private Vector2 RotateAroundOrigin(Vector2 point, Vector2 origin, float rotation)
        { //rotate a given point about a certain origin by a certain amount
            return Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
        } 

        //properties
        public Vector2 Position
        {
            get { return pos; }
        }

        public Vector2 ExpPos
        {
            get { return pos - origin; } //where the top left corner of the explosion will be
        }

        public int Damage
        {
            get { return -10; } //damage the player deals
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

        public Color[] TextureData
        {
            get { return textureData; }
        }

        public Texture2D Texture
        {
            get { return texture; }
        }

        public Bullet[] Bullets
        {
            get { return bullets; }
        }

        public bool BulletUpgraded
        {
            get { return bulletUpgraded; }
            set { bulletUpgraded = value; }
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)(pos.X - origin.X), (int)(pos.Y - origin.Y), texture.Width, texture.Height); }
        }
    }
}
