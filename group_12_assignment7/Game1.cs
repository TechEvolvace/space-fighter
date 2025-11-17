using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace group_12_assignment7;

public class Game1 : Game
{
    // Defines Graphics setup and game state fields
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _barlowFont;
    
    // Defines Game state tracking fields
    private GameState _currentGameState;
    private GameState _previousGameState;
    private enum GameState { TitleScreen, Playing, GameOver, HighScoreEntry, HighScoresView, Instructions, Paused }
    
    // Defines Screen dimensions fields
    private const int SCREEN_WIDTH = 1200;
    private const int SCREEN_HEIGHT = 800;
    
    // Defines UI components fields
    private TitleScreenUI _titleScreenUI;
    private HUDDisplay _hudDisplay;
    private GameOverUI _gameOverUI;
    private HighScoreEntryUI _highScoreEntryUI;
    private ScoreManager _scoreManager;
    private InstructionsUI _instructionsUI;
    private PauseMenuUI _pauseMenuUI;
    
    // Defines Game stats fields
    private int _currentScore;
    private int _enemiesDefeated;
    private bool _playerVictorious;
    
    // Define Pause and input tracking fields
    private KeyboardState _previousKeyboardState;
    private bool _pauseStateChanged = false;
    
    // Defines High scores screen back button and properties
    private Rectangle _highScoresBackButtonRect;
    private bool _isHighScoresBackButtonHovered = false;

    // === GAME OBJECTS & TEXTURES (Falcon/Laser Implementation) ===
    private Falcon _playerFalcon;
    private Texture2D _falconTexture;
    private Texture2D _playerLaserTexture;

    public Game1()
    {
        // Set up graphics manager and window settings
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
        _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
    }

    protected override void Initialize()
    {
        // Initialize initial game state and player stats
        _currentGameState = GameState.TitleScreen;
        _previousGameState = GameState.TitleScreen;
        _currentScore = 0;
        _enemiesDefeated = 0;
        _playerVictorious = false;
        _previousKeyboardState = Keyboard.GetState();
        _pauseStateChanged = false;
        
        // Initialize high scores back button rectangle
        _highScoresBackButtonRect = new Rectangle(
            SCREEN_WIDTH / 2 - 75,
            SCREEN_HEIGHT - 100,
            150,
            50
        );
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Load graphics and fonts
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _barlowFont = Content.Load<SpriteFont>("fonts/Barlow");
        
        // Load Game Object Textures
        _falconTexture = Content.Load<Texture2D>("milfac");
        _playerLaserTexture = Content.Load<Texture2D>("laser");

        // Load and initialize score manager
        _scoreManager = new ScoreManager("highscores.txt", _barlowFont);
        _scoreManager.LoadScores();
        
        // Initialize all UI screens
        _titleScreenUI = new TitleScreenUI(_barlowFont, SCREEN_WIDTH, SCREEN_HEIGHT);
        _hudDisplay = new HUDDisplay(_barlowFont, SCREEN_WIDTH, SCREEN_HEIGHT);
        _gameOverUI = new GameOverUI(_barlowFont, SCREEN_WIDTH, SCREEN_HEIGHT);
        _highScoreEntryUI = new HighScoreEntryUI(_barlowFont, SCREEN_WIDTH, SCREEN_HEIGHT);
        _instructionsUI = new InstructionsUI(_barlowFont, SCREEN_WIDTH, SCREEN_HEIGHT);
        _pauseMenuUI = new PauseMenuUI(_barlowFont, SCREEN_WIDTH, SCREEN_HEIGHT);

        // Initialize the Falcon object after textures are loaded
        Vector2 startPos = new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT * 0.8f);
        _playerFalcon = new Falcon(_falconTexture, _playerLaserTexture, startPos);
    }

    protected override void Update(GameTime gameTime)
    {
        // Get current input states
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();
        
        // Exit game program with ESC (but not during gameplay)
        if (keyboardState.IsKeyDown(Keys.Escape) && _currentGameState != GameState.Playing)
            Exit();
        
        _pauseStateChanged = false;
        
        // Handle pause functionality - requires pressing P key to toggle pause
        if (_currentGameState == GameState.Playing && keyboardState.IsKeyDown(Keys.P) && !_previousKeyboardState.IsKeyDown(Keys.P))
        {
            _previousGameState = GameState.Playing;
            _currentGameState = GameState.Paused;
            _previousKeyboardState = keyboardState;
            _pauseStateChanged = true;
        }

        // Route to appropriate update method based on current state
        switch (_currentGameState)
        {
            case GameState.TitleScreen:
                UpdateTitleScreen(keyboardState, mouseState);
                break;
            case GameState.Playing:
                UpdateGameplay(gameTime, keyboardState, mouseState);
                break;
            case GameState.GameOver:
                UpdateGameOver(keyboardState, mouseState);
                break;
            case GameState.HighScoreEntry:
                UpdateHighScoreEntry(keyboardState);
                break;
            case GameState.HighScoresView:
                UpdateHighScoresView(keyboardState, mouseState);
                break;
            case GameState.Instructions:
                UpdateInstructions(keyboardState, mouseState);
                break;
            case GameState.Paused:
                UpdatePauseMenu(keyboardState, mouseState);
                break;
        }

        // Only update keyboard state if pause state didn't change this frame
        if (!_pauseStateChanged)
        {
            _previousKeyboardState = keyboardState;
        }
        
        base.Update(gameTime);
    }

    private void UpdateTitleScreen(KeyboardState keyboardState, MouseState mouseState)
    {
        // Update title screen and handle button presses
        _titleScreenUI.Update(keyboardState, mouseState);
        
        if (keyboardState.IsKeyDown(Keys.Space) || _titleScreenUI.IsStartButtonPressed)
        {
            StartNewGame();
            _currentGameState = GameState.Playing;
        }
        
        if (_titleScreenUI.IsInstructionsButtonPressed)
        {
            _currentGameState = GameState.Instructions;
        }
        
        if (_titleScreenUI.IsHighScoresButtonPressed)
        {
            _currentGameState = GameState.HighScoresView;
        }
    }

    private void UpdateGameplay(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
    {
        // 1. Update the Falcon (Handles movement and shooting)
        _playerFalcon.Update(gameTime, keyboardState, mouseState);

        // 2. Update HUD with current game stats
        _hudDisplay.UpdateScore(_currentScore);
        _hudDisplay.UpdateHealth(_playerFalcon.Health); // Use Falcon's actual health
        _hudDisplay.UpdateEnemiesDefeated(_enemiesDefeated);
        
        // 3. Check win/loss conditions
        if (_playerFalcon.Health <= 0) // Use Falcon's actual health
        {
            _playerVictorious = false;
            TransitionToGameOver();
        }
        else if (_enemiesDefeated >= 10)
        {
            _playerVictorious = true;
            TransitionToGameOver();
        }
    }

    private void UpdateGameOver(KeyboardState keyboardState, MouseState mouseState)
    {
        // Update game over screen and handle navigation
        _gameOverUI.Update(keyboardState, mouseState);
        
        if (keyboardState.IsKeyDown(Keys.Space) || _gameOverUI.IsReturnToMenuPressed)
        {
            _currentGameState = GameState.TitleScreen;
        }
        else if (keyboardState.IsKeyDown(Keys.Enter) || _gameOverUI.IsPlayAgainPressed)
        {
            StartNewGame();
            _currentGameState = GameState.Playing;
        }
    }

    private void UpdateHighScoreEntry(KeyboardState keyboardState)
    {
        // Update high score entry form
        _highScoreEntryUI.Update(keyboardState);
        
        if (_highScoreEntryUI.IsSubmitted)
        {
            // Save new high score
            string playerName = _highScoreEntryUI.GetPlayerName();
            _scoreManager.AddScore(new HighScore(playerName, _currentScore));
            _scoreManager.SaveScores();
            _currentGameState = GameState.TitleScreen;
        }
    }

    private void UpdateHighScoresView(KeyboardState keyboardState, MouseState mouseState)
    {
        // Check if mouse is hovering over back button
        _isHighScoresBackButtonHovered = _highScoresBackButtonRect.Contains(mouseState.Position);
        
        // Check if Back button is clicked
        if (_isHighScoresBackButtonHovered && mouseState.LeftButton == ButtonState.Pressed)
        {
            _currentGameState = GameState.TitleScreen;
        }
        
        // Also allow ESC to go back
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            _currentGameState = GameState.TitleScreen;
        }
    }

    private void UpdateInstructions(KeyboardState keyboardState, MouseState mouseState)
    {
        // Update instructions screen and handle back button
        _instructionsUI.Update(keyboardState, mouseState);
        
        if (_instructionsUI.IsBackPressed)
        {
            _currentGameState = GameState.TitleScreen;
        }
    }

    private void UpdatePauseMenu(KeyboardState keyboardState, MouseState mouseState)
    {
        // Update pause menu and handle resume/main menu buttons
        _pauseMenuUI.Update(keyboardState, mouseState);
        
        if (_pauseMenuUI.IsResumePressed)
        {
            _currentGameState = GameState.Playing;
            _previousKeyboardState = keyboardState;
            _pauseStateChanged = true;
        }
        
        // Main Menu button should go to TitleScreen
        if (_pauseMenuUI.IsMainMenuPressed)
        {
            _currentGameState = GameState.TitleScreen;
        }
    }

    private void TransitionToGameOver()
    {
        // Set up game over screen
        _gameOverUI.SetGameResult(_playerVictorious, _currentScore);
        
        // If high score, go to entry screen; otherwise show game over
        if (_scoreManager.IsHighScore(_currentScore))
        {
            _currentGameState = GameState.HighScoreEntry;
            _highScoreEntryUI.Reset();
        }
        else
        {
            _currentGameState = GameState.GameOver;
        }
    }

    // Helper function to start a new game by resetting the game stats
    private void StartNewGame()
    {
        // Reset all game stats for new game
        _currentScore = 0;
        _enemiesDefeated = 0;
        _playerVictorious = false;

        // Reset Falcon's state (Health and position)
        _playerFalcon.Health = 100;
        _playerFalcon.Position = new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT * 0.8f);
    }

    // Game will choose which screen to display (a.k.a. draw) that the user is currently in
    protected override void Draw(GameTime gameTime)
    {
        // Clear screen and begin drawing (Background is Black)
        GraphicsDevice.Clear(Color.Black); 
        _spriteBatch.Begin();

        // Draw appropriate screen based on game state
        switch (_currentGameState)
        {
            case GameState.TitleScreen:
                _titleScreenUI.Draw(_spriteBatch);
                break;

            case GameState.Playing:
            case GameState.Paused:
                // Draw all game objects first
                _playerFalcon.Draw(_spriteBatch); // Draw the Falcon and its lasers

                // Then draw the HUD/UI overlays
                _hudDisplay.Draw(_spriteBatch);
                
                if (_currentGameState == GameState.Paused)
                {
                    _pauseMenuUI.Draw(_spriteBatch);
                }
                break;
                
            case GameState.GameOver:
                _gameOverUI.Draw(_spriteBatch);
                break;
            case GameState.HighScoreEntry:
                _highScoreEntryUI.Draw(_spriteBatch);
                _scoreManager.DrawHighScores(_spriteBatch, 100, 200);
                break;
            case GameState.HighScoresView:
                DrawHighScoresScreen();
                break;
            case GameState.Instructions:
                _instructionsUI.Draw(_spriteBatch);
                break;
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    // Helper function to display the High Score screen and its contents
    private void DrawHighScoresScreen()
    {
        // Draw semi-transparent background
        Texture2D backgroundTexture = CreateFilledRectangle(_spriteBatch.GraphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT, Color.Black);
        _spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White * 0.9f);

        // Draw title
        string title = "HIGH SCORES";
        Vector2 titleSize = _barlowFont.MeasureString(title);
        Vector2 titlePosition = new Vector2(
            SCREEN_WIDTH / 2 - (titleSize.X * 2.5f) / 2,
            40
        );
        _spriteBatch.DrawString(_barlowFont, title, titlePosition, Color.Gold, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);

        // Draw high scores list
        _scoreManager.DrawHighScores(_spriteBatch, SCREEN_WIDTH / 2 - 200, 160);

        // Draw BACK button with rounded corners and hover detection
        DrawRoundedButton(_spriteBatch, _highScoresBackButtonRect, "BACK", _isHighScoresBackButtonHovered);
    }

    // Helper funtion to draw the button in a unique shape
    private void DrawRoundedButton(SpriteBatch spriteBatch, Rectangle buttonRect, string text, bool isHovered)
    {
        // Draw button with color change on hover
        Color buttonColor = isHovered ? Color.Cyan : Color.White;
        DrawRoundedRectangle(spriteBatch, buttonRect, 10, buttonColor);
        
        // Draw centered text on button
        Vector2 textSize = _barlowFont.MeasureString(text);
        Vector2 textPosition = new Vector2(
            buttonRect.X + buttonRect.Width / 2 - textSize.X / 2,
            buttonRect.Y + buttonRect.Height / 2 - textSize.Y / 2
        );
        spriteBatch.DrawString(_barlowFont, text, textPosition, Color.Black);
    }

    // Helper function for DrawRoundedbutton function to help draw the button with rounded corners
    private void DrawRoundedRectangle(SpriteBatch spriteBatch, Rectangle rect, int cornerRadius, Color color)
    {
        // Draw main body and edges
        spriteBatch.Draw(
            CreateFilledRectangle(spriteBatch.GraphicsDevice, rect.Width - cornerRadius * 2, rect.Height, color),
            new Vector2(rect.X + cornerRadius, rect.Y),
            Color.White
        );
        
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

    // Helper function for DrawRoundedRectangle function to draw the rounded corners of the button
    private void DrawCorner(SpriteBatch spriteBatch, int centerX, int centerY, int radius, Color color, int corner)
    {
        // Draw circular corners by plotting pixels in circular pattern
        for (int i = 0; i < radius; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                int distanceSquared = i * i + j * j;
                if (distanceSquared <= radius * radius)
                {
                    int x = centerX;
                    int y = centerY;
                    
                    // Adjust coordinates based on corner position
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

    // Helper function to help draw the buttons' unique shape
    private Texture2D CreateFilledRectangle(GraphicsDevice graphicsDevice, int width, int height, Color color)
    {
        // Create a solid color texture for drawing rectangles
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int i = 0; i < data.Length; i++)
            data[i] = color;
        texture.SetData(data);
        return texture;
    }
    
    // Defines public methods for game stat updates
    public void AddScore(int points) => _currentScore += points;
    public void IncrementEnemiesDefeated() => _enemiesDefeated++;
    
    public int GetCurrentScore() => _currentScore;
    public int GetPlayerHealth() => _playerFalcon.Health; // Get health from Falcon
    public int GetEnemiesDefeated() => _enemiesDefeated;
}