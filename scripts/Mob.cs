using Godot;
using System;

public partial class Mob : Node3D
{

    private Player player;






    public override void _Ready()
    {
        player = GetNode<Player>("../PlayerNode/Player");
    }







    public void OnPlayerEnterArea(Node3D body)
    {
        if (body == player)
        {
            GD.Print("Player entered follow area!");
        }
    }

    public void OnPlayerExitArea(Node3D player)
    {
        GD.Print("Player exited follow area!");
    }

    public void OnPlayerHit(Node3D player)
    {
        GD.Print("Player touched!");
    }

    public void OnPlayerJumpOnHead(Node3D player)
    {
        GD.Print("Player hit on head!");
    }

}
