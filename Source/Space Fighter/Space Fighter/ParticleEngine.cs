using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JointProject2
{ //
    public class ParticleEngine
    { //http://rbwhitaker.wikidot.com/2d-particle-engine-3 
        Random random;
        public Vector2 EmitterLocation { get; set; } //the position to emit the particles from
        public List<Particle> particles; //list of particles
        public List<Texture2D> textures; //list of textures to use for these particles
        Color colour, targetColour;

        public ParticleEngine(List<Texture2D> textures, Vector2 location, Color theColour, Color theTargetColour)
        { //initalize variables
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
            colour = theColour;
            targetColour = theTargetColour;
        }

        public void Reset()
        {
            particles.Clear(); //remove all the current particles in the list
        }

        public void Update(int amount, int timeToLive)
        {
            for (int i = 0; i < amount; i++) //add a new particle up until i is equal to amount
            {
                particles.Add(GenerateNewParticle(colour, targetColour, timeToLive)); 
            }

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update(); //update each particle
                if (particles[particle].TTL <= 0) //if it has run out of time to live
                {
                    particles.RemoveAt(particle); //remove it from the list
                    particle--; //take away one from the index so it doesnt go out of bounds
                }
            }
        }

        public void Update()
        { //overloaded method which doesnt create more particles
           
            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update(); //update each particle
                if (particles[particle].TTL <= 0) //if it has run out of time to live
                {
                    particles.RemoveAt(particle); //remove it from the list
                    particle--; //take away one from the index so it doesnt go out of bounds
                }
            }
        }

        private Particle GenerateNewParticle(Color colour, Color targetColour, int timeToLive)
        { //make a new particle with random values
            Texture2D texture = textures[random.Next(textures.Count)]; //get a random texture
            Vector2 position = EmitterLocation; //start position of the particle is at the emitter location
            Vector2 velocity = new Vector2((float)(random.NextDouble() * 2 - 1),  1f * (float)(random.NextDouble() * 2 - 1)); //random velocity
            int ttl = 10 + random.Next(timeToLive); //random time to live

            return new Particle(texture, position, velocity, ttl, colour, targetColour); //return a new particle to  be added to the list above
        }

        public void Draw(SpriteBatch spriteBatch)
        { //draw all the particles
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);// get each particle to draw itself
            }
        }
    }
}