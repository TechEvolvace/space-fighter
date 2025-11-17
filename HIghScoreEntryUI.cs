using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace group_12_assignment7;

public class HighScoreEntryUI
{
    // Defines UI display fields
    private SpriteFont _font;
    private int _screenWidth;
    private int _screenHeight;
    
    // Defines Text input tracking fields
    private string _playerName;
    private int _maxNameLength;
    
    // Defines Submission state fields and associated helper functions
    public bool IsSubmitted { get; private set; }
    private KeyboardState _previousKeyboardState;

    public HighScoreEntryUI(SpriteFont font, int screenWidth, int screenHeight)
    {
        // Initialize UI and input fields
        _font = font;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _playerName = "";
        _maxNameLength = 15;
        IsSubmitted = false;
        _previousKeyboardState = Keyboard.GetState();
    }

    public void Reset()
    {
        // Clear input for next high score entry
        _playerName = "";
        IsSubmitted = false;
        _previousKeyboardState = Keyboard.GetState();
    }

    public void Update(KeyboardState keyboardState)
    {
        // Reset submission flag
        IsSubmitted = false;

        // Handle keyboard input for text entry
        Keys[] pressedKeys = keyboardState.GetPressedKeys();
        foreach (Keys key in pressedKeys)
        {
            // Only process new key presses (edge detection)
            if (_previousKeyboardState.IsKeyDown(key))
                continue;

            // Handle backspace - delete last character
            if (key == Keys.Back && _playerName.Length > 0)
            {
                _playerName = _playerName.Substring(0, _playerName.Length - 1);
            }
            // Handle Enter - submit name if not empty
            else if (key == Keys.Enter)
            {
                if (_playerName.Length > 0)
                    IsSubmitted = true;
            }
            // Handle letter keys
            else if (key >= Keys.A && key <= Keys.Z && _playerName.Length < _maxNameLength)
            {
                // Convert key to character (uppercase if Shift held)
                char keyChar = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift)
                    ? char.ToUpper((char)('A' + (int)key - (int)Keys.A))
                    : char.ToLower((char)('A' + (int)key - (int)Keys.A));
                _playerName += keyChar;
            }
            // Handle spacebar
            else if (key == Keys.Space && _playerName.Length < _maxNameLength)
            {
                _playerName += " ";
            }
        }

        _previousKeyboardState = keyboardState;
    }

    public string GetPlayerName()
    {
        // Return the entered player name
        return _playerName;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw semi-transparent background overlay
        DrawRectangle(spriteBatch, 0, 0, _screenWidth, _screenHeight, Color.Black * 0.8f);

        // Draw title
        string title = "NEW HIGH SCORE!";
        Vector2 titleSize = _font.MeasureString(title);
        Vector2 titlePosition = new Vector2(_screenWidth / 2 - titleSize.X / 2, 100);
        spriteBatch.DrawString(_font, title, titlePosition, Color.Gold, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

        // Draw name entry prompt
        string prompt = "Enter your name:";
        Vector2 promptSize = _font.MeasureString(prompt);
        Vector2 promptPosition = new Vector2(_screenWidth / 2 - promptSize.X / 2, 250);
        spriteBatch.DrawString(_font, prompt, promptPosition, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);

        // Draw input box
        int boxWidth = 400;
        int boxHeight = 50;
        int boxX = _screenWidth / 2 - boxWidth / 2;
        int boxY = 320;
        DrawRectangle(spriteBatch, boxX, boxY, boxWidth, boxHeight, Color.White);

        // Draw player-typed text in box
        Vector2 textPosition = new Vector2(boxX + 10, boxY + 10);
        spriteBatch.DrawString(_font, _playerName, textPosition, Color.Black);

        // Draw blinking cursor using tick count for timing
        if ((Environment.TickCount / 500) % 2 == 0)
        {
            Vector2 cursorPosition = new Vector2(textPosition.X + _font.MeasureString(_playerName).X, textPosition.Y);
            spriteBatch.DrawString(_font, "|", cursorPosition, Color.Black);
        }

        // Draw submission instructions
        string instructions = "Press ENTER to submit";
        Vector2 instructionsSize = _font.MeasureString(instructions);
        Vector2 instructionsPosition = new Vector2(_screenWidth / 2 - instructionsSize.X / 2, 420);
        spriteBatch.DrawString(_font, instructions, instructionsPosition, Color.LimeGreen);
    }

    private void DrawRectangle(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
    {
        // Create and draw a solid color rectangle
        Texture2D texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        spriteBatch.Draw(texture, new Rectangle(x, y, width, height), color);
    }
}