using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Pong_Arena
{
    public class PongArena : Game
    {
        private int player1Lives = 3;
        private int player2Lives = 3;

        private GraphicsDeviceManager graphics;
        private gameStates gameState;
        private SpriteBatch spriteBatch;
        private static Random rand = new Random();
        private List<DynamicObject> listDynamicObject = new List<DynamicObject>();
        private List<Object> listObjects = new List<Object>();

        static int ballSpeed = 10;
        static int screenwidth = 1600;
        static int screenheight = 900;
        private static Vector2 backGroundLocation = new Vector2(screenwidth / 2, screenheight / 2);
        private static Viewport viewPort = new Viewport(0, 0, screenwidth, screenheight);

        //Initialize -- Object dimension need to be even numbers
        private Object ball = new Object("ball", new Vector2(screenwidth / 2, screenheight / 2), new Vector2(rand.Next(screenwidth), rand.Next(screenheight)), 50, 50, ballSpeed);
        private Object paddle1 = new Object("Paddle1", new Vector2(140, screenheight / 2), Vector2.Zero, 40, 120, 18f);
        private Object paddle2 = new Object("Paddle2", new Vector2(screenwidth - 140, screenheight / 2), Vector2.Zero, 40, 120, 18f);

        //Lives
        private Object player1star1 = new Object("Star", new Vector2(40, 2 * screenheight / 6), Vector2.Zero, 30, 30, 0);
        private Object player1star2 = new Object("Star", new Vector2(40, 3 * screenheight / 6), Vector2.Zero, 30, 30, 0);
        private Object player1star3 = new Object("Star", new Vector2(40, 4 * screenheight / 6), Vector2.Zero, 30, 30, 0);

        private Object player2star1 = new Object("Star", new Vector2(screenwidth - 40, 2 * screenheight / 6), Vector2.Zero, 30, 30, 0);
        private Object player2star2 = new Object("Star", new Vector2(screenwidth - 40, 3 * screenheight / 6), Vector2.Zero, 30, 30, 0);
        private Object player2star3 = new Object("Star", new Vector2(screenwidth - 40, 4 * screenheight / 6), Vector2.Zero, 30, 30, 0);

        //Window borders
        private Object border1 = new Object(new Vector2(-500, -500), 500, (int)screenwidth + 1000);
        private Object border2 = new Object(new Vector2(-500, 0), (int)screenheight, 500);
        private Object border3 = new Object(new Vector2(-500, (int)screenheight), 500, (int)screenwidth + 1000);
        private Object border4 = new Object(new Vector2((int)screenwidth, 0), (int)screenheight, 500);

        private Object MainMenuBackground = new Object("Background MainMenu", backGroundLocation, Vector2.Zero, 1080, 1920, 0f); //main menu background
        private Object background = new Object("Background", new Vector2(800, 450), Vector2.Zero, 900, 1600, 0f); //Background maingame
        private Object gameOverBackgound1 = new Object("GameOver1", backGroundLocation, Vector2.Zero, 900, 1600, 0f); //Spawn gameover screen offscreen
        private Object gameOverBackground2 = new Object("GameOver2", backGroundLocation, Vector2.Zero, 900, 1600, 0f);

        private enum gameStates { INIT, START, INGAME, GAMEOVER, QUIT };

        [STAThread]
        static void Main()
        {
            PongArena game = new PongArena();
            game.Run();
        }

        protected PongArena()
        {
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);

            gameState = gameStates.INIT;
            SetupGameState();
            graphics.PreferredBackBufferWidth = screenwidth;
            graphics.PreferredBackBufferHeight = screenheight;

            //Borderless
            IntPtr brdrLss = this.Window.Handle;
            this.Window.Position = new Point(0, 0);
            var control = System.Windows.Forms.Control.FromHandle(brdrLss);
            var form = control.FindForm();
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        protected override void Initialize()
        {
            base.Initialize();
            ball.setBounceObjects(new List<Object>() { paddle1, paddle2, border1, border2, border3, border4 });


            paddle1.Rotate(Math.PI * 0.5); //Paddle1 rotation 90 degrees
            paddle2.Rotate(Math.PI * -0.5); //Paddle2 rotation 90 degrees (ccw)
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ///Loop through all DynamicObjects and set their Textures
            for (int i = 0; i < listDynamicObject.Count; i++)
            {
                listDynamicObject[i].setTexture(Content.Load<Texture2D>(listDynamicObject[i].getName()));
            }
            ///loop through all Objects
            for (int i = 0; i < listObjects.Count; i++)
            {
                if (listObjects[i].getName() != null)
                {
                    listObjects[i].setTexture(Content.Load<Texture2D>(listObjects[i].getName()));
                }
            }
            gameState = gameStates.START;
            SetupGameState();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            ///Loop through DynamicObjects to draw sprites
            for (int i = 0; i < listDynamicObject.Count; i++)
            {
                spriteBatch.Draw(listDynamicObject[i].getTexture(), listDynamicObject[i].getLocation(), listDynamicObject[i].GetSourceRectangle(), Color.White);
            }
            ///loop through Objects to draw sprites
            for (int i = 0; i < listObjects.Count; i++)
            {
                Object x = listObjects[i];
                if (x.getName() != null)
                {
                    spriteBatch.Draw(x.getTexture(), x.getLocation(), x.getSourceRectangle(), Color.White, (float)x.getRotation(), x.getOrigin(), 1, SpriteEffects.None, 1);
                }
            }
            spriteBatch.End();
        }

        protected override void Update(GameTime gameTime)
        {
            InputHandler();

            switch (gameState)
            {
                case gameStates.START:
                    break;
                case gameStates.INGAME:
                    GameStateInGame(gameTime);
                    break;
                case gameStates.GAMEOVER:
                    break;
                case gameStates.QUIT:
                    Exit();
                    break;
            }
        }

        /*************************************************************************************************************************************
        * GAME STATES
        ************************************************************************************************************************************/
        /*
  * Clears listObjects and listDynamicObject and adds the gameState related Objects and DynamicObjects to the lists.
  * This way all objects related to a gameState will be loaded in this gameState only, the rest will not exist in that gameState.
  */
        private void SetupGameState()
        {
            listObjects.Clear();
            switch (gameState)
            {
                //INIT is a the initial gameState and loads all objects into the game in the LoadContent func,
                //after these objects are loaded, the gameState will be set to START 
                case gameStates.INIT:
                    listObjects.Add(ball);
                    listObjects.Add(paddle1);
                    listObjects.Add(paddle2);
                    listObjects.Add(player1star1);
                    listObjects.Add(player1star2);
                    listObjects.Add(player1star3);
                    listObjects.Add(player2star1);
                    listObjects.Add(player2star2);
                    listObjects.Add(player2star3);
                    listObjects.Add(border1);
                    listObjects.Add(border2);
                    listObjects.Add(border3);
                    listObjects.Add(border4);
                    listObjects.Add(background);
                    listObjects.Add(gameOverBackgound1);
                    listObjects.Add(gameOverBackground2);
                    listObjects.Add(MainMenuBackground);
                    break;
                case gameStates.START:
                    listObjects.Add(MainMenuBackground);
                    break;
                case gameStates.INGAME:
                    listObjects.Add(background);
                    listObjects.Add(player1star1);
                    listObjects.Add(player1star2);
                    listObjects.Add(player1star3);
                    listObjects.Add(player2star1);
                    listObjects.Add(player2star2);
                    listObjects.Add(player2star3);
                    listObjects.Add(ball);
                    listObjects.Add(paddle1);
                    listObjects.Add(paddle2);
                    listObjects.Add(border1);
                    listObjects.Add(border2);
                    listObjects.Add(border3);
                    listObjects.Add(border4);
                    break;
                case gameStates.GAMEOVER:
                    if(player1Lives < 1)
                    {
                        listObjects.Add(gameOverBackground2);
                    }
                    else
                    {
                        listObjects.Add(gameOverBackgound1);
                    }
                    break;
                case gameStates.QUIT:
                    Exit();
                    break;
            }
        }

        private void GameStateMainMenu(GameTime gameTime)
        {

        }

        private void GameStateGameOver()
        {

        }

        private void GameStateInGame(GameTime gameTime)
        {
            ///Loop through animList and check if enough time has passed to update to the next frame
            for (int i = 0; i < listDynamicObject.Count; i++)
            {
                listDynamicObject[i].Update(gameTime);
            }
            for (int i = 0; i < listObjects.Count; i++)
            {
                listObjects[i].Update(gameTime);
            }
            //Check if ball hits one of the sides of the screen
            if (ball.CollidesWith(border2)) //Left border
            {
                player1Lives--;
                switch (player1Lives)
                {
                    case 2: listObjects.Remove(player1star3);
                        break;
                    case 1: listObjects.Remove(player1star2);
                        break;
                    case 0: listObjects.Remove(player1star1);
                        break;
                    default: 
                        break;
                }
                RespawnBall();
            }

            if (ball.CollidesWith(border4)) //Right border
            {
                player2Lives--;
                switch (player2Lives)
                {
                    case 2: listObjects.Remove(player2star3);
                        break;
                    case 1: listObjects.Remove(player2star2);
                        break;
                    case 0: listObjects.Remove(player2star1);
                        break;
                    default:
                        break;
                }
                RespawnBall();
            }

            //Increase angle based on position hit on paddle AND handle speedup
            if (ball.CollidesWith(paddle1))
            {
                Vector2 newDir = new Vector2(0, (ball.getOrigin().Y + ball.getLocation().Y - (paddle1.getOrigin().Y + paddle1.getLocation().Y)) / 2000);
                newDir.Normalize();
                newDir += ball.getDirection();
                newDir.Normalize();
                ball.setDirection(newDir);
                if (ball.getDirection().Y > 1) ball.setDirection(new Vector2(ball.getDirection().X, 1)); //set limit
                if (ball.getDirection().Y < -1) ball.setDirection(new Vector2(ball.getDirection().X, -1));
                ball.setSpeed(ball.getSpeed() + 1f);
            }
            if (ball.CollidesWith(paddle2))
            {
                Vector2 newDir = new Vector2(0, (ball.getOrigin().Y + ball.getLocation().Y - (paddle2.getOrigin().Y + paddle1.getLocation().Y)) / 1200);
                if (newDir.Y > 1) newDir.Normalize();
                newDir += ball.getDirection();
                newDir.Normalize();
                ball.setDirection(newDir);
                if (ball.getDirection().Y > 1) ball.setDirection(new Vector2(ball.getDirection().X, 1)); //set limit
                if (ball.getDirection().Y < -1) ball.setDirection(new Vector2(ball.getDirection().X, -1));
                ball.setSpeed(ball.getSpeed() + 1f);
            }

            ball.Rotate(Math.PI * 0.02); //Rotate the ball

            //if a player has 0 lifes left, change gameState to gameStates.GAMEOVER
            if (player2Lives == 0 || player1Lives == 0)
            {
                gameState = gameStates.GAMEOVER;
                SetupGameState();
            }
        }

        /*************************************************************************************************************************************
         * INPUT HANDLING
         ************************************************************************************************************************************/

        private void InputHandler()
        {
            //get current state of Keyboard
            KeyboardState state = Keyboard.GetState();

            //Not gameState related; can be called in any gameState
            if (state.IsKeyDown(Keys.Escape))
            {
                gameState = gameStates.QUIT;
            }

            //gameState START
            if(gameState == gameStates.START)
            {
                if(state.IsKeyDown(Keys.Space))
                {
                    gameState = gameStates.INGAME;
                    SetupGameState();
                }
            }

            //gameState INGAME
            if (gameState == gameStates.INGAME)
            {
                //Player 1
                if (state.IsKeyDown(Keys.W) && paddle1.getLocation().Y > 0 + paddle1.getSourceRectangle().Width / 2)
                {
                    paddle1.setDirection(new Vector2(0, -1)); //Move up

                }
                else if (state.IsKeyDown(Keys.S) && paddle1.getLocation().Y < screenheight - paddle1.getSourceRectangle().Width / 2)
                {
                    paddle1.setDirection(new Vector2(0, 1)); //Move down
                }
                else
                {
                    paddle1.setDirection(Vector2.Zero); //Reset movement
                }

                //Player 2
                if (state.IsKeyDown(Keys.Up) && (paddle2.getLocation().Y > 0 + paddle2.getSourceRectangle().Width / 2))
                {
                    paddle2.setDirection(new Vector2(0, -1));
                }
                else if (state.IsKeyDown(Keys.Down) && paddle2.getLocation().Y < screenheight - paddle2.getSourceRectangle().Width / 2)
                {
                    paddle2.setDirection(new Vector2(0, 1));
                }
                else
                {
                    paddle2.setDirection(new Vector2(0, 0));
                }
            }

            //gameState GAMEOVER
            if (gameState == gameStates.GAMEOVER)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    gameState = gameStates.INGAME;
                    ResetGame();
                    SetupGameState();
                }
            }
        }

        /*************************************************************************************************************************************
        * GAMEPLAY RELATED
        ************************************************************************************************************************************/
        
        private void ResetGame()
        {
            player1Lives = 3;
            player2Lives = 3;
            RespawnBall();
        }

        private void RespawnBall()
        {
            ball.setLocation(new Vector2(screenwidth / 2, screenheight / 2));
            Vector2 dir = new Vector2(-100 + rand.Next(100), -5 + rand.Next(10));
            ball.setSpeed(ballSpeed);
            dir.Normalize();
            ball.setDirection(dir);
        }
    }
}