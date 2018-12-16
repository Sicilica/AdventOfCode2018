namespace AdventOfCode2018
{
    public class Day11 : Challenge
    {
        private const int SERIAL = 7139;

        public override string Part1()
        {
            var grid = new Grid(SERIAL);

            var bestStartX = 0;
            var bestStartY = 0;
            var bestTotalPower = int.MinValue;
            for (int startX = 0; startX <= 300 - 3; startX++)
            {
                var totalPower = 0;
                for (int dx = 0; dx < 3; dx++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        totalPower += grid.At(startX + dx, y);
                    }
                }

                for (int startY = 0; startY <= 300 - 3; startY++)
                {
                    if (startY != 0)
                    {
                        for (int dx = 0; dx < 3; dx++)
                        {
                            totalPower -= grid.At(startX + dx, startY - 1);
                            totalPower += grid.At(startX + dx, startY + 2);
                        }
                    }

                    if (totalPower > bestTotalPower)
                    {
                        bestStartX = startX;
                        bestStartY = startY;
                        bestTotalPower = totalPower;
                    }
                }
            }
            
            return $"{bestStartX + 1},{bestStartY + 1}";
        }

        public override string Part2()
        {
            var grid = new Grid(SERIAL);

            var bestSize = 0;
            var bestStartX = 0;
            var bestStartY = 0;
            var bestTotalPower = int.MinValue;
            for (int size = 1; size <= 300; size++)
            {
                for (int startX = 0; startX <= 300 - size; startX++)
                {
                    var totalPower = 0;
                    for (int dx = 0; dx < size; dx++)
                    {
                        for (int y = 0; y < size; y++)
                        {
                            totalPower += grid.At(startX + dx, y);
                        }
                    }

                    for (int startY = 0; startY <= 300 - size; startY++)
                    {
                        if (startY != 0)
                        {
                            for (int dx = 0; dx < size; dx++)
                            {
                                totalPower -= grid.At(startX + dx, startY - 1);
                                totalPower += grid.At(startX + dx, startY + size - 1);
                            }
                        }

                        if (totalPower > bestTotalPower)
                        {
                            bestSize = size;
                            bestStartX = startX;
                            bestStartY = startY;
                            bestTotalPower = totalPower;
                        }
                    }
                }
            }

            return $"{bestStartX + 1},{bestStartY + 1},{bestSize}";
        }



        private struct Grid
        {
            private readonly int[] data;
            private readonly int width;

            public Grid(int serial, int width = 300, int height = 300)
            {
                this.width = width;
                data = new int[width * height];

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int rackID = (x + 1) + 10;
                        int power = rackID * (y + 1);
                        power += serial;
                        power *= rackID;
                        power = (power / 100) % 10;
                        power -= 5;

                        data[x + y * width] = power;
                    }
                }
            }



            public int At(int x, int y)
            {
                return data[x + y * width];
            }
        }
    }
}
