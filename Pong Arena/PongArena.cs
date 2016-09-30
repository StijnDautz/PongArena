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
        private GraphicsDeviceManager graphics;
        private gameStates gameState;
        private SpriteBatch spriteBatch;
        Viewport viewPort = new Viewport(new Rectangle(0, 0, 1280, 720));
        private List<DynamicObject> listDynamicObject = new List<DynamicObject>();
        private List<Object> listObjects = new List<Object>();
        static float screenwidth = 1280;
        static float screenheight = 720;
        //Initialize -- Object dimension need to be even numbers
        private Object[] arrayObjectAll =
        {
            new Object("ball", new Vector2(100, 100), new Vector2(400, 350),38, 40, 20),
            new Object("paddle1", new Vector2(400, 400), new Vector2(400, 400), 26, 100, 0),
            new Object("ball", new Vector2(0, 100), new Vector2(400, 350),38, 40, 14),
            new Object(new Vector2(-200, -200), (int)screenheight + 400, 200),
            new Object(new Vector2(-200, -200), 200, (int)screenwidth + 400),
            new Object(new Vector2(screenwidth, -200), (int)screenheight + 400, 200),
            new Object(new Vector2(-200, screenheight), 200, (int)screenwidth + 200),
        };
        private DynamicObject[] arrayDynamicObjectAll =
        {

        };

        private enum gameStates { MAINMENU, INGAME, QUIT };

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
            graphics.PreferredBackBufferWidth = 2560;
            graphics.PreferredBackBufferHeight = 1440;

            //adding Objects and Dynamic Objects to load
            listObjects.Add(arrayObjectAll[0]);
            listObjects.Add(arrayObjectAll[1]);
            listObjects.Add(arrayObjectAll[2]);
            listObjects.Add(arrayObjectAll[3]);
            listObjects.Add(arrayObjectAll[4]);
            listObjects.Add(arrayObjectAll[5]);
            listObjects.Add(arrayObjectAll[6]);
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
                case gameStates.QUIT:
                    Exit();
                    break;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            arrayObjectAll[0].setBounceObjects(new List<Object>() { arrayObjectAll[1], arrayObjectAll[3], arrayObjectAll[4], arrayObjectAll[5], arrayObjectAll[6] });
            arrayObjectAll[2].setBounceObjects(new List<Object>() { arrayObjectAll[1], arrayObjectAll[3], arrayObjectAll[4], arrayObjectAll[5], arrayObjectAll[6] });
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
            for (int i = 0; i < arrayObjectAll.Length; i++)
            {
                if (arrayObjectAll[i].getName() != null)
                {
                    arrayObjectAll[i].setTexture(Content.Load<Texture2D>(arrayObjectAll[i].getName()));
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

            arrayObjectAll[1].Rotate(Math.PI * 0.008);

            //perform actions based on input
            InputHandler();
        }

        /*************************************************************************************************************************************
         * INPUT HANDLING
         ************************************************************************************************************************************/

        private void InputHandler()
        {
            KeyboardState state = Keyboard.GetState();
        }
    }
}