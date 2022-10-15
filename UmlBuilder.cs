using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace UmlBuilder
{
	public partial class UmlBuilder : Form
	{
		private Diagram diagram = null;

		public UmlBuilder()
		{
			InitializeComponent();

			loadingPanel.Hide();
			mainPanel.Hide();
			menuPanel.Show();
		}

		private void DropPanel_DragDrop(object sender, DragEventArgs e)
		{
			foreach (string obj in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				if (Directory.Exists(obj))
				{
					string[] files = Directory.GetFiles(obj, ".", SearchOption.AllDirectories);
					foreach (var str in files)
						AddNewItem(str);
				}
				else
					AddNewItem(obj);
			}

			fileDropLabel.Text = "Перетащите файлы сюда";

			void AddNewItem(string str)
			{
				int dotIndex = str.IndexOf('.');
				while (str.IndexOf('.', dotIndex + 1) > 0)
					dotIndex = str.IndexOf('.', dotIndex + 1);

				if (str.IndexOf(".h", dotIndex) > 0 || str.IndexOf(".cpp", dotIndex) > 0 || str.IndexOf(".hpp", dotIndex) > 0)
				{
					string[] words = str.Replace("\\\\", "\\").Split('\\');
					string filePath = "", fileName = "";

					for (int i = 0; i < words.Length - 1; i++)
						filePath += words[i] + "\\";
					fileName = words[words.Length - 1];

					ListViewItem item = new ListViewItem(filePath);
					item.SubItems.Add(fileName);
					filePathsList.Items.Add(item);
				}
			}
		}
		private void DropPanel_DragLeave(object sender, EventArgs e)
		{
			fileDropLabel.Text = "Перетащите файлы сюда";
		}
		private void DropPanel_DragEnter(object sender, DragEventArgs e)
		{
			fileDropLabel.Text = "Отпустите мышь";
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void FilePathsList_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				var focusedItem = filePathsList.FocusedItem;
				if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
					deletionMenuStrip.Show(Cursor.Position);
			}
		}
		private void DeletionMenuItem_Click(object sender, EventArgs e)
		{
			List<int> indices = new List<int>();
			foreach (var t in filePathsList.SelectedIndices)
				indices.Add((int)t);

			for (int i = 0; i < indices.Count; i++)
			{
				if (indices[i] - i < filePathsList.Items.Count)
					filePathsList.Items.Remove(filePathsList.Items[indices[i] - i]);
				else
					return;
			}
		}

		private void UmlPictureBox_Paint(object sender, PaintEventArgs e)
		{
			diagram.Paint(sender, e);
		}
		private void UmlPictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (diagram.MouseMove(sender, e))
				Refresh();
		}
		private void UmlPictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			diagram.MouseUp(sender, e);
			Refresh();
		}
		private void UmlPictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			diagram.MouseDown(sender, e);
			Refresh();
		}
		private void UmlPictureBox_MouseWheel(object sender, MouseEventArgs e)
		{
			diagram.MouseWheel(sender, e);
			Refresh();
		}

		private void MainButton_Click(object sender, EventArgs e)
		{
			if (filePathsList.Items.Count == 0)
			{
				errorProvider.SetError(mainButton, "Перетащите файлы в указанную область!");
				return;
			}

			List<string> files = new List<string>();

			for (int i = 0; i < filePathsList.Items.Count; i++)
			{
				string file = filePathsList.Items[i].Text + filePathsList.Items[i].SubItems[1].Text;
				file = file.Replace("\\", "\\\\");

				/*if (!CppAnalyzer.IsFileCompiled(file))
				{
					errorProvider.SetError(mainButton, "В файле присутствует синтаксическая ошибка!");
					return;
				}*/

				using (FileStream fstream = File.OpenRead(file))
				{
					byte[] array = new byte[fstream.Length];
					fstream.Read(array, 0, array.Length);
					files.Add(System.Text.Encoding.Default.GetString(array));
				}
			}

			diagram = new Diagram(ProgressBar_SetValue, LoadingLabel_SetString);
			if (!diagram.Init(files))
			{
				errorProvider.SetError(mainButton, "В файлах отсутствуют классы!");
				return;
			}
			if (diagram.spritesCount == 1) 
				regroupToolStripMenuItem.Visible = false;
            else 
			{
				regroupToolStripMenuItem.Visible = true;
				nextToolStripMenuItem.Visible = true;
				prevToolStripMenuItem.Visible = false;
			}

			progressBar.Maximum = (diagram.LinesCount > 0 ? diagram.LinesCount * 100 : 1);
			
			errorProvider.Clear();
			diagram.GetNewImage();
		}
		
		private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mainPanel.Hide();
			menuPanel.Show();
		}
		private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string path = saveFileDialog.FileName;
				diagram.Save(path);
			}
		}

		private void BackgroundСolorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult result = colorDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				umlPictureBox.BackColor = colorDialog.Color;
				diagram.BackgroungColor = colorDialog.Color;
				diagram.Draw(false, Size.Empty);
				Refresh();
			}
		}
		private void FontColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult result = colorDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				diagram.FontColor = colorDialog.Color;
				diagram.Draw(false, Size.Empty);
				Refresh();
			}
		}
		private void LineColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult result = colorDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				diagram.LineColor = colorDialog.Color;
				diagram.Draw(false, Size.Empty); ;
				Refresh();
			}
		}
		private void RectangleColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult result = colorDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				diagram.RectangleColor = colorDialog.Color;
				diagram.Draw(false, Size.Empty);
				Refresh();
			}
		}
		private void ChangeLineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!diagram.IsLineRounded)
				changeLineToolStripMenuItem.Text = "Вернуть углы";
			else
				changeLineToolStripMenuItem.Text = "Скруглить углы";

			diagram.IsLineRounded = !diagram.IsLineRounded;
			diagram.Draw(false, Size.Empty);
			Refresh();
		}
		private void NextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (diagram.curSprite < diagram.spritesCount - 1)
			{
				diagram.curSprite++;

				if (diagram.curSprite == diagram.spritesCount - 1)
					nextToolStripMenuItem.Visible = false;

				if (diagram.curSprite > 0)
					prevToolStripMenuItem.Visible = true;

				diagram.GetNewImage();
				Refresh();
			}
		}
		private void PrevToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (diagram.curSprite > 0)
			{
				diagram.curSprite--;

				if (diagram.curSprite == 0)
					prevToolStripMenuItem.Visible = false;

				if (diagram.curSprite < diagram.spritesCount - 1)
					nextToolStripMenuItem.Visible = true;

				diagram.GetNewImage();
				Refresh();
			}
		}

		private void LoadingLabel_SetString(string str)
        {
			loadingLabel.Invoke((Action)(() => loadingLabel.Text = str));
        }
		private void ProgressBar_SetValue(int value)
		{
			progressBar.Invoke(
				(Action)(() => 
					{
						progressBar.Value = value;
						if (value == 0)
						{
							mainPanel.Hide();
							menuPanel.Hide();
							loadingPanel.Show();
						}
						else if (progressBar.Value == progressBar.Maximum)
						{
							diagram.Draw(true, umlPictureBox.Size);
							loadingPanel.Hide();
							menuPanel.Hide();
							mainPanel.Show();
						}
					})
				);
		}

        private void fileDropLabel_Click(object sender, EventArgs e)
        {

        }
    }
}