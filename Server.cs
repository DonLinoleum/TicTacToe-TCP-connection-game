namespace TicTacToe;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

class Server
{
    static async Task Main(String[] args)
    {
        string name;
        string anotherPlayerName;
        NetworkStream stream = Init(out name, out anotherPlayerName);
        char[] spaces = new char[] {' ',' ',' ',' ',' ',' ',' ',' ',' '};
        char player1Char = 'X';
        char player2Char = 'O';
       
        Console.Clear();
        bool isRunning = true;
        while (isRunning)
        {
            SetConsoleColor();
            DrawBoard(spaces);
            Player1Move(spaces, player1Char,stream);
            isRunning = !CheckWinner(spaces,player1Char,ref name, ref anotherPlayerName);
            if (isRunning is false) break;
            Console.ResetColor();
            Console.Clear();

            SetConsoleColor();

            DrawBoard(spaces);
            Player2Move(spaces, player2Char, stream);
            isRunning = !CheckWinner(spaces, player2Char, ref name, ref anotherPlayerName);
            if (isRunning is false) break;
            Console.ResetColor();
            Console.Clear();
        }
        Console.ReadKey();
    }
    private static NetworkStream Init(out string name, out string anotherPlayerName)
    {
        string ip = "127.0.0.1";
        int port = 8080;
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endpoint);
        socket.Listen(1);
        Console.WriteLine("Start server. Awaiting Player2...");

        Socket client = socket.Accept();
        NetworkStream stream = new NetworkStream(client);

        byte[] buffer = new byte[512];
        stream.Read(buffer);
        string getAnotherPlayerName = Encoding.UTF8.GetString(buffer);
        Console.WriteLine($"Player2 Name: {getAnotherPlayerName}");

        Console.Write("What's your name?\n");
        string? getName = Console.ReadLine();
        stream.Write(Encoding.UTF8.GetBytes(getName));
        name = getName;
        anotherPlayerName = getAnotherPlayerName;
        return stream;
    }

    private static void DrawBoard(char[] spaces)
    {
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {spaces[0]}  |  {spaces[1]}  |  {spaces[2]}  ");
        Console.WriteLine("_____|_____|_____");
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {spaces[3]}  |  {spaces[4]}  |  {spaces[5]}  ");
        Console.WriteLine("_____|_____|_____");
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {spaces[6]}  |  {spaces[7]}  |  {spaces[8]}  ");
        Console.WriteLine("     |     |     ");
    }

    private static void SetConsoleColor()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine("########## SuperGame!!!! ##########");
        Console.SetCursorPosition(0, 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.BackgroundColor = ConsoleColor.Yellow;
    }

    private static void Player1Move(char[] spaces, char player1, NetworkStream stream)
    {
        bool isNotCorrectMove = true;
        Console.ResetColor();
        while (isNotCorrectMove)
        {
            Console.WriteLine("Enter a spot to place a marker (1-9): ");
            string? Player1Spot = Console.ReadLine();
            int spacesIndex;
            if (int.TryParse(Player1Spot, out spacesIndex) && spacesIndex < 10 && spacesIndex > 0
                && spaces[--spacesIndex] == ' ')
            {
                spaces[spacesIndex] = player1;
                stream.WriteByte((byte)spacesIndex);
                isNotCorrectMove = false;
            }
            else
                Console.WriteLine("Not correct move! Try again.");
        }
    }

    private static void Player2Move(char[] spaces, char player2, NetworkStream stream)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine("Awaiting Player2 move...");
        int spacesIndex = stream.ReadByte();
        spaces[spacesIndex] = player2;
    }

    private static bool CheckWinner(char[] spaces,char spot,ref string name, ref string anotherPlayerName)
    {
        bool hasWinner = false;
        if (spaces[0] == spot && spaces[1] == spot && spaces[2] == spot)
            hasWinner = true;
        else if (spaces[3] == spot && spaces[4] == spot && spaces[5] == spot)
            hasWinner = true;
        else if (spaces[6] == spot && spaces[7] == spot && spaces[8] == spot)
            hasWinner = true;

        else if (spaces[0] == spot && spaces[3] == spot && spaces[6] == spot)
            hasWinner = true;
        else if (spaces[1] == spot && spaces[4] == spot && spaces[7] == spot)
            hasWinner = true;
        else if (spaces[2] == spot && spaces[5] == spot && spaces[8] == spot)
            hasWinner = true;

        else if (spaces[0] == spot && spaces[4] == spot && spaces[8] == spot)
            hasWinner = true;
        else if (spaces[2] == spot && spaces[4] == spot && spaces[6] == spot)
            hasWinner = true;

        if (hasWinner is true)
        {
            ResetConsoleColor();
            DrawBoard(spaces);
            Console.BackgroundColor = ConsoleColor.Black;
            string winnerName = spot == 'X' ? name : anotherPlayerName;
            Console.WriteLine($"And the winner is - {winnerName}!!!!!!!!!!!!!!!!");
            Console.WriteLine("Thanks for game!!!!!!");
            Console.WriteLine("########################################");
            DrawHappyFace();
        }

        if (spaces.SkipWhile(el => el != ' ').Count() == 0)
        {
            ResetConsoleColor();
            DrawBoard(spaces);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("It's a tie!!!!!!!!!!!!!!");
            Console.WriteLine("Thanks for game!!!!!!");
            Console.WriteLine("########################################");
            DrawHappyFace();
            hasWinner = true;
        }
        return hasWinner;
    }

    private static void ResetConsoleColor()
    {
        Console.ResetColor();
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.BackgroundColor = ConsoleColor.Yellow;
    }

    private static void DrawHappyFace()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(@"
    _.-'''''-._
  .'  _     _  '.
 /   (_)   (_)   \
|  ,           ,  |
|  \`.       .`/  |
 \  '.`'""""'""`.'  /
  '.  `'---'`  .' 
    '-._____.-'");
    }
}


class Client
{
    static async Task Main(String[] args)
    {
        string name;
        string anotherPlayerName;
        NetworkStream stream = Init(out name, out anotherPlayerName);
        char[] spaces = new char[] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        char player1Char = 'X';
        char player2Char = 'O';

        bool isRunning = true;
        Console.Clear();
        while (isRunning)
        {
            SetConsoleColor();
            DrawBoard(spaces);
            Player1Move(spaces, player1Char, stream);
            isRunning = !CheckWinner(spaces, player1Char, ref name, ref anotherPlayerName);
            if (isRunning is false) break;
            Console.ResetColor();
            Console.Clear();

            SetConsoleColor();

            DrawBoard(spaces);
            Player2Move(spaces, player2Char, stream);
            isRunning = !CheckWinner(spaces, player2Char, ref name, ref anotherPlayerName);
            if (isRunning is false) break;
            Console.ResetColor();
            Console.Clear();
        }
        Console.ReadKey();
    }

    private static NetworkStream Init(out string name, out string anotherPlayerName)
    {
        string ip = "127.0.0.1";
        int port = 8080;
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(endpoint);
        Console.WriteLine("Connected to server!");
        NetworkStream stream = new NetworkStream(socket);

        Console.Write("What's your name?\n");
        string? getName = Console.ReadLine();
        byte[] buffer = new byte[512];
        stream.Write(Encoding.UTF8.GetBytes(getName));

        Console.WriteLine("Awaiting Player1...");
        stream.Read(buffer);
        string getAnotherPlayerName = Encoding.UTF8.GetString(buffer);
        name = getName;
        anotherPlayerName = getAnotherPlayerName;
        return stream;
    }

    private static void DrawBoard(char[] spaces)
    {
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {spaces[0]}  |  {spaces[1]}  |  {spaces[2]}  ");
        Console.WriteLine("_____|_____|_____");
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {spaces[3]}  |  {spaces[4]}  |  {spaces[5]}  ");
        Console.WriteLine("_____|_____|_____");
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {spaces[6]}  |  {spaces[7]}  |  {spaces[8]}  ");
        Console.WriteLine("     |     |     ");
    }

    private static void SetConsoleColor()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine("########## SuperGame!!!! ##########");
        Console.SetCursorPosition(0, 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.BackgroundColor = ConsoleColor.Yellow;
    }

    private static void Player2Move(char[] spaces, char player2, NetworkStream stream)
    {
        bool isNotCorrectMove = true;
        Console.ResetColor();
        while (isNotCorrectMove)
        {
            Console.WriteLine("Enter a spot to place a marker (1-9): ");
            string? Player1Spot = Console.ReadLine();
            int spacesIndex;
            if (int.TryParse(Player1Spot, out spacesIndex) && spacesIndex < 10 && spacesIndex > 0
                && spaces[--spacesIndex] == ' ')
            {
                spaces[spacesIndex] = player2;
                stream.WriteByte((byte)spacesIndex);
                isNotCorrectMove = false;
            }
            else
                Console.WriteLine("Not correct move! Try again.");
        }
    }

    private static void Player1Move(char[] spaces, char player1, NetworkStream stream)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine("Awaiting Player1 move...");
        int spacesIndex = stream.ReadByte();
        spaces[spacesIndex] = player1;
    }

    private static bool CheckWinner(char[] spaces, char spot, ref string name, ref string anotherPlayerName)
    {
        bool hasWinner = false;
        if (spaces[0] == spot && spaces[1] == spot && spaces[2] == spot)
            hasWinner = true;
        else if (spaces[3] == spot && spaces[4] == spot && spaces[5] == spot)
            hasWinner = true;
        else if (spaces[6] == spot && spaces[7] == spot && spaces[8] == spot)
            hasWinner = true;

        else if (spaces[0] == spot && spaces[3] == spot && spaces[6] == spot)
            hasWinner = true;
        else if (spaces[1] == spot && spaces[4] == spot && spaces[7] == spot)
            hasWinner = true;
        else if (spaces[2] == spot && spaces[5] == spot && spaces[8] == spot)
            hasWinner = true;

        else if (spaces[0] == spot && spaces[4] == spot && spaces[8] == spot)
            hasWinner = true;
        else if (spaces[2] == spot && spaces[4] == spot && spaces[6] == spot)
            hasWinner = true;

        if (hasWinner)
        {
            ResetConsoleColor();
            DrawBoard(spaces);
            Console.BackgroundColor = ConsoleColor.Black;
            string winnerName = spot == 'O' ? name : anotherPlayerName;
            Console.WriteLine($"And the winner is - {winnerName}!!!!!!!!!!!!!!!!");
            Console.WriteLine("Thanks for game!!!!!!");
            Console.WriteLine("########################################");
            DrawHappyFace();
        }

        if (spaces.SkipWhile(el => el != ' ').Count() == 0)
        {
            ResetConsoleColor();
            DrawBoard(spaces);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("It's a tie!!!!!!!!!!!!!!");
            Console.WriteLine("Thanks for game!!!!!!");
            Console.WriteLine("########################################");
            DrawHappyFace();
            hasWinner = true;
        }
        return hasWinner;
    }

    private static void ResetConsoleColor()
    {
        Console.ResetColor();
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.BackgroundColor = ConsoleColor.Yellow;
    }

    private static void DrawHappyFace()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(@"
    _.-'''''-._
  .'  _     _  '.
 /   (_)   (_)   \
|  ,           ,  |
|  \`.       .`/  |
 \  '.`'""""'""`.'  /
  '.  `'---'`  .' 
    '-._____.-'");
    }
}