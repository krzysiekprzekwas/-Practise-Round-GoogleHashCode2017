using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;


namespace HashCodeGoogle
{
    class Program
    {
        static int rows;
        static int columns;
        static int minIngredient;
        static int maxCells;
        private static int[,] array;

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

            var possibleShapes = GeneratePossibleShapes();

            var possibleShapesReverted = possibleShapes.ToList();
            possibleShapesReverted.Reverse();

            
            var pizzaSlices = new List<Tuple<int, int, int, int>>();
            var pizzaSlicesReverted = new List<Tuple<int, int, int, int>>();

            var tempArray = new int[rows,columns];

            Array.Copy(array,tempArray,array.Length);

            var result1 = CheckPermutation(possibleShapes, tempArray, pizzaSlices);

            Array.Copy(array, tempArray, array.Length);
            var result2 = CheckPermutation(possibleShapesReverted, tempArray, pizzaSlicesReverted);

            SaveResults(output, result1 > result2 ? pizzaSlices : pizzaSlicesReverted);
        }

        private static int CheckPermutation(List<Point> possibleShapes, int[,] tempArray, List<Tuple<int, int, int, int>> pizzaSlices)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (array[i, j] == 0) // If that place was already choped off then move on to the next one
                        continue;

                    foreach (var shape in possibleShapes)
                    {
                        // Count sum of slice in current shape if placed in place IxJ
                        var sumCurrent = SumCurrent(shape, i, j, tempArray);

                        // Check if sum is correct. More explanations on top. 
                        if (sumCurrent >= minIngredient)
                        {
                            // Add new slice to others
                            pizzaSlices.Add(new Tuple<int, int, int, int>(i, j, i + shape.X - 1, j + shape.Y - 1));

                            // Erase slice from pizza matrix
                            CutSliceFromPizza(shape, i, j, tempArray);

                            // Move to next not choped off place
                            break;
                        }
                    }
                }
            }

            int result = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (tempArray[i, j] == 0)
                        ++result;
                }
            }
            return result;
        }

        private static void PrintPossibleShapes(List<Point> possibleShapes )
        {
            for (int i = 0; i < possibleShapes.Count; i++)
            {
                Console.SetCursorPosition(columns + 5, i);
                Console.WriteLine(possibleShapes.ElementAt(i).X + " x " + possibleShapes.ElementAt(i).Y);
            }
        }

        private static void CutSliceFromPizza(Point shape, int i, int j, int[,] array)
        {
            for (int l = 0; l < shape.X; l++)
            {
                for (int f = 0; f < shape.Y; f++)
                {
                    array[i + l, j + f] = 0;
                }
            }
        }

        private static void SaveResults(string output, List<Tuple<int, int, int, int>> pizzaSlices)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(output);
            file.WriteLine(pizzaSlices.Count);

            foreach (var pizza in pizzaSlices)
            {
                file.WriteLine(pizza.Item1 + " " + pizza.Item2 + " " + pizza.Item3 + " " + pizza.Item4);
            }

            file.Close();
        }

        private static int SumCurrent(Point shape, int i, int j, int[,] array)
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
        }

        private static List<Point> GeneratePossibleShapes()
        {
            List<Point> possibleShapes = new List<Point>();
            for (int i = 0; i <= maxCells; i++)
            {
                for (int j = 0; j <= maxCells; j++)
                {
                    if (i * j <= maxCells && i * j >= minIngredient * 2)
                        possibleShapes.Add(new Point(i, j));
                }
            }

            return possibleShapes;
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
