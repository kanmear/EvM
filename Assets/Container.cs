using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Container
{
    public static int Columns = 40;
    public static int Rows = 20;
    public static int Gap = 4;
    
    public static Cell[,] Grid = new Cell[Columns, Rows];
    public static readonly Dictionary<(int, int), bool> HelperGrid = new Dictionary<(int, int), bool>();
}