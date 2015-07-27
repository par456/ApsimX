﻿namespace UserInterface.Views
{
    partial class StaticForestrySystemView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.Scalars = new System.Windows.Forms.DataGridView();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Grid = new System.Windows.Forms.DataGridView();
            this.pBelowGround = new OxyPlot.WindowsForms.PlotView();
            this.pAboveGround = new OxyPlot.WindowsForms.PlotView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Scalars)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.Scalars);
            this.panel1.Controls.Add(this.Grid);
            this.panel1.Controls.Add(this.pBelowGround);
            this.panel1.Controls.Add(this.pAboveGround);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(527, 563);
            this.panel1.TabIndex = 0;
            // 
            // Scalars
            // 
            this.Scalars.AllowUserToAddRows = false;
            this.Scalars.AllowUserToDeleteRows = false;
            this.Scalars.AllowUserToResizeColumns = false;
            this.Scalars.AllowUserToResizeRows = false;
            this.Scalars.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.Scalars.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.Scalars.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Scalars.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Description,
            this.Value});
            this.Scalars.Location = new System.Drawing.Point(0, 0);
            this.Scalars.Name = "Scalars";
            this.Scalars.RowHeadersVisible = false;
            this.Scalars.Size = new System.Drawing.Size(165, 272);
            this.Scalars.TabIndex = 5;
            this.Scalars.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.Scalars_CellEndEdit);
            // 
            // Description
            // 
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Width = 85;
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            this.Value.Width = 59;
            // 
            // Grid
            // 
            this.Grid.AllowUserToAddRows = false;
            this.Grid.AllowUserToDeleteRows = false;
            this.Grid.AllowUserToResizeColumns = false;
            this.Grid.AllowUserToResizeRows = false;
            this.Grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.Grid.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.Grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Grid.Location = new System.Drawing.Point(171, 0);
            this.Grid.Name = "Grid";
            this.Grid.RowHeadersVisible = false;
            this.Grid.Size = new System.Drawing.Size(265, 272);
            this.Grid.TabIndex = 4;
            this.Grid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.Grid_CellEndEdit);
            this.Grid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.Grid_EditingControlShowing);
            this.Grid.SelectionChanged += new System.EventHandler(this.Grid_SelectionChanged);
            this.Grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Grid_KeyDown);
            this.Grid.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Grid_PreviewKeyDown);
            // 
            // pBelowGround
            // 
            this.pBelowGround.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pBelowGround.Location = new System.Drawing.Point(271, 294);
            this.pBelowGround.Name = "pBelowGround";
            this.pBelowGround.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.pBelowGround.Size = new System.Drawing.Size(256, 269);
            this.pBelowGround.TabIndex = 3;
            this.pBelowGround.Text = "plot1";
            this.pBelowGround.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.pBelowGround.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.pBelowGround.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // pAboveGround
            // 
            this.pAboveGround.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pAboveGround.Location = new System.Drawing.Point(0, 275);
            this.pAboveGround.Name = "pAboveGround";
            this.pAboveGround.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.pAboveGround.Size = new System.Drawing.Size(265, 288);
            this.pAboveGround.TabIndex = 2;
            this.pAboveGround.Text = "plot1";
            this.pAboveGround.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.pAboveGround.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.pAboveGround.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Description";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 85;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Value";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 59;
            // 
            // StaticForestrySystemView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "StaticForestrySystemView";
            this.Size = new System.Drawing.Size(527, 563);
            this.Resize += new System.EventHandler(this.ForestryView_Resize);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Scalars)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private OxyPlot.WindowsForms.PlotView pBelowGround;
        private OxyPlot.WindowsForms.PlotView pAboveGround;
        private System.Windows.Forms.DataGridView Grid;
        private System.Windows.Forms.DataGridView Scalars;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}
