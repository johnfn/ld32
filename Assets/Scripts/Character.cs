using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[DisallowMultipleComponent]
public class Character : MonoBehaviour {
    private ControllableStats _stats;
    private PhysicsController2D _physics;
    private CanTakeInput _canTakeInput;
    private HasEnergy _energy;

	void Start() {
        // TODO move friction and vel cap in here, too.

	    _physics = GetComponent<PhysicsController2D>();
	    _stats = GetComponent<ControllableStats>();
	    _canTakeInput = GetComponent<CanTakeInput>();
	    _energy = GetComponent<HasEnergy>();
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

	void Update()
	{
	    var collision = GetComponent<PhysicsController2D>().Collisions;

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
