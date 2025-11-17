using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace group_12_assignment7
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D falconTexture;
        private Texture2D tieTexture;
        private Texture2D explosionTexture;
        private Texture2D laserTexture;

        private Vector2 falconPosition;
        private float falconSpeed = 200f;
        private float falconScale = 0.15f;

        private EnemySpawner spawner;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            falconPosition = new Vector2(400, 300);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            falconTexture = Content.Load<Texture2D>("imgs/MilleniumFalcon");
            tieTexture = Content.Load<Texture2D>("imgs/TieFighter");
            explosionTexture = Content.Load<Texture2D>("imgs/explosion");

            laserTexture = new Texture2D(GraphicsDevice, 1, 1);
            laserTexture.SetData(new[] { Color.White });

            spawner = new EnemySpawner(2f, tieTexture, explosionTexture, laserTexture, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.W)) falconPosition.Y -= falconSpeed * dt;
            if (kb.IsKeyDown(Keys.S)) falconPosition.Y += falconSpeed * dt;
            if (kb.IsKeyDown(Keys.A)) falconPosition.X -= falconSpeed * dt;
            if (kb.IsKeyDown(Keys.D)) falconPosition.X += falconSpeed * dt;

            spawner.Update(gameTime, falconPosition, falconTexture, falconScale);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _spriteBatch.Draw(falconTexture, falconPosition, null, Color.White, 0f,
                Vector2.Zero, falconScale, SpriteEffects.None, 0f);

            spawner.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}



