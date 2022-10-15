namespace UmlBuilder
{
    partial class UmlBuilder
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainButton = new System.Windows.Forms.Button();
            this.dropPanel = new System.Windows.Forms.Panel();
            this.fileDropLabel = new System.Windows.Forms.Label();
            this.filePathsList = new System.Windows.Forms.ListView();
            this.FilePathsColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FileNamesColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.deletionMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deletionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPanel = new System.Windows.Forms.Panel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decorationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundСolorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rectangleColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prevToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.umlPictureBox = new System.Windows.Forms.PictureBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.button = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.loadingPanel = new System.Windows.Forms.Panel();
            this.dropPanel.SuspendLayout();
            this.deletionMenuStrip.SuspendLayout();
            this.menuPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.umlPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.loadingPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainButton
            // 
            this.mainButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mainButton.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mainButton.Location = new System.Drawing.Point(369, 578);
            this.mainButton.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.mainButton.Name = "mainButton";
            this.mainButton.Size = new System.Drawing.Size(260, 61);
            this.mainButton.TabIndex = 0;
            this.mainButton.Text = "Создать диаграмму";
            this.mainButton.UseVisualStyleBackColor = true;
            this.mainButton.Click += new System.EventHandler(this.MainButton_Click);
            // 
            // dropPanel
            // 
            this.dropPanel.AllowDrop = true;
            this.dropPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dropPanel.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.dropPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dropPanel.Controls.Add(this.fileDropLabel);
            this.dropPanel.Location = new System.Drawing.Point(29, 14);
            this.dropPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dropPanel.Name = "dropPanel";
            this.dropPanel.Size = new System.Drawing.Size(924, 83);
            this.dropPanel.TabIndex = 1;
            this.dropPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.DropPanel_DragDrop);
            this.dropPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.DropPanel_DragEnter);
            this.dropPanel.DragLeave += new System.EventHandler(this.DropPanel_DragLeave);
            // 
            // fileDropLabel
            // 
            this.fileDropLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileDropLabel.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fileDropLabel.Location = new System.Drawing.Point(-1, -1);
            this.fileDropLabel.Name = "fileDropLabel";
            this.fileDropLabel.Size = new System.Drawing.Size(924, 83);
            this.fileDropLabel.TabIndex = 0;
            this.fileDropLabel.Text = "Перетащите файлы сюда";
            this.fileDropLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.fileDropLabel.Click += new System.EventHandler(this.fileDropLabel_Click);
            // 
            // filePathsList
            // 
            this.filePathsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filePathsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FilePathsColumn,
            this.FileNamesColumn});
            this.filePathsList.ContextMenuStrip = this.deletionMenuStrip;
            this.filePathsList.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.filePathsList.FullRowSelect = true;
            this.filePathsList.GridLines = true;
            this.filePathsList.HideSelection = false;
            this.filePathsList.Location = new System.Drawing.Point(29, 103);
            this.filePathsList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.filePathsList.Name = "filePathsList";
            this.filePathsList.Size = new System.Drawing.Size(924, 469);
            this.filePathsList.TabIndex = 2;
            this.filePathsList.UseCompatibleStateImageBehavior = false;
            this.filePathsList.View = System.Windows.Forms.View.Details;
            this.filePathsList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FilePathsList_MouseClick);
            // 
            // FilePathsColumn
            // 
            this.FilePathsColumn.Text = "File Paths";
            this.FilePathsColumn.Width = 645;
            // 
            // FileNamesColumn
            // 
            this.FileNamesColumn.Text = "File Names";
            this.FileNamesColumn.Width = 250;
            // 
            // deletionMenuStrip
            // 
            this.deletionMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.deletionMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deletionMenuItem});
            this.deletionMenuStrip.Name = "deletionMenuStrip";
            this.deletionMenuStrip.Size = new System.Drawing.Size(135, 28);
            // 
            // deletionMenuItem
            // 
            this.deletionMenuItem.Name = "deletionMenuItem";
            this.deletionMenuItem.Size = new System.Drawing.Size(134, 24);
            this.deletionMenuItem.Text = "Удалить";
            this.deletionMenuItem.Click += new System.EventHandler(this.DeletionMenuItem_Click);
            // 
            // menuPanel
            // 
            this.menuPanel.Controls.Add(this.mainButton);
            this.menuPanel.Controls.Add(this.dropPanel);
            this.menuPanel.Controls.Add(this.filePathsList);
            this.menuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuPanel.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.menuPanel.Location = new System.Drawing.Point(0, 0);
            this.menuPanel.Name = "menuPanel";
            this.menuPanel.Size = new System.Drawing.Size(982, 653);
            this.menuPanel.TabIndex = 3;
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.menuStrip);
            this.mainPanel.Controls.Add(this.umlPictureBox);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(982, 653);
            this.mainPanel.TabIndex = 3;
            // 
            // menuStrip
            // 
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.decorationToolStripMenuItem,
            this.regroupToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(982, 31);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(64, 27);
            this.fileToolStripMenuItem.Text = "Файл";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(167, 26);
            this.saveAsToolStripMenuItem.Text = "Сохранить";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(167, 26);
            this.exitToolStripMenuItem.Text = "Выйти";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // decorationToolStripMenuItem
            // 
            this.decorationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backgroundСolorToolStripMenuItem,
            this.rectangleColorToolStripMenuItem,
            this.lineColorToolStripMenuItem,
            this.fontColorToolStripMenuItem,
            this.changeLineToolStripMenuItem});
            this.decorationToolStripMenuItem.Name = "decorationToolStripMenuItem";
            this.decorationToolStripMenuItem.Size = new System.Drawing.Size(128, 27);
            this.decorationToolStripMenuItem.Text = "Оформление";
            // 
            // backgroundСolorToolStripMenuItem
            // 
            this.backgroundСolorToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.backgroundСolorToolStripMenuItem.Name = "backgroundСolorToolStripMenuItem";
            this.backgroundСolorToolStripMenuItem.Size = new System.Drawing.Size(256, 26);
            this.backgroundСolorToolStripMenuItem.Text = "Цвет фона";
            this.backgroundСolorToolStripMenuItem.Click += new System.EventHandler(this.BackgroundСolorToolStripMenuItem_Click);
            // 
            // rectangleColorToolStripMenuItem
            // 
            this.rectangleColorToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rectangleColorToolStripMenuItem.Name = "rectangleColorToolStripMenuItem";
            this.rectangleColorToolStripMenuItem.Size = new System.Drawing.Size(256, 26);
            this.rectangleColorToolStripMenuItem.Text = "Цвет прямоугольников";
            this.rectangleColorToolStripMenuItem.Click += new System.EventHandler(this.RectangleColorToolStripMenuItem_Click);
            // 
            // lineColorToolStripMenuItem
            // 
            this.lineColorToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lineColorToolStripMenuItem.Name = "lineColorToolStripMenuItem";
            this.lineColorToolStripMenuItem.Size = new System.Drawing.Size(256, 26);
            this.lineColorToolStripMenuItem.Text = "Цвет линий";
            this.lineColorToolStripMenuItem.Click += new System.EventHandler(this.LineColorToolStripMenuItem_Click);
            // 
            // fontColorToolStripMenuItem
            // 
            this.fontColorToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fontColorToolStripMenuItem.Name = "fontColorToolStripMenuItem";
            this.fontColorToolStripMenuItem.Size = new System.Drawing.Size(256, 26);
            this.fontColorToolStripMenuItem.Text = "Цвет шрифта";
            this.fontColorToolStripMenuItem.Click += new System.EventHandler(this.FontColorToolStripMenuItem_Click);
            // 
            // changeLineToolStripMenuItem
            // 
            this.changeLineToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.changeLineToolStripMenuItem.Name = "changeLineToolStripMenuItem";
            this.changeLineToolStripMenuItem.Size = new System.Drawing.Size(256, 26);
            this.changeLineToolStripMenuItem.Text = "Скруглить линии";
            this.changeLineToolStripMenuItem.Click += new System.EventHandler(this.ChangeLineToolStripMenuItem_Click);
            // 
            // regroupToolStripMenuItem
            // 
            this.regroupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nextToolStripMenuItem,
            this.prevToolStripMenuItem});
            this.regroupToolStripMenuItem.Name = "regroupToolStripMenuItem";
            this.regroupToolStripMenuItem.Size = new System.Drawing.Size(173, 27);
            this.regroupToolStripMenuItem.Text = "Перегруппировать";
            // 
            // nextToolStripMenuItem
            // 
            this.nextToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nextToolStripMenuItem.Name = "nextToolStripMenuItem";
            this.nextToolStripMenuItem.Size = new System.Drawing.Size(183, 26);
            this.nextToolStripMenuItem.Text = "Следующая";
            this.nextToolStripMenuItem.Click += new System.EventHandler(this.NextToolStripMenuItem_Click);
            // 
            // prevToolStripMenuItem
            // 
            this.prevToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.prevToolStripMenuItem.Name = "prevToolStripMenuItem";
            this.prevToolStripMenuItem.Size = new System.Drawing.Size(183, 26);
            this.prevToolStripMenuItem.Text = "Предыдущая";
            this.prevToolStripMenuItem.Click += new System.EventHandler(this.PrevToolStripMenuItem_Click);
            // 
            // umlPictureBox
            // 
            this.umlPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.umlPictureBox.BackColor = System.Drawing.Color.White;
            this.umlPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.umlPictureBox.ErrorImage = null;
            this.umlPictureBox.InitialImage = null;
            this.umlPictureBox.Location = new System.Drawing.Point(0, 28);
            this.umlPictureBox.Name = "umlPictureBox";
            this.umlPictureBox.Size = new System.Drawing.Size(982, 618);
            this.umlPictureBox.TabIndex = 2;
            this.umlPictureBox.TabStop = false;
            this.umlPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.UmlPictureBox_Paint);
            this.umlPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UmlPictureBox_MouseDown);
            this.umlPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UmlPictureBox_MouseMove);
            this.umlPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UmlPictureBox_MouseUp);
            this.umlPictureBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.UmlPictureBox_MouseWheel);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(29, 311);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(924, 30);
            this.progressBar.TabIndex = 3;
            // 
            // button
            // 
            this.button.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button.Font = new System.Drawing.Font("MS Reference Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button.Location = new System.Drawing.Point(349, 568);
            this.button.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(260, 61);
            this.button.TabIndex = 0;
            this.button.Text = "Создать диаграмму";
            this.button.UseVisualStyleBackColor = true;
            this.button.Click += new System.EventHandler(this.MainButton_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "png";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // loadingLabel
            // 
            this.loadingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.loadingLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.loadingLabel.Location = new System.Drawing.Point(0, 278);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(982, 30);
            this.loadingLabel.TabIndex = 4;
            this.loadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // loadingPanel
            // 
            this.loadingPanel.Controls.Add(this.progressBar);
            this.loadingPanel.Controls.Add(this.loadingLabel);
            this.loadingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadingPanel.Location = new System.Drawing.Point(0, 0);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(982, 653);
            this.loadingPanel.TabIndex = 3;
            // 
            // UmlBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 653);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.menuPanel);
            this.Controls.Add(this.loadingPanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.MinimumSize = new System.Drawing.Size(360, 400);
            this.Name = "UmlBuilder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UML Builder";
            this.dropPanel.ResumeLayout(false);
            this.deletionMenuStrip.ResumeLayout(false);
            this.menuPanel.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.umlPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.loadingPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mainButton;
        private System.Windows.Forms.Panel dropPanel;
        private System.Windows.Forms.ListView filePathsList;
        private System.Windows.Forms.ColumnHeader FilePathsColumn;
        private System.Windows.Forms.ColumnHeader FileNamesColumn;
        private System.Windows.Forms.ContextMenuStrip deletionMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deletionMenuItem;
        private System.Windows.Forms.Panel menuPanel;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Label fileDropLabel;
        private System.Windows.Forms.Button button;
        private System.Windows.Forms.PictureBox umlPictureBox;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decorationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundСolorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rectangleColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem regroupToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.ToolStripMenuItem changeLineToolStripMenuItem;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.Panel loadingPanel;
        private System.Windows.Forms.ToolStripMenuItem nextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prevToolStripMenuItem;
    }
}