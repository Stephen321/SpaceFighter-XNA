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
    class AdvancedFighter : Fighter
    {
         bool shielded; //cant take damage until shield hits runs out
         Texture2D shieldTexture;
         int shieldHits;

        public AdvancedFighter(Random randomGen, Texture2D healthTexture, Vector2 screenSize,
            Texture2D theTexture, List<Texture2D> particleTextures, SoundEffect theLaserSound, Color[] theTextureData, Texture2D shieldTex)
            : base(randomGen, healthTexture, screenSize, theTexture, particleTextures, theLaserSound, theTextureData)
        {
            speed = 0.3f; //slower
            bulletColor = Color.LightSteelBlue; //different colored bullets
            particleEngine = new ParticleEngine(particleTextures, Vector2.Zero, Color.LightBlue, Color.SteelBlue); //different colours
            fireRate = 0.025f; //faster fire rate
            damage = 2; //less damage (fires faster)
            maxHealth = 100; //more health
            currentHealth = maxHealth;
            shielded = true;
            shieldHits = 8;
            shieldTexture = shieldTex;
        }

        public override void DescreaseHealth(int damage)
        {
            if (shielded == false)
                base.DescreaseHealth(damage); //take damage only if the shield has ran out of hits
            else
                shieldHits--; //if shielded then decrease the number of hits remaining on the shield
        }

        public override void Update(bool playSounds, Vector2 _playerPos)
        {
            base.Update(playSounds, _playerPos);
            if (shieldHits == 0)
                shielded = false; //remove shield when down to 0 hits remaining on the shield
        }

        protected override void UpdateParticles()
        {
            particleEngine.EmitterLocation = new Vector2(pos.X, pos.Y + origin.Y); //reset position
            particleEngine.EmitterLocation = RotateAroundOrigin(particleEngine.EmitterLocation, pos, rotation); //rotate it around the centre of the player
            particleEngine.Update(4, 25);
        }
        public int ShieldHits
        {
            get { return shieldHits; }
        }

        public bool Shielded
        {
            get { return shielded; }
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D bulletTex)
        {
            base.Draw(spriteBatch, bulletTex);
            if (shielded)
                spriteBatch.Draw(shieldTexture, pos - origin - new Vector2(15, 0), Color.White); //draw the shield texture
        }
    }
}
