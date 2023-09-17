using Godot;
using System;
using Godot.Collections;

public partial class main : Node2D
{

    private AudioStreamPlayer audioPlayer;
    
    
    public override void _Ready()
    {
        audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        audioPlayer.Stream = ResourceLoader.Load<AudioStreamWav>("res://Assets/Sounds/cursor-move.wav");
        Level.Instance.MouseChangedPosition += HandleMouseMove;
    }

    private void HandleMouseMove(Vector2I gridPos)
    {
        audioPlayer.Play(); // TODO: I just wanted to here sounds for now, but this class should not be responsible for audio
    }
}
