using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace group_12_assignment7;

public class HUDDisplay
{
    private SpriteFont _font;
    private int _screenWidth;
    private int _screenHeight;
    
    private int _currentScore;
    private int _playerHealth;

    public HUDDisplay(SpriteFont font, int screenWidth, int screenHeight)
    {
        _font = font;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _currentScore = 0;
        _playerHealth = 100;
    }

    public void UpdateScore(int score)
    {
        _currentScore = score;
    }

    public void UpdateHealth(int health)
    {
        _playerHealth = health;
    }

    // This method is no longer needed but kept for backward compatibility
    public void UpdateEnemiesDefeated(int count)
    {
        // Removed - Score now reflects enemy defeats
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int padding = 20;
        Color textColor = Color.White;

        // Draw score in top-left
        string scoreText = $"Score: {_currentScore}";
        spriteBatch.DrawString(_font, scoreText, new Vector2(padding, padding), textColor, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);

        // Draw health bar at the bottom
        DrawHealthBar(spriteBatch, padding, _screenHeight - 50, 300, 30, _playerHealth);
    }

    private void DrawHealthBar(SpriteBatch spriteBatch, float x, float y, float width, float height, int health)
    {
        // Background (red)
        DrawRectangle(spriteBatch, (int)x, (int)y, (int)width, (int)height, Color.DarkRed);

        // Health fill (green)
        float healthPercentage = health / 100f;
        DrawRectangle(spriteBatch, (int)x, (int)y, (int)(width * healthPercentage), (int)height, Color.LimeGreen);

        // Border (white)
        DrawRectangleBorder(spriteBatch, (int)x, (int)y, (int)width, (int)height, Color.White, 2);
    }

    private void DrawRectangle(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        spriteBatch.Draw(texture, new Rectangle(x, y, width, height), color);
    }

    private void DrawRectangleBorder(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color, int thickness)
    {
        Texture2D texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });

        // Top
        spriteBatch.Draw(texture, new Rectangle(x, y, width, thickness), color);
        // Bottom
        spriteBatch.Draw(texture, new Rectangle(x, y + height - thickness, width, thickness), color);
        // Left
        spriteBatch.Draw(texture, new Rectangle(x, y, thickness, height), color);
        // Right
        spriteBatch.Draw(texture, new Rectangle(x + width - thickness, y, thickness, height), color);
    }
}