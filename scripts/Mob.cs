using Godot;
using System;
using System.Collections.Generic;
using System.Security;

public partial class Mob : Node3D
{
    // Delegates and observer types
    public delegate void MobEvent(Mob mob);

    public MobEvent OnIdle;
    public MobEvent OnChase;
    public MobEvent OnWait;
    public MobEvent OnMobDeath;

    public int mobPoolIndex;



    [Export]
    private Timer chaseTimer;

    [Export]
    private Timer deathTimer;

    private Player player;
    //private CharacterBody3D mob;


    // NPC states
    enum States
    {
        IDLE,
        CHASE,
        WAIT,
        DEAD
    }

    private States currentState;

    private bool playerIsInChaseArea;
    private bool isTouchingPlayer;

    private MobMovementLogic mobMovement;



    public override void _Ready()
    {
        player = GetNode<Player>("/root/Map/PlayerNode/Player");
        mobMovement = GetNode<MobMovementLogic>("CharacterBody3D");
    }

    // ***************************
    //
    //  SPAWN LOGIC
    //
    // ***************************
    public void SpawnMob()
    {
        float minSpawnDistance = 5f;
        float maxSpawnDistance = 10f;

        float distanceFromPlayer = (float)GD.RandRange(minSpawnDistance, maxSpawnDistance);
        Vector3 spawnPosition;

        // Now to check is spawnpoint is empty

        while (true)
        {
            GD.Print("Trying to find position");
            Vector3 directionFromPlayer = new Vector3((float)GD.RandRange(-1, 1), 0f, (float)GD.RandRange(-1, 1)).Normalized();

            spawnPosition = player.GlobalPosition + distanceFromPlayer * directionFromPlayer;
            spawnPosition.Y = 1f;

            if (CheckCollisionAtPoint(spawnPosition))
            {
                GD.Print("Found position!");
                break;
            }
        }


        GlobalPosition = spawnPosition;


        playerIsInChaseArea = false;
        isTouchingPlayer = false;
        currentState = States.IDLE;
    }

    private bool CheckCollisionAtPoint(Vector3 point)
    {

        CollisionShape3D bodyCollisionShape = GetNode<CollisionShape3D>("../Mob/CharacterBody3D/Body/Area3D/CollisionShape3D");
        Shape3D bodyShape = bodyCollisionShape.Shape;
        Transform3D bodyTransform = bodyCollisionShape.Transform;

        // Create query with the properties above
        PhysicsShapeQueryParameters3D bodyQuery = new PhysicsShapeQueryParameters3D();
        bodyQuery.Shape = bodyShape;
        bodyQuery.Transform = new Transform3D(bodyCollisionShape.GlobalTransform.Basis, point);
        bodyQuery.CollideWithBodies = true;
        bodyQuery.CollideWithAreas = true;

        // Check for collision
        PhysicsDirectSpaceState3D currentSpaceState = GetWorld3D().DirectSpaceState;
        Godot.Collections.Array<Godot.Collections.Dictionary> collisions = currentSpaceState.IntersectShape(bodyQuery, maxResults: 1);

        return collisions.Count == 0;
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
                        SwitchState(States.IDLE);
                    }
                }
                break;
            case States.DEAD:
                if (deathTimer.IsStopped())
                {
                    if (OnMobDeath != null)
                    {
                        OnMobDeath(this);
                    }
                }
                break;

        }

        GD.Print(mobMovement.chasingPlayer.ToString());

    }




    private void SwitchState(States newState)
    {

        switch(newState)
        {
            case States.IDLE:
                currentState = newState;
                mobMovement.chasingPlayer = false;
                break;
            case States.WAIT:
                if (currentState != newState)
                {
                    currentState = newState;
                    player.onHurt();
                    chaseTimer.Start();
                    mobMovement.chasingPlayer = false;
                    GD.Print("Player touched!");
                }
                break;
            case States.CHASE:
                currentState = newState;
                if (player != null)
                {
                    mobMovement.chasingPlayer = true;
                }
                break;
            case States.DEAD:
                currentState = newState;
                mobMovement.chasingPlayer = false;
                deathTimer.Start();
                break;
        }
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
