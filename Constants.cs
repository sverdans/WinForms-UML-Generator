using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmlBuilder
{
	public partial class Diagram
	{
		public int LinesCount
		{
			get { return Constants.LinesCount; }
		}
		public bool IsLineRounded
		{
			get { return Line.isRounded; }
			set { Line.isRounded = value; }
		}
		public Color BackgroungColor
		{
			set
			{
				GraphicElement.backgroundColor = new SolidBrush(value);
				GraphicElement.backgroundPen.Color = value;
			}
		}
		public Color LineColor
		{
			set 
			{ 
				GraphicElement.linePen.Color = value;
				GraphicElement.dashedLinePen.Color = value;
				GraphicElement.arrowBrush = new SolidBrush(value);
			}
		}
		public Color FontColor
		{
			set { GraphicElement.fontBrush = new SolidBrush(value); }
		}
		public Color RectangleColor
		{
			set { GraphicElement.pen.Color = value; }
		}

		private static class Constants
		{
			public static int LinesCount { get; private set; }
			public static int LinesAroundObject { get; private set; }
			
			public static int PelForArrow { get; private set; }
			public static int PelBetwAttachmentPoints { get; private set; }
			public static int PelBetwLines { get; private set; }
			public static int PelAroundObject { get; private set; }

			static Constants()
			{
				PelBetwLines = 16;
				PelForArrow = PelBetwLines * 4;
				PelBetwAttachmentPoints = PelBetwLines * 4;
			}
			public static void SetLinesCount(int value)
			{
				LinesCount = value;
				LinesAroundObject = (LinesCount + 3) / 4;
				PelAroundObject = (2 * LinesAroundObject + 1) * PelBetwLines;

				Debug.WriteLine("Количество линий: " + LinesCount);
				Debug.WriteLine("Количество линий вокруг одного объекта: " + LinesAroundObject);
				Debug.WriteLine("Количество пикселей между двумя объектами: " + PelAroundObject);
			}
		}

		private abstract class GraphicElement
		{
			protected static float lineWidth = 4f;
			public static Graphics graphics;
			public static Pen imageRectPen = new Pen(Color.Chocolate, lineWidth);
			public static Pen supportLinePen = new Pen(Color.Green, lineWidth);
			public static Pen objectAreaPen = new Pen(Color.Orange, lineWidth);

			public static Pen pen = new Pen(Color.Black, lineWidth);
			public static Pen linePen = new Pen(Color.Black, lineWidth);
			public static Pen backgroundPen = new Pen(Color.White, lineWidth);
			public static Pen dashedLinePen = new Pen(Color.Black, lineWidth);
			
			public static Brush fontBrush = new SolidBrush(Color.Black);
			public static Brush backgroundColor = new SolidBrush(Color.White);
			public static Brush arrowBrush = new SolidBrush(Color.Black);

			protected static float fontSize = 23f;
			protected static Font underlineFont = new Font("Courier New", fontSize, FontStyle.Underline);
			protected static Font regularFont = new Font("Courier New", fontSize, FontStyle.Regular);
			protected static Font italicFont = new Font("Courier New", fontSize, FontStyle.Italic);
			protected static Font boldFont = new Font("Courier New", fontSize, FontStyle.Bold);

			static GraphicElement()
			{
				dashedLinePen.DashStyle = DashStyle.Dash;

				/*dashedLinePen.DashStyle = DashStyle.Custom;
				float[] dashArray = { 5.0f, 3.0f, 5.0f, 3.0f };
				dashedLinePen.DashPattern = dashArray;*/
			}
		}
	}
}
