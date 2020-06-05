using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace SudokuSolver
{
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private bool _completed;
        private Task _solveTask;

        public MainForm()
        {
            this.InitializeComponent();
            this.Prep();
        }

        private void Prep()
        {
            this.StyleGrid();
            this.FillGrid();
            this.SetBackColours();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this._completed = false;
        }

        private void SetBackColours()
        {
            for (var i = 0; i < this.sudokuGrid.Rows.Count; i++)
            {
                for (var x = 0; x < this.sudokuGrid.ColumnCount; x++)
                {
                    if (x >= 3 && x < 6 && i < 3)
                    {
                        //// Top Middle Cube
                        this.sudokuGrid.Rows[i].Cells[x].Style.BackColor = Color.DarkGray;
                    }
                    else if (x < 3 && i >= 3 && i < 6)
                    {
                        //// Middle Left Cube
                        this.sudokuGrid.Rows[i].Cells[x].Style.BackColor = Color.DarkGray;
                    }
                    else if (x >= 6 && i >= 3 && i < 6)
                    {
                        //// Middle Right Cube
                        this.sudokuGrid.Rows[i].Cells[x].Style.BackColor = Color.DarkGray;
                    }
                    else if (x >= 3 && x < 6 && i >= 6 && i < 10)
                    {
                        //// Bottom Middle Cube
                        this.sudokuGrid.Rows[i].Cells[x].Style.BackColor = Color.DarkGray;
                    }
                }
            }
        }

        private void StyleGrid()
        {
            this.sudokuGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            this.sudokuGrid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            this.sudokuGrid.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.sudokuGrid.RowsDefaultCellStyle.Font = new Font("Arial", 18, FontStyle.Bold);
        }

        private void FillGrid()
        {
            for (var i = 0; i < 9; i++)
            {
                var row = new DataGridViewRow();

                for (var x = 0; x < 9; x++)
                {
                    var cell = new DataGridViewTextBoxCell();
                    row.Cells.Add(cell);
                }

                this.sudokuGrid.Rows.Add(row);
                this.sudokuGrid.Rows[i].Height = 46;
            }
        }

        public int GetCellValue(DataGridViewCell cell)
        {
            return Convert.ToInt32(cell.Value);
        }

        public void Solve()
        {
            if (this._completed)
            {
                return;
            }

            for (var y = 0; y < 9; y++)
            {
                for (var x = 0; x < 9; x++)
                {
                    if (this.GetCellValue(this.sudokuGrid.Rows[y].Cells[x]) == 0)
                    {
                        if (!this.sudokuGrid.Rows[y].Cells[x].ReadOnly)
                        {
                            for (var n = 1; n < 10; n++)
                            {
                                if (this.ValueIsPossible(y, x, n))
                                {
                                    this.SetCellValueOnUi(this.sudokuGrid.Rows[y].Cells[x], n);
                                    this.Solve();
                                    if (this._completed)
                                    {
                                        return;
                                    }
                                    this.SetCellValueOnUi(this.sudokuGrid.Rows[y].Cells[x], null); //// Set to 0 if can't solve
                                }
                            }
                        }
                        

                        return;
                    }
                }
            }

            this._completed = true;
            this._solveTask = null;
        }

        private void SetCellValueOnUi(DataGridViewCell cell, int? value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => this.SetCellValue(cell, value)));
                return;
            }

            this.SetCellValue(cell, value);
        }

        private void SetCellValue(DataGridViewCell cell, int? value)
        {
            this.sudokuGrid.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = value;
        }

        private bool ValueIsPossible(int y, int x, int n)
        {
            for (var i = 0; i < 9; i++)
            {
                if (this.GetCellValue(this.sudokuGrid.Rows[y].Cells[i]) == n)
                {
                    return false;
                }
            }

            for (var i = 0; i < 9; i++)
            {
                if (this.GetCellValue(this.sudokuGrid.Rows[i].Cells[x]) == n)
                {
                    return false;
                }
            }

            var cubeNumber = this.GetColumnCubeNumber(x);
            var rowCube = this.GetRowCubeNumber(y);

            return this.CheckCube(rowCube, cubeNumber, n);
        }

        private bool CheckCube(int rowCube, int cubeNumber, int value)
        {
            var rows = this.GetRows(rowCube);
            var cells = this.GetCells(cubeNumber, rows);

            foreach (DataGridViewCell singleCell in cells)
            {
                if (this.GetCellValue(singleCell) == value)
                {
                    return false;
                }
            }

            return true;
        }

        private List<DataGridViewCell> GetCells(int cubeNumber, List<DataGridViewRow> rows)
        {
            var columns = new List<DataGridViewCell>();

            if (cubeNumber == 1)
            {
                rows.ForEach(singleRow =>
                {
                    columns.Add(singleRow.Cells[0]);
                    columns.Add(singleRow.Cells[1]);
                    columns.Add(singleRow.Cells[2]);
                });
            }
            else if (cubeNumber == 2)
            {
                rows.ForEach(singleRow =>
                {
                    columns.Add(singleRow.Cells[3]);
                    columns.Add(singleRow.Cells[4]);
                    columns.Add(singleRow.Cells[5]);
                });
            }
            else if (cubeNumber == 3)
            {
                rows.ForEach(singleRow =>
                {
                    columns.Add(singleRow.Cells[6]);
                    columns.Add(singleRow.Cells[7]);
                    columns.Add(singleRow.Cells[8]);
                });
            }

            return columns;
        }

        private List<DataGridViewRow> GetRows(int rowCube)
        {
            var rows = new List<DataGridViewRow>();

            if (rowCube == 1)
            {
                rows.Add(this.sudokuGrid.Rows[0]);
                rows.Add(this.sudokuGrid.Rows[1]);
                rows.Add(this.sudokuGrid.Rows[2]);
            }
            else if (rowCube == 2)
            {
                rows.Add(this.sudokuGrid.Rows[3]);
                rows.Add(this.sudokuGrid.Rows[4]);
                rows.Add(this.sudokuGrid.Rows[5]);
            }
            else if (rowCube == 3)
            {
                rows.Add(this.sudokuGrid.Rows[6]);
                rows.Add(this.sudokuGrid.Rows[7]);
                rows.Add(this.sudokuGrid.Rows[8]);
            }

            return rows;
        }

        private int GetRowCubeNumber(int y)
        {
            //// Check the cube we are in
            if (y < 3)
            {
                //// First Cubes
                return 1;
            }

            if (y < 6)
            {
                //// Second Cubes
                return 2;
            }

            if (y < 9)
            {
                //// Last Cubes
                return 3;
            }

            return -1;
        }

        private int GetColumnCubeNumber(int x)
        {
            //// Check the cube we are in
            if (x < 3)
            {
                //// First Cubes
                return 1;
            }

            if (x < 6)
            {
                //// Second Cubes
                return 2;
            }

            if (x < 9)
            {
                //// Last Cubes
                return 3;
            }

            return -1;
        }

        private void sudokuGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var cell = this.sudokuGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            var cellValue = cell.Value;
            try
            {
                var intValue = Convert.ToInt16(cellValue);

                if (intValue >= 10 || intValue <= -1)
                {
                    this.SetCellToEmpty(cell);
                }
            }
            catch (Exception)
            {
                this.SetCellToEmpty(cell); //// Set to 0 as it's not an int at the minute
            }
        }

        private void SetCellToEmpty(DataGridViewCell cell)
        {
            cell.Value = 0;
            cell.Value = null;
        }

        private void sudokuGrid_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            var currentCell = this.sudokuGrid.CurrentCell;
            if (currentCell.RowIndex == -1 || currentCell.ColumnIndex == -1)
            {
                return;
            }

            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                this.SetCellToEmpty(currentCell);
            }
        }

        private void solveBtn_Click(object sender, EventArgs e)
        {
            this._completed = false;
            this.SetSpecificCellsReadOnly();
            this._solveTask = new Task(this.Solve);
            this._solveTask.Start();
        }

        private void SetSpecificCellsReadOnly()
        {
            foreach (DataGridViewRow singleRow in this.sudokuGrid.Rows)
            {
                foreach (DataGridViewCell singleCell in singleRow.Cells)
                {
                    var cellValue = this.GetCellValue(singleCell);

                    if (cellValue >= 1)
                    {
                        singleCell.ReadOnly = true;
                    }
                }
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Are you sure you want to Clear the Board?", @"Clear?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var task = new Task(this.ClearRows);
                task.Start();
            }
        }

        private void ClearRows()
        {
            foreach (DataGridViewRow singleRow in this.sudokuGrid.Rows)
            {
                foreach (DataGridViewCell singleCell in singleRow.Cells)
                {
                    singleCell.ReadOnly = false;
                    this.SetCellValueOnUi(singleCell, null);
                }
            }
        }
    }
}
