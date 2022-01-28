using DSharpPlus;
using DiscordLayer;
using DSharpPlus.Entities;

namespace Memester;
public class PipedPiper
{
    private Client client;
    private bool isRunning = false;
    private (ulong from, List<ulong> dest) pipes;
    public void Start() => isRunning = true;
    public void Stop() => isRunning = false;
    private PipedPiper(ulong from, List<ulong> dest) 
        => this.pipes = (from, dest);
    public static async Task<PipedPiper> Create(ulong from , List<ulong> dest = null) {
        var piper = new PipedPiper(from, dest ?? Client.Targets)
        {
            client = await Client.Create()
        };
        piper.client.Discord.MessageCreated += piper.Pipe;
        return piper;
    }
    private async Task Pipe(DiscordClient _, DSharpPlus.EventArgs.MessageCreateEventArgs e) {
        var prefix = "-broadcast";
        if (isRunning && e.Channel.Id == pipes.from
                && e.Message.Content.StartsWith(prefix)) {
                    foreach (var id in pipes.dest.Where(chid => chid != pipes.from))
                    {
                        var channel = await client.GetChannelAsync(id);
                        await channel.SendMessageAsync(e.Message.Content.Substring(prefix.Length));
                        if(e.Message.Attachments.Count > 0) {
                            foreach (var emb in e.Message.Attachments)
                            {
                                await channel.SendMessageAsync(emb.Url.ToString());
                            }
                        }
                    }
            }
    } 
}
        