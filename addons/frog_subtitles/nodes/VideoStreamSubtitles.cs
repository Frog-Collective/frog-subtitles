using Godot;

namespace Frog;

public partial class VideoStreamSubtitles : StreamSubtitles
{
    [Export] private VideoStreamPlayer? player;

    public override void _Process(double delta)
    {
        if (this.player.Visible && this.player.IsPlaying())
        {
            this.UpdateContent(this.player.StreamPosition);
        }
    }
}
