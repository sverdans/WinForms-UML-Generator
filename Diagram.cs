using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mapper;


namespace UmlBuilder
{
	public partial class Diagram
	{
		private class DijkstraStats
		{
			public int rotations;
			public int distance;
			public bool visited;
			public Point parent;
			public DijkstraStats(int distance, bool visited, Point parent, int rotations)
			{
				this.rotations = rotations;
				this.distance = distance;
				this.visited = visited;
				this.parent = parent;
			}
		}

		private bool isLeftButtonPressed = false;
		
		private Action<int> ProgressBar_SetValue;
		private Action<string> LoadingLabel_SetString;

		private Bitmap image = null;
		private PointF pictureMoveTo;
		private PointF mouseDown;
		private Rectangle imageRect;
		private Size minImageRectSize;
		
		private List<Sprite> prevSprites;
		private List<Sprite> sprites;
		public int curSprite = 0;
		public int spritesCount;

		private Dictionary<Point, List<Point>> points;
		private List<ObjectInfo> objectsInfos;
		private List<Line> communicationLines;

		private Size GetImageRectSize(Size boxSize)
        {
			Size rectSize = Size.Empty;

			float k1 = image.Width / image.Height;
			float k2 = boxSize.Width / boxSize.Height;

			if (k1 > k2)
			{
				rectSize.Width = boxSize.Width;
				rectSize.Height = rectSize.Width * image.Height / image.Width;
			}
			else
			{
				rectSize.Height = boxSize.Height;
				rectSize.Width = rectSize.Height * image.Width / image.Height;
			}

			return rectSize; 
		}
		private void DrawSupportElements(List<SupportLine> supportLines)
		{
			GraphicElement.graphics.DrawRectangle(GraphicElement.imageRectPen, 0, 0, image.Width, image.Height);

			foreach (var o in objectsInfos)
			{
				GraphicElement.graphics.DrawRectangle(GraphicElement.objectAreaPen,
					o.PosForMakingSupportLines.X,
					o.PosForMakingSupportLines.Y,
					o.SizeForMakingSupportLines.X,
					o.SizeForMakingSupportLines.Y);
			}

			foreach (var l in supportLines)
				GraphicElement.graphics.DrawLine(GraphicElement.supportLinePen, l.LE, l.LS);
		}
		public Diagram(Action<int> ProgressBar_ChangeValue, Action<string> LoadingLabel_SetString) 
		{
			communicationLines = new List<Line>();
			sprites = new List<Sprite>();

			this.ProgressBar_SetValue = ProgressBar_ChangeValue;
			this.LoadingLabel_SetString = LoadingLabel_SetString;
		}
		private void GetCommunicationLines()
		{
			if (Constants.LinesCount == 0)
			{
				ProgressBar_SetValue(1);
				return;
			}

			List<SupportLine> supportLines = new List<SupportLine>();
			points = new Dictionary<Point, List<Point>>();
			communicationLines = new List<Line>();

			MakeSupportLines();
			ExtendSupportLines();
			MergeSupportLines();
			DrawSupportElements(supportLines);
			DetectLineIntersections();
			
			List<(ObjectInfo from, ObjectInfo to, LinksType link, int estimatedDistance)> connections =
				new List<(ObjectInfo from, ObjectInfo to, LinksType link, int estimatedDistance)>();

			foreach (var o in objectsInfos)
				foreach (var l in o.links)
					{
						Point p1 = new Point(o.position.X + o.size.X / 2, o.position.Y + o.size.Y / 2);
						Point p2 = new Point(l.connectedTo.position.X + l.connectedTo.size.X / 2, l.connectedTo.position.Y + l.connectedTo.size.Y / 2);
						int distance = (int)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
						connections.Add((o, l.connectedTo, l.linkType, distance));
					}

			connections.Sort((a, b) => b.estimatedDistance.CompareTo(a.estimatedDistance));

            foreach (var c in connections)
            {
				string connectionInfo = $"Соединяются \"{c.from}\" и \"{c.to}\"";
				Debug.WriteLine(connectionInfo);
				LoadingLabel_SetString(connectionInfo);
                communicationLines.Add(new Line(NewDijkstra(c.from, c.to), c.link));
				ProgressBar_SetValue(communicationLines.Count * 100);
            }

            void MakeSupportLines()
			{
				int maxDl = Constants.PelAroundObject;
				int dl;
				for (int i = 1; i <= Constants.LinesAroundObject; i++)
				{
					dl = i * Constants.PelBetwLines + Constants.PelAroundObject;
					supportLines.Add(new SupportLine(0, 0 + dl, image.Width, 0 + dl));
					supportLines.Add(new SupportLine(0, image.Height - dl, image.Width, image.Height - dl));
					supportLines.Add(new SupportLine(0 + dl, 0, 0 + dl, image.Height));
					supportLines.Add(new SupportLine(image.Width - dl, 0, image.Width - dl, image.Height));
				}
				

				foreach (var o in objectsInfos)
				{
					int attachmentPointsCount = (o.attachmentPointsCount + 1) / 2;
					
					int startX = o.position.X + o.size.X / 2 - (attachmentPointsCount * Constants.PelBetwAttachmentPoints) / 2;
					startX += Constants.PelBetwAttachmentPoints - startX % Constants.PelBetwAttachmentPoints;

					for (int i = 0; i < attachmentPointsCount; i++)
					{
						int x = startX + i * Constants.PelBetwAttachmentPoints;
						Point attachmentPoint = new Point(x, o.position.Y);
						o.attachmentPoints.Add(attachmentPoint);
						supportLines.Add(new SupportLine(
							attachmentPoint.X,
							attachmentPoint.Y - maxDl - Constants.PelForArrow,
							attachmentPoint.X,
							attachmentPoint.Y));

						attachmentPoint = new Point(x, o.position.Y + o.size.Y);
						o.attachmentPoints.Add(attachmentPoint);
						supportLines.Add(new SupportLine(
							attachmentPoint.X,
							attachmentPoint.Y,
							attachmentPoint.X,
							attachmentPoint.Y + maxDl + Constants.PelForArrow));
					}

					Point start = new Point(o.PosForMakingSupportLines.X, o.PosForMakingSupportLines.Y);
					Point end = new Point(
						o.PosForMakingSupportLines.X + o.SizeForMakingSupportLines.X,
						o.PosForMakingSupportLines.Y + o.SizeForMakingSupportLines.Y);

					for (int i = 1; i <= Constants.LinesAroundObject; i++)
					{
						dl = i * Constants.PelBetwLines;
						supportLines.Add(new SupportLine(start.X - maxDl, start.Y - dl, end.X + maxDl, start.Y - dl));
						supportLines.Add(new SupportLine(start.X - maxDl, end.Y + dl, end.X + maxDl, end.Y + dl));
						supportLines.Add(new SupportLine(start.X - dl, start.Y - maxDl, start.X - dl, end.Y + maxDl));
						supportLines.Add(new SupportLine(end.X + dl, start.Y - maxDl, end.X + dl, end.Y + maxDl));
					}
				}
			}
			void ExtendSupportLines()
			{
				Point direction, nextLE, nextLS;
				foreach (var l in supportLines)
				{
					if (l.isVertical)
						direction = new Point(0, Constants.PelBetwLines);
					else
						direction = new Point(Constants.PelBetwLines, 0);

					while (!IsCollision(nextLS = Difference(l.LS, direction)))
						l.LS = nextLS;

					while (!IsCollision(nextLE = Sum(l.LE, direction)))
						l.LE = nextLE;
				}

				bool IsCollision(Point p)
				{
					if (p.X >= image.Width || p.X <= 0 || p.Y >= image.Height || p.Y <= 0)
						return true;

					foreach (var o in objectsInfos)
					{
						if (p.X >= o.PosForMakingSupportLines.X && p.X <= o.PosForMakingSupportLines.X + o.SizeForMakingSupportLines.X &&
							p.Y >= o.PosForMakingSupportLines.Y && p.Y <= o.PosForMakingSupportLines.Y + o.SizeForMakingSupportLines.Y)
							return true;
					}
					return false;
				}
			}
			void MergeSupportLines()
			{
				for (int i = 0; i < supportLines.Count; i++)
					for (int j = 0; j < supportLines.Count; j++)
						if (supportLines[i] != supportLines[j] && i != j)
						{
							if (supportLines[i].isVertical && supportLines[j].isVertical)
							{
								if ((supportLines[i].LE.X == supportLines[j].LE.X) &&
									(supportLines[i].LS.Y <= supportLines[j].LS.Y) &&
									(supportLines[i].LE.Y >= supportLines[j].LS.Y))
								{
									supportLines[i].LE = supportLines[i].LE.Y > supportLines[j].LE.Y ? supportLines[i].LE : supportLines[j].LE;
									supportLines.RemoveAt(j);
									if (i > j) i--;
									j--;
								}
							}
							else if (!supportLines[i].isVertical && !supportLines[j].isVertical)
							{
								if ((supportLines[i].LE.Y == supportLines[j].LE.Y) &&
									(supportLines[i].LS.X <= supportLines[j].LS.X) &&
									(supportLines[i].LE.X >= supportLines[j].LS.X))
								{
									supportLines[i].LE = supportLines[i].LE.X > supportLines[j].LE.X ? supportLines[i].LE : supportLines[j].LE;
									supportLines.RemoveAt(j);
									if (i > j) i--;
									j--;
								}
							}
						}
			}
			void DetectLineIntersections()
			{
				foreach (var l in supportLines)
					foreach (var t in supportLines)
					{
						if (l.isVertical && !t.isVertical &&
							l.LE.X >= t.LS.X && l.LE.X <= t.LE.X &&
							t.LE.Y >= l.LS.Y && t.LE.Y <= l.LE.Y)
							l.points.Add(new Point(l.LE.X, t.LE.Y));
						else if (!l.isVertical && t.isVertical &&
							t.LE.X >= l.LS.X && t.LE.X <= l.LE.X &&
							l.LE.Y >= t.LS.Y && l.LE.Y <= t.LE.Y)
							l.points.Add(new Point(t.LE.X, l.LE.Y));
					}
				foreach (var l in supportLines)
					l.AddPointsInMap(points);
			}
			Point Sum(Point p1, Point p2) { return new Point(p1.X + p2.X, p1.Y + p2.Y); }
			Point Difference(Point p1, Point p2) { return new Point(p1.X - p2.X, p1.Y - p2.Y); }

			List<Point> NewDijkstra(ObjectInfo o1, ObjectInfo o2)
			{
				Dictionary<Point, DijkstraStats> map = new Dictionary<Point, DijkstraStats>();

				Point start = new Point(o1.position.X + o1.size.X / 2, o1.position.Y + o1.size.Y / 2);
				Point end = new Point(o2.position.X + o2.size.X / 2, o2.position.Y + o2.size.Y / 2);

				AddPoints();

				foreach (var k in points.Keys)
					map.Add(k, new DijkstraStats(int.MaxValue, false, new Point(-1, -1), int.MaxValue));

				int closedVertexCount = 0;

				Point curPoint = start;
				Point prevPoint = start;

				bool haveOpenVertex = true;
				map[curPoint].distance = 0;
				map[curPoint].rotations = 0;

				while (haveOpenVertex)
				{
					foreach (var t in points[curPoint])
					{
						int distance;
						if (t == end || curPoint == start)
							distance = 1;
						else
							distance = (t.X == curPoint.X ? Math.Abs(t.Y - curPoint.Y) : Math.Abs(t.X - curPoint.X));
						
						int rotation = (t.X != prevPoint.X && t.Y != prevPoint.Y ? 1 : 0);

						if (distance + map[curPoint].distance < map[t].distance)
						{
							map[t].distance = map[curPoint].distance + distance;
							map[t].rotations = map[curPoint].rotations + rotation;
							map[t].parent = curPoint;
						}
						else if ((distance + map[curPoint].distance == map[t].distance) &&
							(map[curPoint].rotations < map[t].rotations))
						{
							map[t].rotations = map[curPoint].rotations + rotation;
							map[t].distance = map[curPoint].distance + distance;
							map[t].parent = curPoint;
						}
					}

					map[curPoint].visited = true;
					closedVertexCount++;

					int progressDelta = closedVertexCount * 100 / map.Count;
					if (progressDelta == 0) progressDelta = 1;
					else if (progressDelta >= 100) progressDelta = 99;
					ProgressBar_SetValue(communicationLines.Count * 100 + progressDelta);

					haveOpenVertex = false;
					int min = int.MaxValue;

					foreach (var k in map.Keys)
					{
						if (!map[k].visited && map[k].distance < min)
						{
							haveOpenVertex = true;
							curPoint = k;
							prevPoint = map[curPoint].parent;
							min = map[k].distance;
						}
					}
				}

				bool dijkstaraError = false;
				List<Point> result = new List<Point> { end };
				Point p = map[end].parent;
				while (p != start)
				{
					if (p.X == -1)
					{
						dijkstaraError = true;
						Debug.WriteLine("Dijkstra Error!");
						break;
					}

					Point father = map[p].parent;
					Point grandfather = map[father].parent;

					if (p.X != grandfather.X &&
						p.Y != grandfather.Y)
					{
						foreach (var t in points[father])
							points[t].Remove(father);
						points.Remove(father);
					}
					else if (p.X == grandfather.X || p.Y == grandfather.Y)
					{

						if (points.ContainsKey(p))
							points[p].Remove(father);
						points[father].Remove(p);
						points[father].Remove(grandfather);
						points[grandfather].Remove(father);
					}

					result.Add(p);
					p = map[p].parent;
				}
				
				result.Add(start);

				if (!dijkstaraError)
				{
					result.Reverse();
					result.RemoveAt(0);
					result.RemoveAt(result.Count - 1);
					o1.attachmentPoints.Remove(result[0]);
					o2.attachmentPoints.Remove(result[result.Count - 1]);
				}
				
				DeletePoints();
				return result;


				void AddPoints()
                {
					points.Add(start, o1.attachmentPoints);
					points.Add(end, o2.attachmentPoints);

					foreach (var o in o1.attachmentPoints)
						points[o].Add(start);
					foreach (var o in o2.attachmentPoints)
						points[o].Add(end);
				}
				void DeletePoints()
                {
					points.Remove(start);
					points.Remove(end);

					foreach (var o in o1.attachmentPoints)
						points[o].Remove(start);
					foreach (var o in o2.attachmentPoints)
						points[o].Remove(end);
				}
			}
		}
		public async void GetNewImage()
		{
			ProgressBar_SetValue(0);

			foreach (var o in objectsInfos)
				o.attachmentPoints.Clear();

			var sprite = sprites[curSprite];

			image = new Bitmap(sprite.Width + 3 * Constants.PelAroundObject, sprite.Height + 3 * Constants.PelAroundObject);
			GraphicElement.graphics = Graphics.FromImage(image);

			foreach (var i in sprite.MappedImages)
			{
				((ObjectInfo)i.ImageInfo).position.X = i.X + 2 * Constants.PelAroundObject + Constants.PelForArrow;
				((ObjectInfo)i.ImageInfo).position.Y = i.Y + 2 * Constants.PelAroundObject + Constants.PelForArrow;
			}

			minImageRectSize = GetImageRectSize(new Size(300, 300));
			await Task.Run(() => GetCommunicationLines());
		}
		public void Draw(bool isNormalize, Size pictureBoxSize)
		{
			if (isNormalize)
			{
				Size rectSize = GetImageRectSize(pictureBoxSize);
				Point rectPos = new Point(
					(pictureBoxSize.Width - rectSize.Width) / 2,
					(pictureBoxSize.Height - rectSize.Height) / 2);
				
				imageRect = new Rectangle(rectPos, rectSize);
			}

			GraphicElement.graphics.FillRectangle(GraphicElement.backgroundColor,
				new Rectangle(0, 0, image.Width, image.Height));
			
			foreach (var o in objectsInfos)
				o.Draw();
			foreach (var l in communicationLines)
				l.Draw();
		}
		public bool Init(List<string> programs)
		{
			curSprite = 0;
			prevSprites = new List<Sprite>();
			sprites = new List<Sprite>();

			pictureMoveTo = mouseDown = PointF.Empty;
			CppAnalyzer analyzer = new CppAnalyzer(programs);
			objectsInfos = analyzer.GetObjectsInfo();

			if (objectsInfos.Count == 0)
				return false;

			int linesCount = 0;
			Bitmap temp = new Bitmap(1, 1);
			GraphicElement.graphics = Graphics.FromImage(temp);

			Debug.WriteLine("_".PadRight(50, '_'));

			foreach (var o in objectsInfos)
				o.IdentifyConnections(objectsInfos);

			foreach (var o in objectsInfos)
				o.DeleteMatches();
			
			Debug.WriteLine("_".PadRight(50, '_'));
			Debug.WriteLine("Name".PadRight(30) + "X".PadRight(10) + "Y".PadRight(10));

			foreach (var o in objectsInfos)
			{
				linesCount += o.links.Count;
				o.MakeRectangle();
			}
			Debug.WriteLine("_".PadRight(50, '_'));

			Constants.SetLinesCount(linesCount);

			Sprite sprite;
			do
			{
				Canvas canvas = new Canvas();
				MapperOptimalEfficiency<Sprite> mapper = new MapperOptimalEfficiency<Sprite>(canvas, prevSprites);
				sprite = mapper.Mapping(objectsInfos);
				if (sprite == null || sprites.Count > 14)
					break;
				prevSprites.Add(sprite);
				sprites.Add(sprite);
			}
			while (true);

			spritesCount = sprites.Count;
			sprites.Sort((a, b) => (a.MaximumSideRatio.CompareTo(b.MaximumSideRatio)));
			
			foreach(var a in sprites)
				Debug.WriteLine(a.MaximumSideRatio);
			
			return true;
		}
		public bool MouseMove(object sender, MouseEventArgs e)
		{
			if (isLeftButtonPressed)
			{
				pictureMoveTo.X = e.Location.X - mouseDown.X;
				pictureMoveTo.Y = e.Location.Y - mouseDown.Y;
				return true;
			}
			return false;
		}
		public void MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			isLeftButtonPressed = false;
			imageRect.X += (int)pictureMoveTo.X;
			imageRect.Y += (int)pictureMoveTo.Y;
			pictureMoveTo = PointF.Empty;
			mouseDown = PointF.Empty;
			//	Cursor.Current = new Cursor("..\\..\\Resources\\grab.cur");
		}
		public void MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			isLeftButtonPressed = true;
			mouseDown = e.Location;
			//	Cursor.Current = new Cursor("..\\..\\Resources\\grabbing.cur");
		}
		public void MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta == 0)
				return;

			PointF globalMousePosition = new Point(
				(e.Location.X - imageRect.X) * image.Width / imageRect.Width,
				(e.Location.Y - imageRect.Y) * image.Height / imageRect.Height);

			float zoom = Math.Sign(e.Delta) / 8f;
			float dx = imageRect.Width * zoom;
			float dy = imageRect.Height * zoom;

			int newWidth = imageRect.Width + (int)dx;
			int newHeight = imageRect.Height + (int)dy;

			if (newHeight < minImageRectSize.Height || newWidth < minImageRectSize.Width)
				return;

			imageRect.Width += (int)dx;
			imageRect.Height += (int)dy;

			imageRect.X = (int)((globalMousePosition.X * imageRect.Width) / image.Width - e.Location.X) * -1;
			imageRect.Y = (int)((globalMousePosition.Y * imageRect.Height) / image.Height - e.Location.Y) * -1;
		}
		public void Paint(object sender, PaintEventArgs e)
		{
			if (image != null)
				e.Graphics.DrawImage(
					image,
					imageRect.X + pictureMoveTo.X,
					imageRect.Y + pictureMoveTo.Y,
					imageRect.Width, imageRect.Height);
		}
		public void Save(string path) { image.Save(path); }
	}
}