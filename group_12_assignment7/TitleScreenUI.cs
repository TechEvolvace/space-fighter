using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace group_12_assignment7;

public class TitleScreenUI
{
    // Defines UI display and button fields
    private SpriteFont _font;
    private int _screenWidth;
    private int _screenHeight;
    
    // Defines Button states and positions fields and helper functions 
    public bool IsStartButtonPressed { get; private set; }
    public bool IsInstructionsButtonPressed { get; private set; }
    public bool IsHighScoresButtonPressed { get; private set; }
    
    private Rectangle _startButtonRect;
    private Rectangle _instructionsButtonRect;
    private Rectangle _highScoresButtonRect;
    
    private bool _isStartHovered;
    private bool _isInstructionsHovered;
    private bool _isHighScoresHovered;

    public TitleScreenUI(SpriteFont font, int screenWidth, int screenHeight)
    {
        // Initialize UI components
        _font = font;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        
        IsStartButtonPressed = false;
        IsInstructionsButtonPressed = false;
        IsHighScoresButtonPressed = false;
        
        _isStartHovered = false;
        _isInstructionsHovered = false;
        _isHighScoresHovered = false;

        // Define button positions
        int buttonWidth = 200;
        int buttonHeight = 60;
        int centerX = screenWidth / 2;
        int startY = screenHeight / 2;
        int spacing = 80;

        _startButtonRect = new Rectangle(centerX - buttonWidth / 2, startY, buttonWidth, buttonHeight);
        _instructionsButtonRect = new Rectangle(centerX - buttonWidth / 2, startY + spacing, buttonWidth, buttonHeight);
        _highScoresButtonRect = new Rectangle(centerX - buttonWidth / 2, startY + spacing * 2, buttonWidth, buttonHeight);
    }

    public void Update(KeyboardState keyboardState, MouseState mouseState)
    {
        // Reset button states each frame
        IsStartButtonPressed = false;
        IsInstructionsButtonPressed = false;
        IsHighScoresButtonPressed = false;
        
        _isStartHovered = false;
        _isInstructionsHovered = false;
        _isHighScoresHovered = false;

        // Check Start button
        if (_startButtonRect.Contains(mouseState.Position))
        {
            _isStartHovered = true;
            if (mouseState.LeftButton == ButtonState.Pressed)
                IsStartButtonPressed = true;
        }

        // Check Instructions button
        if (_instructionsButtonRect.Contains(mouseState.Position))
        {
            _isInstructionsHovered = true;
            if (mouseState.LeftButton == ButtonState.Pressed)
                IsInstructionsButtonPressed = true;
        }

        // Check High Scores button
        if (_highScoresButtonRect.Contains(mouseState.Position))
        {
            _isHighScoresHovered = true;
            if (mouseState.LeftButton == ButtonState.Pressed)
                IsHighScoresButtonPressed = true;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw background
        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, _screenWidth, _screenHeight, Color.Black),
            Vector2.Zero,
            Color.White
        );

        // Draw title
        string title = "SPACE FIGHTER";
        Vector2 titleSize = _font.MeasureString(title);
        Vector2 titlePosition = new Vector2(_screenWidth / 2 - (titleSize.X * 3) / 2, 50);
        spriteBatch.DrawString(_font, title, titlePosition, Color.Gold, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);

        // Draw all three buttons
        DrawRoundedButton(spriteBatch, _startButtonRect, "START", _isStartHovered);
        DrawRoundedButton(spriteBatch, _instructionsButtonRect, "INSTRUCTIONS", _isInstructionsHovered);
        DrawRoundedButton(spriteBatch, _highScoresButtonRect, "HIGH SCORES", _isHighScoresHovered);
    }

    private void DrawRoundedButton(SpriteBatch spriteBatch, Rectangle buttonRect, string text, bool isHovered)
    {
        // Draw button with hover color change
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

        // Draw rounded corners
        int cornerSize = cornerRadius;
        DrawCorner(spriteBatch, rect.X + cornerRadius, rect.Y + cornerRadius, cornerSize, color, 0);
        DrawCorner(spriteBatch, rect.X + rect.Width - cornerRadius, rect.Y + cornerRadius, cornerSize, color, 1);
        DrawCorner(spriteBatch, rect.X + cornerRadius, rect.Y + rect.Height - cornerRadius, cornerSize, color, 2);
        DrawCorner(spriteBatch, rect.X + rect.Width - cornerRadius, rect.Y + rect.Height - cornerRadius, cornerSize, color, 3);
    }

    private void DrawCorner(SpriteBatch spriteBatch, int centerX, int centerY, int radius, Color color, int corner)
    {
        // Draw quarter circle corner pixels
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