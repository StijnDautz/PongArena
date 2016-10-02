using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Pong_Arena
{
    public class PongArena : Game
    {
        private int player1Lives = 3;
        private int player2Lives = 3;

        private GraphicsDeviceManager graphics;
        private gameStates gameState;
        private SpriteBatch spriteBatch;
        private static  Random rand = new Random();
        Viewport viewPort = new Viewport(new Rectangle(0, 0, 1600, 900));
        private List<DynamicObject> listDynamicObject = new List<DynamicObject>();
        private List<Object> listObjects = new List<Object>();
        static float screenwidth = 1600;
        static float screenheight = 900;
        //Initialize -- Object dimension need to be even numbers
        private Object ball = new Object("ball", new Vector2(screenwidth / 2, screenheight / 2), new Vector2(-200 + rand.Next(400), -200 + rand.Next(400)), 50, 50, 10);
        private Object paddle1 = new Object("Paddle1", new Vector2(100, (int)screenheight / 2), Vector2.Zero, 40, 120, 10f);
        private Object paddle2 = new Object("Paddle2", new Vector2((int)screenwidth - 140, (int)screenheight / 2), Vector2.Zero, 40, 120, 10f);

        //Lives
        private Object player1star1 = new Object("Star", new Vector2(40, 2* screenheight / 6), Vector2.Zero, 30, 30, 0);
        private Object player1star2 = new Object("Star", new Vector2(40, 3 * screenheight / 6), Vector2.Zero, 30, 30, 0);
        private Object player1star3 = new Object("Star", new Vector2(40, 4 * screenheight / 6), Vector2.Zero, 30, 30, 0);

        private Object player2star1 = new Object("Star", new Vector2(screenwidth - 40, 2 * screenheight / 6), Vector2.Zero, 30, 30, 0);
        private Object player2star2 = new Object("Star", new Vector2(screenwidth - 40, 3 * screenheight / 6), Vector2.Zero, 30, 30, 0);
        private Object player2star3 = new Object("Star", new Vector2(screenwidth - 40, 4 * screenheight / 6), Vector2.Zero, 30, 30, 0);


        //Window borders
        private Object border1 = new Object(new Vector2(-300, -300), 300, (int)screenwidth + 600);
        private Object border2 = new Object(new Vector2(-300, 0), (int)screenheight, 300);
        private Object border3 = new Object(new Vector2(-300, (int)screenheight), 300, (int)screenwidth + 600);
        private Object border4 = new Object(new Vector2((int)screenwidth, 0), (int)screenheight, 300);

        private Object background = new Object("Background", new Vector2(800, 450), Vector2.Zero, 900, 1600, 0f); //Background maingame

        private Object player1scored = new Object("Player1Scored", new Vector2(-600, screenheight / 2), new Vector2(screenwidth, screenheight / 2), 150, 550, 0);
        private Object player2scored = new Object("Player2Scored", new Vector2(-600, screenheight / 2), new Vector2(screenwidth, screenheight / 2), 150, 550, 0);
            
        private Object[] arrayObjectAll =
        {
            
            
        };
        private DynamicObject[] arrayDynamicObjectAll =
        {

        };

        private enum gameStates { MAINMENU, INGAME, GAMEOVER, QUIT };

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
            
            gameState = gameStates.INGAME;
            graphics.PreferredBackBufferWidth = (int)screenwidth;
            graphics.PreferredBackBufferHeight = (int)screenheight;
            

            //Borderless
            IntPtr brdrLss = this.Window.Handle;
            this.Window.Position = new Point((int)((1920 - screenwidth) / 2), (int)((1080 - screenheight) /2));
            var control = System.Windows.Forms.Control.FromHandle(brdrLss);
            var form = control.FindForm();
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
           

            //adding Objects and Dynamic Objects to load
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

            listObjects.Add(player1scored);
            listObjects.Add(player2scored);
            
        }

        protected override void Update(GameTime gameTime)
        {
            switch (gameState)
            {
                case gameStates.MAINMENU:
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

            //Check if ball hits one of the sides of the screen
            if (ball.CollidesWith(border2))
            {
                player1Lives--;
                switch (player1Lives)
                {
                    case 2: { listObjects.Remove(player1star3); /*listObjects.Remove(ball);*/ listObjects.Add(player2scored); player2scored.setSpeed(5); break; };
                    case 1: { listObjects.Remove(player1star2); /*listObjects.Remove(ball);*/ player2scored.setLocation(new Vector2(-550, screenheight / 2)); listObjects.Add(player2scored); player2scored.setSpeed(5); break; };
                    case 0: { listObjects.Remove(player1star1); /*listObjects.Remove(ball);*/ player2scored.setLocation(new Vector2(-550, screenheight / 2)); listObjects.Add(player2scored); player2scored.setSpeed(5); break; };
                    default: break;
                }
            }

            //During point screen
            if (player1scored.getLocation().X > screenwidth) { listObjects.Remove(player1scored); /*listObjects.Add(ball);*/ }
            if (player2scored.getLocation().X > screenwidth) { listObjects.Remove(player2scored); /*listObjects.Add(ball);*/ }

            if (ball.CollidesWith(border4))
            {
                player2Lives--;
                switch (player2Lives)
                {
                    case 2: { listObjects.Remove(player2star3); listObjects.Remove(ball); listObjects.Add(player1scored); player1scored.setSpeed(5); break; };
                    case 1: { listObjects.Remove(player2star2); listObjects.Remove(ball); player1scored.setLocation(new Vector2(-550, screenheight / 2)); listObjects.Add(player1scored); player1scored.setSpeed(5); break; };
                    case 0: { listObjects.Remove(player2star1); listObjects.Remove(ball); player1scored.setLocation(new Vector2(-550, screenheight / 2)); listObjects.Add(player1scored); player1scored.setSpeed(5); break; };
                    default: break;
                }
            }

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
            for (int i = 0; i < arrayDynamicObjectAll.Length; i++)
            {
                arrayDynamicObjectAll[i].setTexture(Content.Load<Texture2D>(arrayDynamicObjectAll[i].getName()));
            }
            ///loop through all Objects
            for (int i = 0; i < listObjects.Count; i++)
            {
                if (listObjects[i].getName() != null)
                {
                    listObjects[i].setTexture(Content.Load<Texture2D>(listObjects[i].getName()));
                }
            }
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

        /*************************************************************************************************************************************
        * GAME STATES
        ************************************************************************************************************************************/
        private void GameStateMainMenu(GameTime gameTime)
        {

        }

        private void GameStateInGame(GameTime gameTime)
        {
            ///Loop through animList and check if enough time has passed to update to the next frame
            for (int i = 0; i < listDynamicObject.Count; i++)
            {
                listDynamicObject[i].Update(gameTime);
            }
            for(int i = 0; i < listObjects.Count; i++)
            {
                listObjects[i].Update(gameTime);
            }

            
            ball.Rotate(Math.PI * 0.02); //Rotate the ball
            //perform actions based on input
            InputHandler();
        }

        /*************************************************************************************************************************************
         * INPUT HANDLING
         ************************************************************************************************************************************/

        private void InputHandler()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)) gameState = gameStates.QUIT;


            //Player 1
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W) && paddle1.getLocation().Y > 0 + paddle1.getSourceRectangle().Width / 2)
            {
                paddle1.setDirection(new Vector2(0, -1)); //Move up
                
            }
            else if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S) && paddle1.getLocation().Y < screenheight - paddle1.getSourceRectangle().Width / 2)
            {
                paddle1.setDirection(new Vector2(0, 1)); //Move down
            }
            else
            {
                paddle1.setDirection(Vector2.Zero); //Reset movement
            } 

            //Player 2
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up) && (paddle2.getLocation().Y > 0 + paddle2.getSourceRectangle().Width / 2))
            {
                paddle2.setDirection(new Vector2(0, -1));
            }
            else if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down) && paddle2.getLocation().Y < screenheight - paddle2.getSourceRectangle().Width / 2)
            {
                paddle2.setDirection(new Vector2(0, 1));
            }
            else
            {
                paddle2.setDirection(new Vector2(0,0));
            }
        }
    }
}