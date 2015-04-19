using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

[DisallowMultipleComponent]
public class Character : MonoBehaviour {
    private ControllableStats _stats;

    private PhysicsController2D _physics;

    private CanTakeInput _canTakeInput;

    private HasEnergy _energy;

    private bool _currentlyBeingLaunched = false;

    private float _storedFriction;

    void Awake()
    {
	    _physics = GetComponent<PhysicsController2D>();
	    _stats = GetComponent<ControllableStats>();
	    _canTakeInput = GetComponent<CanTakeInput>();
	    _energy = GetComponent<HasEnergy>();
    }

    [UsedImplicitly]
	void Start() {
        // TODO move friction and vel cap in here, too.

        _energy.Dead += Die;
	}

    public void Init()
    {
        _currentlyBeingLaunched = true;
        _storedFriction = _stats.Friction;
    }

    void Die()
    {
        Debug.Log("Uh oh... I died.");
    }
	
    void GetInput()
    {
	    var collision = GetComponent<PhysicsController2D>().Collisions;
        var horizontalForce = 0.0f;
        var velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontalForce = -1.0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontalForce = 1.0f;

        velocity.x += horizontalForce * _stats.HorizontalSpeed * Time.deltaTime;

        if (collision.TouchingBottom && Input.GetKey(KeyCode.Space))
        {
            _physics.SetVerticalForce(_stats.JumpHeight / 60);
        }

        if (!collision.TouchingBottom && !Input.GetKey(KeyCode.Space) && velocity.y > 0)
        {
            _physics.SetVerticalForce(0);
        }

        _physics.AddHorizontalForce(velocity.x);
        _physics.AddVerticalForce(velocity.y);
    }

    private void AbsorbNearbyGuys(CollisionModel collision)
    {
        var guys = collision.TouchedObjects
            .Where(t => t.Object.GetComponent<CanTakeInput>() != null)
            .ToList();

        foreach (var guy in guys)
        {
            var energy = guy.Object.GetComponent<HasEnergy>();

            // Order here is important
            _energy.AddTotalEnergy(energy.HalfBatteriesTotal);
            _energy.AddEnergy(energy.HalfBatteriesLeft);

            Destroy(guy.Object);
        }
    }

    private void TurnOffFrictionWhileFlying(CollisionModel collision)
    {
	    if (_currentlyBeingLaunched)
	    {
	        _stats.Friction = 0;
	    }

	    if (collision.TouchingBottom && _currentlyBeingLaunched)
	    {
            _currentlyBeingLaunched = false;
	        _stats.Friction = _storedFriction;
	    }
    }

	void Update()
	{
	    var collision = GetComponent<PhysicsController2D>().Collisions;

	    TurnOffFrictionWhileFlying(collision);

	    if (_canTakeInput.ActivelyTakingInput)
	    {
	        GetInput();
	        AbsorbNearbyGuys(collision);
	    }

	    CheckForPowerupCollision(collision);
	}

    private void CheckForPowerupCollision(CollisionModel collision)
    {
        var powerup = collision.TouchedObjects
            .FirstOrDefault(t => t.Object.tag == "Powerup");

        if (powerup == null)
        {
            return;
        }

        var powerupObj = powerup.Object;

        _energy.AddTotalEnergy(2);
        _energy.AddEnergy(2);

        Destroy(powerupObj);
    }
}
