using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.Responses;
using Victoria.Responses.Search;
using Victoria.Decoder;
using Victoria.EventArgs;
using Victoria.Payloads;

namespace DJSona.Modules
{
	public class Music : ModuleBase<SocketCommandContext>
	{
		private readonly LavaNode _lavaNode;

		public Music(LavaNode lavaNode)
		{
			_lavaNode = lavaNode;
		}

        private async Task OnTrackEnded(TrackEndedEventArgs args)
        {
            /*if (!args.ShouldPlayNext())
            {
                return;
            }*/

            var player = args.Player;
            if (!player.Queue.TryDequeue(out var queueable))
            {
                await player.TextChannel.SendMessageAsync("Queue completed!");
                return;
            }

            if (!(queueable is LavaTrack track))
            {
                await player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
                return;
            }

            await args.Player.PlayAsync(track);
            await args.Player.TextChannel.SendMessageAsync(
                $"{args.Reason}: {args.Track.Title}\n▶️ Now playing: **{track.Title}**");
        }


        [Command("Play")]
        public async Task PlayAsync([Remainder] string query)
        {
            await JoinAsync();

            if (string.IsNullOrWhiteSpace(query))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            var queries = query.Split(' ');
            
            var searchResponse = await _lavaNode.SearchYouTubeAsync(query);
            if (searchResponse.Status == SearchStatus.LoadFailed ||
                searchResponse.Status == SearchStatus.LoadFailed)
            {
                await ReplyAsync($"I wasn't able to find anything for `{query}`.");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    foreach (var track in searchResponse.Tracks)
                    {
                        player.Queue.Enqueue(track);
                    }

                    await ReplyAsync($"Queued **{searchResponse.Tracks.Count}** tracks.");
                }
                else
                {
                    var track = searchResponse.Tracks.ElementAt(0);
                    player.Queue.Enqueue(track);
                    await ReplyAsync($"Queued: **{track.Title}**");
                }
            }
            else
            {
                var track = searchResponse.Tracks.ElementAt(0);

                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    for (var i = 0; i < searchResponse.Tracks.Count; i++)
                    {
                        if (i == 0)
                        {
                            await player.PlayAsync(track);
                            await ReplyAsync($"▶️ Now Playing: **{track.Title}**");
                        }
                        else
                        {
                            player.Queue.Enqueue(searchResponse.Tracks.ElementAt(i));
                        }
                    }

                    await ReplyAsync($"Queued **{searchResponse.Tracks.Count}** tracks.");
                }
                else
                {
                    await player.PlayAsync(track);
                    await ReplyAsync($"▶️ Now Playing: **{track.Title}**");
                }
            }
            


        }

        [Command("Join")]
        public async Task JoinAsync()
        {
            

            var voiceState = Context.User as IVoiceState;
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                if (voiceState?.VoiceChannel == _lavaNode.GetPlayer(Context.Guild).VoiceChannel) return;
                await ReplyAsync("I'm already connected to a voice channel!");
                return;
            }

            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("Skip")]
        public async Task Skip()
		{
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
			{
                await ReplyAsync("You must be connected to a voice channel!");
                return;
			}

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            if (voiceState.VoiceChannel != player.VoiceChannel)
			{
                await ReplyAsync("You need to be in the same voice channel as me!");
                return;
			}

            if (player.Queue.Count == 0)
			{
                await player.StopAsync();
                await ReplyAsync("⏹️ Skipped, but there are no more songs in the queue!");
                return;
			}

            await player.SkipAsync();
            await ReplyAsync($"⏭️ Skipped! \n▶️ Now playing {player.Track.Title}");
		}
    }
}
