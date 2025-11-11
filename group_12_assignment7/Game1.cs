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
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _barlowFont;
    
    // Game state management
    private GameState _currentGameState;
    private enum GameState { TitleScreen, Playing, GameOver, HighScoreEntry, HighScoresView }
    
    // Screen dimensions
    private const int SCREEN_WIDTH = 1200;
    private const int SCREEN_HEIGHT = 800;
    
    // UI/Game components
    private TitleScreenUI _titleScreenUI;
    private HUDDisplay _hudDisplay;
    private GameOverUI _gameOverUI;
    private HighScoreEntryUI _highScoreEntryUI;
    private ScoreManager _scoreManager;
    
    // Game logic tracking
    private int _currentScore;
    private int _playerHealth;
    private int _enemiesDefeated;
    private bool _playerVictorious;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        // Set screen dimensions
        _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
        _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
    }

    protected override void Initialize()
    {
        _currentGameState = GameState.TitleScreen;
        _currentScore = 0;
        _playerHealth = 100;
        _enemiesDefeated = 0;
        _playerVictorious = false;
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _barlowFont = Content.Load<SpriteFont>("fonts/Barlow");
        
        // Initialize score manager with font
        _scoreManager = new ScoreManager("highscores.txt", _barlowFont);
        _scoreManager.LoadScores();
        
        // Initialize UI components
        _titleScreenUI = new TitleScreenUI(_barlowFont, SCREEN_WIDTH, SCREEN_HEIGHT);
        _hudDisplay = new HUDDisplay(_barlowFont, SCREEN_WIDTH, SCREEN_HEIGHT);
        _gameOverUI = new GameOverUI(_barlowFont, SCREEN_WIDTH, SCREEN_HEIGHT);
        _highScoreEntryUI = new HighScoreEntryUI(_barlowFont, SCREEN_WIDTH, SCREEN_HEIGHT);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();
        
        // Exit with ESC
        if (keyboardState.IsKeyDown(Keys.Escape))
            Exit();

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
                UpdateHighScoresView(keyboardState);
                break;
        }

        base.Update(gameTime);
    }

    private void UpdateTitleScreen(KeyboardState keyboardState, MouseState mouseState)
    {
        _titleScreenUI.Update(keyboardState, mouseState);
        
        // Start game when player presses Space or clicks Start button
        if (keyboardState.IsKeyDown(Keys.Space) || _titleScreenUI.IsStartButtonPressed)
        {
            StartNewGame();
            _currentGameState = GameState.Playing;
        }
        
        // View high scores when clicking High Scores button
        if (_titleScreenUI.IsHighScoresButtonPressed)
        {
            _currentGameState = GameState.HighScoresView;
        }
    }

    private void UpdateGameplay(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
    {
        // Update HUD with current game state
        _hudDisplay.UpdateScore(_currentScore);
        _hudDisplay.UpdateHealth(_playerHealth);
        _hudDisplay.UpdateEnemiesDefeated(_enemiesDefeated);
        
        // TODO: Integrate with Player and Enemy classes from Samuel and James
        // This is where gameplay logic will be called
        
        // Check for game end conditions (placeholder logic)
        // In actual implementation, this will check player/enemy states
        if (_playerHealth <= 0)
        {
            _playerVictorious = false;
            TransitionToGameOver();
        }
        else if (_enemiesDefeated >= 10) // Example: defeat 10 enemies to win
        {
            _playerVictorious = true;
            TransitionToGameOver();
        }
    }

    private void UpdateGameOver(KeyboardState keyboardState, MouseState mouseState)
    {
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
        _highScoreEntryUI.Update(keyboardState);
        
        if (_highScoreEntryUI.IsSubmitted)
        {
            string playerName = _highScoreEntryUI.GetPlayerName();
            _scoreManager.AddScore(new HighScore(playerName, _currentScore));
            _scoreManager.SaveScores();
            _currentGameState = GameState.TitleScreen;
        }
    }

    private void UpdateHighScoresView(KeyboardState keyboardState)
    {
        // Return to title screen when pressing any key
        if (keyboardState.GetPressedKeys().Length > 0)
        {
            _currentGameState = GameState.TitleScreen;
        }
    }

    private void TransitionToGameOver()
    {
        _gameOverUI.SetGameResult(_playerVictorious, _currentScore);
        
        // Check if score qualifies for high scores
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

    private void StartNewGame()
    {
        _currentScore = 0;
        _playerHealth = 100;
        _enemiesDefeated = 0;
        _playerVictorious = false;
        // TODO: Initialize Player and Enemy objects here
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        switch (_currentGameState)
        {
            case GameState.TitleScreen:
                _titleScreenUI.Draw(_spriteBatch);
                break;
                
            case GameState.Playing:
                // TODO: Draw Player and Enemy objects here
                _hudDisplay.Draw(_spriteBatch);
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
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void DrawHighScoresScreen()
    {
        // Draw semi-transparent background
        Texture2D backgroundTexture = CreateFilledRectangle(_spriteBatch.GraphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT, Color.Black);
        _spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White * 0.9f);

        // Draw SINGLE title (removed duplicate from ScoreManager)
        string title = "HIGH SCORES";
        Vector2 titleSize = _barlowFont.MeasureString(title);
        Vector2 titlePosition = new Vector2(
            SCREEN_WIDTH / 2 - (titleSize.X * 2.5f) / 2,
            40  // Moved up a bit to accommodate larger scores list
        );
        _spriteBatch.DrawString(_barlowFont, title, titlePosition, Color.Gold, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);

        // Draw high scores list - ADJUSTED POSITIONING for bigger text
        // X position centered, Y position adjusted to give proper spacing from title
        _scoreManager.DrawHighScores(_spriteBatch, SCREEN_WIDTH / 2 - 200, 160);

        // Draw instructions
        string instructions = "Press any key to return to menu";
        Vector2 instructionsSize = _barlowFont.MeasureString(instructions);
        Vector2 instructionsPosition = new Vector2(
            SCREEN_WIDTH / 2 - instructionsSize.X / 2,
            SCREEN_HEIGHT - 60
        );
        _spriteBatch.DrawString(_barlowFont, instructions, instructionsPosition, Color.LimeGreen, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
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
    
    // Public methods for integration with Player/Enemy classes
    public void AddScore(int points)
    {
        _currentScore += points;
    }
    
    public void UpdatePlayerHealth(int health)
    {
        _playerHealth = health;
    }
    
    public void IncrementEnemiesDefeated()
    {
        _enemiesDefeated++;
    }
    
    public int GetCurrentScore() => _currentScore;
    public int GetPlayerHealth() => _playerHealth;
    public int GetEnemiesDefeated() => _enemiesDefeated;
}