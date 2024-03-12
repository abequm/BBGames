
namespace Task1.API.Services
{
    public class LobbyService
    {
        public Dictionary<int, Task> AwaitingMatches = new Dictionary<int, Task>();
        public Dictionary<int, Task> InGameMatches = new Dictionary<int, Task>();

        public async Task Connect(int userId, int matchId, CancellationToken cancellationToken = default)
        {
            if (AwaitingMatches.Keys.Any(k => k == matchId))
            {
                AwaitingMatches[matchId].Start();
            }
            else
            {
                Task task = new Task(() => { AwaitingMatches.Remove(matchId); });
                AwaitingMatches.Add(matchId, task);
                await task.WaitAsync(cancellationToken);
            }
        }

        public async Task Play(int userId, int matchId, CancellationToken cancellationToken = default)
        {
            if (InGameMatches.Keys.Any(k => k == matchId))
            {
                InGameMatches[matchId].Start();
            }
            else
            {
                Task task = new Task(() => { InGameMatches.Remove(matchId); });
                InGameMatches.Add(matchId, task);
                await task.WaitAsync(cancellationToken);
            }
        }
    }
}
