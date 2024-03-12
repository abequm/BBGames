using Task1.Database;
using Grpc.Core;
using Task1.Database.Entities;
using Task1.Database.Enums;
using GrpcGreeterService;
using Microsoft.EntityFrameworkCore;

namespace Task1.API.Services
{
    public class GameService : Game.GameBase
    {
        public readonly Task1Context _context;
        public readonly LobbyService _lobby;
        public GameService(Task1Context context, LobbyService lobby)
        {
            _context = context;
            _lobby = lobby;
        }

        public override async Task<UserIdentity> Login(UserLogin request, ServerCallContext context)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
            if (user is not null)
            {
                return new UserIdentity() { UserId = user.Id };
            }
            else
            {
                var newUser = new User() { Login = request.Login };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return new UserIdentity() { UserId = newUser.Id };
            }
        }

        public override Task<UserBalance> Balance(UserIdentity request, ServerCallContext context)
        {
            int balance = _context.GameTransactions.Where(gt => gt.User.Id == request.UserId).Sum(gt => gt.Sum);
            return Task.FromResult(new UserBalance { Sum = balance });
        }


        public override async Task<MatchStarted> Connect(ConnectMatch request, ServerCallContext context)
        {
            var match = await _context.MatchHistory.FirstAsync(u => u.Id == request.MatchId);
            var user = await _context.Users.FirstAsync(u => u.Id == request.UserId);

            if (match.Status == MatchStatus.Created)
            {
                match.Player1 = user;
                match.Status = MatchStatus.Awaiting;
            }
            else if (match.Status == MatchStatus.Awaiting)
            {
                match.Player2 = user;
                match.Status = MatchStatus.InGame;
            }
            await _context.SaveChangesAsync();

            await _lobby.Connect(request.UserId, request.MatchId);

            return new MatchStarted { OpponentName = $" " };
        }
        public override async Task<MatchCreated> Create(CreateMatch request, ServerCallContext context)
        {
            User user = await _context.Users.FirstAsync(u => u.Id == request.UserId);
            MatchHistory match = new MatchHistory()
            {
                Sum = request.Sum,
                Player1 = user!,
                Status = MatchStatus.Created
            };
            _context.MatchHistory.Add(match);
            await _context.SaveChangesAsync();
            return new MatchCreated() { MatchId = match.Id, Sum = match.Sum };
        }

        public override async Task<MatchList> Matches(UserIdentity request, ServerCallContext context)
        {
            var matches = _context.MatchHistory.Where(m => m.Status == MatchStatus.Awaiting);
            var matchList = new MatchList();
            matchList.Matches.AddRange(matches.Select(m => new Match { Id = m.Id, Sum = m.Sum }));
            return matchList;
        }

        public override async Task<MatchResults> Play(MatchAction request, ServerCallContext context)
        {
            var match = await _context.MatchHistory
                .Include(u => u.Player2)
                .Include(u => u.Player1).FirstAsync(u => u.Id == request.MatchId);
            bool isPlayerPos1 = match.Player1.Id == request.UserId;
            if (isPlayerPos1)
            {
                match.Player1Action = (Database.Enums.MatchActions)request.Action;
            }
            else
            {
                match.Player2Action = (Database.Enums.MatchActions)request.Action;
            }
            await _context.SaveChangesAsync();
            await _lobby.Play(request.UserId, match.Id);

            var finishedMatch = await _context.MatchHistory.FirstAsync(u => u.Id == request.MatchId);
            finishedMatch.Status = CheckWin(finishedMatch);
            await _context.SaveChangesAsync();

            MatchResults results = new();
            if (finishedMatch.Status != MatchStatus.Draw)
            {
                var userResult = isPlayerPos1 ?
                    (finishedMatch.Status == MatchStatus.Player1Win ? TransactionTypes.GameWon : TransactionTypes.GameLose) :
                    (finishedMatch.Status == MatchStatus.Player2Win ? TransactionTypes.GameWon : TransactionTypes.GameLose);
                GameTransactions transaction = new()
                {
                    User = isPlayerPos1 ? match.Player1 : match.Player2,
                    TransactionType = userResult,
                    Sum = userResult == TransactionTypes.GameWon ? finishedMatch.Sum : -finishedMatch.Sum,
                };
                _context.GameTransactions.Add(transaction);
                await _context.SaveChangesAsync();
                results.Sum = transaction.Sum;
            }

            switch (finishedMatch.Status)
            {
                case MatchStatus.Player1Win:
                    if (request.UserId == finishedMatch.Player1.Id)
                        results.MatchSummary = "Вы выиграли";
                    else
                        results.MatchSummary = "Вы проиграли";
                    break;
                case MatchStatus.Player2Win:
                    if (request.UserId == finishedMatch.Player1.Id)
                        results.MatchSummary = "Вы проиграли";
                    else
                        results.MatchSummary = "Вы выиграли";
                    break;
                case MatchStatus.Draw:
                    results.MatchSummary = "Ничья";
                    break;
                default:
                    results.MatchSummary = "не удалось определить результат";
                    break;
            }
            return results;
        }



        private MatchStatus CheckWin(MatchHistory matchHistory)
        {
            var p1 = matchHistory.Player1Action;
            var p2 = matchHistory.Player2Action;
            if (p1 == p2)
                return MatchStatus.Draw;
            else if (p1 == Database.Enums.MatchActions.K && p2 == Database.Enums.MatchActions.N)
                return MatchStatus.Player1Win;
            else if (p1 == Database.Enums.MatchActions.N && p2 == Database.Enums.MatchActions.B)
                return MatchStatus.Player1Win;
            else if (p1 == Database.Enums.MatchActions.B && p2 == Database.Enums.MatchActions.K)
                return MatchStatus.Player1Win;
            else
                return MatchStatus.Player2Win;
        }
    }
}
