using Godot;
using System;

public partial class Player : CharacterBody3D
{

	// Delegates
	public delegate void PlayerEvent();

	public PlayerEvent onJump;
	public PlayerEvent onHurt;



	private int maxHP = 3;
	private int currentHP;





    public override void _Ready()
    {
		onHurt = TakeDamage;


		currentHP = maxHP;
    }

















	// ****************
	//
	//	PHYSICS
	//
	// ****************


	// Other
	public const float Speed = 5.0f;
	public const float JumpVelocity = 5f;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;


			// Notify observers
			if(onJump != null)
			{
                onJump();
			}
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}














	private void TakeDamage()
	{
		currentHP -= 1;

		// TODO end game if hp is 0


	}









}
