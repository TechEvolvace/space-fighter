using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace group_12_assignment7;

public class HUDDisplay
{
    // Defines UI display fields
    private SpriteFont _font;
    private int _screenWidth;
    private int _screenHeight;
    
    // Defines Game stat tracking fields
    private int _currentScore;
    private int _playerHealth;

    public HUDDisplay(SpriteFont font, int screenWidth, int screenHeight)
    {
        // Initialize HUD with font and screen dimensions
        _font = font;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _currentScore = 0;
        _playerHealth = 100;
    }

    public void UpdateScore(int score)
    {
        // Update current score value for display
        _currentScore = score;
    }

    public void UpdateHealth(int health)
    {
        // Update current health value for bar display
        _playerHealth = health;
    }

    public void UpdateEnemiesDefeated(int count)
    {
        // Placeholder - enemies defeated is reflected in score now
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw all HUD elements on screen
        int padding = 20;
        Color textColor = Color.White;

        // Draw score text in top-left corner
        string scoreText = $"Score: {_currentScore}";
        spriteBatch.DrawString(_font, scoreText, new Vector2(padding, padding), textColor, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);

        // Draw health bar at the bottom of screen
        DrawHealthBar(spriteBatch, padding, _screenHeight - 50, 300, 30, _playerHealth);
    }

    private void DrawHealthBar(SpriteBatch spriteBatch, float x, float y, float width, float height, int health)
    {
        // Draw background (dark red/red)
        DrawRectangle(spriteBatch, (int)x, (int)y, (int)width, (int)height, Color.DarkRed);

        // Draw health fill based on percentage (green)
        float healthPercentage = health / 100f;
        DrawRectangle(spriteBatch, (int)x, (int)y, (int)(width * healthPercentage), (int)height, Color.LimeGreen);

        // Draw white border around health bar
        DrawRectangleBorder(spriteBatch, (int)x, (int)y, (int)width, (int)height, Color.White, 2);
    }

    private void DrawRectangle(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
    {
        // Create and draw a solid color rectangle
        Texture2D texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        spriteBatch.Draw(texture, new Rectangle(x, y, width, height), color);
    }

    private void DrawRectangleBorder(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color, int thickness)
    {
        // Create texture for drawing
        Texture2D texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });

        // Draw top edge
        spriteBatch.Draw(texture, new Rectangle(x, y, width, thickness), color);

        // Draw bottom edge
        spriteBatch.Draw(texture, new Rectangle(x, y + height - thickness, width, thickness), color);

        // Draw left edge
        spriteBatch.Draw(texture, new Rectangle(x, y, thickness, height), color);

        // Draw right edge
        spriteBatch.Draw(texture, new Rectangle(x + width - thickness, y, thickness, height), color);
    }
}