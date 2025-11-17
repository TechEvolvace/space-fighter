using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace group_12_assignment7
{
    public class EnemySpawner
    {
        public List<TieFighter> Enemies = new List<TieFighter>();

        private Texture2D tieTexture;
        private Texture2D explosionTexture;
        private Texture2D laserTexture;

        private float spawnInterval;
        private float timeSinceLastSpawn = 0f;

        private Random rand = new Random();

        private GraphicsDevice graphicsDevice;

        public EnemySpawner(float spawnInterval, Texture2D tieTexture, Texture2D explosionTexture, Texture2D laserTexture, GraphicsDevice graphicsDevice)
        {
            this.spawnInterval = spawnInterval;
            this.tieTexture = tieTexture;
            this.explosionTexture = explosionTexture;
            this.laserTexture = laserTexture;
            this.graphicsDevice = graphicsDevice;
        }

        public void Update(GameTime gameTime, Vector2 falconPosition, Texture2D falconTexture, float falconScale)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeSinceLastSpawn += dt;

            if (timeSinceLastSpawn >= spawnInterval)
            {
                SpawnEnemy();
                timeSinceLastSpawn = 0f;
            }

            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                var enemy = Enemies[i];
                enemy.Update(gameTime, falconPosition, falconTexture, falconScale);
                if (!enemy.IsAlive && !enemy.IsExploding())
                {
                    Enemies.RemoveAt(i);
                }
            }
        }

        private void SpawnEnemy()
            {
                int screenWidth = graphicsDevice.Viewport.Width;
                int screenHeight = graphicsDevice.Viewport.Height;
                int buffer = 100;

                Vector2[] spawnPositions = new Vector2[]
                {
                    new Vector2(screenWidth / 4, -buffer),
                    new Vector2(3 * screenWidth / 4, -buffer),
                    new Vector2(screenWidth / 4, screenHeight + buffer),
                    new Vector2(3 * screenWidth / 4, screenHeight + buffer),
                    new Vector2(-buffer, screenHeight / 4),
                    new Vector2(-buffer, 3 * screenHeight / 4),
                    new Vector2(screenWidth + buffer, screenHeight / 4),
                    new Vector2(screenWidth + buffer, 3 * screenHeight / 4),
                };

                int spawnIndex = rand.Next(spawnPositions.Length);
                Vector2 spawnPos = spawnPositions[spawnIndex];

                TieFighter newEnemy = new TieFighter(spawnPos, tieTexture, explosionTexture, laserTexture);
                Enemies.Add(newEnemy);
            }

        public void Draw(SpriteBatch spriteBatch)
            {
                foreach (var enemy in Enemies)
                {
                    enemy.Draw(spriteBatch);
                }
            }
    }
}
