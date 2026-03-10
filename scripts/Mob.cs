using Godot;
using System;

public partial class Mob : Node3D
{

    [Export]
    private Timer chaseTimer;

    [Export]
    private Timer deathTimer;

    private Player player;
    private CharacterBody3D mob;


    // NPC states
    enum States
    {
        IDLE,
        CHASE,
        WAIT,
        DEAD
    }

    private States currentState;

    private bool playerIsInChaseArea = false;
    private bool isTouchingPlayer = false;

    private float movementSpeed = 3.0f;



    public override void _Ready()
    {
        player = GetNode<Player>("../PlayerNode/Player");


        currentState = States.IDLE;



    }





    // Process

    public override void _PhysicsProcess(double delta)
    {
        

        // Calls appropriate functions based on state
        switch(currentState)
        {
            case States.IDLE:
                // potential idle logic? maybe?
                break;
            case States.CHASE:
                if(isTouchingPlayer)
                {
                    SwitchState(States.WAIT);
                }
                else
                {
                    ChasePlayer(delta);
                }
                break;
            case States.WAIT:
                if (chaseTimer.IsStopped())
                {
                    if (playerIsInChaseArea)
                    {
                        SwitchState(States.CHASE);
                    }
                    else
                    {
                        GD.Print("Chasing after waiting");
                        SwitchState(States.IDLE);
                    }
                }
                break;
            case States.DEAD:
                if (deathTimer.IsStopped())
                {
                    //  death logic
                }
                break;

        }


    }













    private void SwitchState(States newState)
    {

        switch(newState)
        {
            case States.IDLE:
                currentState = newState;
                break;
            case States.WAIT:
                if (currentState != newState)
                {
                    currentState = newState;
                    player.onHurt();
                    chaseTimer.Start();
                    GD.Print("Player touched!");
                }
                break;
            case States.CHASE:
                currentState = newState;
                break;
            case States.DEAD:
                currentState = newState;
                deathTimer.Start();
                break;
        }
    }







    // **************
    //
    //  BEHAVIOUR
    //
    // **************



    private void ChasePlayer(double delta)
    {
        GD.Print("Chasing player at:" + player.GlobalPosition.ToString());

        if (player == null)
        { 
            return;
        }

        Vector3 direction = player.GlobalPosition - GlobalPosition;
        direction.Y = 0;
        direction = direction.Normalized();

        GlobalPosition += direction * movementSpeed * (float)delta;


    }





























    // **************
    //
    //  On actions
    //
    // **************

    // Aggro area
    public void OnPlayerEnterArea(Node3D body)
    {
        if (body == player && currentState != States.DEAD)
        {
            GD.Print("Player entered follow area!");
            SwitchState(States.CHASE);
            playerIsInChaseArea = true;
        }
    }

    public void OnPlayerExitArea(Node3D body)
    {
        if (body == player && currentState != States.DEAD)
        {
            GD.Print("Player exited follow area!");
            SwitchState(States.IDLE);
            playerIsInChaseArea = false;
        }
    }


    // Hits
    public void OnPlayerHit(Node3D body)
    {
        if (body == player && currentState != States.DEAD)
        {
            isTouchingPlayer = true;
            SwitchState(States.WAIT);
        }
    }

    // This is to avoid endless chasing if the player remains in touch with the mob
    public void OnPlayerExitAfterHit(Node3D body)
    {
        if (body == player && currentState != States.DEAD)
        {
            GD.Print("Not touching player");
            isTouchingPlayer = false;
        }
    }

    public void OnPlayerJumpOnHead(Node3D body)
    {
        if (body == player && currentState != States.DEAD)
        {
            GD.Print("Player jumped on head!");
            SwitchState(States.DEAD);
        }
    }

}
