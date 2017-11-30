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
using System.IO;

//Name: Stephen Ennis
//ID: C00181305
//Date Created: 26/ 3/ 2014
//Description: A game where the play uses the mouse to move and fire at different enemies. They have to try
//stay alive and get a highscore. 
//Know Issues/Bugs:
//Corrupt data error rarely on closing the game. 
//

namespace JointProject2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //fonts
        SpriteFont font;
        SpriteFont bigFont;

        //player object and scorekeeper to keep track of scores and health for the player
        Player player; 
        Scorekeeper scoreKeeper;
        //upgrade object the player can get to increase the amount of bullets they can fire
        Upgrade upgrade; 
        //heart object that drops randomly from enemies and the player gets health from it
        Heart heartDrop;

        //backgrounds
        const int MaxBackgrounds = 3;
        Texture2D[] backgrounds = new Texture2D[MaxBackgrounds]; //texture for each background
        Vector2[] backgroundPositions = new Vector2[MaxBackgrounds]; //position of the backgrounds
        Rectangle backgroundRect; //rectangle of the playable area to fit the background image into
        Texture2D splashBackground; //image to be displayed during the splash screen
        Texture2D gameOverOptionsBackground; //image for gameover and options screens

        //asteroids
        List<Asteroid> asteroids = new List<Asteroid>(); //list of asteroids
        int asteroidDestroyedTimer;
        const int AsteroidDestroyedMaxTime = 10;

        //asteroid particles
        ParticleEngine asteroidParticles;
        Texture2D asteroidTexture;
        Color[] asteroidTextureData;

        //spawn chance of objects
        float asteroidSpawnProb = 0.007f;
        float upgradeSpawnProb = 0.004f;
        float enemySpawnProb = 0.005f;
        float fighterSpawnProb = 0.0025f;
        float advancedFighterSpawnProb = 0.0005f;
        float heartSpawnProb = 0.15f;
        
        List<Texture2D> particleTextures = new List<Texture2D>();
        Texture2D explosionSpriteSheet;
        List<ExplosionSprite> explosions = new List<ExplosionSprite>();

        //enemies,fighters,advancedfighters
        List<Enemy> enemies = new List<Enemy>();
        List<Fighter> fighters = new List<Fighter>();
        List<AdvancedFighter> advancedFighters = new List<AdvancedFighter>();
        Color[] enemyTextureData;
        Color[] enemyTextureData2;
        Color[] enemyTextureData3;
        Texture2D enemyTexture;
        Texture2D enemyTexture2;
        Texture2D enemyTexture3;
        Texture2D enemyHealthTexture;
        Texture2D shield;

        //button textures
        Texture2D startButton; //start the game
        Texture2D continueButton;
        Texture2D exitButton;
        Texture2D mainMenuButton;
        Texture2D optionsButton; //go to options
        Texture2D infoButton; //show instructions 
        Texture2D musicButton; //turn off/on music
        Texture2D soundsButton;  //turn off/on sounds
        Color buttonColour = Color.White * 0.8f;
        //button positions
        Vector2 startPos; //location for the location of the start button
        Vector2 continuePos; //location of continue button
        Vector2 exitPos; //location of exit button
        Vector2 mainMenuPos;
        Vector2 optionsPos;
        Vector2 infoPos;
        Vector2 musicPos, soundsPos;
        Vector2 infoBubblePos; //position of the info bubble
        const int ButtonsX = 100; //all these buttons have the same x position
        const int ButtonWidth = 128, ButtonHeight = 48; //height and width of the menu buttons is the same

        //sounds
        Song menusMusic; //play this music when not playing the game (menus,instructions,options,gameover)
        Song gameMusic; //play this music when the game is being played
        SoundEffect rock, laser, laser2, explosion; //other sound effects
        SoundEffect click; //for clicking buttons

        //gamemodes
        const int Splash = 0, Game = 1, Gameover = 2, Options = 3, Pause = 4; //different gamemodes
        int gameMode = Splash; //current gamemode


        //when the player dies these are used to make only one explosion and to wait a while before going to the gameover screen
        int deathTimer;
        bool enableDeathTimer;
        bool playerDeathExplode;

        //bullet texture and data
        Texture2D bulletTex; 
        Color[] bulletTextureData; 

        Texture2D infoBox; //texture used to display bubbles
        Texture2D infoTexture;//texture used to display text on
        bool nameEntered = false; //has the player entered their name yet
        bool showInfo = false; //if the info screen should be shown or not
        bool showContinue = false; //can continue the game if player goes into options from the pause menu while playing
        bool playMusic; //play music if this is true otherwise dont
        bool playSounds; //play sounds if this is true otherwise dont
        string playerName;//the name of the player which they will type in


        Random rnd = new Random();
        MouseState previousMouseState; //info about the state the mouse was in, in the previous frame
        KeyboardState previousKeyboardState; //info about the state of the keyboard in the previous frame


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        { //give starting values to all variables
            // TODO: Add your initialization logic here
            IsMouseVisible = true; //show the mouse

            enableDeathTimer = false;
            deathTimer = 80;
            playerDeathExplode = true;

            //start positions of buttons
            startPos = new Vector2(ButtonsX, 100);
            continuePos = new Vector2(385, 230);
            optionsPos = new Vector2(ButtonsX, 220);
            exitPos = new Vector2(ButtonsX, 280);
            infoPos = new Vector2(ButtonsX, 160);
            musicPos = new Vector2(ButtonsX - 50, 140);
            soundsPos = new Vector2(ButtonsX - 50, 80);
            mainMenuPos = new Vector2(50, 50);

            infoBubblePos = new Vector2(0, 390); //pos of info bubble

            //change size of the screen
            graphics.PreferredBackBufferWidth = 880;
            graphics.PreferredBackBufferHeight = 640;
            graphics.ApplyChanges();

            backgroundRect = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height); //rectangle for the background image to be displayed in
            playerName = "";

            //set up sounds
            playMusic = true;
            playSounds = true;

            asteroidDestroyedTimer = AsteroidDestroyedMaxTime;

            //create new objects
            scoreKeeper = new Scorekeeper();
            player = new Player(Window.ClientBounds.Width, Window.ClientBounds.Height);
            upgrade = new Upgrade();
            heartDrop = new Heart();
            LoadHighScores(); //load any highscores from the xml file from previous saves

            base.Initialize();
        }
        
        private void UpdateBackgrounds()
        { //update the position of the background relative to the player
            Vector2 playerPos = player.Position;
            float speed = 1f;

            for (int i = 0; i < MaxBackgrounds; i++)
            {
                backgroundPositions[i] = -playerPos * speed;
                speed -= 0.2f; //each additional background is slower to give the illusion of a 3d background
            }
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {//load all the content to be used in the game
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
            bigFont = Content.Load<SpriteFont>("Font1");

            explosionSpriteSheet = Content.Load<Texture2D>("explosionspritesheet");
            infoBox = Content.Load<Texture2D>("infobubble");
            infoTexture = Content.Load<Texture2D>("infoTexture");

            shield = Content.Load<Texture2D>("shield");
            //bullet texture and data
            bulletTex = Content.Load<Texture2D>("bullet");
            bulletTextureData = new Color[bulletTex.Width * bulletTex.Height];
            bulletTex.GetData(bulletTextureData);

            //engine particles
            particleTextures.Add(Content.Load<Texture2D>("circle"));
            particleTextures.Add(Content.Load<Texture2D>("star"));
            particleTextures.Add(Content.Load<Texture2D>("triangle"));


            //enemies
            enemyTexture = Content.Load<Texture2D>("enemy"); //load texture and then data for that texture
            enemyTextureData = new Color[enemyTexture.Width * enemyTexture.Height];
            enemyTexture.GetData(enemyTextureData);

            enemyTexture2 = Content.Load<Texture2D>("enemy2");
            enemyTextureData2 = new Color[enemyTexture2.Width * enemyTexture2.Height];
            enemyTexture2.GetData(enemyTextureData2);

            enemyTexture3 = Content.Load<Texture2D>("enemy3");
            enemyTextureData3 = new Color[enemyTexture3.Width * enemyTexture3.Height];
            enemyTexture3.GetData(enemyTextureData3);

            enemyHealthTexture = Content.Load<Texture2D>("healthbar");

            //asteroids
            asteroidTexture = Content.Load<Texture2D>("asteroid");
            asteroidTextureData = new Color[asteroidTexture.Width * asteroidTexture.Height];
            asteroidTexture.GetData(asteroidTextureData);


            //asteroid particles
            List<Texture2D> asteroidParticleTextures = new List<Texture2D>();
            asteroidParticleTextures.Add(Content.Load<Texture2D>("asteroidparticle1"));
            asteroidParticleTextures.Add(Content.Load<Texture2D>("asteroidparticle2"));
            asteroidParticleTextures.Add(Content.Load<Texture2D>("asteroidparticle3"));
            asteroidParticles = new ParticleEngine(asteroidParticleTextures, Vector2.Zero,  Color.Gray, Color.Gray);

            //backgrounds
            backgrounds[0] = Content.Load<Texture2D>("space60");
            backgrounds[1] = Content.Load<Texture2D>("space80");
            backgrounds[2] = Content.Load<Texture2D>("space100");
            gameOverOptionsBackground = Content.Load<Texture2D>("gameoverBackground");
            splashBackground = Content.Load<Texture2D>("splashbackground");

            //load button textures
            startButton = Content.Load<Texture2D>("startbutton");
            continueButton = Content.Load<Texture2D>("continuebutton");
            exitButton = Content.Load<Texture2D>("exitbutton");
            mainMenuButton = Content.Load<Texture2D>("mainmenubutton");
            optionsButton = Content.Load<Texture2D>("optionsbutton");
            musicButton = Content.Load<Texture2D>("musicbutton");
            soundsButton = Content.Load<Texture2D>("soundsbutton");
            infoButton = Content.Load<Texture2D>("infobutton");

            //load sounds
            menusMusic = Content.Load<Song>("music2");
            gameMusic = Content.Load<Song>("music1");
            click = Content.Load<SoundEffect>("click");
            rock = Content.Load<SoundEffect>("rock");
            laser = Content.Load<SoundEffect>("laser");
            laser2 = Content.Load<SoundEffect>("laser2");
            explosion = Content.Load<SoundEffect>("explosion");
            MediaPlayer.Play(menusMusic); //start with menu music
            MediaPlayer.IsRepeating = true; //music will always be repeating

            player.LoadContent(this.Content, "ship", particleTextures, laser);
            scoreKeeper.LoadContent(this.Content, "scoreboard", "healthbar", font, bigFont, infoTexture);
            upgrade.LoadContent(this.Content, "upgrade");
            heartDrop.LoadContent(this.Content, "heart");
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            switch (gameMode)
            {
                case Splash: //if the game mode is currently set to the splash mode
                    UpdateSplash(gameTime); //update the splash mode only
                    break;
                case Game:
                    UpdateGame(gameTime);
                    break;
                case Gameover:
                    UpdateGameOver(gameTime);
                    break;
                case Options:
                    UpdateOptions(gameTime);
                    break;
                case Pause:
                    UpdatePause(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateSplash(GameTime gameTime)
        {//do this while in the splash mode
            if (!Keyboard.GetState().IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyDown(Keys.Escape) && showInfo)
            {
                showInfo = false; //leave the information screen if the escape key was pressed while inside it
            }

            CheckButtons();//check if buttons have being clicked

            //set the current states to be the new previous states
            previousMouseState = Mouse.GetState();
            previousKeyboardState = Keyboard.GetState();
        }

        private void UpdateGameOver(GameTime gameTime)
        {//what to do in the game over mode
            if (!Keyboard.GetState().IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyDown(Keys.Escape) && nameEntered)
            {
                gameMode = Splash; //return to the splash screen
            }

            if (nameEntered == false)
            {
                GetUserInput(); //get the name being entered by the player
            } 

            if (!Keyboard.GetState().IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyDown(Keys.Enter) && nameEntered == false)
            { //when the player presses enter 
                if (playerName == null || playerName == "")
                    playerName = "Default"; //if the player didnt enter anything or they have scored 0
                nameEntered = true; //name has now been entered
                scoreKeeper.UpdateHighScores(playerName); //pass this name to scorekeeper to be added to the highscore table
                SaveHighScores(); //save the new table to an xml file
            }

            CheckButtons(); //check if any buttons have being clicked
            previousKeyboardState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
        }

        private void GetUserInput()
        { //checks what keys the user has pressed and add them onto playerName
            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
            foreach (Keys key in pressedKeys)    //http://www.gamedev.net/topic/457783-xna-getting-text-from-keyboard/ use this link to help me 
            {
                if (previousKeyboardState.IsKeyUp(key)) //if this key was released previously but is now pressed
                {
                    if (key == Keys.Back && playerName.Length > 0) // overflows
                        playerName = playerName.Remove(playerName.Length - 1, 1); //delete the last char at the end of this string
                    else if (key >= Keys.A && key <= Keys.Z && playerName.Length < 8) //if the the key is a character
                    {
                        bool caps = false;
                        foreach (Keys checkKey in pressedKeys) //loop to check if the right of left shift have been pressed to turn on caps
                        {
                            if (checkKey == Keys.LeftShift || checkKey == Keys.RightShift)
                                caps = true;
                        }
                        if (caps)
                            playerName += key.ToString(); //add the pressed keys to the string as capitals
                        else
                            playerName += key.ToString().ToLower(); //add the pressed keys to the string as small letters

                    }// end if else 
                } //end foreach
            }
        }

        private void UpdateGame(GameTime gameTime)
        {//what to do in the game gamemode
            if (!Keyboard.GetState().IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                gameMode = Pause; //pause the game when the escape key is pressed
                if (playMusic) //if music can be played
                    MediaPlayer.Play(menusMusic); //change music
            }

            if (!Keyboard.GetState().IsKeyDown(Keys.R) && previousKeyboardState.IsKeyDown(Keys.R))
            { //press r to reset the highscores while playing
                scoreKeeper.ResetHighScores();
            }

            if (enableDeathTimer) //if the player has just died
            {
                deathTimer--; //decrease this timer 
                if (deathTimer < 0) //when it gets below 0
                {
                    nameEntered = false; //reset nameEntered to false
                    playerName = ""; //reset the player name to an empty string
                    gameMode = Gameover; //then go into game over mode
                }
            }

            SpawnNewObjects(); //spawn new objects randomly

            //update 4 different lists of objects using foreach loops
            foreach (Asteroid a in asteroids)
                a.Update();
            foreach (Enemy e in enemies)
                e.Update(playSounds);
            foreach (Fighter f in fighters)
                f.Update(playSounds, player.Position);
            foreach (AdvancedFighter af in advancedFighters)
                af.Update(playSounds, player.Position);

            if (asteroidDestroyedTimer < AsteroidDestroyedMaxTime) //this is only true when the timer is reset when the asteroid is destroyed
            {
                asteroidParticles.Update(5, 50); //keep adding particles during this time
                asteroidDestroyedTimer++; //increase the timer
            }

            UpdateBackgrounds();
            DetectCollisions(); 
            RemoveObjects();
            if (scoreKeeper.PlayerAlive) //only update the player while they are alive
                player.Update(playSounds); 
            asteroidParticles.Update();
            upgrade.Update();
            heartDrop.Update();

            IsGameOver(); //check if the gameover conditions have occured
            CheckButtons(); //check what buttons have being clicked 
            previousMouseState = Mouse.GetState();
            previousKeyboardState = Keyboard.GetState();
        }

        private void SaveHighScores()
        { //save the highscores to a text and xml file
            try
            {
                //Text:
                StreamWriter outStream = File.CreateText("../../../../SaveFiles/txtFile.txt");
                scoreKeeper.WriteTxt(outStream); //scorekeeper object writes itself to a text file 
                outStream.Close();

                //XML:
                XmlTextWriter writer = new XmlTextWriter("../../../../SaveFiles/xmlFile.xml", null);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteComment("XML save file of the highscores in SpaceFighter");

                writer.WriteStartElement("SpaceFighter"); //root node

                scoreKeeper.WriteXML(writer); //scorekeepr writes itself

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void LoadHighScores()
        {//load the highscores from the xml file
            try
            {
                //XML:
                XmlTextReader reader = new XmlTextReader("../../../../SaveFiles/xmlFile.xml");
                scoreKeeper.ReadXML(reader);    //object loads its own highscores           
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void UpdateOptions(GameTime gameTime)
        {//do this while in the options gamemode
            if (!Keyboard.GetState().IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyDown(Keys.Escape) && showContinue == false)
            {
                gameMode = Splash; //go to splash if in options from the main menu
            }
            if (!Keyboard.GetState().IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyDown(Keys.Escape) && showContinue)
            {
                gameMode = Pause; //go to pause if in options from the game
            }

            CheckButtons();
            previousKeyboardState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
        }

        private void UpdatePause(GameTime gameTime)
        {
            if (!Keyboard.GetState().IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                gameMode = Game; //return to the game
            }

            CheckButtons();
            previousKeyboardState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
        }

        private void IsGameOver()
        { //checks to see if the game has ended
            if (scoreKeeper.PlayerAlive == false) //gameover if true
            {
                if (playerDeathExplode) 
                {
                    if (playMusic)
                        MediaPlayer.Play(menusMusic); //change music
                    playerDeathExplode = false; //make this false so only one explosion is made
                    explosions.Add(new ExplosionSprite(explosionSpriteSheet, new Rectangle((int)player.ExpPos.X, (int)player.ExpPos.Y,
                                                    player.Texture.Width, player.Texture.Height)));
                }
                enableDeathTimer = true; //start the timer that will count down until the gamemode changes
            }
        }

        private void RestartGame()
        { //restart the game by clearing all list, calling objects reset methods and reseting some variables
            scoreKeeper.Reset();
            player.Reset();
            asteroids.Clear();
            asteroidParticles.Reset();
            upgrade.Reset();
            enemies.Clear();
            fighters.Clear();
            advancedFighters.Clear();
            explosions.Clear();
            enableDeathTimer = false;
            deathTimer = 80;
            playerDeathExplode = true;
            heartDrop.Reset();
        }


        private void CheckButtons()
        { //check if buttons were clicked and do something if they are
            if (gameMode == Splash) //if in the splash screen
            {
                if (showInfo == false) //not in the info screen
                {
                    if (CheckButtonClicked(startPos))
                    {
                        gameMode = Game; //if the start button has been clicked then start the game and restart it with any changes
                        if (playMusic)
                            MediaPlayer.Play(gameMusic);
                        RestartGame(); //reset the board and other variables
                    }
                    else if (CheckButtonClicked(optionsPos))
                    {
                        gameMode = Options;
                    }

                    else if (CheckButtonClicked(exitPos))
                    {
                        Exit(); //exit the game
                    }

                    else if (CheckButtonClicked(infoPos))
                    {
                        showInfo = true;
                    }
                }
                else  //in the info screen
                {
                    if (CheckButtonClicked(mainMenuPos)) //only main menu button can be clicked
                        showInfo = false;
                }
            }

            else if (gameMode == Game) //if the game is playing 
            {
                //no buttons in this gamemode to check
            }

            else if (gameMode == Gameover)
            {
                if (CheckButtonClicked(mainMenuPos) && nameEntered) //clicked and name has been entered
                {
                    gameMode = Splash;
                }
            }

            else if (gameMode == Pause)
            {
                if (CheckButtonClicked(mainMenuPos)) 
                {
                    gameMode = Splash;
                }
                else if (CheckButtonClicked(optionsPos))
                {
                    gameMode = Options;
                    showContinue = true; //can continue the game if in entering the options from the pause mode
                }
                else if (CheckButtonClicked(continuePos))
                {
                    gameMode = Game; //if the continue button has been clicked then continue the game
                    if (playMusic)
                        MediaPlayer.Play(gameMusic);
                }

                else if (CheckButtonClicked(exitPos))
                {
                    Exit(); //exit the game
                }
            }

            else //in options screen
            {

                if (CheckButtonClicked(mainMenuPos) && showContinue == false)
                    gameMode = Splash;

                else if (CheckButtonClicked(musicPos))
                {
                    playMusic = !playMusic; //toggle the music to the opposite of what it currently is. (if false it will now be set to true)

                    if (playMusic)
                        MediaPlayer.Play(menusMusic); //start music if playMusic is true
                    else
                        MediaPlayer.Stop(); //stop the music if playMusic is now false
                }

                else if (CheckButtonClicked(soundsPos))
                    playSounds = !playSounds; //toggle the sounds on or off

                else if (CheckButtonClicked(continuePos) && showContinue)
                {
                    gameMode = Game; //if the continue button has been clicked then continue the game
                    if (playMusic)
                        MediaPlayer.Play(gameMusic);
                }

            } //end else 
        }

        private bool CheckButtonClicked(Vector2 pos)
        { //check if the player has clicked the area of the screen of where this button is
            Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, ButtonWidth, ButtonHeight);
            if (rect.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y)) &&
                previousMouseState.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (playSounds)
                    click.Play(); //this will play the click sound 
                return true;
            }
            else
                return false;
        }

        private void SpawnNewObjects()
        { //spawn all new objects random
            if (rnd.NextDouble() < asteroidSpawnProb) //if less than its spawn prob then make a new one and add it to the list
            { //add to the list of asteroids
                asteroids.Add(new Asteroid(rnd, asteroidTexture, asteroidTextureData, Window.ClientBounds.Width, Window.ClientBounds.Height));
            }


            //spawn new powerup
            if (rnd.NextDouble() < upgradeSpawnProb && upgrade.Alive == false && player.BulletUpgraded == false)
            { //spawn if its not already alive and the player hasnt gotten it yet
                upgrade = new Upgrade(rnd, Window.ClientBounds.Width, Window.ClientBounds.Height);
            }

            if (rnd.NextDouble() < enemySpawnProb)
            {
                Vector2 screenSize = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height); //pass in the screen size as a vector
                Enemy newEnemy = new Enemy(rnd, enemyHealthTexture,screenSize, enemyTexture, particleTextures,
                                            laser2, enemyTextureData);
                enemies.Add(newEnemy);
            }

            if (rnd.NextDouble() < fighterSpawnProb)
            {
                Vector2 screenSize = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
                Fighter newFighter = new Fighter(rnd, enemyHealthTexture, screenSize, enemyTexture2, particleTextures,
                                            laser2, enemyTextureData2);
                fighters.Add(newFighter);
            }

            if (rnd.NextDouble() < advancedFighterSpawnProb)
            {
                Vector2 screenSize = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
                AdvancedFighter newAdvancedFighter = new AdvancedFighter(rnd, enemyHealthTexture, screenSize, enemyTexture3, particleTextures,
                                            laser, enemyTextureData3, shield);
                advancedFighters.Add(newAdvancedFighter);
            }

        }

        private void RemoveObjects()
        { //remove any objects from lists which are no longer alive
            for (int i = 0; i < asteroids.Count; i++)
            {
                if (asteroids[i].Alive == false)
                {
                    if (asteroids[i].Destroyed) //if it has been destroyed (and not left the screen where the player cant see it anymore)
                    {
                        asteroidDestroyedTimer = 0;
                        asteroidParticles.EmitterLocation = asteroids[i].Position; //make some asteroid particles
                        if (playSounds)
                            rock.Play();
                    }
                    asteroids.RemoveAt(i);
                    i--; //decrease i so it doesnt go out of bounds
                }
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].Alive == false)
                {
                    if (enemies[i].Destroyed)
                    {
                        explosions.Add(new ExplosionSprite(explosionSpriteSheet, new Rectangle((int)enemies[i].TopLeft.X, (int)enemies[i].TopLeft.Y,
                                                        enemies[i].Texture.Width, enemies[i].Texture.Height))); //make a new explosion
                        if (playSounds)
                            explosion.Play();

                        if (rnd.NextDouble() < heartSpawnProb) //spawn heart if less than this at same place explosion spawns
                            heartDrop = new Heart(enemies[i].Pos.X - heartDrop.halfWidth, enemies[i].Pos.Y - heartDrop.halfHeight);
                    }
                    enemies.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < fighters.Count; i++)
            {
                if (fighters[i].Alive == false)
                {
                    if (fighters[i].Destroyed)
                    {
                        explosions.Add(new ExplosionSprite(explosionSpriteSheet, new Rectangle((int)fighters[i].TopLeft.X, (int)fighters[i].TopLeft.Y,
                                                        fighters[i].Texture.Width, fighters[i].Texture.Height)));
                        if (playSounds)
                            explosion.Play();

                        if (rnd.NextDouble() < heartSpawnProb) //spawn heart if less than this at same place explosion spawns
                            heartDrop = new Heart(fighters[i].Pos.X - heartDrop.halfWidth, fighters[i].Pos.Y - heartDrop.halfHeight);
                    }
                    fighters.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < advancedFighters.Count; i++)
            {
                if (advancedFighters[i].Alive == false)
                {
                    if (advancedFighters[i].Destroyed)
                    {
                        explosions.Add(new ExplosionSprite(explosionSpriteSheet, new Rectangle((int)advancedFighters[i].TopLeft.X, (int)advancedFighters[i].TopLeft.Y,
                                                        advancedFighters[i].Texture.Width, advancedFighters[i].Texture.Height)));
                        if (playSounds)
                            explosion.Play();
                        if (rnd.NextDouble() < heartSpawnProb) //spawn heart if less than this at same place explosion spawns
                            heartDrop = new Heart(advancedFighters[i].Pos.X - heartDrop.halfWidth, advancedFighters[i].Pos.Y - heartDrop.halfHeight);
                    }
                    advancedFighters.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < explosions.Count; i++) //remove explosions that have finished exploding and their alive is false
            {
                if (explosions[i].Alive == false)
                {
                    explosions.RemoveAt(i);
                    i--;
                }
            }
        }

        private void DetectCollisions()
        { //detect all collisions between everything in the game
            //Circle and Rectangle collisions:

            //asteroids
            CheckColsAsteroids();

            //enemies:
            CheckColsEnemies();

            //fighters
            CheckColsFighters();

            //advanced figthers
            CheckColsAdvancedFighters();
            

            //check if the player has collided with the upgrade object
            if (CircleIntersects(player.Rectangle, upgrade.Rectangle) && upgrade.Alive)
            {
                upgrade.Reset();
                player.BulletUpgraded = true;
            }          

            //check if the player has collided with the heath object
            if (CircleIntersects(player.Rectangle, heartDrop.Rectangle) && heartDrop.Alive)
            {
                heartDrop.Reset();
                scoreKeeper.ChangePlayerHealth(heartDrop.Health);
            }          
        }


        private void CheckColsAsteroids()
        { //check if anything has collided with the asteroids
            for (int i = 0; i < asteroids.Count; i++) //loop through every asteroid
            {
                if (asteroids[i].Alive) //only check collisions with the asteroid if it is alive
                {
                    //get the rectangle that encloses all 4 cornors of the transformed rectangle of the asteroid
                    Rectangle boundingAsteroidRect = CalculateBoundingRectangle(asteroids[i].Rectangle, asteroids[i].Transform);

                    //check the object + bound rectangle collide 
                    if (CircleIntersects(player.Rectangle, boundingAsteroidRect))
                    { 
                        {//if the player and an asteroid collide
                            scoreKeeper.ChangePlayerHealth(asteroids[i].Damage); //reduce the players health by the damage of the asteroid
                            asteroids[i].Alive = false; //asteroid is destroyed 
                            asteroids[i].Destroyed = true;
                        }
                    } //ending circle-circle col

                    for (int j = 0; j < enemies.Count; j++)
                    {
                        if (enemies[j].Alive) //only check collison if the enemy is alive
                        {
                            if (CircleIntersects(enemies[j].Rectangle, boundingAsteroidRect))
                            {//if the enemies and an asteroid collide
                                //both asteroid and enemy are destroyed
                                enemies[j].Alive = false;
                                enemies[j].Destroyed = true;
                                asteroids[i].Alive = false;
                                asteroids[i].Destroyed = true;
                            }//end circle- circle col
                        } //end if enemy is alive
                    } //end for that loops through each enemy

                    for (int j = 0; j < fighters.Count; j++) 
                    {
                        if (fighters[j].Alive)
                        {
                            if (CircleIntersects(fighters[j].Rectangle, boundingAsteroidRect))
                            { //see if asteroid has hit the fighter ship
                                //both fighter and asteroid is destroyed
                                fighters[j].Alive = false;
                                fighters[j].Destroyed = true;
                                asteroids[i].Alive = false;
                                asteroids[i].Destroyed = true;
                            }//end circle- circle col
                        } //end if fighter is alive
                    } //end for that loops through each fighter

                    for (int j = 0; j < advancedFighters.Count; j++) 
                    {
                        if (advancedFighters[j].Alive)
                        {
                            if (CircleIntersects(advancedFighters[j].Rectangle, boundingAsteroidRect))
                            {//see if asteroid has hit the advanced fighter ship
                                advancedFighters[j].DescreaseHealth(asteroids[i].Damage - 40); //asteroid does more damage to the fighters but doesnt kill them
                                if (advancedFighters[j].Alive == false)
                                    advancedFighters[j].Destroyed = true;
                                asteroids[i].Alive = false; //asteroid destroyed
                                asteroids[i].Destroyed = true;
                            }//end circle- circle col
                        } //end if advancedfighter is alive
                    } //end for that loops through each advancedfighter

                    for (int j = 0; j < player.Bullets.Count(); j++) //check if any of the player bullets have collided with the asteroids
                    {
                        if (player.Bullets[j].Alive) //only check if alive
                        {
                            if (CircleIntersects(player.Bullets[j].Rectangle, boundingAsteroidRect))
                            {
                                asteroids[i].Velocity = player.Bullets[j].Velocity; //give the asteroid the bullets velocity so it bounces back
                                scoreKeeper.ChangeScore(10); //increase the players score
                                asteroids[i].Hits--; //asteroid hits are reduced ( if 0 then it is no longer alive)
                                player.Bullets[j].Alive = false; //bullet is no longer alive
                            }
                        } //end if 
                    }//end for that loops through each bullet the player has
                } //end the if that checks if the current asteroid is alive
            } //end for that loops through asteroids
        } //end method

        private void CheckColsEnemies()
        { //checks all collisions with the enemies
            for (int i = 0; i < enemies.Count; i++) //loop through every enemy
            {
                if (enemies[i].Alive) //only check if the enemy is alive
                {
                    if (CircleIntersects(enemies[i].Rectangle, player.Rectangle))//if the player and an enemy collide
                    {  
                        scoreKeeper.ChangePlayerHealth(enemies[i].Damage - 10); //player loses health
                        enemies[i].Alive = false; //enemy destroyed 
                        enemies[i].Destroyed = true;
                    }

                    for (int j = 0; j < fighters.Count; j++) //if any of the fighters have collided with the smaller enemies
                    {
                        if (fighters[j].Alive) 
                        {
                            if (CircleIntersects(enemies[i].Rectangle, fighters[j].Rectangle))
                            {
                                fighters[j].DescreaseHealth(enemies[i].Damage - 15);
                                enemies[i].Alive = false;
                                enemies[i].Destroyed = true;
                            }
                        }
                    }

                    for (int j = 0; j < advancedFighters.Count; j++) //if any of the advanced fighters have collided with the smaller enemies
                    {
                        if (advancedFighters[j].Alive)
                        {
                            if (CircleIntersects(enemies[i].Rectangle, advancedFighters[j].Rectangle))
                            {
                                advancedFighters[j].DescreaseHealth(enemies[i].Damage - 15);
                                enemies[i].Alive = false;
                                enemies[i].Destroyed = true;
                            }
                        }
                    }

                    for (int bullet = 0; bullet < enemies[i].Bullets.Count(); bullet++) //check if any of the enemies bullets have collided with the player
                    {
                        if (enemies[i].Bullets[bullet].Alive)
                        {
                            if (CircleRectIntersects(player.Rectangle, enemies[i].Bullets[bullet].Rectangle))
                            {
                                enemies[i].Bullets[bullet].Alive = false;
                                scoreKeeper.ChangePlayerHealth(enemies[i].Damage);
                            }
                        }
                    }



                    for (int j = 0; j < player.Bullets.Count(); j++) //check if any of the bullets have collided with the enemies
                    {
                        if (player.Bullets[j].Alive)
                        {
                            if (CircleRectIntersects(enemies[i].Rectangle, player.Bullets[j].Rectangle))
                            {
                                scoreKeeper.ChangeScore(10);
                                player.Bullets[j].Alive = false;
                                enemies[i].DescreaseHealth(player.Damage);
                                if (enemies[i].Alive == false)  //if the enemy was killed as a result of the player hitting them
                                {
                                    scoreKeeper.ChangeScore(50);
                                    enemies[i].Destroyed = true;  //then allow sounds and particles to be created/played    
                                } //end if
                            } //end circle - rect collision check
                        }//end if bullet is alive
                    } //end the for loop through the players array of bullets
                } //end if the enemy is alive
            } //end for looping through enemies
        } //end the method

        private void CheckColsFighters()
        { //check for collisions with the fighters
            for (int i = 0; i < fighters.Count; i++) //loop through all fighters
            {
                if (fighters[i].Alive) //only check if the fighter is alive
                {
                    for (int j = 0; j < advancedFighters.Count; j++) //if any of the fighters have collided with the advanced fighters
                    {
                        if (advancedFighters[j].Alive) //only if alive
                        {
                            if (CircleIntersects(fighters[i].Rectangle, advancedFighters[j].Rectangle))
                            {
                                advancedFighters[j].DescreaseHealth(fighters[i].Damage - 15);
                                fighters[i].Alive = false;
                                fighters[i].Destroyed = true;
                            }
                        }
                    }

                    for (int bullet = 0; bullet < fighters[i].Bullets.Count(); bullet++) //check if any of the fighter bullets have collided with the player
                    {
                        if (fighters[i].Bullets[bullet].Alive)
                        {
                            if (CircleRectIntersects(player.Rectangle, fighters[i].Bullets[bullet].Rectangle))
                            {
                                fighters[i].Bullets[bullet].Alive = false;
                                scoreKeeper.ChangePlayerHealth(fighters[i].Damage);
                            }
                        }
                    }

                    if (CircleIntersects(fighters[i].Rectangle, player.Rectangle)) //if the player and an enemy collide
                    {
                        scoreKeeper.ChangePlayerHealth(fighters[i].Damage - 5);
                        fighters[i].Alive = false;
                        fighters[i].Destroyed = true;
                    }

                    for (int j = 0; j < player.Bullets.Count(); j++) //check if any of the player bullets have collided with the fighters
                    {
                        if (player.Bullets[j].Alive)
                        {
                            if (CircleRectIntersects(fighters[i].Rectangle, player.Bullets[j].Rectangle))
                            {
                                scoreKeeper.ChangeScore(20);
                                player.Bullets[j].Alive = false;
                                fighters[i].DescreaseHealth(player.Damage);
                                if (fighters[i].Alive == false)  //if the fighter was killed as a result of the player hitting them
                                {
                                    scoreKeeper.ChangeScore(75);
                                    fighters[i].Destroyed = true;  //then allow sounds and particles to be created/played    
                                } //end if
                            } //end circle - rect collision check between fighter and player bullets
                        }//end if player bullet is alive
                    } //end the for loop through the players array of bullets
                } //end if the fighter is alive
            } //end for looping through fighters
        } //end the method

        private void CheckColsAdvancedFighters()
        {//check if the advancedfighters have collided with anything
            for (int i = 0; i < advancedFighters.Count; i++) //loop through fighters
            {
                if (advancedFighters[i].Alive) //only if alive
                {
                    for (int bullet = 0; bullet < advancedFighters[i].Bullets.Count(); bullet++) //check if any of the advanced fighter bullets have collided with the player
                    {
                        if (advancedFighters[i].Bullets[bullet].Alive)
                        {
                            if (CircleRectIntersects(player.Rectangle, advancedFighters[i].Bullets[bullet].Rectangle))
                            {
                                advancedFighters[i].Bullets[bullet].Alive = false;
                                scoreKeeper.ChangePlayerHealth(advancedFighters[i].Damage);
                            }
                        }
                    }

                    if (CircleIntersects(advancedFighters[i].Rectangle, player.Rectangle)) //if the player and an advanced fighter collide
                    {
                        scoreKeeper.ChangePlayerHealth(advancedFighters[i].Damage - 15);
                        advancedFighters[i].Alive = false;
                        advancedFighters[i].Destroyed = true;
                    }

                    for (int j = 0; j < player.Bullets.Count(); j++) //check if any of the player bullets have collided with the fighters
                    {
                        if (player.Bullets[j].Alive)
                        {
                            if (CircleRectIntersects(advancedFighters[i].Rectangle, player.Bullets[j].Rectangle))
                            {
                                scoreKeeper.ChangeScore(35);
                                player.Bullets[j].Alive = false;
                                advancedFighters[i].DescreaseHealth(player.Damage);
                                if (advancedFighters[i].Alive == false)  //if the fighter was killed as a result of the player hitting them
                                {
                                    scoreKeeper.ChangeScore(100);
                                    advancedFighters[i].Destroyed = true;  //then allow sounds and particles to be created/played    
                                } //end if
                            } //end circle - rect collision check between advancedfighter and player bullets
                        }//end if player bullet is alive
                    } //end the for loop through the players array of bullets
                } //end if the advancedfighter is alive
            } //end for looping through advancedfighter
        } //end the method

        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }						   

        private bool CircleIntersects(Rectangle rect1, Rectangle rect2)
        {//check if two circles have collided, get the circles from the rectangles passed in
            int radius1, radius2; //radi of circles
            //get centres of both circles
            Vector2 centre1 = new Vector2(rect1.X + rect1.Width / 2, rect1.Y + rect1.Height / 2);
            Vector2 centre2 = new Vector2(rect2.X + rect2.Width / 2, rect2.Y + rect2.Height / 2);
 
            //Set radius to the largest of the width and height of the objects
            radius1 = Math.Max(rect1.Width, rect1.Height) / 2;
            radius2 = Math.Max(rect2.Width, rect2.Height) / 2;


            //get distance between both centres
            double distance = (centre2 - centre1).Length();

            //Check if distance between the circles is greater than the two radius
            if (distance > radius1 + radius2)
                return false;  //If it is then there is no collision between the objects so return false
            else
                return true; //collision
        }

        private bool CircleRectIntersects(Rectangle circleRect, Rectangle rect)
        {//check if a circle and rectangle has collided, get circle from the rect passed in
            int radius;
            //get centres of the circle
            Vector2 centre = new Vector2(circleRect.X + circleRect.Width / 2, circleRect.Y + circleRect.Height / 2);

            //Set radius to the largest of the width and height of the object
            radius = Math.Max(circleRect.Width, circleRect.Height) / 2;
            
            if (centre.X + radius < rect.Left || centre.X - radius > rect.Right || centre.Y + radius < rect.Top || centre.Y - radius > rect.Bottom)
                return false; //no collision
            else
                return true; //collision

        }

        public static bool IntersectPixels(
                       Matrix transformA, int widthA, int heightA, Color[] dataA,
                       Matrix transformB, int widthB, int heightB, Color[] dataB)
        { //optimized pixel per pixel collision detection
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        ///
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            switch (gameMode)
            {
                case Splash: //if the current gamemode is set to splash 
                    DrawSplash(); //draw the splash screen
                    break;
                case Game:
                    DrawGame(gameTime);
                    break;
                case Pause:
                    DrawGame(gameTime);
                    DrawPause();
                    break;
                case Gameover:
                    DrawGameOver();
                    break;
                case Options:
                    DrawOptions();
                    break;

            }
           
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawGameOver()
        {//draw this when the gamemode is gameover

            mainMenuPos = new Vector2(30, 270); //change the position of the main menu button for this screen

            spriteBatch.Draw(gameOverOptionsBackground, backgroundRect, Color.White); //draw backgrund


            spriteBatch.Draw(infoTexture, new Rectangle(30, 160, 160, 90), Color.White * 0.8f); //draw the infoTexture where the strings will be drawn on top of
            if (playerName != null) //error checking
                spriteBatch.DrawString(font, "Name: " + playerName, new Vector2(40, 170), Color.SteelBlue); //display the players name
            //display the score the player got
            spriteBatch.DrawString(font, "Score: " + String.Format("{0:D4}", scoreKeeper.Score), new Vector2(40, 210), Color.SteelBlue);

            if (nameEntered == false) //if the player has entered their name yet
            { //tell the player to enter their name
                spriteBatch.Draw(infoTexture, new Rectangle(30, 85, 140, 45), Color.White * 0.8f);
                spriteBatch.Draw(mainMenuButton, mainMenuPos, Color.White * 0.5F); //the player cant click this button until they enter their name so display it at 0.5 alpha
                spriteBatch.DrawString(bigFont, "Enter Name: ", new Vector2(40, 95), Color.SteelBlue); //display instrcutons
            }
            else //name entered
            {
                spriteBatch.Draw(mainMenuButton, mainMenuPos, buttonColour); //draw the full opaque button now as it can be clicked
            }

            scoreKeeper.DrawHighScores(spriteBatch);// highscore table draws itself
        }

        private void DrawPause()
        { //draw this during pause
            //change positions of buttons and draw them
            continuePos = new Vector2(385, 230);
            spriteBatch.Draw(continueButton, continuePos, buttonColour);
            mainMenuPos = new Vector2(385, 300);
            spriteBatch.Draw(mainMenuButton, mainMenuPos, buttonColour);
            optionsPos = new Vector2(385, 360);
            spriteBatch.Draw(optionsButton, optionsPos, buttonColour);
            exitPos = new Vector2(385, 420);
            spriteBatch.Draw(exitButton, exitPos, buttonColour);
        }

        private void DrawGame(GameTime gameTime)
        {//draw this during game play
            for (int i = 0; i < MaxBackgrounds; i++) //draw all backgrounds
            {
                spriteBatch.Draw(backgrounds[i], backgroundPositions[i], Color.White);
            }

            //all objects draw themselves
            foreach (Asteroid a in asteroids)
                a.Draw(spriteBatch);

            foreach (Enemy e in enemies)
                e.Draw(spriteBatch, bulletTex);

            foreach (Fighter f in fighters)
                f.Draw(spriteBatch, bulletTex);

            foreach (AdvancedFighter af in advancedFighters)
                af.Draw(spriteBatch, bulletTex);

            foreach (ExplosionSprite exp in explosions)
                exp.Draw(spriteBatch, gameTime);

            //draw any asteroid particles
            asteroidParticles.Draw(spriteBatch);
            upgrade.Draw(spriteBatch);
            heartDrop.Draw(spriteBatch);
            if (scoreKeeper.PlayerAlive) //only draw the player if they are alive
                player.Draw(spriteBatch, font, bulletTex);
            scoreKeeper.Draw(spriteBatch);
        }

        private void DrawSplash()
        {//during splash mode

            if (showInfo) //if player clicked the show info button
            {
                spriteBatch.Draw(splashBackground, backgroundRect, Color.White);
                spriteBatch.Draw(infoBox, infoBubblePos, Color.White); //draw info box which contains instructions
                mainMenuPos = new Vector2(ButtonsX + 30, 200); //change the position of the main menu button for this screen
                spriteBatch.Draw(mainMenuButton, mainMenuPos, buttonColour); //draw the main menu button on this screen
            }
            else //not in the info screen
            {
                spriteBatch.Draw(splashBackground, backgroundRect, Color.White);
                spriteBatch.Draw(startButton, startPos, buttonColour);
                optionsPos = new Vector2(ButtonsX, 220);
                spriteBatch.Draw(optionsButton, optionsPos, buttonColour);
                //draw other buttons
                exitPos = new Vector2(ButtonsX, 280);
                spriteBatch.Draw(exitButton, exitPos, buttonColour);
                spriteBatch.Draw(infoButton, infoPos, buttonColour);
            }
        }

        private void DrawOptions()
        {
            string onOrOff = ""; //string to show if music/sounds is off or on

            spriteBatch.Draw(gameOverOptionsBackground, backgroundRect, Color.White);//background
            //draw buttons
            spriteBatch.Draw(musicButton, musicPos, buttonColour);
            spriteBatch.Draw(soundsButton, soundsPos, buttonColour);
            scoreKeeper.DrawHighScores(spriteBatch);
            if (showContinue) //if player can continue (came from pause menu)
            {
                continuePos = new Vector2(ButtonsX - 50, 200);
                spriteBatch.Draw(continueButton, continuePos, buttonColour);
            }
            else //came from main menu (splash)
            {
                mainMenuPos = new Vector2(ButtonsX - 50, 200); //change the position of the main menu button for this screen
                spriteBatch.Draw(mainMenuButton, mainMenuPos, buttonColour);
            }

            if (playMusic)
                onOrOff = "On"; //music can be played
            else
                onOrOff = "Off";
            spriteBatch.DrawString(font, onOrOff, new Vector2(musicPos.X + 80, musicPos.Y + 18), Color.White);
            if (playSounds)
                onOrOff = "On"; //sounds can be played
            else
                onOrOff = "Off";
            spriteBatch.DrawString(font, onOrOff, new Vector2(soundsPos.X + 80, soundsPos.Y + 18), Color.White);
        }
    }
}
