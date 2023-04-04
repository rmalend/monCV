using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

using Newtonsoft.Json;
using System.IO; 

namespace DemoMaterial1
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    public List<Material> materialList = null;
    public ProgramDemo programDemo = null;
    public MaterialDemo materialDemo = null;

    private void InitMasterGridView()
    {
        const int MAX_MATERIAL = 3;           // limitons a 3 pieces / programme
            
        LoadJsonData();                       // lecture des fichiers JSon en entrée

        DataGridViewColumn column = new DataGridViewTextBoxColumn();
        column.Name = "Program ID";
        column.HeaderText = "ID";
        column.ReadOnly = true;
        column.Frozen = true;
        column.MinimumWidth = 40;
        column.Width = 40;
        column.DefaultCellStyle.SelectionBackColor = Color.White;
        column.DefaultCellStyle.SelectionBackColor = Color.White;
 
        column.ValueType = typeof(string);
        dataGridView1.Columns.Add(column);

        // colonne pour le libellé du programmem pas de selection
        column = new DataGridViewTextBoxColumn();
        column.Name = "sName";
        column.HeaderText = "Program";
        column.ReadOnly = false;
        column.MinimumWidth = 50;
        column.Width = 100;
        dataGridView1.Columns.Add(column);
        column.DefaultCellStyle.SelectionBackColor = Color.White;
        column.DefaultCellStyle.SelectionBackColor = Color.White;

        // premiere colonne de materiau
        column = new DataGridViewTextBoxColumn();
        column.Name = "2";
        column.HeaderText = "";
        column.ReadOnly = false;
        dataGridView1.Columns.Add(column);
        column = new DataGridViewTextBoxColumn();
        column.Name = "3";
        column.HeaderText = "";
        column.ReadOnly = true;
        column.MinimumWidth = 60;
        column.Width = 100;
        dataGridView1.Columns.Add(column);
        // deuxieme colonne de materiau
        column = new DataGridViewTextBoxColumn();
        column.Name = "4";
        column.HeaderText = "";
        column.ReadOnly = false;
        dataGridView1.Columns.Add(column);

        // Populate the Material rows for each program
        foreach (ProgramItem program in programDemo.programs)
        {
            string sid = program.ID.ToString();
            string sname = program.sName;
            string name = program.pieces[0].sName;
            int numpieces = program.pieces.Length;
            string[] rowparams = new string[] { sid, sname, "", "", "" };
            for (int k = 0; k < numpieces && k < MAX_MATERIAL; k++)
            {
                rowparams[k + 2] = program.pieces[k].sName;
            }
            dataGridView1.Rows.Add(rowparams);
        }

        dataGridView1.AutoResizeColumns();
        dataGridView1.ClearSelection();
        // Set the background color for all rows and for alternating rows. 
        // The value for alternating rows overrides the value for all rows. 
        dataGridView1.AlternatingRowsDefaultCellStyle.BackColor     = Color.Ivory;
        dataGridView1.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Empty;
        foreach (DataGridViewColumn col in dataGridView1.Columns)       
                col.SortMode = DataGridViewColumnSortMode.NotSortable;       
        labelMaterial.Text = "";
        InitGridViewEvents();
    }

    private void InitDetailGridView(int row, int col)
    {
            int idMaterial = 0;
            if (row < 0 || col < 0) return;
            if (row >= programDemo.programs.Length) return;
            if (col >= programDemo.programs[row].pieces.Length) return;
            ClearDetailGridView();
            idMaterial = programDemo.programs[row].pieces[col].idMaterial;
            Material material = materialList.Find( x => x.id == idMaterial);
            if (material != null)
            {
                labelMaterial.Text = material.name;
                FillDetailGridViewRows(material);
                dataGridDetail.Visible = true;
            }
            else { 
                labelMaterial.Text = "";
                dataGridDetail.Visible = false;
            }
            dataGridDetail.ClearSelection();
    }
    private void ClearDetailGridView() 
    { 
          dataGridDetail.Rows.Clear();
          dataGridDetail.Columns.Clear();
          labelMaterial.Text = "";
          dataGridDetail.Visible = false;
    }

    private void FillDetailGridViewRows(Material material)
    {
            if (material == null) return;
            labelMaterial.Text = material.name;
            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column = new DataGridViewTextBoxColumn();
            column.ValueType = typeof(string);
            column.Name = material.name;
            column.HeaderText = material.name;
            column.ReadOnly = true;
            column.Frozen = true;
            column.MinimumWidth = 40;
            dataGridDetail.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.HeaderCell.Value = "masseVolumique";
            column.ValueType = typeof(string);
            column.Name = "masseVolumique";
            column.HeaderText  = "masseVolumique";
            column.ReadOnly = true;
            column.Frozen = true;
            column.MinimumWidth = 150;
            column.Width = 200;
            dataGridDetail.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.HeaderCell.Value = "coeffPoisson";
            column.ValueType = typeof(string);
            column.Name = "coeffPoisson";
            column.HeaderText  = "coeffPoisson";
            column.ReadOnly = true;
            column.Frozen = true;
            column.MinimumWidth = 200;
            column.Width = 200;
            this.dataGridDetail.Columns.Add(column);

            string[] temperaturerow = new string[] { "Temperature", "", ""};
            string[] densiterow = new string[] { "Densite", "", ""};
            string[] nuxrow     = new string[] { "nuX", "", ""};
            //
            InsertProperties(material.masseVolumique[0].data,temperaturerow,densiterow, nuxrow ,1); 
            InsertProperties(material.coeffPoisson[0].data,  temperaturerow,densiterow, nuxrow ,2);  

            dataGridDetail.Rows.Add(temperaturerow);  
            dataGridDetail.Rows.Add(densiterow);
            dataGridDetail.Rows.Add(nuxrow);

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            style.ForeColor = Color.IndianRed;
            style.BackColor = Color.Ivory;

            // Set the selection background color for all the detail cells.
            dataGridDetail.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridDetail.DefaultCellStyle.SelectionForeColor = Color.Black;

    }

        /// <summary>
        /// // inserer les propietés du materiau dans une ligne de gridViewDetail
        /// </summary> 
    private void InsertProperties(_propriete[] props,string[] tr ,string[] dr , string[] nr, int detailcolumn  )
    {
            foreach ( _propriete prop in props)
            {
                if ( prop.Temperature != null)
                {
                    int[] temp = prop.Temperature;
                    List<int> templist = temp.ToList();
                    templist.Sort();
                    string strtemp = null;
                    foreach (int str in templist)
                    {
                        if ( string.IsNullOrEmpty(strtemp))  
                             strtemp = str.ToString() ;
                        else strtemp = strtemp + ", " + str ;
                    }
                    tr[detailcolumn] = strtemp;
                }
                if ( prop.Densite != null)
                {
                    string strdens = null;
                    foreach (string str in prop.Densite)
                    {
                        if ( string.IsNullOrEmpty(strdens))  
                             strdens = str ;
                        else strdens = strdens + ", " + str ;
                    }
                    dr[detailcolumn] = strdens;
                }
                if ( prop.nuX != null)
                {
                    string strnux = "";
                    foreach (string str in prop.nuX)
                    {
                        if ( string.IsNullOrEmpty(strnux))
                            strnux = str ;
                        else strnux = strnux + ", " + str ;
                    }
                    nr[detailcolumn] = strnux;
                }
            }
    }

    private void LoadJsonData()
    {
  
#if DEBUG

            string programFilepath = @"program.json";     //
            string materialFilepath = @"materials.json";  // ralph
#else
           string programFilepath = @"program.json";  // ok
           string materialFilepath = @"materials.json";   // ok
#endif
            string strJSONProgram = "";
            string strJSONMaterial = "";

            try
            {
                using (StreamReader sread = File.OpenText(materialFilepath))
                {
                    strJSONMaterial = sread.ReadToEnd();
                }
                materialList = JsonConvert.DeserializeObject<List<Material>>(strJSONMaterial);
            }
            catch
            {
                Console.WriteLine(" ---- File not found Exception : " + materialFilepath);
            }
            try
            {
                using (StreamReader sr = File.OpenText(programFilepath))
                {
                    strJSONProgram = sr.ReadToEnd();
                }
                programDemo = (ProgramDemo)JsonConvert.DeserializeObject(strJSONProgram, typeof(ProgramDemo));

                dataGridView1.AutoResizeColumns();
            }
            catch
            {
                Console.WriteLine(" ---- File not found Exception : " + programFilepath);

            }
     }
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


  #region Windows Form Designer generated code

   
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridDetail = new System.Windows.Forms.DataGridView();
            this.labelMaterial = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDetail)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(50, 12);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.ReadOnly = true;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(500, 153);
            this.dataGridView1.TabIndex = 0;
            // 
            // dataGridDetail
            // 
            this.dataGridDetail.AllowUserToAddRows = false;
            this.dataGridDetail.AllowUserToDeleteRows = false;
            this.dataGridDetail.AllowUserToResizeColumns = false;
            this.dataGridDetail.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dataGridDetail.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridDetail.BackgroundColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ActiveBorder;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridDetail.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridDetail.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridDetail.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridDetail.Enabled = false;
            this.dataGridDetail.Location = new System.Drawing.Point(204, 226);
            this.dataGridDetail.MultiSelect = false;
            this.dataGridDetail.Name = "dataGridDetail";
            this.dataGridDetail.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridDetail.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridDetail.RowHeadersVisible = false;
            this.dataGridDetail.RowHeadersWidth = 60;
            this.dataGridDetail.RowTemplate.ReadOnly = true;
            this.dataGridDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridDetail.Size = new System.Drawing.Size(555, 164);
            this.dataGridDetail.TabIndex = 1;
            this.dataGridDetail.Visible = false;
            // 
            // labelMaterial
            // 
            this.labelMaterial.AutoSize = true;
            this.labelMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMaterial.Location = new System.Drawing.Point(295, 199);
            this.labelMaterial.Name = "labelMaterial";
            this.labelMaterial.Size = new System.Drawing.Size(125, 24);
            this.labelMaterial.TabIndex = 1;
            this.labelMaterial.Text = "label1Material";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(834, 483);
            this.Controls.Add(this.labelMaterial);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.dataGridDetail);
            this.Name = "Form1";
            this.Text = "Demo materials Form";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDetail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion



        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dataGridDetail;
        private Label labelMaterial;
  }
}

