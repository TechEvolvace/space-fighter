using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace group_12_assignment7;

public class PauseMenuUI
{
    // UI display fields
    private SpriteFont _font;
    private int _screenWidth;
    private int _screenHeight;
    
    // Button states and rectangles
    public bool IsResumePressed { get; private set; }
    public bool IsMainMenuPressed { get; private set; }
    
    private Rectangle _resumeButtonRect;
    private Rectangle _menuButtonRect;
    private bool _isResumeHovered;
    private bool _isMenuHovered;
    
    // Input tracking for edge detection
    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;

    public PauseMenuUI(SpriteFont font, int screenWidth, int screenHeight)
    {
        // Initialize UI components
        _font = font;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        
        IsResumePressed = false;
        IsMainMenuPressed = false;
        _isResumeHovered = false;
        _isMenuHovered = false;
        
        _previousKeyboardState = Keyboard.GetState();
        _previousMouseState = Mouse.GetState();

        // Define button positions
        int buttonWidth = 180;
        int buttonHeight = 60;
        int spacing = 50;
        int centerX = screenWidth / 2;

        _resumeButtonRect = new Rectangle(
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

    public void Update(KeyboardState keyboardState, MouseState mouseState)
    {
        // Reset button states each frame
        IsResumePressed = false;
        IsMainMenuPressed = false;
        _isResumeHovered = false;
        _isMenuHovered = false;

        // Check Resume button with edge detection (only on initial click)
        if (_resumeButtonRect.Contains(mouseState.Position))
        {
            _isResumeHovered = true;
            if (mouseState.LeftButton == ButtonState.Pressed && 
                _previousMouseState.LeftButton == ButtonState.Released)
            {
                IsResumePressed = true;
            }
        }

        // Check Main Menu button with edge detection (only on initial click)
        if (_menuButtonRect.Contains(mouseState.Position))
        {
            _isMenuHovered = true;
            if (mouseState.LeftButton == ButtonState.Pressed && 
                _previousMouseState.LeftButton == ButtonState.Released)
            {
                IsMainMenuPressed = true;
            }
        }

        // Allow P key to resume (edge detection)
        if (keyboardState.IsKeyDown(Keys.P) && !_previousKeyboardState.IsKeyDown(Keys.P))
        {
            IsResumePressed = true;
        }

        // Update input state tracking
        _previousKeyboardState = keyboardState;
        _previousMouseState = mouseState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw semi-transparent overlay
        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, _screenWidth, _screenHeight, Color.Black),
            Vector2.Zero,
            Color.White * 0.8f
        );

        int padding = 40;
        int lineSpacing = 35;
        float textScale = 1.5f;

        // Draw title
        string title = "GAME PAUSED";
        Vector2 titleSize = _font.MeasureString(title);
        Vector2 titlePosition = new Vector2(
            _screenWidth / 2 - (titleSize.X * 2.5f) / 2,
            20
        );
        spriteBatch.DrawString(_font, title, titlePosition, Color.Gold, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);

        // Draw separator line below title
        DrawLine(spriteBatch, padding, 70, _screenWidth - padding, 70, Color.Gold, 2);

        int contentStartY = 100;

        // Draw controls section
        string controlsTitle = "CONTROLS:";
        spriteBatch.DrawString(_font, controlsTitle, new Vector2(padding, contentStartY), Color.Cyan, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

        string[] controls = new string[]
        {
            "W / UP ARROW - Move up",
            "S / DOWN ARROW - Move down",
            "A / LEFT ARROW - Move left",
            "D / RIGHT ARROW - Move right",
            "SPACE - Shoot weapon",
            "P - Pause or Resume game (May need to press P twice to pause the game)"
        };

        int controlY = contentStartY + lineSpacing;
        foreach (string control in controls)
        {
            spriteBatch.DrawString(_font, control, new Vector2(padding + 20, controlY), Color.White, 0f, Vector2.Zero, textScale * 0.8f, SpriteEffects.None, 0f);
            controlY += (int)(lineSpacing * 0.85f);
        }

        // Draw objective section
        int objectiveStartY = controlY + lineSpacing;
        string objectiveTitle = "OBJECTIVE:";
        spriteBatch.DrawString(_font, objectiveTitle, new Vector2(padding, objectiveStartY), Color.Cyan, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

        string objective = "Defeat all Tie Fighters while keeping your health above 0.";
        DrawWrappedText(spriteBatch, objective, padding + 20, objectiveStartY + lineSpacing, _screenWidth - padding * 2, textScale * 0.8f, Color.White);

        // Draw RESUME and MAIN MENU buttons
        DrawButton(spriteBatch, _resumeButtonRect, "RESUME", _isResumeHovered);
        DrawButton(spriteBatch, _menuButtonRect, "MAIN MENU", _isMenuHovered);

        // Draw resume hint at bottom
        string resumeHint = "Press P to resume or click RESUME";
        Vector2 hintSize = _font.MeasureString(resumeHint);
        Vector2 hintPosition = new Vector2(
            _screenWidth / 2 - hintSize.X / 2,
            _screenHeight - 50
        );
        spriteBatch.DrawString(_font, resumeHint, hintPosition, Color.LimeGreen, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
    }

    private void DrawButton(SpriteBatch spriteBatch, Rectangle buttonRect, string text, bool isHovered)
    {
        // Draw button with hover color change
        Color buttonColor = isHovered ? Color.Cyan : Color.White;
        DrawRoundedRectangle(spriteBatch, buttonRect, 10, buttonColor);

        // Draw centered text on button
        Vector2 textSize = _font.MeasureString(text);
        Vector2 textPosition = new Vector2(
            buttonRect.X + buttonRect.Width / 2 - textSize.X / 2,
            buttonRect.Y + buttonRect.Height / 2 - textSize.Y / 2
        );
        spriteBatch.DrawString(_font, text, textPosition, Color.Black);
    }

    private void DrawRoundedRectangle(SpriteBatch spriteBatch, Rectangle rect, int cornerRadius, Color color)
    {
        // Draw main body
        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, rect.Width - cornerRadius * 2, rect.Height, color),
            new Vector2(rect.X + cornerRadius, rect.Y),
            Color.White
        );

        // Draw top edge
        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, rect.Width, cornerRadius, color),
            new Vector2(rect.X, rect.Y + cornerRadius),
            Color.White
        );

        // Draw bottom edge
        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, rect.Width, cornerRadius, color),
            new Vector2(rect.X, rect.Y + rect.Height - cornerRadius),
            Color.White
        );

        // Draw corners
        int cornerSize = cornerRadius;
        DrawCorner(spriteBatch, rect.X + cornerRadius, rect.Y + cornerRadius, cornerSize, color, 0);
        DrawCorner(spriteBatch, rect.X + rect.Width - cornerRadius, rect.Y + cornerRadius, cornerSize, color, 1);
        DrawCorner(spriteBatch, rect.X + cornerRadius, rect.Y + rect.Height - cornerRadius, cornerSize, color, 2);
        DrawCorner(spriteBatch, rect.X + rect.Width - cornerRadius, rect.Y + rect.Height - cornerRadius, cornerSize, color, 3);
    }

    private void DrawCorner(SpriteBatch spriteBatch, int centerX, int centerY, int radius, Color color, int corner)
    {
        // Draw circular corner by plotting pixels in quarter circle pattern
        for (int i = 0; i < radius; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                int distanceSquared = i * i + j * j;
                if (distanceSquared <= radius * radius)
                {
                    int x = centerX;
                    int y = centerY;

                    if (corner == 0) { x -= i; y -= j; }
                    else if (corner == 1) { x += i; y -= j; }
                    else if (corner == 2) { x -= i; y += j; }
                    else if (corner == 3) { x += i; y += j; }

                    spriteBatch.Draw(
                        CreateFilledRectangle(spriteBatch.GraphicsDevice, 1, 1, color),
                        new Vector2(x, y),
                        Color.White
                    );
                }
            }
        }
    }

    private void DrawLine(SpriteBatch spriteBatch, int x1, int y1, int x2, int y2, Color color, int thickness)
    {
        // Calculate line length and angle
        float length = (float)System.Math.Sqrt(System.Math.Pow(x2 - x1, 2) + System.Math.Pow(y2 - y1, 2));
        float angle = (float)System.Math.Atan2(y2 - y1, x2 - x1);

        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, (int)length, thickness, color),
            new Vector2(x1, y1),
            null,
            Color.White,
            angle,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0f
        );
    }

    private void DrawWrappedText(SpriteBatch spriteBatch, string text, int x, int y, int maxWidth, float scale, Color color)
    {
        // Word wrapping - split text across multiple lines if too wide
        string[] words = text.Split(' ');
        string currentLine = "";

        foreach (string word in words)
        {
            string testLine = currentLine + (currentLine.Length > 0 ? " " : "") + word;
            Vector2 lineSize = _font.MeasureString(testLine);

            if (lineSize.X * scale > maxWidth && currentLine.Length > 0)
            {
                spriteBatch.DrawString(_font, currentLine, new Vector2(x, y), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                y += (int)(_font.LineSpacing * scale);
                currentLine = word;
            }
            else
            {
                currentLine = testLine;
            }
        }

        if (currentLine.Length > 0)
        {
            spriteBatch.DrawString(_font, currentLine, new Vector2(x, y), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }

    private Texture2D CreateFilledRectangle(GraphicsDevice graphicsDevice, int width, int height, Color color)
    {
        // Create solid color texture for drawing shapes
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int i = 0; i < data.Length; i++)
            data[i] = color;
        texture.SetData(data);
        return texture;
    }
}