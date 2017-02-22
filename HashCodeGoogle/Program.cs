using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;


namespace HashCodeGoogle
{
    internal class Program
    {
        private static int _rows;
        private static int _columns;
        private static int _minIngredient;
        private static int _maxCells;
        private static int[,] _array;

        private static void Main(string[] args)
        {

            using (TextReader reader = File.OpenText("small.in"))
            {
                var slices = DividePizza(reader);
                if (slices.Equals(null))
                {
                    Console.WriteLine("Input File is incorrect!");
                    Console.ReadKey();
                    return;
                }

                SaveResults("smallResults.txt", slices);
            }

            using (TextReader reader = File.OpenText("medium.in"))
            {
                var slices = DividePizza(reader);
                if (slices.Equals(null))
                {
                    Console.WriteLine("Input File is incorrect!");
                    Console.ReadKey();
                    return;
                }
                SaveResults("mediumResults.txt", slices);
            }

            using (TextReader reader = File.OpenText("big.in"))
            {
                var slices = DividePizza(reader);
                if (slices.Equals(null))
                {
                    Console.WriteLine("Input File is incorrect!");
                    Console.ReadKey();
                    return;
                }
                SaveResults("bigResults.txt", slices);
            }

            using (TextReader reader = File.OpenText("example.in"))
            {
                var slices = DividePizza(reader);
                if (slices.Equals(null))
                {
                    Console.WriteLine("Input File is incorrect!");
                    Console.ReadKey();
                    return;
                }
                SaveResults("exampleResults.txt", slices);
            }

        }

        private static List<Tuple<int, int, int, int>> DividePizza(TextReader reader)
        {
            // Read data from small.in and initialize Lists
            var readOk = ReadInputData(reader);

            // If file wasn't read correctly, abort
            if (readOk == false)
                return null;

            // Generate all possible shapes
            var possibleShapes = GeneratePossibleShapes();

            // Get same shapes but reversed order
            var possibleShapesReverted = possibleShapes.ToList();
            possibleShapesReverted.Reverse();

            // Varables for future cutted slices
            var pizzaSlices = new List<Tuple<int, int, int, int>>();
            var pizzaSlicesReverted = new List<Tuple<int, int, int, int>>();

            // Copy pizza to new array
            var tempArray = new int[_rows,_columns];
            Array.Copy(_array,tempArray,_array.Length);

            // Caclculate how many pieces you can cut off using normal order of shapes
            var result1 = CheckPermutation(possibleShapes, tempArray, pizzaSlices); // TEMP ARRAY IS MODIFIED

            // Copy pizza once again
            Array.Copy(_array, tempArray, _array.Length);
            // Caclculate how many pieces you can cut off using reversed order of shapes
            var result2 = CheckPermutation(possibleShapesReverted, tempArray, pizzaSlicesReverted);

            // Return better set of slices
            return result1 > result2 ? pizzaSlices : pizzaSlicesReverted;
        }

        private static int CheckPermutation(List<Point> possibleShapes, int[,] tempArray, ICollection<Tuple<int, int, int, int>> pizzaSlices)
        {
            for (var i = 0; i < _rows; i++)
            {
                for (var j = 0; j < _columns; j++)
                {
                    if (_array[i, j] == 0) // If that place was already choped off then move on to the next one
                        continue;

                    foreach (var shape in possibleShapes)
                    {
                        // Cont number of ingredients pairs slice in current shape if placed in place IxJ
                        var sumCurrent = SumCurrent(shape, i, j, tempArray);

                        // Check if it's not enough ingredients try next shape
                        if (sumCurrent < _minIngredient) continue;

                        // Add new slice to list
                        pizzaSlices.Add(new Tuple<int, int, int, int>(i, j, i + shape.X - 1, j + shape.Y - 1));

                        // Erase slice from pizza matrix
                        CutSliceFromPizza(shape, i, j, tempArray);

                        // Move to next not choped off place
                        break;
                    }
                }
            }

            // Calculate how many pieces you menaged to cut off
            var result = CalculateResult(tempArray);


            return result;
        }

        private static int CalculateResult(int[,] tempArray)
        {
            var result = 0;
            for (var i = 0; i < _rows; i++)
            {
                for (var j = 0; j < _columns; j++)
                {
                    // If piece is cut off then increase result
                    if (tempArray[i, j] == 0)
                        ++result;
                }
            }
            return result;
        }

        private static void CutSliceFromPizza(Point shape, int i, int j, int[,] array)
        {
            for (var l = 0; l < shape.X; l++)
            {
                for (var f = 0; f < shape.Y; f++)
                {
                    // Set piece to cut off state
                    array[i + l, j + f] = 0;
                }
            }
        }

        private static void SaveResults(string output, IReadOnlyCollection<Tuple<int, int, int, int>> pizzaSlices)
        {
            var file = new System.IO.StreamWriter(output);
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
            
            for (var l = 0; l < shape.X; l++)
            {
                for (var f = 0; f < shape.Y; f++)
                {
                    if (i + l >= _rows || j + f >= _columns || array[i + l, j + f] == 0)
                        return 0;

                    if (array[i + l, j + f] == 1)
                        ++mushroomCounter;
                    else if (array[i + l, j + f] == 2)
                    {
                        ++tomatoCounter;
                    }
                }
            }

            // Return number of pars tomato-mushroom
            return Math.Min(tomatoCounter,mushroomCounter);
        }

        private static bool ReadInputData(TextReader reader)
        {
            var text = reader.ReadLine();
            if (text == null)
                return false;

            var bits = text.Split(' ');

            _rows = int.Parse(bits[0]);
            _columns = int.Parse(bits[1]);
            _minIngredient = int.Parse(bits[2]);
            _maxCells = int.Parse(bits[3]);
            

            _array = new int[_rows, _columns];

            for (int i = 0; i < _rows; i++)
            {
                text = reader.ReadLine();
                if (text == null)
                    return false;
                for (int j = 0; j < _columns; j++)
                {
                    if (text.ElementAt(j) == 'M')
                        _array[i, j] = 1;
                    else if (text.ElementAt(j) == 'T')
                        _array[i, j] = 2;
                }
            }
            return true;
        }

        private static List<Point> GeneratePossibleShapes()
        {
            List<Point> possibleShapes = new List<Point>();
            for (int i = 0; i <= _maxCells; i++)
            {
                for (int j = 0; j <= _maxCells; j++)
                {
                    if (i * j <= _maxCells && i * j >= _minIngredient * 2)
                        possibleShapes.Add(new Point(i, j));
                }
            }

            return possibleShapes;
        }
    }
}
