using Godot;
using System;
using Godot.Collections;

public partial class Emitter : Node2D
{
    [Export] public Godot.Collections.Array<Texture2D> Textures { get; set; }
    [Export] public float ProjectileMaxLifetime { get; set; } = 5.0f;
    
    public Rect2 BoundingBox = new Rect2();

    private Array<Projectile> _projectiles = new Array<Projectile>();
    private Area2D _sharedArea;
   
    private Color _debugColor = Color.Color8(0,0,0); 
    
    public override void _Ready()
    {
        _sharedArea = GetNode<Area2D>("SharedArea");

        Array<Color> debugColors = new Array<Color>() { Colors.Azure, Colors.Blue, Colors.Lime, Colors.ForestGreen, Colors.Violet};
        _debugColor = debugColors[(int)GD.Randi() % debugColors.Count];
    }

    public override void _PhysicsProcess(double delta)
    {
        Transform2D transform2D = new Transform2D();
        Array<Projectile> projectilesMarkedForDeletion = new Array<Projectile>();

        int i = 0;
        foreach (var p in _projectiles)
        {
            // filter out projectiles that should be destroyed
            if (p.Lifetime >= ProjectileMaxLifetime || !BoundingBox.HasPoint(p.Position))
            {
                projectilesMarkedForDeletion.Add(p);
                continue;
            }
            
            // calculate new position
            Vector2 offset = p.Direction.Normalized() * p.Speed * (float)delta;

            p.Position += offset;
            transform2D.Origin = p.Position;
            // While the creation/destruction process of shapes created by the physics server is done directly via the
            // generated resource id, actually "moving" it inside the area requires us to use the offset number under
            // which it was registered (think of it as the child offset, if we were using CollisionShape2D nodes).
            // That's why we pass "i" as the second parameter to the method, and why It is very important to ensure a
            // consistent order between the registration offset and the bullet's offset inside the array, otherwise
            // detection shapes can overlap.
            PhysicsServer2D.AreaSetShapeTransform(_sharedArea.GetRid(), i, transform2D);
            p.Lifetime += (float)delta;
            i++;
        }
        
        // delete all marked projectiles before redraw
        foreach (var p in projectilesMarkedForDeletion)
        {
            PhysicsServer2D.FreeRid(p.ShapeId);
            _projectiles.Remove(p); // method acts in-place and doesn't return a value.
        }
        
        QueueRedraw(); // former Update()
    }

    public override void _Draw()
    {
        foreach (var p in _projectiles)
        {
            Vector2 offset = Textures[0].GetSize() / 2.0f;
            DrawTexture(Textures[0], p.Position - offset);
        }
        DrawRect(BoundingBox, _debugColor, false, 1.0f);
    }

    public void SpawnProjectile()
    {
        var p1 = RegisterProjectile(this.GlobalPosition, Vector2.Down, 1000.0f);
        ConfigureProjectile(p1);
        _projectiles.Add(p1);
        
        var p2 = RegisterProjectile(this.GlobalPosition, Vector2.Up, 1000.0f);
        ConfigureProjectile(p2);
        _projectiles.Add(p2);
        
        var p3 = RegisterProjectile(this.GlobalPosition, Vector2.Right, 1000.0f);
        ConfigureProjectile(p3);
        _projectiles.Add(p3);
        
        var p4 = RegisterProjectile(this.GlobalPosition, Vector2.Left, 1000.0f);
        ConfigureProjectile(p4);
        _projectiles.Add(p4);
    }

    private void ConfigureProjectile(Projectile projectile)
    {
        Transform2D transform = new Transform2D(0, this.Position);
        transform.Origin = projectile.Position;
        
        // set up collision-shape
        Rid shapeId = PhysicsServer2D.CircleShapeCreate();
        PhysicsServer2D.ShapeSetData(shapeId, 8);
        PhysicsServer2D.AreaAddShape(_sharedArea.GetRid(), shapeId, transform);
        // 
        projectile.ShapeId = shapeId;

    }

    Projectile RegisterProjectile(Vector2 initialPosition, Vector2 initialDirection, float initialSpeed)
    {
        return new Projectile(initialPosition, initialDirection, initialSpeed);
    }
}

public partial class Projectile : GodotObject
{
    public Vector2 Position;
    public Vector2 Direction;
    public float Speed;
    public Rid ShapeId;
    public float Lifetime = 0.0f;
    

    public Projectile(Vector2 position, Vector2 direction, float speed)
    {
        Position = position;
        Direction = direction;
        Speed = speed;
    }
}