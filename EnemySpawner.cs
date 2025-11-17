using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace group_12_assignment7
{
    public class EnemySpawner
    {
        public List<TieFighter> TieFighters { get; private set; } = new();
        private float spawnTimer = 0f;
        private float spawnInterval = 2f; // Spawn every 2 seconds
        private Random random = new Random();

        // Define 8 spawn points offscreen (x,y)
        private Vector2[] spawnPoints;

        private int screenWidth;
        private int screenHeight;

        public EnemySpawner(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            spawnPoints = new Vector2[]
            {
                new Vector2(-100, -100), // top-left
                new Vector2(screenWidth / 2, -100), // top-center
                new Vector2(screenWidth + 100, -100), // top-right
                new Vector2(screenWidth + 100, screenHeight / 2), // middle-right
                new Vector2(screenWidth + 100, screenHeight + 100), // bottom-right
                new Vector2(screenWidth / 2, screenHeight + 100), // bottom-center
                new Vector2(-100, screenHeight + 100), // bottom-left
                new Vector2(-100, screenHeight / 2) // middle-left
            };
        }

        public void Update(GameTime gameTime, Texture2D tieTexture, Texture2D explosionTexture, Texture2D laserTexture)
        {
            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (spawnTimer >= spawnInterval)
            {
                SpawnTieFighter(tieTexture, explosionTexture, laserTexture);
                spawnTimer = 0f;
            }

            // You can also update TieFighters here if desired
        }

        private void SpawnTieFighter(Texture2D tieTexture, Texture2D explosionTexture, Texture2D laserTexture)
        {
            // Pick a random spawn point from the 8 predefined positions
            Vector2 spawnPos = spawnPoints[random.Next(spawnPoints.Length)];

            TieFighter tie = new TieFighter(spawnPos, tieTexture, explosionTexture, laserTexture);
            TieFighters.Add(tie);
        }
    }
}



