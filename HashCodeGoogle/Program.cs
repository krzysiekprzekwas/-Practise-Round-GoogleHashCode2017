using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;


/*
 * Google Hash Code Pizza Project by NavySol
 * 
 * Pizza is kept in two dimentional array of ints. Every mushroom is replaced with value of 1 and every tomato with value of 2.
 * Cuting off slices will set value of their cells to zero.
 * 
 * Then the list of all possible shapes of slice is generated basing on two factors:
 *  - shape can't be bigger than max amount of cells
 *  - shape must be big enough to fit mimimum ingredients from both species
 * 
 * When it's done we start interating thru every cell in pizza.
 * If it hasn't been already chopped off we're trying to fit its place shape as small as possible, which will have enough ingredients.
 * Checking ingredients is based on control sum. If shape would have only mushrooms then that sum will be same as amount of cells in shape.
 * On the other hand if it would be full of tomatoes control sum will be exacly twice that amount.
 * With this said we know that sum should be between this two values and that sum cannot include any zero value (choped off slice).
 * 
 * If shape fits we add it to result list same way as it will be printed.
 * Then we're seting choped off cells to zero and go to next ineration.
 * If there's no shape that would fit we just skip that cell.

 * Future optimalization:
 * 
 * - before saving, we should iterate thru list of slices, and try to "strech" them if this is possible to eliminate lost spaces.
 * - with streching done, last thing should be to generate every permutation of shapes. Right now they are ordered as every loop-generated variable.
 *   It have huge effect on overal performance. In my opinion this algorithm may be quite accurate, at least accurate enough for Paris !

*/

namespace HashCodeGoogle
{
    class Program
    {
        static int rows;
        static int columns;
        static int minIngredient;
        static int maxCells;
        private static int[,] array;
        private static List<Point> possibleShapes;
        private static List<Tuple<int, int, int, int>> pizzaSlices;

        static void Main(string[] args)
        {


            using (TextReader reader = File.OpenText("small.in"))
            {
                DividePizza(reader, "smallResults.txt");
            }

            using (TextReader reader = File.OpenText("medium.in"))
            {
                DividePizza(reader, "mediumResults.txt");
            }

            using (TextReader reader = File.OpenText("big.in"))
            {
                DividePizza(reader, "bigResults.txt");
            }

            using (TextReader reader = File.OpenText("example.in"))
            {
                DividePizza(reader, "example.txt");
            }

        }

        private static void DividePizza(TextReader reader, string output)
        {
            // Read data from small.in and initialize Lists
            ReadInputData(reader);

            Console.Clear();
            // Prints data as matrix
            //PrintPizza();

            // Generates all possible and valid sizes of slices
            GeneratePossibleShapes();

            // Show possible shapes and their order
            //PrintPossibleShapes();


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (array[i, j] == 0) // If that place was already choped off then move on to the next one
                        continue;
                    
                        //Console.Clear();
                        //PrintCursor(i, j, ConsoleColor.Red);      // Uncoment this shit for some cool debugging stuff
                        //Console.ReadKey();
                        

                    foreach (var shape in possibleShapes)
                    {
                        // Count sum of slice in current shape if placed in place IxJ
                        var sumCurrent = SumCurrent(shape, i, j);

                        // Check if sum is correct. More explanations on top. 
                        if (sumCurrent >= minIngredient)
                        {
                            // Add new slice to others
                            pizzaSlices.Add(new Tuple<int, int, int, int>(i, j, i + shape.X - 1, j + shape.Y - 1));

                            // Erase slice from pizza matrix
                            CutSliceFromPizza(shape, i, j);

                            // Move to next not choped off place
                            break;
                        }
                    }
                    //PrintCursor(i, j, ConsoleColor.White);
                }
            }

            // Save results to results.txt in current directory
            SaveResults(output);
        }

        private static void PrintPossibleShapes()
        {
            for (int i = 0; i < possibleShapes.Count; i++)
            {
                Console.SetCursorPosition(columns + 5, i);
                Console.WriteLine(possibleShapes.ElementAt(i).X + " x " + possibleShapes.ElementAt(i).Y);
            }
        }

        private static void CutSliceFromPizza(Point shape, int i, int j)
        {
            for (int l = 0; l < shape.X; l++)
            {
                for (int f = 0; f < shape.Y; f++)
                {
                    array[i + l, j + f] = 0;
                    //Console.SetCursorPosition(j+f, i + l);
                    //Console.Write(0);
                }
            }
        }

        private static void SaveResults(string output)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(output);
            file.WriteLine(pizzaSlices.Count);

            foreach (var pizza in pizzaSlices)
            {
                file.WriteLine(pizza.Item1 + " " + pizza.Item2 + " " + pizza.Item3 + " " + pizza.Item4);
            }

            file.Close();
        }

        private static int SumCurrent(Point shape, int i, int j)
        {
            int tomatoCounter = 0, mushroomCounter = 0;
            

            for (int l = 0; l < shape.X; l++)
            {
                for (int f = 0; f < shape.Y; f++)
                {
                    if (i + l >= rows || j + f >= columns || array[i + l, j + f] == 0)
                        return 0;

                    switch (array[i + l, j + f])
                    {
                        case 1:
                            ++mushroomCounter;
                            break;
                        case 2:
                            ++tomatoCounter;
                            break;
                    }
                }
            }
            return Math.Min(tomatoCounter,mushroomCounter);
        }

        private static void ReadInputData(TextReader reader)
        {
            string text = reader.ReadLine();
            string[] bits = text.Split(' ');

            rows = int.Parse(bits[0]);
            columns = int.Parse(bits[1]);
            minIngredient = int.Parse(bits[2]);
            maxCells = int.Parse(bits[3]);

            array = new int[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                text = reader.ReadLine();
                for (int j = 0; j < columns; j++)
                {
                    if (text.ElementAt(j) == 'M')
                        array[i, j] = 1;
                    else if (text.ElementAt(j) == 'T')
                        array[i, j] = 2;
                }
            }

            possibleShapes = new List<Point>();

            pizzaSlices = new List<Tuple<int, int, int, int>>();
        }

        private static void GeneratePossibleShapes()
        {
            for (int i = 0; i <= maxCells; i++)
            {
                for (int j = 0; j <= maxCells; j++)
                {
                    if (i * j <= maxCells && i * j >= minIngredient * 2)
                        possibleShapes.Add(new Point(i, j));
                }
            }
        }

        private static void PrintPizza()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Console.Write(array[i, j]);
                }
                Console.WriteLine();
            }
        }

        private static void PrintCursor(int l, int m, ConsoleColor color)
        {
            Console.SetCursorPosition(m,l);
            Console.ForegroundColor = color;
            Console.Write(array[l, m]);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
