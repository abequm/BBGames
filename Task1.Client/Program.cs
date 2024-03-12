using Grpc.Net.Client;
using GrpcGreeterClient;


using var channel = GrpcChannel.ForAddress("http://localhost:5000");
var client = new Game.GameClient(channel);


async Task Play(int matchId, int userId)
{
    Console.Clear();
    Console.WriteLine("Ожидание опонента");
    var match = await client.ConnectAsync(new ConnectMatch { MatchId = matchId, UserId = userId });
m3: Console.Clear();
    Console.WriteLine("Введите 1 - Камень, 2 - Ножницы или 3 - Бумага");
    var k = Console.ReadKey().Key;
    if (new[] { ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3 }.Contains(k))
    {
        var result = await client.PlayAsync(new MatchAction { MatchId = matchId, Action = (MatchActions)k, UserId = userId });
        Console.WriteLine(result.MatchSummary);
        Console.WriteLine($"Ваш баланс изменен: {result.Sum}\nНажмите любую кнопку чтобы продолжить");
        Console.ReadKey();
        Console.ReadKey();
    }
    else
    {
        goto m3;
    }
}




Console.WriteLine("Введите имя пользователя");
var name = Console.ReadLine();

var userId = await client.LoginAsync(new UserLogin { Login = name });

var command = "";
try
{


    while (command != "exit")
    {
        Console.Clear();
        Console.WriteLine(@"
    Меню
    1. Баланс
    2. Список игр
    3. Подключится к игре
    4. Создать игру
            
    exit Выйти
        ");
        command = Console.ReadLine();
        switch (command)
        {
            case "1":
                var balance = (await client.BalanceAsync(userId)).Sum;
                Console.Clear();
                Console.WriteLine(@$"Ваш баланс {balance}
                                 Нажмите любую кнопку чтобы продолжить");
                Console.ReadKey();
                break;
            case "2":
                var list = (await client.MatchesAsync(userId));
                Console.Clear();
                foreach (var m in list.Matches)
                {
                    Console.WriteLine($"{m.Id} | {m.Sum}");
                }
                Console.WriteLine(@$"Нажмите любую кнопку чтобы продолжить");
                Console.ReadKey();
                break;
            case "3":
                Console.Clear();
                Console.WriteLine("Введите номер игры\n");
                var matchId = Console.ReadLine();
                await Play(int.Parse(matchId), userId.UserId);
                break;
            case "4":
            m4: Console.Clear();
                Console.WriteLine("Введите ставку(только числа)");
                var bet = Console.ReadLine();
                if (int.TryParse(bet, out int betInt))
                {
                    var match = await client.CreateAsync(new CreateMatch { Sum = betInt, UserId = userId.UserId });
                    await Play(match.MatchId, userId.UserId);
                }
                else
                    goto m4;
                break;


        }
    }

}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
    Console.ReadKey();
    throw;
}