using System.Diagnostics;
class Program
{
    static Random rnd = new Random();

    const int BOARD_SIZE = 10;
    const int TREASURES_COUNT = 10;
    const int TRAPS_COUNT = 5;

    const int EMPTY = 0;
    const int TREASURE = 1;
    const int TRAP = 2;
    const int REVEALED = 3;

    static void Main()
    {
        bool isEnd = false;
        while (!isEnd)
        {
            Console.WriteLine("1 - Угадать ответ");
            Console.WriteLine("2 - ФИО и группа");
            Console.WriteLine("3 - Сортировка");
            Console.WriteLine("4 - Игра Найди сокровище");
            Console.WriteLine("5 - Выход");
            Console.Write("Выберите пункт меню: ");

            int userChoice = ReturnInt();
            switch (userChoice)
            {
                case 1: GuessAnswer(GetAnswer(ReturnDouble(), ReturnDouble())); break;
                case 2: AboutStudent(); break;
                case 3: SortArray(GetArray(GetLength())); break;
                case 4: Game(); break;
                case 5: isEnd = End(); break;
                default: Console.WriteLine("Ошибка"); break;
            }
        }
    }

    static double GetAnswer(double a, double b)
    {
        return Math.Round(Math.PI * (Math.Log(Math.Pow(b, 5)) / Math.Sin(a) + 1), 2);
    }

    static void GuessAnswer(double answer)
    {
        int cnt = 0;
        while (cnt < 3)
        {
            Console.WriteLine("Попытайтесь угадать ответ: ");
            double guess = ReturnDouble();

            if (guess == answer)
            {
                Console.WriteLine("Вы отгадали!");
                return;
            }
            else
            {
                cnt++;
                Console.WriteLine($"У вас осталось {3 - cnt} попыток");
            }
        }

        Console.WriteLine($"Ответ был: {answer}");
    }

    static void AboutStudent()
    {
        Console.WriteLine("Уваров Лев Вячеславович 6101-090301D");
    }

    static int[] GetArray(int length)
    {
        int[] nums = new int[length];
        for (int i = 0; i < length; i++)
        {
            nums[i] = rnd.Next(1, 101);
        }

        return nums;
    }

    static void SortArray(int[] original)
    {
        Console.WriteLine("Исходный массив:");
        PrintArray(original);

        int[] copy1 = CopyArray(original);
        int[] copy2 = CopyArray(original);

        Stopwatch sw = Stopwatch.StartNew();
        BubbleSort(copy1);
        sw.Stop();
        double bubbleTime = sw.Elapsed.TotalMilliseconds;

        Console.WriteLine("После пузырьковой сортировки:");
        PrintArray(copy1);
        Console.WriteLine($"Время пузырьковой сортировки: {bubbleTime:F4} мс");

        sw.Restart();
        InsertionSort(copy2);
        sw.Stop();
        double insertionTime = sw.Elapsed.TotalMilliseconds;

        Console.WriteLine("После сортировки вставками:");
        PrintArray(copy2);
        Console.WriteLine($"Время сортировки вставками: {insertionTime:F4} мс");

        if (bubbleTime < insertionTime)
            Console.WriteLine("Пузырьковая сортировка была быстрее.");
        else if (insertionTime < bubbleTime)
            Console.WriteLine("Сортировка вставками была быстрее.");
        else
            Console.WriteLine("Обе сортировки заняли одинаковое время.");
    }

    static void PrintArray(int[] nums)
    {
        if (nums.Length > 10)
        {
            Console.WriteLine("Массив не может быть выведен на экран, так как длина массива больше 10.");
        }
        else if (nums.Length <= 0)
        {
            Console.WriteLine("Длина массива не может быть меньше или равна 0");
        }
        else
        {
            Console.Write(string.Join(" ", nums) + "\n");
        }
    }

    static int GetLength()
    {
        Console.Write("Введите длину массива: ");
        int length = ReturnInt();
        while (length <= 0)
        {
            Console.Write("Длина массива не может быть меньше 0. Введите длину массива: ");
            length = ReturnInt();
        }

        return length;
    }

    static int[] CopyArray(int[] array)
    {
        int[] copy = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            copy[i] = array[i];
        }

        return copy;
    }

    static int[] BubbleSort(int[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = 0; j < array.Length - i - 1; j++)
            {
                if (array[j] > array[j + 1])
                {
                    int temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                }
            }
        }

        return array;
    }

    static int[] InsertionSort(int[] arr)
    {
        for (int i = 1; i < arr.Length; i++)
        {
            int key = arr[i];
            int j = i - 1;

            while (j >= 0 && arr[j] > key)
            {
                arr[j + 1] = arr[j];
                j--;
            }

            arr[j + 1] = key;
        }

        return arr;
    }

    static int ReturnInt()
    {
        int a;
        while (!int.TryParse(Console.ReadLine(), out a))
        {
            Console.Write("Введите целочисленное число: ");
        }

        return a;
    }

    static double ReturnDouble()
    {
        double a;
        while (!double.TryParse(Console.ReadLine(), out a))
        {
            Console.Write("Введите число с плавающей точкой: ");
        }

        return a;
    }
    

    static void Game()
    {
        int[,] board = new int[BOARD_SIZE, BOARD_SIZE];
        int[,] revealed = new int[BOARD_SIZE, BOARD_SIZE];

        InitializeBoard(board);
        int foundTreasures = 0;
        bool gameOver = false;

        Console.WriteLine("Добро пожаловать в игру 'Найди сокровище'!");
        Console.WriteLine($"На поле 10x10 спрятано {TREASURES_COUNT} сокровищ и {TRAPS_COUNT} ловушек.");
        Console.WriteLine("Вводите координаты в формате: A3, B7 и т.д. (A-J и 1-10)\n");

        while (!gameOver)
        {
            PrintBoard(revealed, foundTreasures);

            string input = GetUserInput();
            if (!IsValidInput(input))
            {
                Console.WriteLine("Неверный формат. Пример: A3, B7. Попробуйте снова.");
                continue;
            }

            int row = input[0] - 'A';
            int col = int.Parse(input.Substring(1)) - 1;

            if (row < 0 || row >= BOARD_SIZE || col < 0 || col >= BOARD_SIZE)
            {
                Console.WriteLine("Координаты вне поля. Допустимо A-J и 1-10.");
                continue;
            }

            if (revealed[row, col] == REVEALED)
            {
                Console.WriteLine("Эта клетка уже открыта. Выберите другую.");
                continue;
            }

            revealed[row, col] = REVEALED;

            if (board[row, col] == TRAP)
            {
                Console.WriteLine("Ловушка! Игра окончена. Вы проиграли.");
                ShowFinalBoard(board, revealed);
                gameOver = true;
            }
            else if (board[row, col] == TREASURE)
            {
                foundTreasures++;
                Console.WriteLine("Вы нашли сокровище!");
                if (foundTreasures == TREASURES_COUNT)
                {
                    Console.WriteLine("Поздравляем! Вы нашли все сокровища и победили!");
                    gameOver = true;
                }
            }
            else
            {
                Console.WriteLine("Здесь пусто. Продолжайте поиски.");
            }
        }
    }

    static void InitializeBoard(int[,] board)
    {
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                board[i, j] = EMPTY;
            }

            PlaceItems(board, TREASURE, TREASURES_COUNT);
            PlaceItems(board, TRAP, TRAPS_COUNT);
        }
    }

    
    static void PlaceItems(int[,] board, int itemType, int count)
        {
            int placed = 0;
            while (placed < count)
            {
                int row = rnd.Next(BOARD_SIZE);
                int col = rnd.Next(BOARD_SIZE);

                if (board[row, col] == EMPTY)
                {
                    board[row, col] = itemType;
                    placed++;
                }
            }
        }


    static void PrintBoard(int[,] revealed, int foundTreasures)
    {
        Console.WriteLine($"\nНайдено сокровищ: {foundTreasures}/{TREASURES_COUNT}");
        Console.Write("   ");
        for (int j = 0; j < BOARD_SIZE; j++)
        {
            Console.Write($"{j + 1,3}");
        }

        Console.WriteLine();

        for (int i = 0; i < BOARD_SIZE; i++)
        {
            Console.Write($"{(char)('A' + i),2} ");
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                if (revealed[i, j] == REVEALED)
                {
                    Console.Write("  *");
                }
                else
                {
                    Console.Write("  .");
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }


    static string GetUserInput()
    {
        Console.Write("Введите координаты (например, A3): ");
        return Console.ReadLine().Trim().ToUpper();
    }


    static bool IsValidInput(string input)
    {
        if (input.Length < 2 || input.Length > 3)
        {
            return false;
        }

        char row = input[0];
        string colStr = input.Substring(1);

        if (row < 'A' || row > 'J')
        {
            return false;
        }

        return int.TryParse(colStr, out int col) && col >= 1 && col <= 10;
    }


    static void ShowFinalBoard(int[,] board, int[,] revealed)
    {
        Console.WriteLine("\n=== Финальное поле ===");
        Console.Write("   ");
        for (int j = 0; j < BOARD_SIZE; j++)
        {
            Console.Write($"{j + 1,3}");
        }

        Console.WriteLine();

        for (int i = 0; i < BOARD_SIZE; i++)
        {
            Console.Write($"{(char)('A' + i),2} ");
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                if (revealed[i, j] == REVEALED)
                {
                    if (board[i, j] == TREASURE)
                    {
                        Console.Write("  T");
                    }
                    else if (board[i, j] == TRAP)
                    {
                        Console.Write("  X");
                    }
                    else
                    {
                        Console.Write("  *");
                    }
                }
                else
                {
                    Console.Write("  ?");
                }
            }

            Console.WriteLine();
        }
    }


    static bool End()
    {
        Console.Write("Введите <д>, чтобы выйти из программы или <н>, чтобы остаться в программе: ");

        string choice;
        do
        {
            choice = Console.ReadLine();

            if (choice == "д")
            {
                return true;
            }
            else if (choice == "н")
            {
                Console.WriteLine("Остаемся в программе");
                return false;
            }
            else
            {
                Console.WriteLine("Введите 'д' или 'н'");
            }
        } while (true);
    }
}

