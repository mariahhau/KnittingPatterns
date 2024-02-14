
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;


namespace KnittingPatterns
{
    public partial class Form1 : Form

    {
        public Graphics g;
        public Graphics graph;

        public Graphics rowG;
        public Graphics rowGraph;
        Bitmap rowSurface;

        public Graphics colG;
        public Graphics colGraph;
        Bitmap colSurface;

        public SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        public SolidBrush brush2 = new System.Drawing.SolidBrush(System.Drawing.Color.Gray);
        public Pen pen = new Pen(System.Drawing.Color.Gray);


        public int cellSize = 40;
        
        private List<List<int>> stitchList = new List<List<int>>();

        private (int, int) currentCell = (-1,-1);
        private (int, int) previousCell = (-1, -1);

        Bitmap surface;
        private Point mouseOffsetPos;
        private bool isMouseDown = false;

        private int gridX = 0;
        private int gridY = 0;

        private int stitchCount = 0;
        private int rowCount = 0;

        public Graphics yarnG;
        public Graphics yarnGraph;
        Bitmap ballSurface;






        public Form1()
        {
            InitializeComponent();
            g = canvasPanel.CreateGraphics();
           
            yarnG = yarnBall_color.CreateGraphics();
            yarnG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            ballSurface = new Bitmap(yarnBall_color.Width, yarnBall_color.Height);
            yarnBall_color.BackgroundImage = ballSurface;
            yarnGraph = Graphics.FromImage(ballSurface);

            rowG = rowNumberPanel.CreateGraphics();
            rowSurface = new Bitmap(rowNumberPanel.Width, 3000);
            rowGraph = Graphics.FromImage(rowSurface);
            rowNumberPanel.BackgroundImage = rowSurface;
            rowGraph.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            colG = colNumberPanel.CreateGraphics();
            colSurface = new Bitmap(3000, colNumberPanel.Height);
            colGraph = Graphics.FromImage(colSurface);
            colNumberPanel.BackgroundImage = colSurface;
            colGraph.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            surface = new Bitmap(3000, 3000);
            graph = Graphics.FromImage(surface);

            canvasPanel.BackgroundImageLayout = ImageLayout.None;


            gridX = 0; 
            gridY = 0;

            stitchCount = (int)stitches.Value;
            rowCount = (int)rows.Value;
            
            


            // initialize list with zeros
            for (int i = 0; i < rows.Value; i++)
            {
                stitchList.Add(new List<int>());
               
                for (int j = 0; j < stitches.Value; j++)
                {
                    stitchList[i].Add(0);
                }
               
            }
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            graph.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            brush.Color = color1.BackColor;
            yarnGraph.FillEllipse(brush, new Rectangle(yarnBall_color.Width / 14, 5, 3 * yarnBall_color.Width / 4 - 1, 4 * yarnBall_color.Height / 5));

            draw_columns();
            draw_rows();
            draw_stitches();
            canvasPanel.BackgroundImage = surface;

            this.canvasPanel.MouseWheel += new MouseEventHandler(this.canvasPanel_MouseWheel);

            splitContainer1.SplitterWidth = 1;

         

        }



        

        private void canvasPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            rowNumberPanel.Location = new Point(rowNumberPanel.Location.X, canvasPanel.Location.Y);
            g = canvasPanel.CreateGraphics();

        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            var col = Math.Floor((double)((e.X-gridX)/ cellSize));
            var row = Math.Floor((double)((e.Y-gridY)/ cellSize));
            


            currentCell = ((int)row, (int)col);
            previousCell = ((int)row, (int)col);
           

            if (row >= 0 && row < (double)rows.Value)
            {
                if (col >= 0 && col < (double)stitches.Value)
                {
                    g.FillRectangle(brush, (int)col * cellSize + pen.Width, (int)row * cellSize + pen.Width, cellSize - pen.Width, cellSize - pen.Width);
                    
                    if (brush.Color.ToArgb() == background_color.BackColor.ToArgb())
                    {
                        stitchList[(int)row][(int)col] = 0;
                    }
                     else stitchList[(int)row][(int)col] = brush.Color.ToArgb();
                   
                    graph.FillRectangle(brush, (int)col * cellSize + pen.Width, (int)row * cellSize + pen.Width, cellSize - pen.Width, cellSize - pen.Width);
                    
                   
                }

            }
            

        }
        

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            
            if(e.Button == MouseButtons.Left)
            {
               
                var x = Math.Floor((double)((e.X-gridX)  -  ((e.X-gridX) % cellSize)));
                var y = Math.Floor((double)((e.Y-gridY) - ((e.Y-gridY) % cellSize)));
              
                var col = Math.Floor((double)((e.X-gridX) / cellSize));
                var row = Math.Floor((double)((e.Y-gridY) / cellSize));

                currentCell = ((int)row, (int)col);

                if (currentCell != previousCell)
                {
                    previousCell = ((int)row, (int)col);
                    
                    if (row >= 0 && row < (double)rows.Value)
                    {
                            if (col >= 0 && col < (double)stitches.Value)
                            { 
                                    if (stitchList[(int)row][(int)col] != brush.Color.ToArgb())
                                    {
                                            if (brush.Color.ToArgb() == background_color.BackColor.ToArgb())
                                            {
                                                stitchList[(int)row][(int)col] = 0;
                                            }
                                            else stitchList[(int)row][(int)col] = brush.Color.ToArgb();
                                            
                                            g.FillRectangle(brush, (float)x + pen.Width, (float)y  + pen.Width, cellSize - pen.Width, cellSize - pen.Width );


                                    }

                            }
                    }
                } 


            }

        }

        private void canvasPanel_MouseUp(object sender, MouseEventArgs e)
        {
            currentCell = (-1, -1);
        }


        private void TopPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                mouseOffsetPos = new Point(-e.X, -e.Y);
                isMouseDown = true;
            }
        }

        private void TopPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffsetPos.X, mouseOffsetPos.Y);
                this.Location = mousePos;
            }
        }

        private void TopPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                isMouseDown= false;
            }
        }

        private void exit_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void background_color_click(object sender, EventArgs e)
        {
            brush.Color = background_color.BackColor;
            yarnGraph.FillEllipse(brush, new Rectangle(yarnBall_color.Width / 14, 5, 3 * yarnBall_color.Width / 4 - 1, 4 * yarnBall_color.Height / 5));
            yarnBall_color.Invalidate();
        }

               

        private void clear_button_Click(object sender, EventArgs e)
        {
            
            for (int i = 0; i < stitchList.Count; i++)
            {
                for (int j = 0; j < stitchList[i].Count; j++)
                {
                    stitchList[i][j] = 0;
                }

            }
            draw_stitches();
            canvasPanel.Invalidate();
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Png Files (*png) | *.png";
            sfd.DefaultExt = "png";
            sfd.AddExtension = true;

            if(sfd.ShowDialog() == DialogResult.OK)
            {
                surface.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

     
        private void stitches_ValueChanged(object sender, EventArgs e)
        {
                       
            if (stitchCount < stitches.Value)
            {
                if (stitchList[0].Count < stitches.Value)
                {

                    for (int i = 0; i < rows.Value; i++)
                    {
                        for (int j = stitchList[i].Count; j < stitches.Value; j++)
                        {
                            Console.WriteLine(stitchList[i].Count);
                            stitchList[i].Add(0);
                        }

                    }
                }

                    Region region = new Region(new Rectangle(stitchCount * cellSize , 0, this.Size.Width, this.Size.Height));
                   
                    if (stitches.Value * cellSize + gridX > canvasPanel.Width)
                    {
                        
                        canvasPanel.Width = (int)stitches.Value * cellSize + gridX;
                        colNumberPanel.Width = canvasPanel.Width;
                        

                    }
                    canvasPanel.Invalidate(region, false);
                    colNumberPanel.Invalidate(region, false);



                    draw_rows();
                    draw_columns(stitchCount); 
                    draw_stitches();
                

                
            }
                        
            else
            {
                for (int i = 0; i < rows.Value; i++)
                {
                    for (int j = (int)stitches.Value; j < stitchList[i].Count; j++)
                    {
                        stitchList[i][j] = 0;
                    }                   
                }
                
                Region region = new Region(new Rectangle((int)stitches.Value * cellSize + (int)pen.Width, 0, this.Size.Width, this.Size.Height));
                colNumberPanel.Invalidate(region, false);
                canvasPanel.Invalidate(region, false);

                brush2.Color = canvasPanel.BackColor; 
                g.FillRegion(brush2, region);
                graph.FillRegion(brush2, region);
                colG.FillRegion(brush2, region);
                colGraph.FillRegion(brush2, region);






            }
            
        }

        private void rows_ValueChanged(object sender, EventArgs e)
        {
           
            if (rowCount < rows.Value)
            {
                if (stitchList.Count < rows.Value)
                {
                    for (int i = stitchList.Count; i < rows.Value; i++)
                    {
                        List<int> newList = Enumerable.Repeat(0, stitchList[0].Count).ToList();
                        stitchList.Add(newList);

                    }
                }

               
                Region region = new Region(new Rectangle(0,rowCount * cellSize, this.Size.Width, this.Size.Height));
                if (rows.Value * cellSize + gridY > canvasPanel.Height)
                {
                   
                    
                   canvasPanel.Height = (int)rows.Value * cellSize + gridY;
                    rowNumberPanel.Height = canvasPanel.Height;
                }

                canvasPanel.Invalidate(region, false);
                rowNumberPanel.Invalidate(region, false);
               

                draw_rows();
                draw_columns(0, (rowCount - 1) * cellSize);
                draw_stitches();


            

            }
            
            else
            {

                for (int i = (int)rows.Value; i < stitchList.Count; i++)
                {
                    
                    for (int j = 0; j < stitchList[i].Count; j++)
                    {
                        stitchList[i][j] = 0;
                    }


                }
                Region region = new Region(new Rectangle(0, (int)rows.Value * cellSize + (int)pen.Width, this.Size.Width, this.Size.Height));
                Region rowRegion = new Region(new Rectangle(0, (int)rows.Value * cellSize + gridY + (int)pen.Width, this.Size.Width, this.Size.Height));
                canvasPanel.Invalidate(region, false);
                rowNumberPanel.Invalidate(rowRegion, false);
                
                brush2.Color = canvasPanel.BackColor;
                g.FillRegion(brush2, region);
                graph.FillRegion(brush2, region);
                rowG.FillRegion(brush2, rowRegion);
                rowGraph.FillRegion(brush2, rowRegion);



            }
            
        }

        private Random rnd = new Random();

        private void draw_columns(int x = 0, int y = 0)
        {
            pen.Color = Color.Gray; 
            brush2.Color = Color.Black;
            for (; x <= stitches.Value; ++x)
            {
                if (x < stitches.Value && y == 0)
                {
                    colGraph.DrawString(x + 1 + "", new Font("Arial", 9, FontStyle.Regular), brush2,  x * cellSize + cellSize / 3, 1);
                }
                
                 graph.DrawLine(pen,  x * cellSize,  y,  x * cellSize, (int)rows.Value * cellSize);
                 
            }
            
        }
        

        private void draw_rows()
        {
            pen.Color = Color.Gray; 
            
            int y = rowCount == (int)rows.Value ? 0 : rowCount;
            if (rowCount > rows.Value) { 
                y = (int)rows.Value;
            }
            for (; y <= rows.Value; ++y)
            {
                
                if (y < rows.Value)
                {
                    brush2.Color = Color.Black;
                                      
                    rowGraph.DrawString(y + 1 + "", new Font("Arial", 9), brush2, 0, y * cellSize + cellSize/3);
                 
                    
                }
               
                graph.DrawLine(pen, 0,  y * cellSize,  (int)stitches.Value * cellSize , y * cellSize);
                
            }

        }

        
        private void updateValues(object sender, PaintEventArgs e)
        {
            stitchCount = (int)stitches.Value;
            rowCount = (int)rows.Value;
            
            canvasPanel.BackgroundImage = surface;
            yarnBall_color.BackgroundImage = ballSurface;
            rowNumberPanel.BackgroundImage = rowSurface;
            colNumberPanel.BackgroundImage = colSurface;
            

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Console.WriteLine(Size.ToString() + " resize") ;
                       
        }

        private void fullScreen_button_Click(object sender, EventArgs e)
        {

            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                fullScreen_button.Text = "-";
            } else
            {
                this.WindowState = FormWindowState.Normal;
                fullScreen_button.Text = "☐";
            }
            
            
        }

        private void minimizeWindow_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void gridSizeDown_Click(object sender, EventArgs e)
        {
            if (cellSize > 4)
            {

                cellSize -= 2;
                graph.Clear(canvasPanel.BackColor);
                rowGraph.Clear(rowNumberPanel.BackColor);
                colGraph.Clear(colNumberPanel.BackColor);
                
                canvasPanel.Invalidate();
                rowNumberPanel.Invalidate();
                colNumberPanel.Invalidate();

                draw_columns();
                draw_rows();
                draw_stitches();

            }
        }

        private void gridSizeUp_Click(object sender, EventArgs e)
        {
           
            if (cellSize < 60)
            {
                if (stitches.Value * cellSize + gridX > canvasPanel.Width)
                {

                    canvasPanel.Width = (int)stitches.Value * cellSize + gridX;
                    colNumberPanel.Width = canvasPanel.Width;
                }

                if (rows.Value * cellSize + gridY > canvasPanel.Height)
                {


                    canvasPanel.Height = (int)rows.Value * cellSize + gridY;
                    rowNumberPanel.Height = canvasPanel.Height;
                }

                cellSize += 2;
                graph.Clear(canvasPanel.BackColor);
                rowGraph.Clear(rowNumberPanel.BackColor);
                colGraph.Clear(colNumberPanel.BackColor);

                canvasPanel.Invalidate();
                rowNumberPanel.Invalidate();
                colNumberPanel.Invalidate();
                
                draw_columns();
                draw_rows();
                draw_stitches();

            }

        }


        private void draw_stitches()
        {

            for (int i = 0; i < rows.Value; i++) 
            {
                for (int j = 0; j < stitches.Value; j++) 
                {
                    if (j >= stitchList[i].Count) { Console.WriteLine("oho"); continue; }

                    if ((stitchList[i][j]) != 0)
                    {
                        brush2.Color = Color.FromArgb(stitchList[i][j]);
                        graph.FillRectangle(brush2, j * cellSize + pen.Width,  i * cellSize + pen.Width, cellSize-pen.Width, cellSize-pen.Width);
                        
                    } else
                    {
                        brush2.Color = background_color.BackColor;
                        graph.FillRectangle(brush2, j * cellSize + pen.Width, i * cellSize + pen.Width, cellSize - pen.Width, cellSize - pen.Width);
                    }
                }
            }

        }


        private void brush_button_MouseClick(object sender, MouseEventArgs e)
        {
            brush.Color = color1.ForeColor;

        }

        private void stitches_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                
            }
        }

        private void stitches_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
               
            }
        }

        private void rows_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                
            }
        }

        private void rows_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
               
                
            }
        }

        private void change_color1_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK){color1.BackColor = cd.Color;}
        }

        private void change_color2_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK){ color2.BackColor = cd.Color;}
        }

        private void change_color3_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK){ color3.BackColor = cd.Color; }
        }

        private void change_color4_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK) { color4.BackColor = cd.Color; }
        }

        private void color1_Click(object sender, EventArgs e)
        {
            brush.Color = color1.BackColor;
            yarnGraph.FillEllipse(brush, new Rectangle(yarnBall_color.Width / 14, 5, 3 * yarnBall_color.Width / 4 - 1, 4 * yarnBall_color.Height / 5));
            yarnBall_color.Invalidate();
            

        }

        private void color2_Click(object sender, EventArgs e)
        {
            brush.Color = color2.BackColor;
            yarnGraph.FillEllipse(brush, new Rectangle(yarnBall_color.Width / 14, 5, 3 * yarnBall_color.Width / 4 - 1, 4 * yarnBall_color.Height / 5));
            yarnBall_color.Invalidate();
        }

        private void color3_Click(object sender, EventArgs e)
        {
            brush.Color = color3.BackColor;
            yarnGraph.FillEllipse(brush, new Rectangle(yarnBall_color.Width / 14, 5, 3 * yarnBall_color.Width / 4 - 1, 4 * yarnBall_color.Height / 5));
            yarnBall_color.Invalidate();
        }

        private void color4_Click(object sender, EventArgs e)
        {
            brush.Color = color4.BackColor;
            yarnGraph.FillEllipse(brush, new Rectangle(yarnBall_color.Width / 14, 5, 3 * yarnBall_color.Width / 4 - 1, 4 * yarnBall_color.Height / 5));
            yarnBall_color.Invalidate();
        }

        private void change_background_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK) 
            {
                background_color.BackColor = cd.Color;
                draw_stitches();
                canvasPanel.Invalidate();

            }
        }

        private void canvas_parent_Scroll(object sender, ScrollEventArgs e)
        {
            
            
    
        }

        private void rowNumberPanel_Paint(object sender, PaintEventArgs e)
        {
            
           
        }

        private void splitContainer1_Panel2_Scroll(object sender, ScrollEventArgs e)
        {
            var diff = e.OldValue - e.NewValue;
            
            if (e.ScrollOrientation == 0)
            {
                colNumberPanel.Location = new Point(colNumberPanel.Location.X + diff, colNumberPanel.Location.Y);    
              
            } else
            {
                
                rowNumberPanel.Location = new Point(rowNumberPanel.Location.X, (rowNumberPanel.Location.Y + diff));
            }
            g = canvasPanel.CreateGraphics();
            




        }

        
    }
}
