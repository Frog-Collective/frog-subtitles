#if TOOLS
using Godot;

namespace Frog;

[Tool]
public partial class SubtitlesPlugin : EditorPlugin
{
    private SubtitlesImportPlugin? importPlugin;

    public override void _EnterTree()
    {
        Script videoStreamSubtitlesScript = GD.Load<Script>("res://addons/frog_subtitles/nodes/VideoStreamSubtitles.cs");
        Texture2D videoStreamSubtitlesIcon = GD.Load<Texture2D>("res://addons/frog_subtitles/icons/video_subtitles.svg");
        this.AddCustomType(nameof(VideoStreamSubtitles), nameof(RichTextLabel), videoStreamSubtitlesScript, videoStreamSubtitlesIcon);

        Script audioStreamSubtitlesScript = GD.Load<Script>("res://addons/frog_subtitles/nodes/AudioStreamSubtitles.cs");
        Texture2D audioStreamSubtitlesIcon = GD.Load<Texture2D>("res://addons/frog_subtitles/icons/audio_subtitles.svg");
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
