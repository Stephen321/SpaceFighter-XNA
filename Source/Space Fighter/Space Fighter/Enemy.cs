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
    class Enemy
    {
        Texture2D texture; //texture to display
        Random rnd = new Random();
        const int MaxBullets = 10;
        Bullet[] bullets = new Bullet[MaxBullets];
        Color[] textureData;
        
        bool alive;
        int screenWidth, screenHeight;
        const float Acceleration = 0.8f; //float to hold the acceleration 
        SoundEffect laser;
        int direction; //direction to move in
        const int Up = 1, Down = 2, Right = 3, Left = 4; //constants representing directions
        const int OutSideScreenBorder = 200; //border around the screen where the ship will be destroyed
        bool destroyed; //create explosion and particles when this dies if destroyed by something other than leaving the screen if true
        

        //health:
        const int HealthBarMaxLength = 70;
        Texture2D healthBackgroundTex;
        Texture2D currentHealthTex;
        Rectangle healthRect;
        Rectangle backgroundHealthRect;
        Color healthBarColor;
        Color backHealthBarColor;
        int healthBarWidth;

        protected Color bulletColor;
        protected float speed;
        protected float fireRate;
        protected ParticleEngine particleEngine;
        protected Vector2 pos; //vector to hold position of image
        protected Vector2 velocity; //vector to hold the velocity
        protected float rotation;
        protected int damage = 5;
        protected int maxHealth;
        protected int currentHealth;
        protected Vector2 origin;

        public Enemy(Random randomGen, Texture2D healthTexture, Vector2 screenSize, Texture2D theTexture, List<Texture2D> particleTextures, SoundEffect theLaserSound, Color[] theTextureData)
        {//initalize variables
            maxHealth = 25;
            fireRate = 0.01f;
            screenWidth = (int)screenSize.X; //set the screen height and width
            screenHeight = (int)screenSize.Y;
            texture = theTexture; 
            GetPosition(randomGen); //get a random position around the edge of the screen
            GetVelocity();//get a velocity depending on what side of the screen this position was
            GetRotation(); //get a rotation depending on direction
            speed = 4;
            laser = theLaserSound;
            textureData = theTextureData;
            origin.X = texture.Width / 2;
            origin.Y = texture.Height / 2;
            alive = true;
            bulletColor = Color.White;
            textureData = theTextureData;
            particleEngine = new ParticleEngine(particleTextures, Vector2.Zero, Color.Blue, Color.Yellow);

            for (int i = 0; i < MaxBullets; i++)
            {
                bullets[i] = new Bullet(screenWidth, screenHeight);
            }

            //health:
            currentHealth = maxHealth;
            backHealthBarColor = Color.Black;
            healthBarColor = Color.Green;
            currentHealthTex = healthTexture;
            healthBackgroundTex = currentHealthTex;
            healthBarWidth = HealthBarMaxLength;
        }

        public void ResetHealth()
        //Reset back to defaults
        {
            currentHealth = maxHealth;
            GetHealthBarLength();
        }

        private void GetPosition(Random rnd)
        {//get a random position around the edges of the screen
            if (rnd.NextDouble() > 0.75)
            {
                float x = (float)rnd.NextDouble() *
                    (screenWidth - texture.Width) + texture.Width / 2;
                float y = -texture.Height;
                pos = new Vector2(x, y);
                direction = Down; //change the direction depending on what pos it is given
            }
            else if (rnd.NextDouble() > 0.5)
            {
                float x = (float)rnd.NextDouble() *
                    (screenWidth - texture.Width) + texture.Width / 2;
                float y = screenHeight + texture.Height;
                pos = new Vector2(x, y);
                direction = Up;
            }
            else if (rnd.NextDouble() > 0.25)
            {
                float x = -texture.Width;
                float y = (float)rnd.NextDouble() *
                    (screenHeight - texture.Height) + texture.Height / 2;
                pos = new Vector2(x, y);
                direction = Right;
            }
            else
            {
                float x = screenWidth + texture.Width;
                float y = (float)rnd.NextDouble() *
                    (screenHeight - texture.Height) + texture.Height / 2;
                pos = new Vector2(x, y);
                direction = Left;
            }
        }

        protected virtual void GetVelocity()
        { //get velocity depending on what direction it has
            if (direction == Down)
                velocity = new Vector2(0, 1);
            else if (direction == Up)
                velocity = new Vector2(0, -1);
            else if (direction == Right)
                velocity = new Vector2(1, 0);
            else if (direction == Left)
                velocity = new Vector2(-1, 0);
        }

        protected virtual void GetRotation()
        { //get rotation depending on its direction
            if (direction == Down)
                rotation = 180 * (float)(Math.PI / 180);
            else if (direction == Up)
                rotation = 0 * (float)(Math.PI / 180);
            else if (direction == Right)
                rotation = 90 * (float)(Math.PI / 180);
            else if (direction == Left)
                rotation = 270 * (float)(Math.PI / 180);
        }

        protected void Move()
        { 
            pos += speed * velocity;// update the position of the enemy
        }

        public void Update(bool playSounds)
        //Move down the screen
        {
            Move();
            if (rnd.NextDouble() < fireRate) //fire bullets if less than the fire rate
            {
                foreach (Bullet b in bullets)
                {
                    if (b.Alive == false && (pos.X > 0 && pos.X + texture.Width < screenWidth && pos.Y > 0 && pos.Y + texture.Height < screenHeight))
                    {    //if on the screen and there's a bullet which isnt alive at the moment then fire it
                        b.Fire(new Vector2(pos.X, pos.Y - 35), pos, rotation, velocity, playSounds, laser);
                        break;
                    }
                }
            }
            if (pos.X < -OutSideScreenBorder || pos.X > screenWidth + OutSideScreenBorder || pos.Y < -OutSideScreenBorder || pos.Y > screenHeight + OutSideScreenBorder)
            {
                //destroyed false if it moved pass the border around the screen (player didnt see it get destroyed)
                destroyed = false;
                alive = false;
            }

            UpdateParticles();
            foreach (Bullet b in bullets)
                b.Update();
        }

        public void Reset()
        { //reset alie to false and get new values again for other variables
            GetPosition(rnd);
            GetVelocity();
            GetRotation();
            speed = 4;
            alive = false;
            particleEngine.Reset();
            foreach (Bullet b in bullets)
                b.Reset();
            ResetHealth();
        }

        protected void GetHealthBarLength()
        //Get the new length of the health bar
        {
            healthBarWidth = (int)((HealthBarMaxLength) * ((float)currentHealth / maxHealth)); //Get a percentage of the enemies remaining health and multiply this by the
            //length of the healthbar to make the texture representing health smaller
            //e.g if currentHealth/maxHealth is 20/100. Then 250 * 0.2 is 50. The green rectangle is now 50 
            healthBarColor = Color.Lerp(Color.Red, Color.Green, (float)currentHealth * 2 / maxHealth);
        }

        public virtual void DescreaseHealth(int damage)
        //To decrease the enemies health by the amount passed to it.
        {
            if (currentHealth + damage > 0) //If the damage dealt will not kill the enemy by putting their health 0 or lower 
            {
                currentHealth += damage;
            }
            else
            {//enemy is now dead
                currentHealth = 0;
                alive = false;
                destroyed = true;
            }
            GetHealthBarLength();
        }

        protected virtual void UpdateParticles()
        { //update particles to do with the player
            particleEngine.EmitterLocation = new Vector2(pos.X, pos.Y + origin.Y); //reset position
            particleEngine.EmitterLocation = RotateAroundOrigin(particleEngine.EmitterLocation, pos, rotation); //rotate it around the centre of the player
            particleEngine.Update(6, 15);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D bulletTex)
        {
            foreach (Bullet b in bullets)
                b.Draw(spriteBatch, bulletTex, bulletColor);

            particleEngine.Draw(spriteBatch);
            if (alive)
            {
                spriteBatch.Draw(texture, pos, null, Color.White,
                            rotation, origin, 1.0F, SpriteEffects.None, 0.0f);
                //health:

                healthRect = new Rectangle((int)pos.X - 40, (int)pos.Y - 50, healthBarWidth, 15);
                backgroundHealthRect = new Rectangle((int)pos.X - 40, (int)pos.Y - 50, HealthBarMaxLength, 15);
                spriteBatch.Draw(healthBackgroundTex, backgroundHealthRect, backHealthBarColor * 0.7f);
                spriteBatch.Draw(currentHealthTex, healthRect, healthBarColor * 0.7f);
            }
        }

        protected Vector2 RotateAroundOrigin(Vector2 point, Vector2 origin, float rotation)
        { //rotate a given point about a certain origin by a certain amount
            return Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
        } 

        //properties
        public Vector2 TopLeft //where the explosion should be top left corner
        {
            get { return pos - origin; }
        }

        public Vector2 Pos
        {
            get { return pos; }
        }

        public bool Destroyed
        {
            get { return destroyed; }
            set { destroyed = value; }
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

        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }

        public int Damage
        {
            get { return -damage; } //10 damage to the player
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)(pos.X - origin.X), (int)(pos.Y - origin.Y), texture.Width, texture.Height); }
        }

        
    }
}
