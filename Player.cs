using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace HorrorGame;

public class Player
{
    public Rectangle Bounds { get; private set; }
    public Vector2 Position { get; set; }
    public bool IsHiding { get; private set; } = false;
    
    private SoundEffect _hideSound;
    private SoundEffect _footstepSound;
    
    private Texture2D _idleTexture;
    private Texture2D _walkTexture;
    private Texture2D _currentTexture;
    
    private float _walkSpeed = 200f;
    private float _runSpeed = 400f;
    private float _currentSpeed;
    public bool IsFacingRight => _facingRight;
    
    private float _stamina = 100f;
    private float _maxStamina = 100f;
    private float _staminaDrain = 30f;  
    private float _staminaRegen = 20f;  
    
    private int _width;
    private int _height;
    private Locker _currentLocker;
    private bool _facingRight = true;
    private KeyboardState _lastKeyboardState;
    
    private float _footstepTimer = 0f;
    private readonly float _stepInterval = 0.6f;
    
    public float Stamina => _stamina;
    public float MaxStamina => _maxStamina;
    
    public bool CheckCollisionWithEnemy(Enemy enemy)
    {
        return enemy.CanDamagePlayer(this);
    }

    public void TakeDamage()
    {
        if (IsHiding)
        {
            ExitLocker();
        }
        Position = new Vector2(60, 160);
        UpdateBounds();
    }

    public Player(Texture2D idleTexture, Texture2D walkTexture, 
                  Vector2 startPosition, int width, int height, 
                  SoundEffect hideSound, SoundEffect footstepSound)
    {
        _idleTexture = idleTexture;
        _walkTexture = walkTexture;
        _currentTexture = idleTexture;
        Position = startPosition;
        _width = width;
        _height = height;
        _hideSound = hideSound;
        _footstepSound = footstepSound;
        _currentSpeed = _walkSpeed;
        UpdateBounds();
    }
    
    public void Update(GameTime gameTime, Locker locker, int worldWidth)
    {
        KeyboardState keyboard = Keyboard.GetState();
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        bool isEPressed = keyboard.IsKeyDown(Keys.E) && _lastKeyboardState.IsKeyUp(Keys.E);
        
        // Обновление выносливости
        UpdateStamina(deltaTime, keyboard);

        if (IsHiding)
        {
            if (isEPressed)
            {
                _hideSound?.Play();
                ExitLocker();
            }
        
            _lastKeyboardState = keyboard;
            return;
        }

        if (isEPressed && locker != null) 
        {
            float distance = Math.Abs((Bounds.X + Bounds.Width / 2) - (locker.Bounds.X + locker.Bounds.Width / 2));

            if (distance < 100 && !locker.IsOccupied)
            {
                _hideSound?.Play();
                EnterLocker(locker);
                _lastKeyboardState = keyboard;
                return; 
            }
        }

        float moveX = 0;
        bool isMoving = false;

        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
        {
            moveX = -1;
            isMoving = true;
            _facingRight = false;
        }
        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
        {
            moveX = 1;
            isMoving = true;
            _facingRight = true;
        }
        
        // Бег (Shift)
        bool isRunning = keyboard.IsKeyDown(Keys.LeftShift) && _stamina > 0 && isMoving;
        _currentSpeed = isRunning ? _runSpeed : _walkSpeed;

        // Звук шагов (быстрее при беге)
        if (isMoving && !IsHiding)
        {
            float stepInterval = isRunning ? 0.2f : 0.4f;
            _footstepTimer += deltaTime;
            if (_footstepTimer >= stepInterval)
            {
                _footstepTimer = 0f;
                _footstepSound?.Play();
            }
        }
        else
        {
            _footstepTimer = 0f;
        }

        _currentTexture = isMoving ? _walkTexture : _idleTexture;

        float newX = Position.X + moveX * _currentSpeed * deltaTime;
        newX = MathHelper.Clamp(newX, 20, worldWidth - _width - 20);
    
        Position = new Vector2(newX, Position.Y);
        UpdateBounds();
        _lastKeyboardState = keyboard;
    }
    
    private void UpdateStamina(float deltaTime, KeyboardState keyboard)
    {
        bool isRunning = keyboard.IsKeyDown(Keys.LeftShift);
        bool isMoving = keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.D) ||
                        keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.Right);
        
        if (isRunning && isMoving && !IsHiding && _stamina > 0)
        {
            // Тратим выносливость при беге
            _stamina -= _staminaDrain * deltaTime;
            _stamina = MathHelper.Clamp(_stamina, 0, _maxStamina);
        }
        else
        {
            // Восстанавливаем выносливость
            _stamina += _staminaRegen * deltaTime;
            _stamina = MathHelper.Clamp(_stamina, 0, _maxStamina);
        }
    }
    
    private void EnterLocker(Locker locker)
    {
        IsHiding = true;
        _currentLocker = locker;
        locker.IsOccupied = true;
        UpdateBounds();
    }
    
    private void ExitLocker()
    {
        IsHiding = false;
        if (_currentLocker != null)
        {
            _currentLocker.IsOccupied = false;

            Position = new Vector2(
                _currentLocker.Bounds.X + (_currentLocker.Bounds.Width - _width) / 2 + 20,
                _currentLocker.Bounds.Y + (_currentLocker.Bounds.Height - _height) / 2 + 80
            );

            UpdateBounds();
            _currentLocker = null;
        }
    }
    
    public void UpdateBounds()
    {
        Bounds = new Rectangle((int)Position.X, (int)Position.Y, _width, _height);
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsHiding) return;

        SpriteEffects effect = _facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(_currentTexture, Bounds, null, Color.White, 0f, Vector2.Zero, effect, 0f);
    }
}