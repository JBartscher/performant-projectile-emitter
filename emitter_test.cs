using Godot;
using System;

public partial class emitter_test : Node2D
{
    public override void _Ready()
    {
        Timer timer = GetNode<Timer>("Timer");

        var e1 = GetNode<Emitter>("Emitter");
        var e2 = GetNode<Emitter>("Emitter2");
        var e3 = GetNode<Emitter>("Emitter3");
        var e4 = GetNode<Emitter>("Emitter4");

        e1.BoundingBox = GetViewportRect().Grow(2.0f);
        e2.BoundingBox = GetViewportRect().Grow(2.0f);
        e3.BoundingBox = GetViewportRect().Grow(2.0f);
        e4.BoundingBox = GetViewportRect().Grow(2.0f);
        
        timer.Timeout += () =>
        {
            GD.Print("Timeout!");
            
            e1.SpawnProjectile();
            e2.SpawnProjectile();
            e3.SpawnProjectile();
            e4.SpawnProjectile();
        };
    }
    
}
