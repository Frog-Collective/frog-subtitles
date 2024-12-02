using System.Diagnostics;
using Godot;

namespace Frog;

public abstract partial class StreamSubtitles : RichTextLabel
{
    [Export] private Subtitles? subtitles;

    private SubtitleEntry? currentEntry;
    private string template;

    public override void _Ready()
    {
        Debug.Assert(this.subtitles != null);
        this.template = this.Text;
        this.Text = string.Empty;
    }

    protected void UpdateContent(double currentTime)
    {
        if (this.currentEntry != null && currentTime > this.currentEntry.EndTime)
        {
            this.currentEntry = null;
            this.Text = string.Empty;
        }

        if (this.currentEntry == null)
        {
            // Search for a valid entry...
            foreach (SubtitleEntry entry in this.subtitles.Entries)
            {
                if (currentTime >= entry.StartTime && currentTime <= entry.EndTime)
                {
                    this.currentEntry = entry;
                    break;
                }
            }

            if (this.currentEntry != null)
            {
                if (string.IsNullOrEmpty(this.template))
                {
                    this.Text = this.currentEntry.Content;
                }
                else
                {
                    this.Text = string.Format(this.template, this.currentEntry.Content);
                }
            }
        }
    }
}
