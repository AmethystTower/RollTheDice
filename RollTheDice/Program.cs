using System;

//used for Sleep()
using System.Threading;
//used for DllImport
using System.Runtime.InteropServices;
//used for dynamic lists/arrays
using System.Collections.Generic;

namespace RollTheDice
{
    class Program
    {
        //import & initialize _getch() function.
        [DllImport("msvcrt.dll")]
        static extern char _getch();
        static bool twoNumbers;

        static void Main()
        {
            //text variables.
            char text;
            string number;

            //booleans
            bool victory;
            bool normal;
            bool isValid;

            //pointless array for this but added this as a request from a teacher.
            int[] diceNum = new int[2];

            //number variables.
            double money = 0;
            double bet;
            float ratio = 0f;

            //statistics using a dynamic array.
            List<bool> WLRatio = new List<bool>();
            int win = 0;
            int loss = 0;

            //UTF-8 Support
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //introduce to game
            Console.WriteLine("\n\tWelcome to Roll The Dice! Press any key to continue.");
            Console.ReadKey();
            Console.Clear();

            //let user choose the game mode.
            //user has to press valid key to get out of the loop.
            for (;;)
            {
                Console.WriteLine("\n\tChoose the game mode that you would like to play:\n\n\tN (Normal) - The dice can roll a number between 1-6 which equals a chance of ~1,667% to win.\n\tA (Advanced) - Choose a custom minimum and maximum number for the dice.");

                text = _getch();
                text = char.ToLower(text);

                //user chose normal.
                if (text == 'n')
                {
                    normal = true;
                    Console.Clear();
                    break;
                }
                //user chose advanced.
                else if (text == 'a')
                {
                    normal = false;
                    Console.Clear();

                    //let user input custom values.
                    Console.WriteLine("\n\tChoose a minimum and maximum amount for the dice.\n\tNote: Maximum number must be higher than 0 and no negative numbers allowed.");
                    for (int i = 0; i < 2; i++)
                    {
                        number = Console.ReadLine();
                        isValid = int.TryParse(number, out diceNum[i]);

                        //fallback to reduce i incase the user inputs something false or if maximum number is smaller than minimum number.
                        if (!isValid || diceNum[i] < 0)
                            i--;
                    }

                    //another fallback incase the minimum number is higher than the maximum number. convert maximum to minimum and vice versa.
                    if (diceNum[0] > diceNum[1])
                    {
                        int temp = diceNum[0];

                        diceNum[0] = diceNum[1];
                        diceNum[1] = temp;
                    }
                    //maximum number is 0 or equal to minimum number, increase it by +1.
                    if (diceNum[1] <= 0 || diceNum[1] <= diceNum[0])
                        diceNum[1]++;

                    Console.Clear();
                    break;
                }
                //invalid input.
                else
                    Console.Clear();
            }

            //let user input a custom money value.
            while (money < 10)
                money = InputStringAsDouble("\n\tChoose an amount of money that you would like to play with. Money is input as €.\n\n\tRequires a minimum amount of 10€.", "\n\tNumber contains too many characters!", "\n\tNumber is too little!", 8, 2, 9999, ',');

            for (;;)
            {
                if (money < 0.5)
                    break;

                //ask user if he wants to play. yes = continue, no = end, invalid = try again.
                Console.WriteLine("\n\tWould you like to roll the dice?\n\n\tBank: " + money + "€ in bank.\n\n\tY (Yes) - set a bet, Roll the Dice and try to guess the number.\n\tN (No) - Quit.");
                text = _getch();
                text = char.ToLower(text);

                if (text == 'y')
                {
                    for (;;)
                    {
                        Console.Clear();
                        //let user input an amount he wants to bet with.
                        bet = InputStringAsDouble("\n\tChoose an amount of money that you would like to bet.\n\n\tRequires a minimum amount of 0,5€.\n\tYou have " + money + "€ in the bank.", "\n\tNumber contains too many characters!", "\n\tNumber is too little!", 9, 1, 999999999, ',');

                        if (bet < 0.5 || bet > money)
                        {
                            if (bet < 0.5)
                                Console.WriteLine("\n\tBet amount must be at least 0,5€.");
                            else
                                Console.WriteLine("\n\tYou don't have enough money to bet with this value!");

                            Thread.Sleep(2000);
                            continue;
                        }

                        break;
                    }
                    //execute function that lets player and computer guess numbers. returns true if the user won, otherwise false.
                    victory = ExecuteTurn(normal, diceNum);
                }
                else if (text == 'n')
                {
                    Console.Clear();
                    Console.WriteLine("\n\tYou are quitting with " + money + "€ left in your bank.");
                    Thread.Sleep(3000);
                    return;
                }
                else
                {
                    Console.WriteLine("\n\tInvalid input! Try again...");
                    Thread.Sleep(2000);
                    Console.Clear();
                    continue;
                }

                Thread.Sleep(1000);

                //did user win?
                if (victory)
                {
                    if(!twoNumbers)
                        Console.WriteLine("\n\tWell done! Your number is correct and you've gained " + bet*3 + "€!\n\t" + money + "€ + " + bet*3 + "€ = " + (money + bet*3) + "€");
                    else
                        Console.WriteLine("\n\tWell done! Your number is correct and you've gained " + bet*2 + "€!\n\t" + money + "€ + " + bet*2 + "€ = " + (money + bet*2) + "€");
                    money += bet*3;
                    WLRatio.Add(true);
                }
                else
                {
                    Console.WriteLine("\n\tYou lost! Your number was incorect and you've lost " + bet + "€!\n\t" + money + "€ - " + bet + "€ = " + (money - bet) + "€");
                    money -= bet;
                    WLRatio.Add(false);
                }

                //calculate Win/Loss Ratio. outcome true = win
                win = 0;
                loss = 0;

                foreach (bool outcome in WLRatio)
                {
                    if (outcome)
                        win++;
                    else
                        loss++;
                }

                //only calculate ratio when both stats are present. force ints to be used as floats for calculation to get the decimals.
                if (win > 0 && loss > 0)
                    ratio = ((float)win / (float)loss);

                Thread.Sleep(3000);
                Console.Clear();
            }

            //player lost. force him to leave.
            Console.Clear();
            Console.WriteLine("\n\tYou've lost!\n\tYour Win/Loss ratio equals: " + ratio.ToString("0.00") + " with " + win + " games won & " + loss + " games lost.\n\n\tPress and key to close the application.");
            Console.ReadKey();
            return;
        }

        static bool ExecuteTurn(bool normal, int[] diceNum)
        {
            char[] input = new char[2];
            string[] intCheck = new string[2];
            int[] number = new int[2];

            //SECOND NUMBER IS RUSHED, might look bad.
            twoNumbers = false;

            //initial console clear.
            Console.Clear();

            //give user option to put one or two numbers.
            for (; ; )
            {
                Console.WriteLine("\n\tHow many numbers would you like to guess?\n\n\t1 - You can guess one number.\n\t2 - You can guess two numbers.");

                //no tolower needed as the user has to input numbers anyway.
                char text = _getch();

                //user chose 1.
                if (text == '1')
                {
                    Console.Clear();
                    break;
                }
                //user chose 2.
                else if (text == '2')
                {
                    twoNumbers = true;
                    Console.Clear();
                    break;
                }
                //invalid input.
                else
                    Console.Clear();
            }

            for (;;)
            {
                Console.Clear();

                if (normal)
                {
                    Console.WriteLine("\n\tInput a number between 1-6.");
                    //get input as char and convert to string for TryParse which returns an int.
                    input[0] = _getch();
                    intCheck[0] = Char.ToString(input[0]);

                    if (twoNumbers)
                    {
                        Console.Clear();
                        Console.WriteLine("\n\tInput another number between 1-6.");
                        //get input as char and convert to string for TryParse which returns an int.
                        input[1] = _getch();
                        intCheck[1] = Char.ToString(input[1]);
                    }
                }
                else
                {
                    Console.WriteLine("\n\tWrite a number between " + diceNum[0] + "-" + diceNum[1] + " and then press enter.");
                    intCheck[0] = Console.ReadLine();

                    if (twoNumbers)
                    {
                        Console.Clear();
                        Console.WriteLine("\n\tWrite another number between " + diceNum[0] + "-" + diceNum[1] + " and then press enter.");
                        intCheck[1] = Console.ReadLine();
                    }
                }

                //this check is pretty ugly, but it's all in one if. it has been rushed after all.
                if ((!int.TryParse(intCheck[0], out number[0]) || (normal && (number[0] > 6 || number[0] < 1)) || (!normal && (number[0] > diceNum[1] || number[0] < diceNum[0]))) || (twoNumbers && (!int.TryParse(intCheck[1], out number[1]) || (normal && (number[1] > 6 || number[1] < 1)) || (!normal && (number[1] > diceNum[1] || number[1] < diceNum[0])))))
                {
                    Console.WriteLine("\n\tInvalid input! Try again...");
                    Thread.Sleep(2000);
                    continue;
                }
                else
                {
                    Console.WriteLine("\n\tYour number is: " + number[0]);

                    if(twoNumbers)
                        Console.WriteLine("\tYour second number is: " + number[1]+"\n");
                    break;
                }
            }

            Thread.Sleep(500);

            int dice = RollDice(normal, diceNum);
            Console.WriteLine("\tThe dice rolled: " + dice);

            if (number[0] == dice || (twoNumbers && number[1] == dice))
                return true;
            else
                return false;
        }

        static int RollDice(bool normal, int[] diceNum)
        {
            Random diceRoll = new Random();

            if (normal)
                return diceRoll.Next(6) + 1;  //+1 so that it's 1-6 instead of 0-5.

            else
                return diceRoll.Next(diceNum[0], diceNum[1]+1); //+1 for the second number to account for the 0.
        }

        //function to check if input is an actual number.
        static bool CheckValidity(char c)
        {
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return true;
                default:
                    return false;
            }
        }

        static double InputStringAsDouble(string prompt, string tooBig, string tooLittle, int maxSize, int minSize, int maxVal, char ignoreChar)
        {
            char[] characters;
            string text;
            double money;

            bool ignoreFirstChar;

            for (;;)
            {
                ignoreFirstChar = true;
                bool invalid = false;

                Console.WriteLine(prompt);
                text = Console.ReadLine();

                //initial string is empty? continue.
                if (String.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine("\n\tYour input is invalid!");
                    Thread.Sleep(2000);
                    Console.Clear();
                    continue;
                }

                characters = text.ToCharArray();

                //check validity of input.
                for (short i = 0; i<characters.Length; i++)
                {
                    //input contains anything but a number? give error and restart input process.
                    if (!CheckValidity(characters[i]))
                    {
                        if (characters[i] == ignoreChar && ignoreFirstChar)
                        {
                            ignoreFirstChar = false;
                            continue;
                        }

                        Console.WriteLine("\n\tYour input is invalid!");
                        Thread.Sleep(2000);
                        Console.Clear();
                        invalid = true;
                        break;
                    }

                    if (characters.Length > maxSize || characters.Length < minSize)
                    {
                        if(characters.Length > maxSize)
                          Console.WriteLine(tooBig);
                        else
                          Console.WriteLine(tooLittle);

                        Thread.Sleep(2000);
                        Console.Clear();
                        invalid = true;
                        break;
                    }
                }

                if (invalid)
                    continue;

                money = Convert.ToDouble(text);

                if (money > maxVal)
                {
                    Console.WriteLine(tooBig);
                    Thread.Sleep(2000);
                    Console.Clear();
                    invalid = true;
                }

                if (!invalid)
                {
                    //starting money has been calculated. return value and clear console.
                    Console.Clear();
                    return money;
                }
            }
        }
    }
}
