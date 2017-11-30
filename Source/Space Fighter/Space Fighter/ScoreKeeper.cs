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

namespace JointProject2
{
    class Scorekeeper
    //Contains data and methods for health, lives and score.
    {

        int score; 
        int currentHealth;
        const int HealthBarMaxLength = 250; //length of the health bar to draw when full health
        const int MaxHealth = 200;

        Texture2D highScoreTexture; //texture for the background of the highscore table
        Rectangle highScoreRect; //position , width , height of the highscore table
        Texture2D scoreboard; //texture for the scoreboard at the top of the screen which has the players health and score
        Texture2D healthBackground; //texture for the bar behind the green->red bar of the players health
        Texture2D currentHealthTex; //texture for the green->red bar of the players health
        //position and size of the health bar
        Rectangle healthRect; 
        Rectangle backgroundHealthRect;
        //colors of the bars
        Color healthBarColor;
        Color backHealthBarColor;
        SpriteFont font; //to draw score,health and highscores
        SpriteFont bigFont;
        bool playerAlive;
        const int MaxHighScores = 5;
        int[] highScores = new int[MaxHighScores];
        string[] highScoreNames = new string[MaxHighScores];

        public Scorekeeper()
        {//set initial values
            playerAlive = true;
            score = 0;
            currentHealth = MaxHealth;
            backgroundHealthRect = new Rectangle(320, 15, HealthBarMaxLength, 20);
            healthRect = backgroundHealthRect;
            backHealthBarColor = Color.Black;
            healthBarColor = Color.Green;
            highScoreRect = new Rectangle(20, 350, 200, 150);
            
        }

        public void LoadContent(ContentManager theContentManager, string theAssetName, string theAssetName2, SpriteFont theFont, SpriteFont theFont2, Texture2D highscoreT)
        { //load in textures and fonts
            highScoreTexture = highscoreT;
            font = theFont;
            bigFont = theFont2;
            scoreboard = theContentManager.Load<Texture2D>(theAssetName);
            currentHealthTex = theContentManager.Load<Texture2D>(theAssetName2);
            healthBackground = currentHealthTex;
        }

        public void UpdateHighScores(string playerName)
        {//update the highscores with the new players name and their score
            int[] temp = new int[MaxHighScores + 1]; //create a temp array 1 bigger than the highscores array
            string[] stringTemp = new string[MaxHighScores + 1]; //create one for the string array aswell
            highScores.CopyTo(temp, 0); //copy over the highscores arrays
            highScoreNames.CopyTo(stringTemp, 0);
            temp[temp.Length - 1] = score; //last element of the temporary array is set to the new score
            stringTemp[stringTemp.Length - 1] = playerName; //last element of temp string array is set to new name 
            Sort(temp, stringTemp); //sort both arrays in descending order
            
            for (int i = 0; i < MaxHighScores; i++) //last element of the sorted array will be lost
            {
                highScoreNames[i] = stringTemp[i]; //put all the values in the temp arrays back into the normal arrays
                highScores[i] = temp[i];
            }
        }

        private void Sort(int[] array, string[] stringArray)
        { //sort the array with the biggest value first
            int n = array.Length; //the amount of numbers
            int maxElem; //current maximum element of the pass

            for (int pass = 0; pass < n; pass++)
            {
                maxElem = pass;
                for (int currentElem = pass + 1; currentElem < n; currentElem++)
                {
                    if (array[maxElem] <= array[currentElem])
                        maxElem = currentElem; //get the max element of this pass
                }
                //swap the values in the int array and also the relevant string in the string array
                int temp = array[pass]; 
                string stringTemp = stringArray[pass];
                array[pass] = array[maxElem];
                stringArray[pass] = stringArray[maxElem];
                array[maxElem] = temp;
                stringArray[maxElem] = stringTemp;
            }

        }

        public void ChangeScore(int scoreChange)
        //Increase or decrease the score by the amount passed to it
        {
            score += scoreChange;
        }

        public void Reset()
        //Reset back to defaults
        {
            score = 0;
            currentHealth = MaxHealth;
            GetHealthBarLength();
            playerAlive = true;
        }

        public void ResetHighScores()
        {//reset highscores back to default
            for (int i = 0; i < MaxHighScores; i++)
            {
                highScores[i] = 0;
                highScoreNames[i] = " ";
            }
        }
        public void GetHealthBarLength()
        //Get the new length of the health bar
        {
            healthRect.Width = (int)((HealthBarMaxLength) * ((float)currentHealth / MaxHealth)); //Get a percentage of the players remaining health and multiply this by the
            //length of the healthbar to make the texture representing health smaller
            //e.g if currentHealth/maxHealth is 20/100. Then 250 * 0.2 is 50. The green rectangle is now 50 
            healthBarColor = Color.Lerp(Color.Red, Color.Green, (float)currentHealth * 2 / MaxHealth );
        }


        public void ChangePlayerHealth(int amount)
        //To decrease the players health by the amount passed to it and may decrease lives. Check if they have been killed and restart or exit.
        {
            if (amount < 0) //if amount is negative
            {
                if (currentHealth + amount > 0) //If the damage dealt will not kill the player by putting their health 0 or lower 
                {
                    currentHealth += amount;
                }
                else
                { //player is dead
                    currentHealth = 0;
                    playerAlive = false;
                }
            }
            else //if positive
            {
                if (currentHealth + amount > MaxHealth) //if it would increase the players health above the max then set it to the max
                {
                    currentHealth = MaxHealth;
                }
                else
                    currentHealth += amount;
            }
            GetHealthBarLength();
        }

        public void Draw(SpriteBatch spriteBatch)
        //Draw the health and score of the player
        {
            spriteBatch.Draw(scoreboard, new Vector2(200, 5), Color.White);
            spriteBatch.Draw(healthBackground, backgroundHealthRect, backHealthBarColor);
            spriteBatch.Draw(currentHealthTex, healthRect, healthBarColor);
            spriteBatch.DrawString(font, "Score: " + String.Format("{0:D4}", score), new Vector2(205, 15), Color.White);
            spriteBatch.DrawString(font, "Health: " + currentHealth, new Vector2(425, 15), Color.White);
        }

        public void DrawHighScores(SpriteBatch spriteBatch)
        {//draw the highscores to the screen
            Vector2 highscorePos = new Vector2(highScoreRect.X + 20, highScoreRect.Y + 15); // position is slightly within the rectangle for the texture
            spriteBatch.Draw(highScoreTexture, highScoreRect, Color.White * 0.8f);
            spriteBatch.DrawString(bigFont, "HighScore Table: ", highscorePos, Color.SteelBlue);
            for (int i = 0; i < MaxHighScores; i++)
            {
                highscorePos.Y += 20; //increase the pos to move the next highscore down
                spriteBatch.DrawString(font, (i + 1).ToString() + ". " + highScoreNames[i] + ": " + highScores[i], highscorePos, Color.SteelBlue); 
            }

        }

        public void WriteTxt(StreamWriter outFile)
        { //write the highscores out to a text file
            for (int i = 0; i < MaxHighScores; i++)
            {
                outFile.Write(highScoreNames[i] + ",");
                outFile.Write(highScores[i].ToString());
                outFile.WriteLine();
            }
        }

        public void WriteXML(XmlTextWriter outFile)
        { //write the highscors out to a xml file
            outFile.WriteStartElement("HighScoreTable");

            for (int i = 0; i < MaxHighScores; i++)
            {
                outFile.WriteStartElement("HighScore");

                outFile.WriteStartElement("name");
                outFile.WriteString(highScoreNames[i]);
                outFile.WriteEndElement();

                outFile.WriteStartElement("score");
                outFile.WriteString(highScores[i].ToString());
                outFile.WriteEndElement();

                outFile.WriteEndElement();
            }
            outFile.WriteEndElement();
        }

        public void ReadXML(XmlTextReader inFile)
        { //read in the highscores from the xml file
            string elementName;
            int curHighScore = -1; //which highscore to read into

            while (inFile.Read())
            {
                elementName = "";
                XmlNodeType nType = inFile.NodeType;
                if (nType == XmlNodeType.Element) //if the node is an element node
                {
                    elementName = inFile.Name.ToString();
                }
                if (elementName == "HighScore")
                    curHighScore++;
                else if (elementName == "name")
                    highScoreNames[curHighScore] = inFile.ReadString();
                else if (elementName == "score")
                    highScores[curHighScore] = Convert.ToInt32(inFile.ReadString());
            }
        }

        public bool PlayerAlive
        {
            get { return playerAlive; }
        }

        public int Score
        //Property to get the score
        {
            get { return score; }
            set { score = value; }
        }
    }
}
