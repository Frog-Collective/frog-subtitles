#if TOOLS
using Godot;

namespace FrogSubtitle;

[Tool]
public partial class SubtitlesPlugin : EditorPlugin
{
    private SubtitlesImportPlugin importPlugin;

    public override void _EnterTree()
    {
        var videoStreamSubtitlesScript = GD.Load<Script>("res://addons/FrogSubtitles/Nodes/VideoStreamSubtitles.cs");
        var videoStreamSubtitlesIcon = GD.Load<Texture2D>("res://addons/FrogSubtitles/Icons/VideoSubtitles.svg");
        this.AddCustomType(nameof(VideoStreamSubtitles), nameof(RichTextLabel), videoStreamSubtitlesScript, videoStreamSubtitlesIcon);
		
        var audioStreamSubtitlesScript = GD.Load<Script>("res://addons/FrogSubtitles/Nodes/AudioStreamSubtitles.cs");
        var audioStreamSubtitlesIcon = GD.Load<Texture2D>("res://addons/FrogSubtitles/Icons/AudioSubtitles.svg");
        this.AddCustomType(nameof(AudioStreamSubtitles), nameof(RichTextLabel), audioStreamSubtitlesScript, audioStreamSubtitlesIcon);

        this.importPlugin = new SubtitlesImportPlugin();
        this.AddImportPlugin(this.importPlugin);
    }

    public override void _ExitTree()
    {
        this.RemoveImportPlugin(this.importPlugin);
        this.importPlugin = null;

        this.RemoveCustomType(nameof(AudioStreamSubtitles));
        this.RemoveCustomType(nameof(VideoStreamSubtitles));
    }
}
#endif
