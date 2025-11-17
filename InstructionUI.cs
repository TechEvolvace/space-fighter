using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace group_12_assignment7;

public class InstructionsUI
{
    // Defines UI display fields
    private SpriteFont _font;
    private int _screenWidth;
    private int _screenHeight;
    
    // Defines Button states
    public bool IsBackPressed { get; private set; }
    private Rectangle _backButtonRect;
    private bool _isBackButtonHovered;
    
    // Defines Input tracking
    private KeyboardState _previousKeyboardState;

    public InstructionsUI(SpriteFont font, int screenWidth, int screenHeight)
    {
        // Initialize UI
        _font = font;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        IsBackPressed = false;
        _isBackButtonHovered = false;
        _previousKeyboardState = Keyboard.GetState();

        // Define back button
        int buttonWidth = 150;
        int buttonHeight = 50;
        _backButtonRect = new Rectangle(
            screenWidth / 2 - buttonWidth / 2,
            screenHeight - 100,
            buttonWidth,
            buttonHeight
        );
    }

    public void Update(KeyboardState keyboardState, MouseState mouseState)
    {
        // Reset button state
        IsBackPressed = false;
        _isBackButtonHovered = false;

        // Check back button hover and click
        if (_backButtonRect.Contains(mouseState.Position))
        {
            _isBackButtonHovered = true;
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                IsBackPressed = true;
            }
        }

        // Allow ESC to go back (edge detection)
        if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            IsBackPressed = true;
        }

        _previousKeyboardState = keyboardState;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw semi-transparent background
        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, _screenWidth, _screenHeight, Color.Black),
            Vector2.Zero,
            Color.White * 0.85f
        );

        int padding = 40;
        int lineSpacing = 40;
        int startY = 60;
        float textScale = 1.6f;

        // Draw title
        string title = "INSTRUCTIONS";
        Vector2 titleSize = _font.MeasureString(title);
        Vector2 titlePosition = new Vector2(
            _screenWidth / 2 - (titleSize.X * 2.2f) / 2,
            20
        );
        spriteBatch.DrawString(_font, title, titlePosition, Color.Cyan, 0f, Vector2.Zero, 2.2f, SpriteEffects.None, 0f);

        // Draw game objective section
        string objectiveTitle = "OBJECTIVE:";
        spriteBatch.DrawString(_font, objectiveTitle, new Vector2(padding, startY), Color.Gold, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

        string objective = "Defeat all Tie Fighter enemies to win the battle.";
        DrawWrappedText(spriteBatch, objective, padding, startY + lineSpacing, _screenWidth - padding * 2, textScale, Color.White);

        // Draw controls section
        int controlsStartY = startY + lineSpacing * 3;
        string controlsTitle = "CONTROLS:";
        spriteBatch.DrawString(_font, controlsTitle, new Vector2(padding, controlsStartY), Color.Gold, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

        string[] controls = new string[]
        {
            "W / UP ARROW - Move player up",
            "S / DOWN ARROW - Move player down",
            "A / LEFT ARROW - Move player left",
            "D / RIGHT ARROW - Move player right",
            "SPACE - Shoot weapon",
            "P - Pause / Resume game (May need to press P twice to pause the game)"
        };

        int controlY = controlsStartY + lineSpacing;
        foreach (string control in controls)
        {
            spriteBatch.DrawString(_font, control, new Vector2(padding + 20, controlY), Color.White, 0f, Vector2.Zero, textScale * 0.85f, SpriteEffects.None, 0f);
            controlY += (int)(lineSpacing * 0.9f);
        }

        // Draw tips section
        int tipsStartY = controlY + lineSpacing;
        string tipsTitle = "TIPS:";
        spriteBatch.DrawString(_font, tipsTitle, new Vector2(padding, tipsStartY), Color.Gold, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

        string[] tips = new string[]
        {
            "- Avoid enemy fire to keep your health high",
            "- Higher score = more Tie Fighters defeated",
            "- Stay on the screen - don't go out of bounds!",
            "- Quick reflexes will help you survive longer"
        };

        int tipY = tipsStartY + lineSpacing;
        foreach (string tip in tips)
        {
            spriteBatch.DrawString(_font, tip, new Vector2(padding + 20, tipY), Color.LimeGreen, 0f, Vector2.Zero, textScale * 0.85f, SpriteEffects.None, 0f);
            tipY += (int)(lineSpacing * 0.9f);
        }

        // Draw back button
        DrawRoundedButton(spriteBatch, _backButtonRect, "BACK", _isBackButtonHovered);
    }

    private void DrawRoundedButton(SpriteBatch spriteBatch, Rectangle buttonRect, string text, bool isHovered)
    {
        // Draw button with hover effect
        Color buttonColor = isHovered ? Color.Cyan : Color.White;
        DrawRoundedRectangle(spriteBatch, buttonRect, 10, buttonColor);

        // Draw centered text
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

        // Draw top and bottom edges
        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, rect.Width, cornerRadius, color),
            new Vector2(rect.X, rect.Y + cornerRadius),
            Color.White
        );

        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, rect.Width, cornerRadius, color),
            new Vector2(rect.X, rect.Y + rect.Height - cornerRadius),
            Color.White
        );

        // Draw four corner circles
        int cornerSize = cornerRadius;
        DrawCorner(spriteBatch, rect.X + cornerRadius, rect.Y + cornerRadius, cornerSize, color, 0);
        DrawCorner(spriteBatch, rect.X + rect.Width - cornerRadius, rect.Y + cornerRadius, cornerSize, color, 1);
        DrawCorner(spriteBatch, rect.X + cornerRadius, rect.Y + rect.Height - cornerRadius, cornerSize, color, 2);
        DrawCorner(spriteBatch, rect.X + rect.Width - cornerRadius, rect.Y + rect.Height - cornerRadius, cornerSize, color, 3);
    }

    private void DrawCorner(SpriteBatch spriteBatch, int centerX, int centerY, int radius, Color color, int corner)
    {
        // Draw circular corner pixels
        for (int i = 0; i < radius; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                int distanceSquared = i * i + j * j;
                if (distanceSquared <= radius * radius)
                {
                    int x = centerX;
                    int y = centerY;

                    // Adjust based on corner position
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

    private void DrawWrappedText(SpriteBatch spriteBatch, string text, int x, int y, int maxWidth, float scale, Color color)
    {
        // Word wrapping for text
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
        // Create solid color texture
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int i = 0; i < data.Length; i++)
            data[i] = color;
        texture.SetData(data);
        return texture;
    }
}