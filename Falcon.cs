using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace group_12_assignment7;

public class Falcon
{
    // *** PUBLIC FIELDS (Accessible by Game1 for collision/HUD) ***
    public Vector2 Position { get; set; }
    public int Health { get; set; } = 100;
    public Texture2D Texture { get; private set; }
    public Texture2D LaserTexture { get; private set; } // Texture for the laser projectiles
    public List<Laser> Lasers { get; private set; } = new List<Laser>();

    // Bounding Box for Collision Detection
    public Rectangle BoundingBox
    {
        // Adjust the offset (e.g., / 4) if the texture has significant empty space
        get { return new Rectangle((int)Position.X - Texture.Width / 2, (int)Position.Y - Texture.Height / 2, Texture.Width, Texture.Height); }
    }

    // *** PRIVATE FIELDS ***
    private float shootCooldownTimer = 0.0f;
    private const float SHOOT_COOLDOWN = 0.25f; // Fires 4 times per second
    private const float MOVEMENT_SPEED = 400.0f; // Pixels per second
    private GraphicsDevice _graphicsDevice;


    // *** CONSTRUCTOR ***
    // Requires the Falcon texture and the Laser texture
    public Falcon(Texture2D falconTexture, Texture2D laserTexture, Vector2 startPosition, GraphicsDevice graphicsDevice)
    {
        Texture = falconTexture;
        LaserTexture = laserTexture;
        Position = startPosition;
        _graphicsDevice = graphicsDevice;
    }

    // *** UPDATE METHOD ***
    public void Update(GameTime gameTime, KeyboardState kState, MouseState mState)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        shootCooldownTimer -= delta;

        // 1. Handle Movement (WASD)
        HandleMovement(kState, delta);

        // 2. Handle Shooting (Mouse Click)
        HandleShooting(mState);

        // 3. Update all active lasers and remove inactive ones
        for (int i = Lasers.Count - 1; i >= 0; i--)
        {
            Lasers[i].Update(gameTime);
            if (!Lasers[i].IsActive)
            {
                Lasers.RemoveAt(i);
            }
        }
    }

    // Helper method to implement the 4 movement inputs
    private void HandleMovement(KeyboardState kState, float delta)
        {
            Vector2 direction = Vector2.Zero;

            if (kState.IsKeyDown(Keys.W)) direction.Y -= 1;
            if (kState.IsKeyDown(Keys.S)) direction.Y += 1;
            if (kState.IsKeyDown(Keys.A)) direction.X -= 1;
            if (kState.IsKeyDown(Keys.D)) direction.X += 1;

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                Position += direction * MOVEMENT_SPEED * delta;
            }

            // Clamp to current window size
            int width = Texture.Width / 2;
            int height = Texture.Height / 2;
            int screenWidth = _graphicsDevice.Viewport.Width;
            int screenHeight = _graphicsDevice.Viewport.Height;

            Position = Vector2.Clamp(Position,
                                    new Vector2(width, height),
                                    new Vector2(screenWidth - width, screenHeight - height));
        }



    // Helper method to implement the shooting input
    private void HandleShooting(MouseState mState)
    {
        if (mState.LeftButton == ButtonState.Pressed && shootCooldownTimer <= 0)
        {
            // The shooting target is the mouse cursor position
            Shoot(new Vector2(mState.X, mState.Y));
            shootCooldownTimer = SHOOT_COOLDOWN;
        }
    }

    // Creates a new Laser object and adds it to the list
    private void Shoot(Vector2 targetPosition)
    {
        // Get the direction vector from the Falcon's current position to the mouse target
        Vector2 direction = targetPosition - Position;

        if (direction != Vector2.Zero)
        {
            direction.Normalize();
            // Create a new laser (Friendly: true, speed: 800.0f)
            Lasers.Add(new Laser(LaserTexture, Position, direction * 800.0f, true));
        }
    }

    // Reduces health when hit by an enemy laser or collision
    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
    private const float SCALE = 0.25f;
    // *** DRAW METHOD ***
    public void Draw(SpriteBatch spriteBatch)
    {

        spriteBatch.Draw(Texture,
                         Position,
                         null,
                         Color.White,
                         0f, // No rotation needed for this top-down view
                         new Vector2(Texture.Width / 2, Texture.Height / 2), // Origin is center
                         SCALE,
                         SpriteEffects.None,
                         0f);

        foreach (var laser in Lasers)
        {
            laser.Draw(spriteBatch);
        }
    }
}