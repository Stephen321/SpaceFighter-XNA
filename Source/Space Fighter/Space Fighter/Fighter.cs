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
    class Fighter : Enemy
    {
        protected Vector2 playerPos;

        public Fighter(Random randomGen, Texture2D healthTexture, Vector2 screenSize,
            Texture2D theTexture, List<Texture2D> particleTextures, SoundEffect theLaserSound, Color[] theTextureData)
            : base(randomGen, healthTexture, screenSize, theTexture, particleTextures, theLaserSound, theTextureData)
        {
            speed = 0.9f; //slower
            bulletColor = Color.Magenta; //different colored bullets
            particleEngine = new ParticleEngine(particleTextures, Vector2.Zero, Color.Purple, Color.Blue); //different colours
            fireRate = 0.007f; //slow fire rate
            damage = 10; //twice the damage
            maxHealth = 50; //more health
            currentHealth = maxHealth;
        }

        public virtual void Update(bool playSounds, Vector2 _playerPos)
        {
            base.Update(playSounds);
            playerPos = _playerPos; //get the new player pos
            GetRotation(); //get the new rotation depending on player pos
            GetVelocity(); //get the new velocity depending on player pos
        }

        protected override void UpdateParticles()
        {
            particleEngine.EmitterLocation = new Vector2(pos.X, pos.Y + origin.Y); //reset position
            particleEngine.EmitterLocation = RotateAroundOrigin(particleEngine.EmitterLocation, pos, rotation); //rotate it around the centre of the player
            particleEngine.Update(4, 25);
        }
        protected override void GetRotation()
        {//now faces towards the player
            double targetX = playerPos.X - pos.X;
            double targetY = playerPos.Y - pos.Y;
            rotation = (float)Math.Atan2(targetY, targetX) + (float)(Math.PI * 0.5);//get rotation to face the mouse
        }

        protected override void GetVelocity()
        { //now moves towards the player
            velocity = playerPos - pos; //get vector to the mouse
            velocity.Normalize(); //normalise this 
        }
    }
}
