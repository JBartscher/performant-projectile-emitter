using System;
using Godot;

namespace PerformantPhysicsEntities;

public delegate void OnLifetimeBeginEventHandler();
public delegate void OnDestroyEventHandler();

public interface IProjectile
{ 
    Vector2 Position { get; set; } 
    Vector2 Direction  { get; set; } 
    float Speed { get; set; } 
    Rid ShapeId { get; set; } 
    float Lifetime  { get; set; }

    event OnLifetimeBeginEventHandler LifetimeBegin;  
    event OnDestroyEventHandler Destroy;  
}