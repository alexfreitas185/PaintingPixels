using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

namespace PaintingPixels
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("========== START OF TESTING ==========");

            GridOfColours myGrid = new GridOfColours(15, 15);
            Console.WriteLine("");
            Console.WriteLine("Grid Details:");
            Console.WriteLine(myGrid.ToString());
            
            Console.WriteLine("");
            Console.WriteLine("Painting range 1, 8 with red...");
            int ix =  myGrid.FillCell(0, 7, Color.Red);
            Console.WriteLine("Filled cells: \n" + myGrid.OutputFilledCells());

            Console.WriteLine("");
            Console.WriteLine($"Painting cell {ix} with green...");
            myGrid.FillCell(ix, Color.Green);
            Console.WriteLine("Filled cells: \n" + myGrid.OutputFilledCells());

            Console.WriteLine("");
            Console.WriteLine("Painting range 1, 1 with blue...");
            myGrid.FillCell(0, 0, Color.Blue);
            Console.WriteLine("Filled cells: \n" + myGrid.OutputFilledCells());

            Console.WriteLine("");
            Console.WriteLine("Painting row 2, cols 6-12 with gray...");
            myGrid.FillRow(2, 5, 11, Color.Gray);
            Console.WriteLine("Filled cells: \n" + myGrid.OutputFilledCells());

            Console.WriteLine("");
            Console.WriteLine("Change one of the cells from Gray to Yellow (should flood to adjacents)...");
            myGrid.FillCellFlood(2, 11, Color.Yellow);
            Console.WriteLine("Filled cells: \n" + myGrid.OutputFilledCells());

            Console.WriteLine("");
            Console.WriteLine("Resetting grid...");
            myGrid.ResetGrid();
            Console.WriteLine("Filled cells: \n" + myGrid.OutputFilledCells());


            //SMALLER Grid
            myGrid = new GridOfColours(5, 5);
            Console.WriteLine("");
            Console.WriteLine("New Grid Details:");
            Console.WriteLine(myGrid.ToString());

            Console.WriteLine("");
            Console.WriteLine("Painting row 2, cols 2-4 with gray...");
            myGrid.FillRow(1, 1, 3, Color.Gray);
            Console.WriteLine("Filled cells: \n" + myGrid.OutputFilledCells());

            Console.WriteLine("");
            Console.WriteLine("Painting row 3, cols 2-4 with gray...");
            myGrid.FillRow(2, 1, 3, Color.Gray);
            Console.WriteLine("Filled cells: \n" + myGrid.OutputFilledCells());

            Console.WriteLine("");
            Console.WriteLine("Painting row 4, cols 2-4 with gray...");
            myGrid.FillRow(3, 1, 3, Color.Gray);
            Console.WriteLine("Filled cells: \n" + myGrid.OutputFilledCells());

            Console.WriteLine("");
            Console.WriteLine("Flood the square from the center (3, 3) with pink...");
            myGrid.FillCellFlood(2, 3, Color.Pink);
            Console.WriteLine("Filled cells: \n" + myGrid.OutputFilledCells());

            Console.WriteLine("");
            Console.WriteLine("========== END OF TESTING ==========");
            Console.WriteLine(Environment.NewLine + Environment.NewLine);
            
        }


        /// <summary>
        /// 
        /// </summary>
        public class GridOfColours
        {
            public enum AdjCellPos
            {
                Top, Right, Bottom, Left
            }

            #region Properties

            public int Rows { get; }

            public int Columns { get; }

            public GridCell[] GridCells 
            {
                get { return _gridCells; } 
            }
            GridCell[] _gridCells;

            #endregion 


            public GridOfColours(int p_Rows, int p_Cols)
            {
                Rows = p_Rows;
                Columns = p_Cols;
                BuildGrid();
            }


            #region Private Methods

            private void BuildGrid()
            {
                _gridCells = new GridCell[this.Rows * this.Columns];
                int absIndex = -1;
                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Columns; c++)
                    {
                        absIndex += 1;
                        GridCell cell = new GridCell(r, c, absIndex);
                        this.GridCells[absIndex] = cell;
                    }
                }
            }

            private int FindCellAbsoluteIndex(int p_Row, int p_Col)
            {
                int absIndex = -1;
                try
                {
                    GridCell cellToColour = this.GridCells.Where(cell => cell.Row == p_Row && cell.Column == p_Col).ToArray()[0];
                    if (cellToColour != null)
                    {
                        absIndex = cellToColour.AbsIndex;
                    }
                }
                catch (Exception ex)
                {
                    //TODO: Implement error raising/logging.
                    return -1;
                }

                return absIndex;

            }

            #endregion


            #region Public Methods

            /// <summary>
            /// Find all adjacent cells and return them on a list.
            /// </summary>
            public List<Tuple<GridCell, AdjCellPos>> GetAdjCells(GridCell p_Cell)
            {
                List<Tuple<GridCell, AdjCellPos>> adjCells = new List<Tuple<GridCell, AdjCellPos>>();

                foreach (GridCell eachCell in this.GridCells)
                {
                    if (eachCell.Row >= (p_Cell.Row + 2) || eachCell.Column >= (p_Cell.Column + 2)) //2 rows or cols apart, not adjacent at all.
                        continue;
                    

                    //Check if Top
                    if ( eachCell.Row == (p_Cell.Row -1) && eachCell.Column == p_Cell.Column )
                    {
                        adjCells.Add(new Tuple<GridCell, AdjCellPos>(eachCell, AdjCellPos.Top));
                        continue;
                    }

                    //Check if Right
                    if ( eachCell.Row == p_Cell.Row && eachCell.Column == (p_Cell.Column + 1) )
                    {
                        adjCells.Add(new Tuple<GridCell, AdjCellPos>(eachCell, AdjCellPos.Right));
                        continue;
                    }

                    //Check if Bottom
                    if (eachCell.Row == (p_Cell.Row + 1) && eachCell.Column == p_Cell.Column)
                    {
                        adjCells.Add(new Tuple<GridCell, AdjCellPos>(eachCell, AdjCellPos.Bottom));
                        continue;
                    }

                    //Check if Left
                    if (eachCell.Row == p_Cell.Row && eachCell.Column == (p_Cell.Column - 1))
                    {
                        adjCells.Add(new Tuple<GridCell, AdjCellPos>(eachCell, AdjCellPos.Left));
                        continue;
                    }

                    if (adjCells.Count >= 4)
                        break;

                }
                               
                return adjCells;
                
            }
            
            /// <summary>
            /// Returns the absolute index of the cell.
            /// </summary>
            public int FillCell(int p_Row, int p_Col, Color p_Color)
            {
                int absIndex = FindCellAbsoluteIndex(p_Row, p_Col);
                FillCell(absIndex, p_Color);
                return absIndex;
            }

            public void FillCell(int p_Index, Color p_Color)
            {
                try
                {
                    this.GridCells[p_Index].Colour = p_Color;
                }
                catch (Exception ex)
                {
                    //TODO: Implement error raising/logging.
                }

            }

            public void FillRow(int p_Row, int p_ColFrom, int p_ColTo, Color p_Color)
            {
                foreach (GridCell eachCell in this.GridCells)
                {
                    if (eachCell.Row < p_Row)
                        continue;

                    if (eachCell.Row > p_Row)
                        return;

                    if (eachCell.Row == p_Row && eachCell.Column >= p_ColFrom && eachCell.Column <= p_ColTo)
                    {
                        eachCell.Colour = p_Color;
                    }

                }
            }

            public void FillColumn(int p_Col, int p_RowFrom, int p_RowTo, Color p_Color)
            {
                foreach (GridCell eachCell in this.GridCells)
                {
                    if (eachCell.Row < p_RowFrom || eachCell.Column < p_Col || eachCell.Column > p_Col)
                        continue;

                    if (eachCell.Row > p_RowTo)
                        return;

                    eachCell.Colour = p_Color;

                }
            }

            /// <summary>
            /// Paints a cell and all adjacent ones that had the same colour.
            /// </summary>
            /// <returns>The absolute index of the cell.</returns>
            public int FillCellFlood(int p_Row, int p_Col, Color p_Color)
            {
                int absIndex = FindCellAbsoluteIndex(p_Row, p_Col);
                FillCellFlood(absIndex, p_Color);
                return absIndex;
            }

            /// <summary>
            /// Paints a cell and all adjacent ones that had the same colour.
            /// </summary>
            /// <param name="p_CellIndex">The absolute index of the aimed cell.</param>
            public void FillCellFlood(int p_CellIndex, Color p_Color)
            {
                try
                {
                    GridCell cell = this.GridCells[p_CellIndex];
                    Color origColour = cell.Colour; //It may not have a colour
                    cell.Colour = p_Color;

                    List<Tuple<GridCell, AdjCellPos>> adjCells = this.GetAdjCells(cell);
                    foreach (Tuple<GridCell, AdjCellPos> eachCell in adjCells)
                    {
                        if (eachCell.Item1.Colour == origColour)
                        {
                            FillCellFlood(eachCell.Item1.AbsIndex, p_Color);
                        }

                    }

                }
                catch (Exception ex)
                {
                    //TODO: Implement error raising/logging.
                }

            }

            public string OutputFilledCells()
            {
                StringBuilder sb = new StringBuilder();

                foreach (GridCell cell in this.GridCells)
                {
                    if (cell.Colour != null && cell.Colour != Color.Empty)
                    {
                        if (sb.Length>=1)
                            sb.Append(", " + Environment.NewLine);

                        sb.Append("{ ");
                        sb.Append($"Cell: [{cell.Row}, {cell.Column}] Absolute: {cell.AbsIndex}, Colour: {cell.Colour.ToString()}");
                        sb.Append(" }");
                    }
                }

                return sb.ToString();
            }

            public void ResetGrid()
            {
                this._gridCells = null;
                BuildGrid();
            }

            public override string ToString()
            {
                return $"Rows: {this.Rows}, Columns: {this.Columns}, Cells: {this.GridCells.Length}";
            }
            
            #endregion


            #region Not Implemented

            /// <summary>
            /// This hasn't been asked but a grid must have this option.
            /// </summary>
            public void FillRange(int p_RowFrom, int p_ColFrom, int p_RowTo, int p_ColTo)
            {
                throw new NotImplementedException("Not implemented. But why are you using this if you never asked?");
            }

            /// <summary>
            /// This hasn't been asked but a grid must have this option.
            /// </summary>
            public void FillRange(int p_AbsIndexFrom, int p_AbsIndexTo)
            {
                throw new NotImplementedException("Not implemented. But why are you using this if you never asked?");
            }

            #endregion

        }

        public class GridCell
        {
            public int Row { get; }

            public int Column { get; }

            /// <summary>
            /// Absolute index in the grid.
            /// </summary>
            public int AbsIndex { get; }

            public Color Colour { get; set; }



            public GridCell(int p_Row, int p_Col, int p_Index)
            {
                this.Row = p_Row;
                this.Column = p_Col;
                this.AbsIndex = p_Index;
            }
              
            

        }

    }
}
