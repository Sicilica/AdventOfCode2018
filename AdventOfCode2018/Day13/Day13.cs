using System;
using System.Collections.Generic;

namespace AdventOfCode2018
{
    public class Day13 : Challenge
    {
        public override string Part1()
        {
            ReadInput(out var map, out var carts);

            var sortedCarts = new SortedDictionary<int, Cart>();
            while (true)
            {
                foreach (var cart in carts) sortedCarts.Add(cart.X + cart.Y * map.Width, cart);

                foreach (var cart in sortedCarts.Values)
                {
                    cart.Move(map);

                    foreach (var otherCart in carts)
                    {
                        if (cart == otherCart) continue;

                        if (cart.X == otherCart.X && cart.Y == otherCart.Y) return $"{cart.X},{cart.Y}";
                    }
                }

                sortedCarts.Clear();
            }
        }

        public override string Part2()
        {
            ReadInput(out var map, out var carts);

            var sortedCarts = new SortedDictionary<int, Cart>();
            while (true)
            {
                foreach (var cart in carts) sortedCarts.Add(cart.X + cart.Y * map.Width, cart);

                foreach (var cart in sortedCarts.Values)
                {
                    if (!carts.Contains(cart)) continue;

                    cart.Move(map);

                    Cart collisionWith = null;
                    foreach (var otherCart in carts)
                    {
                        if (cart == otherCart) continue;

                        if (cart.X == otherCart.X && cart.Y == otherCart.Y)
                        {
                            collisionWith = otherCart;
                            break;
                        }
                    }

                    if (collisionWith != null)
                    {
                        carts.Remove(cart);
                        carts.Remove(collisionWith);
                    }
                }

                if (carts.Count == 1)
                {
                    foreach (var cart in carts) return $"{cart.X},{cart.Y}";
                }

                sortedCarts.Clear();
            }
        }



        private void ReadInput(out Map map, out HashSet<Cart> carts)
        {
            using (var stream = GetResource("Day13/input.txt"))
            using (var reader = new System.IO.StreamReader(stream))
            {
                carts = new HashSet<Cart>();
                map = new Map(reader.ReadToEnd().Replace("\r", "").Split('\n'), carts);
            }
        }



        private class Cart
        {
            private int dx;
            private int dy;
            private int turnCount;

            public int X;
            public int Y;

            public Cart(int x, int y, Direction dir)
            {
                X = x;
                Y = y;

                switch (dir)
                {
                    case Direction.PosX:
                        dx = 1;
                        break;
                    case Direction.PosY:
                        dy = 1;
                        break;
                    case Direction.NegX:
                        dx = -1;
                        break;
                    case Direction.NegY:
                        dy = -1;
                        break;
                }
            }



            public void Move(Map map)
            {
                X += dx;
                Y += dy;

                switch (map.At(X, Y))
                {
                    case Track.Intersection:
                        switch (turnCount++)
                        {
                            case 0:
                                // left turn
                                if (dx == 0)
                                {
                                    dx = dy;
                                    dy = 0;
                                }
                                else
                                {
                                    dy = -dx;
                                    dx = 0;
                                }
                                break;
                            case 1:
                                // straight
                                break;
                            case 2:
                                // right turn
                                if (dx == 0)
                                {
                                    dx = -dy;
                                    dy = 0;
                                }
                                else
                                {
                                    dy = dx;
                                    dx = 0;
                                }

                                turnCount = 0;
                                break;
                        }
                        break;
                    case Track.SameSignTurn:
                        dx += dy;   // nevermind the 2 variable swap
                        dy = dx - dy;
                        dx = dx - dy;
                        break;
                    case Track.FlipSignTurn:
                        dx += dy;
                        dy = dx - dy;
                        dx = dx - dy;

                        dx = -dx;
                        dy = -dy;
                        break;
                }
            }
        }

        private enum Direction
        {
            PosX,
            PosY,
            NegX,
            NegY
        }

        private class Map
        {
            private readonly Track[] data;

            public readonly int Width;
            public readonly int Height;

            public Map(string[] raw, HashSet<Cart> carts)
            {
                Width = raw[0].Length;
                Height = raw.Length;
                data = new Track[Width * Height];

                for (int i = Width * Height - 1; i >= 0; i--)
                {
                    switch (raw[i / Width][i % Width])
                    {
                        case ' ':
                        case '|':
                        case '-':
                            data[i] = Track.Straight;
                            break;
                        case '+':
                            data[i] = Track.Intersection;
                            break;
                        case '\\':
                            data[i] = Track.SameSignTurn;
                            break;
                        case '/':
                            data[i] = Track.FlipSignTurn;
                            break;

                        case '^':
                            data[i] = Track.Straight;
                            carts.Add(new Cart(i % Width, i / Width, Direction.NegY));
                            break;
                        case '<':
                            data[i] = Track.Straight;
                            carts.Add(new Cart(i % Width, i / Width, Direction.NegX));
                            break;
                        case '>':
                            data[i] = Track.Straight;
                            carts.Add(new Cart(i % Width, i / Width, Direction.PosX));
                            break;
                        case 'v':
                            data[i] = Track.Straight;
                            carts.Add(new Cart(i % Width, i / Width, Direction.PosY));
                            break;

                        default:
                            throw new Exception("Unable to parse track");
                    }
                }
            }



            public Track At(int x, int y)
            {
                return data[x + y * Width];
            }
        }

        private enum Track
        {
            Straight,
            Intersection,
            SameSignTurn,   // ie, +X -> +Y, symbolized by "\"
            FlipSignTurn    // ie, +X -> -Y, symbolized by "/"
        }
    }
}
