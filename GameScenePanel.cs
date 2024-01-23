using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Data;

namespace TowerDefense
{
    enum GameState
    {
        Failed = 0,
        Ongoing = 1,
        Completed = 2,
        Exit = 3
    }

    internal class GameScenePanel : Panel
    {
        Form form;

        Game game;
        Image gameSceneImage;
        Image gamePropertiesImage;

        Button nextWaveButton;
        Button exitButton;

        Panel towerListPanel;
        List<Label> towerListLabels;
        List<PictureBox> towerListPictureBoxes;

        const int towerItemPadding = 10;
        const int towerItemWidth = 160;
        const int towerItemHeight = 50 + 2 * towerItemPadding;

        int selected = -1;
        int mouseX;
        int mouseY;

        bool waveInProcess = false;
        int gameState = 1;
        bool gameOver = false;

        public GameScenePanel()
        {
            initPanel();       
            initDrawImages();
            initButtons();
            initTowerList();
        }

        private void initPanel()
        {
            BackColor = System.Drawing.Color.White;
            Location = new System.Drawing.Point(3, 3);
            Name = "game_scene_panel";
            Size = new System.Drawing.Size(1181, 854);
            TabIndex = 6;
            Visible = false;
            VisibleChanged += new EventHandler(onVisible);
            Paint += new System.Windows.Forms.PaintEventHandler(onPaint);
            DoubleBuffered = true;

        }

        private void initDrawImages()
        {
            gameSceneImage = new Bitmap(GridParams.GridSizeX * GridParams.TileSize, GridParams.GridSizeY * GridParams.TileSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gamePropertiesImage = new Bitmap(GridParams.GridSizeX * GridParams.TileSize, GridParams.StartY, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        private void initButtons()
        {
            nextWaveButton = new Button();
            nextWaveButton.Text = "NEXT WAVE ->";
            nextWaveButton.AutoSize = false;
            nextWaveButton.Width = 140;
            nextWaveButton.Height = 40;
            nextWaveButton.Location = new Point(805, 10);
            nextWaveButton.BackColor = Color.White;
            nextWaveButton.Click += nextWaveButtonClick;
            Controls.Add(nextWaveButton);

            exitButton = new Button();
            exitButton.Text = "EXIT";
            exitButton.AutoSize = false;
            exitButton.Width = 140;
            exitButton.Height = 40;
            exitButton.Location = new Point(805, 70);
            exitButton.BackColor = Color.White;
            exitButton.Click += exitButtonClick;
            Controls.Add(exitButton);
        }

        private void initTowerList()
        {
            towerListPanel = new Panel();
            towerListPanel.Size = new Size(towerItemWidth, this.Height - 120); // Assume your form height is large enough
            towerListPanel.Location = new Point(0, 150);
            towerListPanel.AutoScroll = true; // If the content exceeds the size of the Panel, scroll bars are automatically displayed
            Controls.Add(towerListPanel);

            MouseMove += getMousePosition;
            Click += placeTower; //Add Place Tower Event

            towerListLabels=new List<Label>();
            towerListPictureBoxes=new List<PictureBox>();
        }

        public void onVisible(object sender, EventArgs e)
        {
            if(Visible)
            {
                int startX = 0;
                int startY = 0;

                foreach (var towerIdx in game.Level.towerSelection)
                {
                    var tower = Tower.produceTower(towerIdx, new Tile(0, 0));

                    Label lbl = new Label();
                    lbl.Text = tower.getName() + "\n" + tower.cost.ToString();
                    lbl.AutoSize = false; // Do not automatically resize, set it to a specified width
                    lbl.Font = new Font("serial", 9, FontStyle.Bold);
                    lbl.Width = towerItemWidth - 50 - 3 * towerItemPadding; // Reserve space for images and extra padding
                    lbl.Height = 50;
                    lbl.Location = new Point(startX + towerItemPadding, startY + towerItemPadding + (50 - lbl.Height) / 2); // Center the label and image on the same line
                    lbl.Click += Item_Click; // Add click event handler
                    towerListPanel.Controls.Add(lbl);
                    towerListLabels.Add(lbl);

                    // Create and set up PictureBox
                    PictureBox pb = new PictureBox();
                    pb.Name = tower.getName();
                    pb.Image = tower.getTexture();
                    pb.SizeMode = PictureBoxSizeMode.StretchImage;
                    pb.Height = 50;
                    pb.Width = 50;
                    pb.Location = new Point(lbl.Width + 2 * towerItemPadding, startY + towerItemPadding); // draw pictures
                    pb.Click += Item_Click; // Add click event handler
                    towerListPanel.Controls.Add(pb);
                    towerListPictureBoxes.Add(pb);

                    startY += towerItemHeight;
                }
            }
            else
            {
                foreach (var label in towerListLabels)
                {
                    Controls.Remove(label);
                }

                foreach (var pb in towerListPictureBoxes)
                {
                    Controls.Remove(pb);
                }

                towerListLabels.Clear();
                towerListPictureBoxes.Clear();
            }
        }

        public void SetGame(Game game)
        {
            this.game = game;
        }

        public void nextWaveButtonClick(object sender, EventArgs e)
        {
            if (waveInProcess) return;

            waveInProcess = true;

            Thread thread = new Thread(() => { game.waveRun(this); });
            thread.Start();
        }

        public void waveCallback(int val)
        {
            //callback: 0:failed, 1:wave success, 2:level complete

            waveInProcess = false;
            gameState = val;
        }

        public void exitButtonClick(object sender, EventArgs e)
        {
            gameState = (int) GameState.Exit;
        }

        public void onPaint(object sender, PaintEventArgs e)
        {
            // The game ends normally and stops drawing.
            if (gameOver) return;

            // Game ends normally message box
            if (gameState == (int)GameState.Failed)
            {
                gameOver = true;
                MessageBox.Show("Sorry. Please try again", "Sentinel Siege: Tower Defense Titans", MessageBoxButtons.OK);
                game.gameResult = 0;
                this.Visible = false;
                
            }

            if (gameState == (int)GameState.Completed)
            {
                gameOver = true;
                MessageBox.Show("Congratulations, you passed!", "Tower Defense Titans", MessageBoxButtons.OK);
                game.gameResult = 2;
                this.Visible = false;
            }

            if (gameState == (int)GameState.Exit)
            {
                gameOver = true;
                game.gameResult = 3;
                this.Visible = false;
            }

            // Draw the game scene onto a picture
            Graphics sceneG = Graphics.FromImage(gameSceneImage);
            Graphics propertiesG = Graphics.FromImage(gamePropertiesImage);

            // draw grid
            paintGrid(sceneG);

            // Draw towers and enemies
            game.paint(sceneG);

            // Draw game properties
            paintProperties(propertiesG);

            // Monitor button status
            if (waveInProcess) nextWaveButton.Enabled = false;
            else nextWaveButton.Enabled = true;

            // Draw scene pictures to the screen
            Graphics g = e.Graphics;
            g.DrawImage(gameSceneImage, GridParams.StartX, GridParams.StartY, GridParams.GridSizeX * GridParams.TileSize, GridParams.GridSizeY * GridParams.TileSize);
            g.DrawImage(gamePropertiesImage, GridParams.StartX, 0, GridParams.GridSizeX * GridParams.TileSize, GridParams.StartY);
        }

        private void paintGrid(Graphics g)
        {
            // record path
            bool[,] roadMark = new bool[GridParams.GridSizeX, GridParams.GridSizeY];

            // Draw starting point and end point
            if (game.Level.path.Count > 0)
            {
                roadMark[game.Level.path.First().x, game.Level.path.First().y] = true;
                g.DrawImage(Resource1.entrance, GridParams.TileSize * game.Level.path.First().x, GridParams.TileSize * game.Level.path.First().y, GridParams.TileSize, GridParams.TileSize);

                roadMark[game.Level.path.Last().x, game.Level.path.Last().y] = true;
                g.DrawImage(Resource1.exit, GridParams.TileSize * game.Level.path.Last().x, GridParams.TileSize * game.Level.path.Last().y, GridParams.TileSize, GridParams.TileSize);
            }

            // draw path
            for (int i= 1;i < game.Level.path.Count() - 1;++i)
            {
                var tile = game.Level.path[i];
                roadMark[tile.x, tile.y] = true;
                g.DrawImage(Resource1.road, GridParams.TileSize * tile.x, GridParams.TileSize * tile.y, GridParams.TileSize, GridParams.TileSize);
            }

            // Draw other grids
            for (int i = 0; i < GridParams.GridSizeX; ++i)
            {
                for (int j = 0; j < GridParams.GridSizeY; ++j)
                {
                    if (!roadMark[i, j])
                    {
                        g.DrawImage(Resource1.tile, GridParams.TileSize * i, GridParams.TileSize * j, GridParams.TileSize, GridParams.TileSize);
                    }
                }
            }
        }

        private void paintProperties(Graphics g)
        {
            SolidBrush clearBrush = new SolidBrush(Color.White);
            SolidBrush propBrush = new SolidBrush(Color.Black);
            Font propFont = new Font("Back", 24);
            g.FillRectangle(clearBrush, 0, 0, GridParams.GridSizeX * GridParams.TileSize, GridParams.StartY);
            g.DrawString("Gold: " + game.Money.ToString() + "\n" + "Castle Life: " + game.BaseHP.ToString(), propFont, propBrush, 10, 20);
            g.DrawString(String.Format("Finished Waves: " + game.CurrentWave.ToString() + "/" + game.Level.waves.Count()), propFont, propBrush, 240, 20);
        }

        private void Item_Click(object sender, EventArgs e)
        {
            string name = null;

            if (sender is Label lbl)
            {
                // If you click on Label, you can get the character name through lbl.Text
                //MessageBox.Show("You clicked on " + lbl.Text);
                name = lbl.Text;
            }
            else if (sender is PictureBox pb)
            {
                // If you click on the PictureBox, Name gets the name
                //MessageBox.Show("You clicked on " + pb.Name);
                name = pb.Name;
            }

            if (name != null)
            {
                if (name.Equals("Canon")) selected = 1;
                if (name.Equals("Ice Tower")) selected = 2;
                if (name.Equals("Mage Tower")) selected = 3;
                if (name.Equals("Artillery Tower")) selected = 4;
                if (name.Equals("Archer Tower")) selected = 5;
            }
        }

        private void getMousePosition(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
        }

        private void placeTower(object sender, EventArgs e)
        {
            if (selected == -1) return;

            int gridX = (mouseX - GridParams.StartX) / GridParams.TileSize;
            int gridY = (mouseY - GridParams.StartY) / GridParams.TileSize;

            if (gridX < 0 || gridY < 0 || gridX >= GridParams.GridSizeX || gridY >= GridParams.GridSizeY)
                return;

            int result = game.placeTower(gridX, gridY, selected);
            if (result == 1) MessageBox.Show("Not enough Gold!");
            if (result == 2) MessageBox.Show("This tile is occupied!");

            selected = -1;
        }
    }
}
