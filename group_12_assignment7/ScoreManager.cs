using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace group_12_assignment7;

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

    public override string ToString()
    {
        return $"{PlayerName}|{Score}|{DateAchieved:yyyy-MM-dd HH:mm:ss}";
    }
}

public class ScoreManager
{
    private List<HighScore> _highScores;
    private string _filePath;
    private SpriteFont _font;
    private const int MAX_SCORES = 10;

    public ScoreManager(string filePath, SpriteFont font)
    {
        _filePath = filePath;
        _font = font;
        _highScores = new List<HighScore>();
    }

    public void LoadScores()
    {
        _highScores.Clear();

        if (!File.Exists(_filePath))
            return;

        try
        {
            string[] lines = File.ReadAllLines(_filePath);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

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

            // Sort by score descending
            _highScores = _highScores.OrderByDescending(x => x.Score).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading scores: {ex.Message}");
        }
    }

    public void SaveScores()
    {
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
        if (_highScores.Count < MAX_SCORES)
            return true;

        return score > _highScores[MAX_SCORES - 1].Score;
    }

    public void AddScore(HighScore score)
    {
        _highScores.Add(score);
        _highScores = _highScores.OrderByDescending(x => x.Score).ToList();

        // Keep only top 10
        if (_highScores.Count > MAX_SCORES)
            _highScores = _highScores.Take(MAX_SCORES).ToList();
    }

    // REMOVED the title drawing from this method - now only draws the scores list
    // Title is handled in Game1.cs DrawHighScoresScreen() to avoid duplication
    public void DrawHighScores(SpriteBatch spriteBatch, float x, float y)
    {
        // INCREASED SCALE from 1.0f to 1.8f for better readability
        float scale = 1.8f;
        
        for (int i = 0; i < MAX_SCORES; i++)
        {
            string scoreText;
            Color textColor;

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

            // INCREASED SPACING from 30 to 50 pixels between entries
            spriteBatch.DrawString(_font, scoreText, new Vector2(x, y + i * 50), textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }

    public List<HighScore> GetHighScores()
    {
        return new List<HighScore>(_highScores);
    }
}