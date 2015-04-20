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

    private FollowText _followText;

    private bool _currentlyBeingLaunched = false;

    private float _storedFriction;

    private UnconventionalGun _gun;

    private bool _trolledYet = false;

    void Awake()
    {
	    _physics = GetComponent<PhysicsController2D>();
	    _stats = GetComponent<ControllableStats>();
	    _canTakeInput = GetComponent<CanTakeInput>();
	    _energy = GetComponent<HasEnergy>();
        _followText = GetComponent<FollowText>();
        _gun = GetComponent<UnconventionalGun>();
    }

    [UsedImplicitly]
	void Start() {
        // TODO move friction and vel cap in here, too.

        _energy.Dead += Die;

        if (!Manager.Instance.Debug && !_trolledYet)
        {
            Manager.Instance.Dialog.ShowDialog(Dialogs.YoureDumb);

            _trolledYet = true;
        }
	}

    // Really the only way this gets called is if you just launched one.
    public void Init()
    {
        _currentlyBeingLaunched = true;
        _storedFriction = _stats.Friction;

        _followText.SayText(Util.RandomElem(new List<string> {
            "Wheee!",
            "Hello world!",
            "I'm ALIVE!!!",
            "Aaaahhhh!",
            "Ready to go!",
            "Whoa! Scary!",
            "=D =D =D =D",
            "Woohoo!",
            "Yikes!",
            "Look out below!"
        }));
    }

    public void SaySuckFlavorText()
    {
        _followText.SayText(Util.RandomElem(new List<string> {
            "You suck! Literally... hehe",
            "You're such a sucker!",
            "I am being assimilated o_o",
            "Ahhh!",
            "I don't wanna die!",
            "You suck a lot!",
            "For the greater good!",
            "Whoa!"
        }), false);
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
	        CheckForProfessor();
	    }

	    CheckForPowerupCollision(collision);
	}

    private void CheckForProfessor()
    {
        var profs = Professor.Professors;

        foreach (var p in profs)
        {
            if (p.HasTalked) return;

            var dist = (p.transform.position - transform.position).magnitude;

            if (dist < 0.5)
            {
                if (!_gun.enabled)
                {
                    Manager.Instance.Dialog.ShowDialog(Dialogs.GiveGun);

                    _gun.enabled = true;
                    p.HasTalked = true;
                }
            }
        }
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
