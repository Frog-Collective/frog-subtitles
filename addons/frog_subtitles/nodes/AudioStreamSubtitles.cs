using Godot;

namespace FrogSubtitle;

public partial class AudioStreamSubtitles : StreamSubtitles
{
    [Export] private AudioStreamPlayer? player;

    public override void _Process(double delta)
    {
        if (this.player.Playing)
        {
            this.UpdateContent(this.player.GetPlaybackPosition());
        }
    }
}
