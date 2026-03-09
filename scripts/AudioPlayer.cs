using Godot;
using System;


public partial class AudioPlayer : AudioStreamPlayer
{
    [Export]
    private Player player;







    public override void _Ready()
    {
        player.onJump += PlayJumpSound;
    }




   



    private void PlayJumpSound()
    {
        GD.Print("Jump SOUND");
    }


}
