using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System; // Needed for Math.Atan2 for rotation

namespace group_12_assignment7;

public class Laser
{
    // *** PUBLIC FIELDS (Used by Game1 for checking collisions) ***
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; private set; }
    public Texture2D Texture { get; private set; }
    public bool IsFriendly { get; private set; } // True if fired by Falcon, False if fired by TieFighter
    public bool IsActive { get; set; } = true;

    // Defines the area used for collision checks
    public Rectangle BoundingBox
    {
        get { return new Rectangle((int)Position.X - Texture.Width / 2, (int)Position.Y - Texture.Height / 2, Texture.Width, Texture.Height); }
    }

    // *** CONSTRUCTOR ***
    public Laser(Texture2D texture, Vector2 position, Vector2 velocity, bool isFriendly)
    {
        this.Texture = texture;
        // Start the laser slightly offset to launch from the center of the ship
        this.Position = position; 
        this.Velocity = velocity;
        this.IsFriendly = isFriendly;
    }

    // *** UPDATE METHOD ***
    public void Update(GameTime gameTime)
    {
        if (!IsActive) return;

        // Move the laser based on its velocity and the time elapsed (delta time)
        Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Check if the laser has moved off-screen (Cleanup)
        // Assuming a standard screen size (e.g., 1280x720) with a buffer
        if (Position.X < -50 || Position.X > 1330 || Position.Y < -50 || Position.Y > 770)
        {
            IsActive = false;
        }
    }
    private const float SCALE = 1f / 50f;
    // *** DRAW METHOD ***
    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;
        
        // Calculate the rotation angle based on the direction of travel
        float rotation = (float)Math.Atan2(Velocity.Y, Velocity.X);
        
        // Draw the texture, using the rotation and the center of the texture as the origin
        spriteBatch.Draw(Texture, 
                         Position, 
                         null, 
                         Color.White, 
                         rotation, 
                         new Vector2(Texture.Width / 2, Texture.Height / 2), 
                         SCALE, 
                         SpriteEffects.None, 
                         0f);
    }
}