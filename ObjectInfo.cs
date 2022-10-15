using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System;
using Mapper;

namespace UmlBuilder
{
	public partial class Diagram
	{
		private enum LinksType
		{
			Dependency,
			Association,
			Aggregation,
			Composition,
			Realization,
			Inheritance
		}

		private class Field
		{
			public string str;

			public string accessType;
			public string type;
			public string name;
			public bool isStatic;

			public Field(string accessType, bool isStatic, string type, string name)
			{
				this.accessType = accessType;
				this.isStatic = isStatic;
				this.type = type;
				this.name = name;

				switch (accessType)
				{
					case "private": str += " - "; break;
					case "protected": str += " # "; break;
					case "public": str += " + "; break;
				}
				str += name + " : " + type;
			}
		}

		private class Method
		{
			public string str;

			public string accessType;
			public string returnType;
			public string name;
			public string realisation;
			public bool isStatic;
			public bool isAbstact;
			public List<Field> arguments;

			public Method(string accessType, string returnType, string name, List<Field> arguments, string realisation, bool isStatic, bool isAbstact)
			{
				this.accessType = accessType;
				this.returnType = returnType;
				this.name = name;
				this.arguments = arguments;
				this.isStatic = isStatic;
				this.isAbstact = isAbstact;
				this.realisation = realisation;

				switch (accessType)
				{
					case "private": str += " - "; break;
					case "protected": str += " # "; break;
					case "public": str += " + "; break;
				}
				str += name + "(";

				for (int i = 0; i < arguments.Count; i++)
				{
					str += arguments[i].name + " : " + arguments[i].type;
					if (i < arguments.Count - 1)
						str += ", ";
				}
				str += ") : " + returnType;
			}
		}

		private abstract class ObjectInfo : GraphicElement, IImageInfo
		{
			public Point position;
			public Point size;
			
			public string name;
			public string announcedIn;
			public List<(LinksType linkType, ObjectInfo connectedTo)> links;
			public List<Point> attachmentPoints;
			
			public int attachmentPointsCount = 2;

			public ObjectInfo()
			{
				links = new List<(LinksType linkType, ObjectInfo connectedTo)>();
				attachmentPoints = new List<Point>();
			}

			public Point PosForMakingSupportLines 
			{ 
				get 
				{
					return new Point(
						position.X - Constants.PelForArrow,
						position.Y - Constants.PelForArrow); 
				} 
			}
			public Point SizeForMakingSupportLines 
			{ 
				get 
				{ 
					return new Point(
						size.X + 2 * Constants.PelForArrow,
						size.Y + 2 * Constants.PelForArrow); 
				} 
			}
			public int Width 
			{ 
				get 
				{ 
					return size.X + Constants.PelAroundObject + 2 * Constants.PelForArrow; 
				} 
			}
			public int Height 
			{ 
				get 
				{ 
					return size.Y + Constants.PelAroundObject + 2 * Constants.PelForArrow; 
				} 
			}
			public int Area { get { return Width * Height; } }
			
			protected void NormalizeSize()
			{
				size.X += Constants.PelBetwLines - size.X % Constants.PelBetwLines;
				size.Y += Constants.PelBetwLines - size.Y % Constants.PelBetwLines;
				Debug.WriteLine(ToString().PadRight(30) + size.X.ToString().PadRight(10) + size.Y.ToString().PadRight(10));
			}
			public virtual void IdentifyConnections(List<ObjectInfo> objects) 
			{
                foreach (var o in objects)
                    if (CppAnalyzer.IndexOfKeyWord(announcedIn, o.name) != -1)
                    {
                        links.Add((LinksType.Composition, o));
                        attachmentPointsCount++;
                        o.attachmentPointsCount++;
                        break;
                    }
            }
			public virtual void DeleteMatches()
            {
				foreach (var l in links)
					Debug.WriteLine($"{name} связан с {l.connectedTo} - {l.linkType}");
				
				for (int i = 0; i < links.Count; i++)
				{
					if (links[i].linkType == LinksType.Composition)
					{
						int k = links.FindIndex(a => a.linkType == LinksType.Aggregation && a.connectedTo == links[i].connectedTo);
						if (k >= 0)
						{
							links.RemoveAt(k);
							if (k <= i)
								i--;
							attachmentPointsCount--;
							links[i].connectedTo.attachmentPointsCount--;
							
						}
					}
				}
            }
			public abstract void MakeRectangle();
			public abstract void Draw();
			public override string ToString() { return name; }
		}

		private class ClassInfo : ObjectInfo
		{
			public string father;
			public bool isAbstact = false;

			public List<Field> fields;
			public List<Method> methods;

			public ClassInfo() : base()
			{
				fields = new List<Field>();
				methods = new List<Method>();
			}
			public ClassInfo(string name, string father, string announcedIn, List<Field> attributes, List<Method> methods) : base()
			{
				if (announcedIn != string.Empty)
					Debug.WriteLine($"{name} объявлен в {announcedIn}");

				this.name = name;
				this.father = father;
				this.announcedIn = announcedIn;
				this.fields = attributes;
				this.methods = methods;

				foreach (var a in methods)
					if (a.isAbstact)
						isAbstact = true;
			}
			public override void IdentifyConnections(List<ObjectInfo> objects)
			{
				base.IdentifyConnections(objects);

				foreach (var o in objects)
					if (this != o && CppAnalyzer.IndexOfKeyWord(father, o.name) != -1)
						AddInList(LinksType.Inheritance, o);

                foreach (var o in objects)
                    foreach (var f in fields)
                        if (this != o && CppAnalyzer.IndexOfKeyWord(f.type, o.name) != -1)
                        {
                            AddInList(LinksType.Aggregation, o);
                            break;
                        }

                foreach (var o in objects)
                {
                    bool isBreak = false;
                    foreach (var m in methods)
                    {
                        if (isBreak) break;
                        foreach (var a in m.arguments)
                            if (this != o && CppAnalyzer.IndexOfKeyWord(a.type, o.name) != -1)
                            {
                                AddInList(LinksType.Dependency, o);
                                isBreak = true;
                                break;
                            }
                    }
                }
                void AddInList(LinksType link, ObjectInfo obj)
				{
					if (link == LinksType.Aggregation)
						obj.links.Add((link, this));
					else
						links.Add((link, obj));
					
					attachmentPointsCount++;
					obj.attachmentPointsCount++;
				}
			}
			public override void MakeRectangle()
			{
				int stringLength;

				size.X = (int)graphics.MeasureString("_" + name + "_", regularFont).Width;

				stringLength = (attachmentPointsCount + 2) * Constants.PelBetwAttachmentPoints;
				if (stringLength > size.X)
					size.X = stringLength;

				foreach (var f in fields)
				{
					stringLength = (int)graphics.MeasureString(f.str + "_", regularFont).Width;
					if (stringLength > size.X)
						size.X = stringLength;
				}

				foreach (var m in methods)
				{
					stringLength = (int)graphics.MeasureString(m.str + "_", regularFont).Width;
					if (stringLength > size.X)
						size.X = stringLength;
				}

				size.Y = (methods.Count + fields.Count + 7) * regularFont.Height;
				NormalizeSize();
			}
			public override void Draw()
			{
				float x = position.X;
				float y = position.Y;
				float namePosX = (size.X - graphics.MeasureString(name, regularFont).Width) / 2;

				graphics.DrawRectangle(pen, x, y, size.X, size.Y);
				y += regularFont.Height;
				graphics.DrawString(name, (isAbstact ? italicFont : regularFont), fontBrush, x + namePosX, y);
				y += 2 * regularFont.Height;
				graphics.DrawLine(pen, x, y, x + size.X, y);
				y += regularFont.Height;

				foreach (var f in fields)
				{
					graphics.DrawString(f.str, (f.isStatic ? underlineFont : regularFont), fontBrush, x, y);
					y += regularFont.Height;
				}

				y += regularFont.Height;
				graphics.DrawLine(pen, x, y, x + size.X, y);
				y += regularFont.Height;

				foreach (var m in methods)
				{
					graphics.DrawString(m.str, regularFont, fontBrush, x, y);
					y += regularFont.Height;
				}
			}
		}

		private class EnumInfo : ObjectInfo
		{
			public string[] enumerations;

			public EnumInfo(string name, string announcedIn, string[] enumerations) : base()
			{
				if (announcedIn != string.Empty)
					Debug.WriteLine($"{name} объявлен в {announcedIn}");

				this.name = name;
				this.announcedIn = announcedIn;
				this.enumerations = enumerations;
			}

			public override void MakeRectangle()
			{
				int stringLength;

				size.X = (int)graphics.MeasureString("_" + name + "_", regularFont).Width;
				
				stringLength = (int)graphics.MeasureString("_<<enumeration>>_", regularFont).Width;
				if (stringLength > size.X)
					size.X = stringLength;
				
				stringLength = (attachmentPointsCount + 2) * Constants.PelBetwAttachmentPoints;
				if (stringLength > size.X)
					size.X = stringLength;

				foreach (var e in enumerations)
				{
					stringLength = (int)graphics.MeasureString(e + "_", regularFont).Width;
					if (stringLength > size.X)
						size.X = stringLength;
				}

				size.Y = (enumerations.Length + 2 + 4) * regularFont.Height;
				NormalizeSize();
			}
			public override void Draw()
			{
				float x = position.X;
				float y = position.Y;
				float namePosX = (size.X - graphics.MeasureString(name, regularFont).Width) / 2;
				float typePosX = (size.X - graphics.MeasureString("<<enumeration>>", regularFont).Width) / 2;

				graphics.DrawRectangle(pen, x, y, size.X, size.Y);
				y += regularFont.Height;
				graphics.DrawString("<<enumeration>>", regularFont, fontBrush, x + typePosX, y);
				y += regularFont.Height;
				graphics.DrawString(name, regularFont, fontBrush, x + namePosX, y);
				y += 2 * regularFont.Height;
				graphics.DrawLine(pen, x, y, x + size.X, y);
				y += regularFont.Height;
				foreach (var e in enumerations)
				{
					graphics.DrawString(e, regularFont, fontBrush, x, y);
					y += regularFont.Height;
				}
			}
		}
	}
}