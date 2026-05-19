using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame;

public class Enemy
{
    public Rectangle Bounds { get; private set; }
    public Vector2 Position { get; private set; }
    public bool IsActive { get; set; } = true;
    
    private Texture2D _texture;
    private float _speedWalk = 100f;
    private float _speedChase = 450f;
    private float _currentSpeed;
    private int _width = 900;
    private int _height = 1650;
    
    private float _killRangeX = 150f;   
    
    private float _damageCooldown = 1.0f;
    private float _damageTimer = 0f;
    
    private float _patrolLeft = 700;
    private float _patrolRight = 2500;
    private bool _movingRight = true;
    
    private float _visionRange = 400f;
    
    public Enemy(Texture2D texture, Vector2 startPosition)
    {
        _texture = texture;
        Position = startPosition;
        _currentSpeed = _speedWalk;
        UpdateBounds();
    }
    
    public void Update(GameTime gameTime, Player player, Locker locker)
    {
        if (!IsActive) return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_damageTimer > 0)
            _damageTimer -= deltaTime;
        
        bool canSeePlayer = CheckIfCanSeePlayer(player);
        
        if (canSeePlayer && !player.IsHiding)
        {
            _currentSpeed = _speedChase;
            
            if (player.Position.X > Position.X)
                Position = new Vector2(Position.X + _currentSpeed * deltaTime, Position.Y);
            else
                Position = new Vector2(Position.X - _currentSpeed * deltaTime, Position.Y);
        }
        else
        {
            _currentSpeed = _speedWalk;
            Patrol(deltaTime);
        }
        
        UpdateBounds();
    }
    
    private bool CheckIfCanSeePlayer(Player player)
    {
        float distance = MathHelper.Distance(Position.X, player.Position.X);
        return distance < _visionRange;
    }
    
    private void Patrol(float deltaTime)
    {
        if (_movingRight)
        {
            Position = new Vector2(Position.X + _currentSpeed * deltaTime, Position.Y);
            if (Position.X >= _patrolRight)
                _movingRight = false;
        }
        else
        {
            Position = new Vector2(Position.X - _currentSpeed * deltaTime, Position.Y);
            if (Position.X <= _patrolLeft)
                _movingRight = true;
        }
    }
    
    private void UpdateBounds()
    {
        Bounds = new Rectangle((int)Position.X, (int)Position.Y, _width, _height);
    }
    
    public bool CanDamagePlayer(Player player)
    {
        if (player.IsHiding) return false;  
        if (_damageTimer > 0)
            return false;
        
        float enemyCenterX = Position.X + _width / 2;
        float playerCenterX = player.Position.X + player.Bounds.Width / 2;
        float distanceX = MathHelper.Distance(enemyCenterX, playerCenterX);
        
        if (distanceX < _killRangeX)
        {
            _damageTimer = _damageCooldown;
            return true;
        }
        
        return false;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsActive)
        {
            spriteBatch.Draw(_texture, Bounds, Color.White);
            
            // ДЛЯ ОТЛАДКИ: показываем зону убийства (раскомментируй если нужно)
            // float killZoneWidth = _killRangeX * 2;
            // Rectangle killZone = new Rectangle(
            //     (int)(Position.X + _width / 2 - _killRangeX),
            //     (int)Position.Y,
            //     (int)killZoneWidth,
            //     _height
            // );
            // spriteBatch.Draw(_texture, killZone, Color.Red * 0.3f);
        }
    }
}