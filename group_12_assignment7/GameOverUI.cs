using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace group_12_assignment7;

public class GameOverUI
{
    private SpriteFont _font;
    private int _screenWidth;
    private int _screenHeight;
    private bool _playerVictorious;
    private int _finalScore;
    
    public bool IsReturnToMenuPressed { get; private set; }
    public bool IsPlayAgainPressed { get; private set; }
    
    private Rectangle _playAgainButtonRect;
    private Rectangle _menuButtonRect;
    private bool _isPlayAgainHovered;
    private bool _isMenuHovered;

    public GameOverUI(SpriteFont font, int screenWidth, int screenHeight)
    {
        _font = font;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _playerVictorious = false;
        _finalScore = 0;
        IsReturnToMenuPressed = false;
        IsPlayAgainPressed = false;
        
        // Define button dimensions
        int buttonWidth = 180;
        int buttonHeight = 50;
        int spacing = 40;
        int centerX = screenWidth / 2;
        
        _playAgainButtonRect = new Rectangle(
            centerX - buttonWidth - spacing / 2,
            screenHeight - 150,
            buttonWidth,
            buttonHeight
        );
        
        _menuButtonRect = new Rectangle(
            centerX + spacing / 2,
            screenHeight - 150,
            buttonWidth,
            buttonHeight
        );
    }

    public void SetGameResult(bool victorious, int score)
    {
        _playerVictorious = victorious;
        _finalScore = score;
    }

    public void Update(KeyboardState keyboardState, MouseState mouseState)
    {
        IsReturnToMenuPressed = false;
        IsPlayAgainPressed = false;
        _isPlayAgainHovered = false;
        _isMenuHovered = false;
        
        // Check play again button
        if (_playAgainButtonRect.Contains(mouseState.Position))
        {
            _isPlayAgainHovered = true;
            if (mouseState.LeftButton == ButtonState.Pressed)
                IsPlayAgainPressed = true;
        }
        
        // Check menu button
        if (_menuButtonRect.Contains(mouseState.Position))
        {
            _isMenuHovered = true;
            if (mouseState.LeftButton == ButtonState.Pressed)
                IsReturnToMenuPressed = true;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw semi-transparent overlay
        DrawRectangle(spriteBatch, 0, 0, _screenWidth, _screenHeight, Color.Black * 0.8f);

        // Draw title
        string title = _playerVictorious ? "VICTORY!" : "GAME OVER";
        Color titleColor = _playerVictorious ? Color.LimeGreen : Color.Red;
        Vector2 titleSize = _font.MeasureString(title);
        Vector2 titlePosition = new Vector2(_screenWidth / 2 - titleSize.X / 2, 100);
        spriteBatch.DrawString(_font, title, titlePosition, titleColor, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);

        // Draw message
        string message = _playerVictorious ? "You defeated all Tie Fighters!" : "You were defeated...";
        Vector2 messageSize = _font.MeasureString(message);
        Vector2 messagePosition = new Vector2(_screenWidth / 2 - messageSize.X / 2, 250);
        spriteBatch.DrawString(_font, message, messagePosition, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);

        // Draw final score
        string scoreText = $"Final Score: {_finalScore}";
        Vector2 scoreSize = _font.MeasureString(scoreText);
        Vector2 scorePosition = new Vector2(_screenWidth / 2 - scoreSize.X / 2, 350);
        spriteBatch.DrawString(_font, scoreText, scorePosition, Color.Cyan, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);

        // Draw buttons
        DrawButton(spriteBatch, _playAgainButtonRect, "PLAY AGAIN", _isPlayAgainHovered);
        DrawButton(spriteBatch, _menuButtonRect, "MAIN MENU", _isMenuHovered);
    }

    private void DrawButton(SpriteBatch spriteBatch, Rectangle buttonRect, string text, bool isHovered)
    {
        Color buttonColor = isHovered ? Color.Cyan : Color.White;
        DrawRectangle(spriteBatch, buttonRect.X, buttonRect.Y, buttonRect.Width, buttonRect.Height, buttonColor);
        
        Vector2 textSize = _font.MeasureString(text);
        Vector2 textPosition = new Vector2(
            buttonRect.X + buttonRect.Width / 2 - textSize.X / 2,
            buttonRect.Y + buttonRect.Height / 2 - textSize.Y / 2
        );
        spriteBatch.DrawString(_font, text, textPosition, Color.Black);
    }

    private void DrawRectangle(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        spriteBatch.Draw(texture, new Rectangle(x, y, width, height), color);
    }
}