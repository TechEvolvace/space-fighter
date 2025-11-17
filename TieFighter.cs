using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace group_12_assignment7
{
    public class EnemyLaser
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public const float SPEED = 6f;
    }

    public class TieFighter
{
    public Vector2 Position;
    public Texture2D Texture;
    private Texture2D explosionTexture;
    private Texture2D laserTexture;

    public bool IsAlive = true;
    private bool isExploding = false;

    private List<EnemyLaser> EnemyLasers = new();

    private float moveSpeed = 90f;
    private float shootCooldown = 1.2f;
    private float timeSinceLastShot = 0f;

    private float scale = 0.05f;

    private float explosionTimer = 0f;
    private float explosionDuration = 0.75f;

    public TieFighter(Vector2 position, Texture2D texture, Texture2D explosionTexture, Texture2D laserTexture)
        {
            Position = position;
            Texture = texture;
            this.explosionTexture = explosionTexture;
            this.laserTexture = laserTexture;
        }

        public void Update(GameTime gameTime, Vector2 falconPosition, Texture2D falconTexture, float falconScale)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (isExploding)
        {
            explosionTimer += dt;
            scale *= 1.03f;
            if (explosionTimer >= explosionDuration)
            {
                IsAlive = false;
            }
            UpdateLasers();
            return;
        }

        Vector2 falconCenter = falconPosition + new Vector2(
            falconTexture.Width * falconScale / 2f,
            falconTexture.Height * falconScale / 2f
        );

        Vector2 tieCenter = Position + new Vector2(Texture.Width * scale / 2f, Texture.Height * scale / 2f);

        Vector2 direction = falconCenter - tieCenter;
        if (direction.Length() > 0)
        {
            direction.Normalize();
            tieCenter += direction * moveSpeed * dt;
            Position = tieCenter - new Vector2(Texture.Width * scale / 2f, Texture.Height * scale / 2f);
        }

        Rectangle tieRect = new Rectangle((int)Position.X, (int)Position.Y,
            (int)(Texture.Width * scale), (int)(Texture.Height * scale));
        Rectangle falconRect = new Rectangle((int)falconPosition.X, (int)falconPosition.Y,
            (int)(falconTexture.Width * falconScale), (int)(falconTexture.Height * falconScale));

        if (tieRect.Intersects(falconRect))
        {
            Explode();
        }

        timeSinceLastShot += dt;
        if (timeSinceLastShot >= shootCooldown)
        {
            Shoot(falconCenter);
            timeSinceLastShot = 0f;
        }

        UpdateLasers();
    }
    private void Explode()
    {
        isExploding = true;
        explosionTimer = 0f;
    }

    private void Shoot(Vector2 targetPosition)
    {
        Vector2 tieCenter = Position + new Vector2(Texture.Width * scale / 2f, Texture.Height * scale / 2f);
        Vector2 dir = targetPosition - tieCenter;
        dir.Normalize();

        EnemyLaser laser = new EnemyLaser
        {
            Position = tieCenter,
            Velocity = dir * EnemyLaser.SPEED
        };

        EnemyLasers.Add(laser);
    }

    private void UpdateLasers()
    {
        for (int i = EnemyLasers.Count - 1; i >= 0; i--)
        {
            EnemyLaser l = EnemyLasers[i];
            l.Position += l.Velocity;

            if (l.Position.X < -50 || l.Position.X > 2000 ||
                l.Position.Y < -50 || l.Position.Y > 1200)
            {
                EnemyLasers.RemoveAt(i);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
        {
            if (isExploding && explosionTimer < explosionDuration)
            {
                Rectangle tieRect = new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    (int)(explosionTexture.Width * scale),
                    (int)(explosionTexture.Height * scale)
                );
                spriteBatch.Draw(explosionTexture, tieRect, Color.White);
            }
            else if (IsAlive)
            {
                Rectangle tieRect = new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    (int)(Texture.Width * scale),
                    (int)(Texture.Height * scale)
                );
                spriteBatch.Draw(Texture, tieRect, Color.White);
            }

            foreach (var laser in EnemyLasers)
            {
                Rectangle laserRect = new Rectangle((int)laser.Position.X, (int)laser.Position.Y, 6, 6);
                spriteBatch.Draw(laserTexture, laserRect, Color.Red);
            }
        }

    public bool IsExploding()
        {
            return isExploding;
        }
    }
}