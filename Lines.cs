using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmlBuilder
{
	public partial class Diagram
	{
		private class SupportLine
		{
			public Point LS;
			public Point LE;
			public bool isVertical;
			public List<Point> points;
			public SupportLine(Point LS, Point LE)
			{
				this.LS = LS;
				this.LE = LE;
				isVertical = (LS.X == LE.X);
				points = new List<Point>();
			}
			public SupportLine(int x1, int y1, int x2, int y2)
			{
				LS = new Point(x1, y1);
				LE = new Point(x2, y2);
				isVertical = (x1 == x2);
				points = new List<Point>();
			}
			private void SortPoints()
			{
				if (!points.Any(p => p.X == LS.X && p.Y == LS.Y))
					points.Add(LS);
				if (!points.Any(p => p.X == LE.X && p.Y == LE.Y))
					points.Add(LE);

				if (isVertical)
					points.Sort((a, b) => a.Y.CompareTo(b.Y));
				else
					points.Sort((a, b) => a.X.CompareTo(b.X));
			}
			public void AddPointsInMap(Dictionary<Point, List<Point>> map)
			{
				List<Point> dotF = new List<Point>();
				SortPoints();

				dotF.Add(points[1]);
				if (!map.ContainsKey(points[0]))
					map.Add(points[0], dotF);
				else
					map[points[0]].AddRange(dotF);

				for (int i = 1; i < points.Count - 1; i++)
				{
					List<Point> dots = new List<Point>();
					dots.Add(points[i - 1]);
					dots.Add(points[i + 1]);
					if (!map.ContainsKey(points[i]))
						map.Add(points[i], dots);
					else
						map[points[i]].AddRange(dots);
				}

				List<Point> dotL = new List<Point>();

				int lastIndex = points.Count - 1;
				dotL.Add(points[lastIndex - 1]);
				if (!map.ContainsKey(points[lastIndex]))
					map.Add(points[lastIndex], dotL);
				else
					map[points[lastIndex]].AddRange(dotL);
			}
		}

		private class Line : GraphicElement
		{
			private static int diameter = 2 * Constants.PelBetwLines;

			public static bool isRounded = false;
			private List<Point> points;
			private List<(Point pt, short startAngle)> drawingPoints;
			private LinksType links;

			public Line(List<Point> points, LinksType links)
			{
				this.points = points;
				this.links = links;
				List<int> insertIndex = new List<int>();
				drawingPoints = new List<(Point pt, short startAngle)>();
				for (int i = 0; i < points.Count - 2; i++)
				{
					int next = i + 1;
					if (points[i + 2].X != points[i].X &&
						points[i + 2].Y != points[i].Y)
					{
						Point ptPrev, ptCur, ptNext;
						short startAngle;

						if (points[i].X < points[i + 2].X && points[i].Y > points[i + 2].Y)
						{
							if (points[next].Y == points[i].Y)
							{
								ptPrev = new Point(points[next].X - Constants.PelBetwLines, points[next].Y);
								ptNext = new Point(points[next].X, points[next].Y - Constants.PelBetwLines);
								ptCur = new Point(points[next].X - diameter, points[next].Y - diameter);
								startAngle = 0;
							}
							else
							{
								ptPrev = new Point(points[next].X, points[next].Y + Constants.PelBetwLines);
								ptNext = new Point(points[next].X + Constants.PelBetwLines, points[next].Y);
								ptCur = new Point(points[next].X, points[next].Y);
								startAngle = 180;
							}
						}
						else if (points[i].X < points[i + 2].X && points[i].Y < points[i + 2].Y)
						{
							if (points[next].Y == points[i].Y)
							{
								ptPrev = new Point(points[next].X - Constants.PelBetwLines, points[next].Y);
								ptNext = new Point(points[next].X, points[next].Y + Constants.PelBetwLines);
								ptCur = new Point(points[next].X - diameter, points[next].Y);
								startAngle = 270;
							}
							else
							{
								ptPrev = new Point(points[next].X, points[next].Y - Constants.PelBetwLines);
								ptNext = new Point(points[next].X + Constants.PelBetwLines, points[next].Y);
								ptCur = new Point(points[next].X, points[next].Y - diameter);
								startAngle = 90;
							}
						}
						else if (points[i].X > points[i + 2].X && points[i].Y < points[i + 2].Y)
						{
							if (points[next].Y == points[i].Y)
							{
								ptPrev = new Point(points[next].X + Constants.PelBetwLines, points[next].Y);
								ptNext = new Point(points[next].X, points[next].Y + Constants.PelBetwLines);
								ptCur = new Point(points[next].X, points[next].Y);
								startAngle = 180;
							}
							else
							{
								ptPrev = new Point(points[next].X, points[next].Y - Constants.PelBetwLines);
								ptNext = new Point(points[next].X - Constants.PelBetwLines, points[next].Y);
								ptCur = new Point(points[next].X - diameter, points[next].Y - diameter);
								startAngle = 0;
							}
						}
						else
						{
							if (points[next].Y == points[i].Y)
							{
								ptPrev = new Point(points[next].X + Constants.PelBetwLines, points[next].Y);
								ptNext = new Point(points[next].X, points[next].Y - Constants.PelBetwLines);
								ptCur = new Point(points[next].X, points[next].Y - diameter);
								startAngle = 90;

							}
							else
							{
								ptPrev = new Point(points[next].X, points[next].Y + Constants.PelBetwLines);
								ptNext = new Point(points[next].X - Constants.PelBetwLines, points[next].Y);
								ptCur = new Point(points[next].X - diameter, points[next].Y);
								startAngle = 270;
							}
						}

						drawingPoints.Add((points[i], -1));
						if (ptPrev != points[i])
							drawingPoints.Add((ptPrev, -1));
						drawingPoints.Add((ptCur, startAngle));
						if (ptNext != points[i + 2])
							drawingPoints.Add((ptNext, -1));

						points.Insert(i + 2, ptNext);
						insertIndex.Add(i + 2);
						i++;
					}
					else
						drawingPoints.Add((points[i], -1));
				}
				drawingPoints.Add((points[points.Count - 1], -1));

				for (int i = 0; i < insertIndex.Count; i++)
					points.RemoveAt(insertIndex[i] - i);
			}
			public void Draw()
			{
				Pen pen = (links == LinksType.Dependency ? dashedLinePen : linePen);

				if (!isRounded)
					graphics.DrawLines(pen, points.ToArray());
				else
				{
					for (int i = 0; i < drawingPoints.Count - 1; i++)
					{
						if (drawingPoints[i].startAngle < 0 && drawingPoints[i + 1].startAngle < 0)
							graphics.DrawLine(pen, drawingPoints[i].pt, drawingPoints[i + 1].pt);
						else if (drawingPoints[i].startAngle > 0)
							graphics.DrawArc(pen, drawingPoints[i].pt.X, drawingPoints[i].pt.Y, diameter, diameter, drawingPoints[i].startAngle, 90);
						else
						{
							graphics.DrawArc(pen, drawingPoints[i + 1].pt.X, drawingPoints[i + 1].pt.Y, diameter, diameter, drawingPoints[i + 1].startAngle, 90);
							i++;
						}
					}
				}
				DrawArrow(links, points[points.Count - 1], points[points.Count - 2]);

				void DrawArrow(LinksType linkType, Point to, Point prev)
				{
					int k;
					if (prev.Y < to.Y)
						k = 1;
					else
						k = -1;

					switch (linkType)
					{
						case LinksType.Inheritance:
						{
							Point up = new Point(to.X, to.Y - k * 3 * Constants.PelBetwLines);
							Point left = new Point(up.X - Constants.PelBetwLines, up.Y);
							Point right = new Point(up.X + Constants.PelBetwLines, up.Y);
							graphics.DrawLine(backgroundPen, to, up);
							graphics.DrawLine(linePen, left, right);
							graphics.DrawLine(linePen, to, right);
							graphics.DrawLine(linePen, to, left);
							break;
						}
						case LinksType.Composition:
						{
							Point[] pts = new Point[4];
							Point up = new Point(to.X, to.Y - k * 3 * Constants.PelBetwLines);
							Point left = new Point(up.X - Constants.PelBetwLines, (int)(up.Y + k * 1.5 * Constants.PelBetwLines));
							Point right = new Point(up.X + Constants.PelBetwLines, (int)(up.Y + k * 1.5 * Constants.PelBetwLines));
							pts[0] = up;
							pts[1] = left;
							pts[2] = to;
							pts[3] = right;
							graphics.FillPolygon(arrowBrush, pts);
							break;
						}
						case LinksType.Aggregation:
						{
							Point up = new Point(to.X, to.Y - k * 3 * Constants.PelBetwLines);
							Point left = new Point(up.X - Constants.PelBetwLines, (int)(up.Y + k * 1.5 * Constants.PelBetwLines));
							Point right = new Point(up.X + Constants.PelBetwLines, (int)(up.Y + k * 1.5 * Constants.PelBetwLines));
							graphics.DrawLine(backgroundPen, to, up);
							graphics.DrawLine(linePen, up, right);
							graphics.DrawLine(linePen, up, left);
							graphics.DrawLine(linePen, to, right);
							graphics.DrawLine(linePen, to, left);
							break;
						}
						case LinksType.Dependency:
                        {
							int y = to.Y - k * 3 * Constants.PelBetwLines;
							Point left = new Point(to.X - Constants.PelBetwLines, y);
							Point right = new Point(to.X + Constants.PelBetwLines, y);
							graphics.DrawLine(linePen, to, right);
							graphics.DrawLine(linePen, to, left);
							break;
						}
					}
				}
			}
		}
	}
}
