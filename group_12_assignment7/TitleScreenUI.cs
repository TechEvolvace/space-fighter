using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace group_12_assignment7;

public class TitleScreenUI
{
    private SpriteFont _font;
    private int _screenWidth;
    private int _screenHeight;
    public bool IsStartButtonPressed { get; private set; }
    public bool IsHighScoresButtonPressed { get; private set; }
    
    private Rectangle _startButtonRect;
    private Rectangle _highScoresButtonRect;
    private bool _isStartButtonHovered;
    private bool _isHighScoresButtonHovered;
    
    private const float TITLE_SCALE = 3.5f; // Increased from 2f
    private const int BUTTON_WIDTH = 200;
    private const int BUTTON_HEIGHT = 60;
    private const int BUTTON_CORNER_RADIUS = 15; // For rounded corners

    public TitleScreenUI(SpriteFont font, int screenWidth, int screenHeight)
    {
        _font = font;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        IsStartButtonPressed = false;
        IsHighScoresButtonPressed = false;
        _isStartButtonHovered = false;
        _isHighScoresButtonHovered = false;
        
        // Define button dimensions - positioned side by side
        int centerX = screenWidth / 2;
        int buttonSpacing = 50;
        int buttonAreaY = screenHeight - 150;
        
        // START button (left)
        _startButtonRect = new Rectangle(
            centerX - buttonSpacing / 2 - BUTTON_WIDTH,
            buttonAreaY,
            BUTTON_WIDTH,
            BUTTON_HEIGHT
        );
        
        // HIGH SCORES button (right)
        _highScoresButtonRect = new Rectangle(
            centerX + buttonSpacing / 2,
            buttonAreaY,
            BUTTON_WIDTH,
            BUTTON_HEIGHT
        );
    }

    public void Update(KeyboardState keyboardState, MouseState mouseState)
    {
        IsStartButtonPressed = false;
        IsHighScoresButtonPressed = false;
        _isStartButtonHovered = false;
        _isHighScoresButtonHovered = false;
        
        // Check START button
        if (_startButtonRect.Contains(mouseState.Position))
        {
            _isStartButtonHovered = true;
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                IsStartButtonPressed = true;
            }
        }
        
        // Check HIGH SCORES button
        if (_highScoresButtonRect.Contains(mouseState.Position))
        {
            _isHighScoresButtonHovered = true;
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                IsHighScoresButtonPressed = true;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw semi-transparent background
        spriteBatch.Draw(
            texture: CreateFilledRectangle(spriteBatch.GraphicsDevice, _screenWidth, _screenHeight, Color.Black),
            position: Vector2.Zero,
            color: Color.White * 0.7f
        );

        // Draw title - LARGER and properly centered with scale
        string title = "SPACE FIGHTER";
        Vector2 titleSize = _font.MeasureString(title);
        // Account for scale when centering
        Vector2 titlePosition = new Vector2(
            _screenWidth / 2 - (titleSize.X * TITLE_SCALE) / 2,
            80
        );
        spriteBatch.DrawString(_font, title, titlePosition, Color.Cyan, 0f, Vector2.Zero, TITLE_SCALE, SpriteEffects.None, 0f);

        // Draw START button with rounded corners
        DrawRoundedButton(spriteBatch, _startButtonRect, "START", _isStartButtonHovered);
        
        // Draw HIGH SCORES button with rounded corners
        DrawRoundedButton(spriteBatch, _highScoresButtonRect, "HIGH SCORES", _isHighScoresButtonHovered);
    }

    private void DrawRoundedButton(SpriteBatch spriteBatch, Rectangle buttonRect, string text, bool isHovered)
    {
        Color buttonColor = isHovered ? Color.Cyan : Color.White;
        
        // Draw rounded rectangle (button background)
        DrawRoundedRectangle(spriteBatch, buttonRect, BUTTON_CORNER_RADIUS, buttonColor);
        
        // Draw button text centered
        Vector2 textSize = _font.MeasureString(text);
        Vector2 textPosition = new Vector2(
            buttonRect.X + buttonRect.Width / 2 - textSize.X / 2,
            buttonRect.Y + buttonRect.Height / 2 - textSize.Y / 2
        );
        spriteBatch.DrawString(_font, text, textPosition, Color.Black);
    }

    private void DrawRoundedRectangle(SpriteBatch spriteBatch, Rectangle rect, int cornerRadius, Color color)
    {
        // Draw main rectangle body
        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, rect.Width - cornerRadius * 2, rect.Height, color),
            new Vector2(rect.X + cornerRadius, rect.Y),
            Color.White
        );
        
        // Draw top rectangle
        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, rect.Width, cornerRadius, color),
            new Vector2(rect.X, rect.Y + cornerRadius),
            Color.White
        );
        
        // Draw bottom rectangle
        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, rect.Width, cornerRadius, color),
            new Vector2(rect.X, rect.Y + rect.Height - cornerRadius),
            Color.White
        );
        
        // Draw four corner circles (using small filled rectangles to approximate rounded corners)
        int cornerSize = cornerRadius;
        
        // Top-left corner
        DrawCorner(spriteBatch, rect.X + cornerRadius, rect.Y + cornerRadius, cornerSize, color, 0);
        
        // Top-right corner
        DrawCorner(spriteBatch, rect.X + rect.Width - cornerRadius, rect.Y + cornerRadius, cornerSize, color, 1);
        
        // Bottom-left corner
        DrawCorner(spriteBatch, rect.X + cornerRadius, rect.Y + rect.Height - cornerRadius, cornerSize, color, 2);
        
        // Bottom-right corner
        DrawCorner(spriteBatch, rect.X + rect.Width - cornerRadius, rect.Y + rect.Height - cornerRadius, cornerSize, color, 3);
    }

    private void DrawCorner(SpriteBatch spriteBatch, int centerX, int centerY, int radius, Color color, int corner)
    {
        // Simplified corner drawing using small filled rectangles
        for (int i = 0; i < radius; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                int distanceSquared = i * i + j * j;
                if (distanceSquared <= radius * radius)
                {
                    int x = centerX;
                    int y = centerY;
                    
                    if (corner == 0) // Top-left
                    {
                        x -= i;
                        y -= j;
                    }
                    else if (corner == 1) // Top-right
                    {
                        x += i;
                        y -= j;
                    }
                    else if (corner == 2) // Bottom-left
                    {
                        x -= i;
                        y += j;
                    }
                    else if (corner == 3) // Bottom-right
                    {
                        x += i;
                        y += j;
                    }
                    
                    spriteBatch.Draw(
                        CreateFilledRectangle(spriteBatch.GraphicsDevice, 1, 1, color),
                        new Vector2(x, y),
                        Color.White
                    );
                }
            }
        }
    }

    private Texture2D CreateFilledRectangle(GraphicsDevice graphicsDevice, int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int i = 0; i < data.Length; i++)
            data[i] = color;
        texture.SetData(data);
        return texture;
    }
}