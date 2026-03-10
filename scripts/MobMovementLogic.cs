using Godot;
using System;

public partial class MobMovementLogic : CharacterBody3D
{
	public const float Speed = 3.0f;
	//public const float JumpVelocity = 0f;

	Vector3 direction;
	Player player;
	public bool chasingPlayer = false;

    public override void _Ready()
    {
        player = GetNode<Player>("/root/Map/PlayerNode/Player");
    }



	public override void _PhysicsProcess(double delta)
	{

		if(chasingPlayer)
        {

            Vector3 velocity = Velocity;

            // Add the gravity.
            //if (!IsOnFloor())
            //{
            //    velocity += GetGravity() * (float)delta;
            //}

            //// Handle Jump.
            //if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            //{
            //	velocity.Y = JumpVelocity;
            //}



            Vector3 direction = player.GlobalPosition - GlobalPosition;
            direction.Y = 0;
            direction = direction.Normalized();


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
            GD.Print("Velocity: " + Velocity.ToString());

            MoveAndSlide();
        }

	}
}
