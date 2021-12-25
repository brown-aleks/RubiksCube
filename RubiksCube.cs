using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Rubiks
{
    public enum AxisOfRotation { X, Xm, Y, Ym, Z, Zm }
    public class PartOfEdge
        {
            public byte I { get; }
            public byte J { get; }
            public byte K { get; }
            public PartOfEdge(byte i, byte j, byte k)
            {
                I = i;
                J = j;
                K = k;
            }
        }
    public class Edge
    {
        private PartOfCube[,,] part;
        public double Angl { get; private set; }
        public AxisOfRotation axis;
        public PartOfEdge[,] arrEdge;
        public Edge() { }
        public Edge(PartOfCube[,,] part, PartOfEdge[,] arrEdge, AxisOfRotation axis)
        { this.part = part; this.arrEdge = arrEdge; this.axis = axis; Angl = 90; }
        public void ResAngl() { Angl = 90; }
        public void Rotacion(bool Clockwise)
        {
            double k = 3;
            foreach (var item in arrEdge)
            {
                part[item.I, item.J, item.K].Rotation = part[item.I, item.J, item.K].Rotation * matrixRot(Clockwise ? k : -k);
            }
            Angl -= k;
            if ( Angl <= 0 )
            {
                    PartOfCube TempPart1 = part[arrEdge[0, 0].I, arrEdge[0, 0].J, arrEdge[0, 0].K];
                    PartOfCube TempPart2 = part[arrEdge[0, 1].I, arrEdge[0, 1].J, arrEdge[0, 1].K];
                if (Clockwise)
                {
                    part[arrEdge[0, 0].I, arrEdge[0, 0].J, arrEdge[0, 0].K] = part[arrEdge[0, 2].I, arrEdge[0, 2].J, arrEdge[0, 2].K];
                    part[arrEdge[0, 2].I, arrEdge[0, 2].J, arrEdge[0, 2].K] = part[arrEdge[2, 2].I, arrEdge[2, 2].J, arrEdge[2, 2].K];
                    part[arrEdge[2, 2].I, arrEdge[2, 2].J, arrEdge[2, 2].K] = part[arrEdge[2, 0].I, arrEdge[2, 0].J, arrEdge[2, 0].K];
                    part[arrEdge[2, 0].I, arrEdge[2, 0].J, arrEdge[2, 0].K] = TempPart1;

                    part[arrEdge[0, 1].I, arrEdge[0, 1].J, arrEdge[0, 1].K] = part[arrEdge[1, 2].I, arrEdge[1, 2].J, arrEdge[1, 2].K];
                    part[arrEdge[1, 2].I, arrEdge[1, 2].J, arrEdge[1, 2].K] = part[arrEdge[2, 1].I, arrEdge[2, 1].J, arrEdge[2, 1].K];
                    part[arrEdge[2, 1].I, arrEdge[2, 1].J, arrEdge[2, 1].K] = part[arrEdge[1, 0].I, arrEdge[1, 0].J, arrEdge[1, 0].K];
                    part[arrEdge[1, 0].I, arrEdge[1, 0].J, arrEdge[1, 0].K] = TempPart2;
                }
                else
                {
                    part[arrEdge[0, 0].I, arrEdge[0, 0].J, arrEdge[0, 0].K] = part[arrEdge[2, 0].I, arrEdge[2, 0].J, arrEdge[2, 0].K];
                    part[arrEdge[2, 0].I, arrEdge[2, 0].J, arrEdge[2, 0].K] = part[arrEdge[2, 2].I, arrEdge[2, 2].J, arrEdge[2, 2].K];
                    part[arrEdge[2, 2].I, arrEdge[2, 2].J, arrEdge[2, 2].K] = part[arrEdge[0, 2].I, arrEdge[0, 2].J, arrEdge[0, 2].K];
                    part[arrEdge[0, 2].I, arrEdge[0, 2].J, arrEdge[0, 2].K] = TempPart1;

                    part[arrEdge[0, 1].I, arrEdge[0, 1].J, arrEdge[0, 1].K] = part[arrEdge[1, 0].I, arrEdge[1, 0].J, arrEdge[1, 0].K];
                    part[arrEdge[1, 0].I, arrEdge[1, 0].J, arrEdge[1, 0].K] = part[arrEdge[2, 1].I, arrEdge[2, 1].J, arrEdge[2, 1].K];
                    part[arrEdge[2, 1].I, arrEdge[2, 1].J, arrEdge[2, 1].K] = part[arrEdge[1, 2].I, arrEdge[1, 2].J, arrEdge[1, 2].K];
                    part[arrEdge[1, 2].I, arrEdge[1, 2].J, arrEdge[1, 2].K] = TempPart2;
                }
                //Console.WriteLine(RubiksCube.Info(part));
                foreach (var item in part)
                {
                    item.RotationRound();
                }
                //Console.WriteLine(RubiksCube.InfoMatrixRotation(part));
            }
        }
        private Matrix4 matrixRot(double DeltaAngle)
        {
            return axis switch
            {
                AxisOfRotation.X => Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(DeltaAngle)),
                AxisOfRotation.Xm => Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(-DeltaAngle)),
                AxisOfRotation.Y => Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(-DeltaAngle)),
                AxisOfRotation.Ym => Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(DeltaAngle)),
                AxisOfRotation.Z => Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(DeltaAngle)),
                AxisOfRotation.Zm => Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(-DeltaAngle)),
                _ => Matrix4.Identity,
            };
        }
    }
    public class PartOfCube
    {
        private Matrix4 rotacion;
        public string Name { get; }
        public Matrix4 Rotation { get => rotacion; set => rotacion = value; }       // Текущее положение поворота кубика относительно центра всего рубика.
        public Matrix4 Position { get; }   // стартовое расположение кубика
        public byte Front { get; }         //  фасад - индекс текстуры
        public byte Top { get; }           //  верх
        public byte Bottom { get; }        //  низ
        public byte Left { get; }          //  лево
        public byte Right { get; }         //  право
        public byte Rear { get; }          //  тыл

        public PartOfCube(string name, Matrix4 Pos, byte front = 0, byte top = 0, byte bottom = 0, byte left = 0, byte right = 0, byte rear = 0)
        {
            Name = name;
            Position = Pos;
            Front = front;
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
            Rear = rear;
            Rotation = Matrix4.Identity;    //  Единичная матрица, указывающая, что начальные углы вращения относительно центра всей группы равны нулю.
        }
        public void RotationRound()
        {
            rotacion = new( (float)Math.Round(rotacion.M11, MidpointRounding.AwayFromZero), (float)Math.Round(rotacion.M12, MidpointRounding.AwayFromZero), (float)Math.Round(rotacion.M13, MidpointRounding.AwayFromZero), (float)Math.Round(rotacion.M14, MidpointRounding.AwayFromZero),
                            (float)Math.Round(rotacion.M21, MidpointRounding.AwayFromZero), (float)Math.Round(rotacion.M22, MidpointRounding.AwayFromZero), (float)Math.Round(rotacion.M23, MidpointRounding.AwayFromZero), (float)Math.Round(rotacion.M24, MidpointRounding.AwayFromZero),
                            (float)Math.Round(rotacion.M31, MidpointRounding.AwayFromZero), (float)Math.Round(rotacion.M32, MidpointRounding.AwayFromZero), (float)Math.Round(rotacion.M33, MidpointRounding.AwayFromZero), (float)Math.Round(rotacion.M34, MidpointRounding.AwayFromZero),
                            (float)Math.Round(rotacion.M41, MidpointRounding.AwayFromZero), (float)Math.Round(rotacion.M42, MidpointRounding.AwayFromZero), (float)Math.Round(rotacion.M43, MidpointRounding.AwayFromZero), (float)Math.Round(rotacion.M44, MidpointRounding.AwayFromZero));
        }
    }
    public class RubiksCube
    {
        public PartOfCube[,,] Part =
        {
            {
                {
                    new PartOfCube("RWB 000", Matrix4.CreateTranslation(-2.0f,-2.0f, 2.0f), 10,0,0,8,0,0),      // Ф,В,Н,Л,П,Т
                    new PartOfCube("RW_ 001", Matrix4.CreateTranslation( 0.0f,-2.0f, 2.0f), 10,0,0,0,0,0),      // 0  - Белый
                    new PartOfCube("RWG 002", Matrix4.CreateTranslation( 2.0f,-2.0f, 2.0f), 10,0,0,0,4,0)       // 1  - Белый Литер
                },                                                                                              // 2  - Жёлтый
                {                                                                                               // 3  - Жёлтый Литер
                    new PartOfCube("RB_ 010", Matrix4.CreateTranslation(-2.0f, 0.0f, 2.0f), 10,0,0,8,0,0),      // 4  - Зелёный
                    new PartOfCube("R__ 011", Matrix4.CreateTranslation( 0.0f, 0.0f, 2.0f), 11,0,0,0,0,0),      // 5  - Зелёный Литер       //  Красный Литер
                    new PartOfCube("RG_ 012", Matrix4.CreateTranslation( 2.0f, 0.0f, 2.0f), 10,0,0,0,4,0)       // 6  - Ораньжевый
                },                                                                                              // 7  - Ораньжевый Литер
                {                                                                                               // 8  - Синий
                    new PartOfCube("RYB 020", Matrix4.CreateTranslation(-2.0f, 2.0f, 2.0f), 10,2,0,8,0,0),      // 9  - Синий Литер
                    new PartOfCube("RY_ 021", Matrix4.CreateTranslation( 0.0f, 2.0f, 2.0f), 10,2,0,0,0,0),      // 10 - Красный
                    new PartOfCube("RYG 022", Matrix4.CreateTranslation( 2.0f, 2.0f, 2.0f), 10,2,0,0,4,0)       // 11 - Красный Литер
                }
            },
            {
                {
                    new PartOfCube("WB_ 100", Matrix4.CreateTranslation(-2.0f,-2.0f, 0.0f), 0,0,0,8,0,0),       // Ф,В,Н,Л,П,Т
                    new PartOfCube("W__ 101", Matrix4.CreateTranslation( 0.0f,-2.0f, 0.0f), 0,0,1,0,0,0),       // 0  - Белый               //  Белый Литер
                    new PartOfCube("WG_ 102", Matrix4.CreateTranslation( 2.0f,-2.0f, 0.0f), 0,0,0,0,4,0)        // 1  - Белый Литер
                },                                                                                              // 2  - Жёлтый
                {                                                                                               // 3  - Жёлтый Литер
                    new PartOfCube("B__ 110", Matrix4.CreateTranslation(-2.0f, 0.0f, 0.0f), 0,0,0,9,0,0),       // 4  - Зелёный             //  Синий Литер
                    new PartOfCube("cen 111", Matrix4.CreateTranslation( 0.0f, 0.0f, 0.0f), 0,0,0,0,0,0),       // 5  - Зелёный Литер
                    new PartOfCube("G__ 112", Matrix4.CreateTranslation( 2.0f, 0.0f, 0.0f), 0,0,0,0,5,0)        // 6  - Ораньжевый          //  Зелёный Литер
                },                                                                                              // 7  - Ораньжевый Литер
                {                                                                                               // 8  - Синий
                    new PartOfCube("YB_ 120", Matrix4.CreateTranslation(-2.0f, 2.0f, 0.0f), 0,2,0,8,0,0),       // 9  - Синий Литер
                    new PartOfCube("Y__ 121", Matrix4.CreateTranslation( 0.0f, 2.0f, 0.0f), 0,3,0,0,0,0),       // 10 - Красный             //  Жёлтый Литер
                    new PartOfCube("YG_ 122", Matrix4.CreateTranslation( 2.0f, 2.0f, 0.0f), 0,2,0,0,4,0)        // 11 - Красный Литер
                }
            },
            {
                {
                    new PartOfCube("OWB 200", Matrix4.CreateTranslation(-2.0f,-2.0f,-2.0f), 0,0,0,8,0,6),       // Ф,В,Н,Л,П,Т
                    new PartOfCube("OW_ 201", Matrix4.CreateTranslation( 0.0f,-2.0f,-2.0f), 0,0,0,0,0,6),       // 0  - Белый
                    new PartOfCube("OWG 202", Matrix4.CreateTranslation( 2.0f,-2.0f,-2.0f), 0,0,0,0,4,6)        // 1  - Белый Литер
                },                                                                                              // 2  - Жёлтый
                {                                                                                               // 3  - Жёлтый Литер
                    new PartOfCube("OB_ 210", Matrix4.CreateTranslation(-2.0f, 0.0f,-2.0f), 0,0,0,8,0,6),       // 4  - Зелёный
                    new PartOfCube("O__ 211", Matrix4.CreateTranslation( 0.0f, 0.0f,-2.0f), 0,0,0,0,0,7),       // 5  - Зелёный Литер       //  Ораньжевый Литер
                    new PartOfCube("OG_ 212", Matrix4.CreateTranslation( 2.0f, 0.0f,-2.0f), 0,0,0,0,4,6)        // 6  - Ораньжевый
                },                                                                                              // 7  - Ораньжевый Литер
                {                                                                                               // 8  - Синий
                    new PartOfCube("OYB 220", Matrix4.CreateTranslation(-2.0f, 2.0f,-2.0f), 0,2,0,8,0,6),       // 9  - Синий Литер
                    new PartOfCube("OY_ 221", Matrix4.CreateTranslation( 0.0f, 2.0f,-2.0f), 0,2,0,0,0,6),       // 10 - Красный
                    new PartOfCube("OYG 222", Matrix4.CreateTranslation( 2.0f, 2.0f,-2.0f), 0,2,0,0,4,6)        // 11 - Красный Литер
                }
            }
        };

        PartOfEdge[,] arrEdgeOfOrange = {
            {new(2, 0, 2),new(2, 0, 1),new(2, 0, 0)},
            {new(2, 1, 2),new(2, 1, 1),new(2, 1, 0)},
            {new(2, 2, 2),new(2, 2, 1),new(2, 2, 0)}
        };
        PartOfEdge[,] arrEdgeOfOrangeMiddle = {
            {new(1, 0, 2),new(1, 0, 1),new(1, 0, 0)},
            {new(1, 1, 2),new(1, 1, 1),new(1, 1, 0)},
            {new(1, 2, 2),new(1, 2, 1),new(1, 2, 0)}
        };
        PartOfEdge[,] arrEdgeOfRed = {
            {new(0, 0, 0),new(0, 0, 1),new(0, 0, 2)},
            {new(0, 1, 0),new(0, 1, 1),new(0, 1, 2)},
            {new(0, 2, 0),new(0, 2, 1),new(0, 2, 2)}
        };
        PartOfEdge[,] arrEdgeOfRedMiddle = {
            {new(1, 0, 0),new(1, 0, 1),new(1, 0, 2)},
            {new(1, 1, 0),new(1, 1, 1),new(1, 1, 2)},
            {new(1, 2, 0),new(1, 2, 1),new(1, 2, 2)}
        };
        PartOfEdge[,] arrEdgeOfGreen = {
            {new(0, 0, 2),new(1, 0, 2),new(2, 0, 2)},
            {new(0, 1, 2),new(1, 1, 2),new(2, 1, 2)},
            {new(0, 2, 2),new(1, 2, 2),new(2, 2, 2)}
        };
        PartOfEdge[,] arrEdgeOfGreenMiddle = {
            {new(0, 0, 1),new(1, 0, 1),new(2, 0, 1)},
            {new(0, 1, 1),new(1, 1, 1),new(2, 1, 1)},
            {new(0, 2, 1),new(1, 2, 1),new(2, 2, 1)}
        };
        PartOfEdge[,] arrEdgeOfBlue = {
            {new(2, 0, 0),new(1, 0, 0),new(0, 0, 0)},
            {new(2, 1, 0),new(1, 1, 0),new(0, 1, 0)},
            {new(2, 2, 0),new(1, 2, 0),new(0, 2, 0)}
       };
        PartOfEdge[,] arrEdgeOfBlueMiddle = {
            {new(2, 0, 1),new(1, 0, 1),new(0, 0, 1)},
            {new(2, 1, 1),new(1, 1, 1),new(0, 1, 1)},
            {new(2, 2, 1),new(1, 2, 1),new(0, 2, 1)}
       };
        PartOfEdge[,] arrEdgeOfYellow = {
            {new(2, 2, 2),new(2, 2, 1),new(2, 2, 0)},
            {new(1, 2, 2),new(1, 2, 1),new(1, 2, 0)},
            {new(0, 2, 2),new(0, 2, 1),new(0, 2, 0)}
        };
        PartOfEdge[,] arrEdgeOfYellowMiddle = {
            {new(2, 1, 2),new(2, 1, 1),new(2, 1, 0)},
            {new(1, 1, 2),new(1, 1, 1),new(1, 1, 0)},
            {new(0, 1, 2),new(0, 1, 1),new(0, 1, 0)}
        };
        PartOfEdge[,] arrEdgeOfWhite = {
            {new(0, 0, 2),new(0, 0, 1),new(0, 0, 0)},
            {new(1, 0, 2),new(1, 0, 1),new(1, 0, 0)},
            {new(2, 0, 2),new(2, 0, 1),new(2, 0, 0)}
        };
        PartOfEdge[,] arrEdgeOfWhiteMiddle = {
            {new(0, 1, 2),new(0, 1, 1),new(0, 1, 0)},
            {new(1, 1, 2),new(1, 1, 1),new(1, 1, 0)},
            {new(2, 1, 2),new(2, 1, 1),new(2, 1, 0)}
        };

        public Edge White, WhiteMiddle, Yellow, YellowMiddle, Green, GreenMiddle, Orange, OrangeMiddle, Blue, BlueMiddle, Red, RedMiddle;

        private class RotEdge
        {
            public Edge Edge { get; }
            public bool Clockwise { get; }
            public bool Repay { get; set; }
            public RotEdge(Edge edge, bool clockwise)
            {
                Edge = edge;
                Clockwise = clockwise;
                Repay = true;
            }
            public RotEdge(Edge edge, bool clockwise, bool repay)
            {
                Edge = edge;
                Clockwise = clockwise;
                Repay = repay;
            }
        }

        Queue<RotEdge> QueueRot = new Queue<RotEdge>();
        Stack<RotEdge> BackQueueRot = new Stack<RotEdge>();
        public void Rotacion()
        {
            if (QueueRot.Count != 0)
            {
                RotEdge rotEdge = QueueRot.Peek();
                if (rotEdge.Edge.Angl <= 0)
                {
                    rotEdge.Edge.ResAngl();
                    if (rotEdge.Repay)
                    {
                        BackQueueRot.Push(new(rotEdge.Edge, !rotEdge.Clockwise, false));
                    } 
                    QueueRot.Dequeue();
                    return;
                }
                rotEdge.Edge.Rotacion(rotEdge.Clockwise);
            }
        }
        public void Rotacion(Edge edge, bool clockwise)
        {
            QueueRot.Enqueue(new(edge, clockwise));
        }
        public void Untangle()
        {
            if (QueueRot.Count <= 0 && BackQueueRot.Count > 0)
            {
                QueueRot = new Queue<RotEdge>(BackQueueRot);
                BackQueueRot = new Stack<RotEdge>();
            }
        }
        public void Confuse(int rot)
        {
            for (int i = 0; i < rot; i++)
            {
                Random v = new();
                switch (v.Next(25))
                {
                    case 1: QueueRot.Enqueue(new(White, true)); break;
                    case 2: QueueRot.Enqueue(new(White, false)); break;
                    case 3: QueueRot.Enqueue(new(Yellow, true)); break;
                    case 4: QueueRot.Enqueue(new(Yellow, false)); break;
                    case 5: QueueRot.Enqueue(new(Green, true)); break;
                    case 6: QueueRot.Enqueue(new(Green, false)); break;
                    case 7: QueueRot.Enqueue(new(Orange, true)); break;
                    case 8: QueueRot.Enqueue(new(Orange, false)); break;
                    case 9: QueueRot.Enqueue(new(Blue, true)); break;
                    case 10: QueueRot.Enqueue(new(Blue, false)); break;
                    case 11: QueueRot.Enqueue(new(Red, true)); break;
                    case 12: QueueRot.Enqueue(new(Red, false)); break;
                    case 13: QueueRot.Enqueue(new(WhiteMiddle, true)); break;
                    case 14: QueueRot.Enqueue(new(WhiteMiddle, false)); break;
                    case 15: QueueRot.Enqueue(new(YellowMiddle, true)); break;
                    case 16: QueueRot.Enqueue(new(YellowMiddle, false)); break;
                    case 17: QueueRot.Enqueue(new(GreenMiddle, true)); break;
                    case 18: QueueRot.Enqueue(new(GreenMiddle, false)); break;
                    case 19: QueueRot.Enqueue(new(OrangeMiddle, true)); break;
                    case 20: QueueRot.Enqueue(new(OrangeMiddle, false)); break;
                    case 21: QueueRot.Enqueue(new(BlueMiddle, true)); break;
                    case 22: QueueRot.Enqueue(new(BlueMiddle, false)); break;
                    case 23: QueueRot.Enqueue(new(RedMiddle, true)); break;
                    case 24: QueueRot.Enqueue(new(RedMiddle, false)); break;
                    default:
                        break;
                }
            }
        }
        public RubiksCube()
        {
            Red = new(Part, arrEdgeOfRed, AxisOfRotation.Zm);
            RedMiddle = new(Part, arrEdgeOfRedMiddle, AxisOfRotation.Zm);
            Orange = new(Part, arrEdgeOfOrange, AxisOfRotation.Z);
            OrangeMiddle = new(Part, arrEdgeOfOrangeMiddle, AxisOfRotation.Z);
            Green = new(Part, arrEdgeOfGreen, AxisOfRotation.Xm);
            GreenMiddle = new(Part, arrEdgeOfGreenMiddle, AxisOfRotation.Xm);
            Blue = new(Part, arrEdgeOfBlue, AxisOfRotation.X);
            BlueMiddle = new(Part, arrEdgeOfBlueMiddle, AxisOfRotation.X);
            Yellow = new(Part, arrEdgeOfYellow, AxisOfRotation.Y);
            YellowMiddle = new(Part, arrEdgeOfYellowMiddle, AxisOfRotation.Y);
            White = new(Part, arrEdgeOfWhite, AxisOfRotation.Ym);
            WhiteMiddle = new(Part, arrEdgeOfWhiteMiddle, AxisOfRotation.Ym);
        }
        public static string Info(PartOfCube[,,] Part)
        {
            string str = "";
            str += "part\n-----------------------------------\n";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        str += $"{Part[i,j,k].Name}\t";
                    }
                    str += "\n";
                }
                str += "\n";
            }
            str += "----------------------------------------\n";
            return str;
        }
        public static string InfoMatrixRotation(PartOfCube[,,] Part)
        {
            string str = "";
            str += "part\n-----------------------------------\n";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        str += $"{Part[i,j,k].Rotation}\t";
                    }
                    str += "\n";
                }
                str += "\n";
            }
            str += "----------------------------------------\n";
            return str;
        }

    }
}