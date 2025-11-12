using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace group_12_assignment7;

// Data structure for storing a single high score entry
public class HighScore
{
    public string PlayerName { get; set; }
    public int Score { get; set; }
    public DateTime DateAchieved { get; set; }

    public HighScore(string name, int score)
    {
        PlayerName = name;
        Score = score;
        DateAchieved = DateTime.Now;
    }

    // Convert high score to file format
    public override string ToString()
    {
        return $"{PlayerName}|{Score}|{DateAchieved:yyyy-MM-dd HH:mm:ss}";
    }
}

// Manages loading, saving, and displaying high scores
public class ScoreManager
{
    // High scores list and file storage
    private List<HighScore> _highScores;
    private string _filePath;
    private SpriteFont _font;
    private const int MAX_SCORES = 10;

    public ScoreManager(string filePath, SpriteFont font)
    {
        // Initialize score manager with file path and font for display
        _filePath = filePath;
        _font = font;
        _highScores = new List<HighScore>();
    }

    public void LoadScores()
    {
        // Load high scores from file
        _highScores.Clear();

        // If file doesn't exist yet, start with empty list
        if (!File.Exists(_filePath))
            return;

        try
        {
            // Parse file and convert to HighScore objects
            string[] lines = File.ReadAllLines(_filePath);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Parse line format: Name|Score|DateTime
                string[] parts = line.Split('|');
                if (parts.Length == 3)
                {
                    string name = parts[0];
                    if (int.TryParse(parts[1], out int score) && DateTime.TryParse(parts[2], out DateTime date))
                    {
                        _highScores.Add(new HighScore(name, score) { DateAchieved = date });
                    }
                }
            }

            // Sort scores from highest to lowest
            _highScores = _highScores.OrderByDescending(x => x.Score).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading scores: {ex.Message}");
        }
    }

    public void SaveScores()
    {
        // Write all high scores back to file
        try
        {
            string[] lines = _highScores.Select(x => x.ToString()).ToArray();
            File.WriteAllLines(_filePath, lines);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving scores: {ex.Message}");
        }
    }

    public bool IsHighScore(int score)
    {
        // Check if score qualifies for high score list
        if (_highScores.Count < MAX_SCORES)
            return true; // List not full yet
        
        return score > _highScores[MAX_SCORES - 1].Score; // Better than lowest score
    }

    public void AddScore(HighScore score)
    {
        // Add new score and keep list sorted and trimmed to top 10
        _highScores.Add(score);
        _highScores = _highScores.OrderByDescending(x => x.Score).ToList();

        // Remove scores beyond top 10
        if (_highScores.Count > MAX_SCORES)
            _highScores = _highScores.Take(MAX_SCORES).ToList();
    }

    public void DrawHighScores(SpriteBatch spriteBatch, float x, float y)
    {
        // Draw ranked list of high scores on screen
        float scale = 1.8f;

        for (int i = 0; i < MAX_SCORES; i++)
        {
            string scoreText;
            Color textColor;

            // Draw score entry or placeholder
            if (i < _highScores.Count)
            {
                HighScore score = _highScores[i];
                scoreText = $"{i + 1}. {score.PlayerName,-15} {score.Score}";
                textColor = Color.White;
            }
            else
            {
                scoreText = $"{i + 1}. --";
                textColor = Color.Gray;
            }

            // Draw with increased spacing between entries
            spriteBatch.DrawString(_font, scoreText, new Vector2(x, y + i * 50), textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }

    public List<HighScore> GetHighScores()
    {
        // Return copy of high scores list
        return new List<HighScore>(_highScores);
    }
}