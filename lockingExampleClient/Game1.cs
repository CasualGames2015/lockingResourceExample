using Microsoft.AspNet.SignalR.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sprites;
using System.Collections.Generic;

namespace lockingExampleClient
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        IHubProxy proxy;
        HubConnection connection;
        List<SimpleSprite> _collectables = new List<SimpleSprite>();

        public bool Started = false;
        private SpriteFont font;
        private bool Ended;

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
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            connection = new HubConnection("http://localhost:5366/");
            proxy = connection.CreateHubProxy("exclusiveGameHub");

            connection.StateChanged += Connection_StateChanged;

            // Deal with the Message when a client joins the game
            proxy.On<int, int>("createOpponentCollectable", (x, y) =>
            {
                _collectables.Add(new SimpleSprite(
                    Content.Load<Texture2D>("collectable"), new Vector2(x, y)));
            });
            // Message recieved for removing a collectable from the collection in this client
            // It has alread been removed form the calling client which gave rise to this message
            proxy.On<int, int>("removed", (x, y) =>
            {
                // Safe pattern for retrieving the one to be removed
              var removed =  _collectables.Find(c => (int)c.Position.X == x && c.Position.Y == y);
                // if it's right remove it
                if (removed != null)
                    _collectables.Remove(removed);
            });

            proxy.On("end", () =>
            {
                // Can't access Exit() method directly as that would be cross thread
                Ended = true;
            });

            connection.Start();
            base.Initialize();
        }

       

        private void Connection_StateChanged(StateChange State)
        {
            // Wait for a connection and start if we have connected
            if(State.NewState == ConnectionState.Connected )
            {
                Started = true;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // if all the collectables are collected then the server will send a message and Ended will be true
            if (Ended) this.Exit();
            
            // Check for Mouse clicks
            foreach (var item in _collectables)
                item.Update(gameTime);

            // if for newly clicked collectable(s)
            var deleted = _collectables.FindAll(c => c.Visible == false);
            // safe delete from the collectables in this thread
            // Note you cannot remove from a generic collection (Lists, stacks, Dictionaries etc) while you are iterating 
            // remove the items marked for deletion NOTE: visible used here
            foreach (var item in deleted)
            { 
                // Safely remove in this thread
                _collectables.Remove(item);
                // Tell the server to remove that one from it's own collection and in turn tell all the other clients to remove it
                // from their collections
                proxy.Invoke("remove", new object[] { (int)item.Position.X, (int)item.Position.Y });
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            if (!Started)
                spriteBatch.DrawString(font, "Waiting for Server Connection",new Vector2(10,10),Color.White);

            // TODO: Add your drawing code here
            foreach (var item in _collectables)
                {
                    item.draw(spriteBatch);
                }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
