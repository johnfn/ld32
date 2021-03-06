﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public enum CollisionSide
{
    Top,
    Bottom,
    Right,
    Left
}

public class Collision
{
    /** Side of the collision (relative to the object being collided with) */
    public CollisionSide Side;
    public GameObject Object;

    public override string ToString()
    {
        return String.Format("[Collision] Name: {0} | Side: {1}", Object.name, Side.ToString());
    }
}

public class CollisionModel
{
    public bool TouchingTop;

    public bool JustTouchingTop
    {
        get { return TouchingTop && !OldModel.TouchingTop; }
    }

    public bool TouchingBottom;

    public bool JustTouchingBottom
    {
        get { return TouchingBottom && !OldModel.TouchingBottom; }
    }

    public bool TouchingRight;

    public bool TouchingLeft;

    public bool Resolved;

    public List<Collision> TouchedObjects = new List<Collision>();

    public List<Collision> PreviouslyTouchedObjects
    {
        get
        {
            return OldModel == null ? new List<Collision>() : OldModel.TouchedObjects;
        }
    }

    public CollisionModel OldModel;

    public CollisionModel()
    {
        // Note: Calling Reset() in here will cause infinite recursion. 
    }

    public void Reset()
    {
        OldModel = new CollisionModel
        {
            TouchingTop = TouchingTop,
            TouchingBottom = TouchingBottom,
            TouchingLeft = TouchingLeft,
            TouchingRight = TouchingRight,

            TouchedObjects = TouchedObjects
        };

        TouchingBottom = false;
        TouchingTop = false;
        TouchingRight = false;
        TouchingLeft = false;

        Resolved = false;

        TouchedObjects = new List<Collision>();
    }
}

[RequireComponent(typeof(BoxCollider2D))]
[DisallowMultipleComponent]
public class PhysicsController2D : MonoBehaviour
{
    private BoxCollider2D _collider;

    private Transform _transform;

    public float Width { get { return _collider.bounds.size.x; } }

    public float Height { get { return _collider.bounds.size.y; } }

    public LayerMask WallMask;

    public CollisionModel Collisions;

    private Vector3 _velocity;

    private ControllableStats _stats;

    public Vector3 Velocity
    {
        get
        {
            return _velocity;
        }
    }

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _transform = GetComponent<Transform>();
        _stats = GetComponent<ControllableStats>();

        _velocity = Vector3.zero;

        Collisions = new CollisionModel();
    }

    public void AddHorizontalForce(float force)
    {
        SetHorizontalForce(_velocity.x + force);
    }

    public void SetHorizontalForce(float force) {
        _velocity.x = force;

        var thingsOnTop = Collisions.PreviouslyTouchedObjects.Where(c => c.Side == CollisionSide.Top);

        foreach (var c in thingsOnTop)
        {
            var physics = c.Object.GetComponent<PhysicsController2D>();

            if (physics)
            {
                physics.SetHorizontalForce(force);
            }
        }
    }

    public void AddVerticalForce(float force)
    {
        _velocity.y += force;
    }

    public void SetVerticalForce(float force)
    {
        _velocity.y = force;
    }

    [UsedImplicitly]
	void Update()
    {
        Collisions.Reset();

        ApplyGravity(ref _velocity);
        ApplyFriction(ref _velocity);
        CapVelocity(ref _velocity);

        CheckForCollisions(ref _velocity);

        if (_transform.localScale != Vector3.one)
        {
            Debug.LogError("localScale needs to be 1/1/1 or else collision is sad.");
        }
    }

    void ApplyGravity(ref Vector3 velocity)
    {
        velocity.y -= _stats.Gravity * Time.deltaTime;
    }

    void ApplyFriction(ref Vector3 velocity)
    {
        if (Math.Abs(velocity.x) < _stats.Friction)
        {
            velocity.x = 0;
        }
        else
        {
            velocity.x -= Math.Sign(velocity.x) * _stats.Friction;
        }
    }

    void CapVelocity(ref Vector3 velocity)
    {
        if (Math.Abs(velocity.x) > _stats.MaxHorizontalSpeed)
        {
            velocity.x = Math.Sign(velocity.x) * _stats.MaxHorizontalSpeed;
        }
    }

    void CheckForCollisions(ref Vector3 velocity)
    {
        // Check for vertical collisions
        const int numRays = 8;
        const float skinWidth = .01f;

        foreach (Collision t in Collisions.OldModel.TouchedObjects)
        {
            if (t.Object.tag != "Pushable") continue;

            if (t.Side != CollisionSide.Right && t.Side != CollisionSide.Left)
            {
                continue;
            }

            var otherPhysics = t.Object.GetComponent<PhysicsController2D>();

            otherPhysics.AddHorizontalForce(velocity.x * .99f);

            var vel = otherPhysics.Velocity;
            otherPhysics.CheckForCollisions(ref vel);
        }

        // Note: We shoot out the rays about epsilon away from our actual edge for a complicated
        // reason I don't want to get into right now. 

        if (Math.Abs(velocity.y) > .0001f)
        {
            var firstRayOrigin = transform.position + new Vector3(-Width / 2, Math.Sign(velocity.y) * Height / 2, 0.0f);
            var rayDirection = Vector2.up * Math.Sign(velocity.y);

            for (var i = 0; i < numRays; i++)
            {
                var xOffset = i * (Width - skinWidth * 2) / (numRays - 1) + skinWidth;
                var rayOrigin = firstRayOrigin + new Vector3(xOffset, 0.0f, 0.0f);
                var raycastHits = Physics2D.RaycastAll(rayOrigin, rayDirection, Mathf.Abs(velocity.y), WallMask);

                Debug.DrawRay(rayOrigin, rayDirection * Math.Abs(velocity.y), Color.blue);

                foreach (var hit in raycastHits)
                {
                    if (hit.collider.gameObject == gameObject) continue;

                    var newVelocity = hit.point.y - rayOrigin.y;

                    if (Math.Abs(newVelocity) < Math.Abs(velocity.y))
                    {
                        var otherPhysics = hit.collider.gameObject.GetComponent<PhysicsController2D>();
                        var myCollision = (new Collision
                        {
                            Side = velocity.y > 0 ? CollisionSide.Top : CollisionSide.Bottom,
                            Object = hit.collider.gameObject
                        });

                        Collisions.TouchedObjects.Add(myCollision);

                        if (otherPhysics && !otherPhysics.Collisions.TouchedObjects.Any(t => t.Object == gameObject))
                        {
                            otherPhysics.Collisions.TouchedObjects.Add(new Collision
                            {
                                Side = myCollision.Side == CollisionSide.Top ? CollisionSide.Bottom : CollisionSide.Top,
                                Object = gameObject
                            });
                        }

                        if (velocity.y > 0)
                        {
                            Collisions.TouchingTop = true;
                        }
                        else
                        {
                            Collisions.TouchingBottom = true;
                        }

                        velocity.y = newVelocity;

                        break;
                    }
                }
            }
        }

        // We need to separate x and y translation rather than do them both at the same time afterwards so
        // we check (x, 0) and (x, y) rather than (x, 0) and (0, y) (which would allow us to go diagonally into walls). 

        _transform.Translate(new Vector3(0, velocity.y, 0));

        // Check for horizontal collisions

        if (Math.Abs(velocity.x) > .0001f)
        {
            var firstRayOrigin = transform.position + new Vector3(Math.Sign(velocity.x) * Width / 2, -Height / 2, 0.0f);
            var rayDirection = Vector2.right * Math.Sign(velocity.x);

            for (var i = 0; i < numRays; i++)
            {
                var yOffset = i * (Height - skinWidth * 2) / (numRays - 1) + skinWidth;
                var rayOrigin = firstRayOrigin + new Vector3(0.0f, yOffset, 0.0f);
                var raycastHits = Physics2D.RaycastAll(rayOrigin, rayDirection, Math.Abs(velocity.x), WallMask);

                Debug.DrawRay(rayOrigin + velocity, rayDirection * Math.Abs(velocity.x) * 20, Color.blue);

                foreach (var hit in raycastHits)
                {
                    if (hit.collider.gameObject == gameObject) continue;

                    var newVelocity = hit.point.x - rayOrigin.x;

                    if (Math.Abs(newVelocity) < Math.Abs(velocity.x))
                    {
                        var otherPhysics = hit.collider.gameObject.GetComponent<PhysicsController2D>();
                        velocity.x = newVelocity;
                        Collisions.TouchedObjects.Add(new Collision {
                            Side = velocity.x > 0 ? CollisionSide.Right : CollisionSide.Left,
                            Object = hit.collider.gameObject 
                        });

                        if (otherPhysics && !otherPhysics.Collisions.TouchedObjects.Any(t => t.Object == gameObject))
                        {
                            otherPhysics.Collisions.TouchedObjects.Add(new Collision
                            {
                                Side = velocity.x > 0 ? CollisionSide.Left : CollisionSide.Right,
                                Object = gameObject
                            });
                        }

                        break;
                    }
                }
            }
        }

        _transform.Translate(new Vector3(velocity.x, 0, 0));
    }
}
