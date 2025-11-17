Project: group_12_assignment7 - Millennium Falcon: Tie Fighter Assault

1. Game Description

This project is a 2D top-down space shooter implemented using MonoGame. The player controls the iconic Millennium Falcon and must survive waves of enemy Tie Fighters while utilizing a mouse-based aiming system. The game features multiple screens (Title, High Scores, Game Over, Pause) and a scoring system.

2. How to Run the Game

    Prerequisites: Ensure you have the MonoGame SDK and the .NET runtime installed.

    Open Project: Open the group_12_assignment7.csproj file in Visual Studio Code.

    Build: Build the solution to ensure the Content Pipeline runs and all assets are compiled.

    Run: Execute the project (F5 or Run button).

3. Game Controls

The game uses a minimum of five inputs (WASD + Mouse Click), meeting the assignment requirements.

Player Controls (Keyboard/Mouse)

    W: Move Falcon Up

    A: Move Falcon Left

    S: Move Falcon Down

    D: Move Falcon Right

    Left Mouse Button: Fire Laser (Aims at cursor position)

    P: Pause / Unpause Game (Extra Credit Feature)

Navigation (UI)

    SPACE / ENTER: Start Game (from Title Screen), Play Again (from Game Over).

    Mouse Click: Interact with on-screen buttons (Start, Instructions, Back).

    ESC: Exit the game (when not in the Playing state).

4. Win and Lose Conditions

The game incorporates a time-based challenge and health management to determine victory or defeat.

Win Condition: Defeat Tie Fighters. A Victory screen will display.

Lose Condition: Damage is taken from enemy laser fire and collisions with Tie Fighters. A Defeat screen will display.

5. Project Structure (Core Classes)

The game logic is primarily driven by three custom classes:

Game1.cs (Phuc Dinh): The main game loop handles state management (GameState enum), UI drawing (HUD, Menus), enemy spawning (via a timer), and all collision detection between objects.

Falcon.cs (Samuel Suh): Manages the player's ship, including position, health, WASD movement input, cooldown-based mouse shooting, and managing its collection of friendly Laser projectiles.

TieFighter.cs (James Woo - To Be Implemented): Manages enemy ships, including movement toward the Falcon, health, randomized shooting cooldowns, and managing its collection of enemy Laser projectiles.

Laser.cs (Utility): A lightweight utility class used by both Falcon and TieFighter to track projectile movement, position, and collision area.