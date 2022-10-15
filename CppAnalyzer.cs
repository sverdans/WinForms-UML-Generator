using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UmlBuilder
{
    public partial class Diagram
	{
		private class CppAnalyzer
		{
			static public bool IsFileCompiled(string filepath)
			{
				string objectFileName = Directory.GetCurrentDirectory() + "\\test.o";
				string gpp = @"C:\MinGW\bin\g++";

				Process gppProcess = new Process();
				gppProcess.StartInfo.RedirectStandardInput = true;
				gppProcess.StartInfo.RedirectStandardError = true;
				gppProcess.StartInfo.RedirectStandardOutput = true;
				gppProcess.StartInfo.UseShellExecute = false;
				gppProcess.StartInfo.FileName = "cmd.exe";
				gppProcess.StartInfo.CreateNoWindow = true;
				gppProcess.StartInfo.Arguments = "/C \"" + gpp + "\" -std=c++11 -o \"" + objectFileName + "\" -c \"" + filepath + "\"";
				gppProcess.Start();

				if (File.Exists(objectFileName))
				{
					File.Delete(objectFileName);
					return true;
				}
				else
				{
					Debug.WriteLine("Ошибка в создании объектного файла: " + gppProcess.StandardError.ReadToEnd());
					return false;
				}
			}
			static public int IndexOfKeyWord(string str, string keyWord, int startIndex = 0)
			{
				int result;
				while ((result = str.IndexOf(keyWord, startIndex)) != -1)
				{
					char l = (result - 1 < 0 ? ' ' : str[result - 1]);
					char r = (result + keyWord.Length >= str.Length ? ' ' : str[result + keyWord.Length]);

					if (char.IsNumber(l) || char.IsLetter(l) || l == '_' ||
						char.IsNumber(r) || char.IsLetter(r) || r == '_')
						startIndex = result + keyWord.Length + 1;
					else
						return result;
					if (startIndex >= str.Length)
						return -1;
				}
				return result;
			}
			static public int IndexOfPairedSymbol(string str, int startIndex, char symbolOpen, char symbolClose)
            {
				int symbolCount = 0;
				for (int i = str.IndexOf(symbolOpen, startIndex); i < str.Length; i++)
				{
					if (str[i] == symbolOpen)
						symbolCount++;
					if (str[i] == symbolClose)
						symbolCount--;
					if (symbolCount == 0)
						return i;
				}
				return -1;
            }
			static public (int, string) FirstIndexOfWords(string str, string[] keywords, int startIndex = 0)
            {
				int result = -1;
				int firstSymbol = str.Length;
				int[] indices = new int[keywords.Length];
				
				for (int i = 0; i < indices.Length; i++)
					indices[i] = IndexOfKeyWord(str, keywords[i], startIndex);
				
				for (int i = 0; i < indices.Length; i++)
					if (indices[i] != -1 && indices[i] < firstSymbol)
					{
						firstSymbol = indices[i];
						result = i;
					}
				return (result == -1 ? (-1, "") : (indices[result], keywords[result]));
			}
			
			private List<(string realisation, string announcedIn, string type)> objects;
			private List<ObjectInfo> objectsInfo;
			private List<string> programs;

			public CppAnalyzer(List<string> programs)
			{
				this.programs = new List<string>();
				foreach (var p in programs)
					this.programs.Add(Init(p));

				objects = new List<(string realisation, string announcedIn, string type)>();
				objectsInfo = new List<ObjectInfo>();
			}
			private string Init(string program)
			{
				while (program.IndexOf("/*") != -1)
				{
					int firstIndex = program.IndexOf("/*");
					program = program.Remove(firstIndex, program.IndexOf("*/") - firstIndex + 2);
				}
				while (program.IndexOf("//") != -1)
				{
					int firstIndex = program.IndexOf("//");
					program = program.Remove(firstIndex, program.IndexOf("\n", firstIndex) - firstIndex);
				}
				while (program.IndexOf('\'') != -1)
				{
					int firstIndex = program.IndexOf('\'');
					program = program.Remove(firstIndex, program.IndexOf('\'', firstIndex) - firstIndex + 1);
				}
				while (program.IndexOf('"') != -1)
				{
					int firstIndex = program.IndexOf('"');
					program = program.Remove(firstIndex, program.IndexOf('"', firstIndex + 1) - firstIndex + 1);
				}

				program = program.Replace("*", " * ");
				program = program.Replace("&", " & ");
				program = program.Replace("=", " = ");

				int prevTemplateIndex = IndexOfKeyWord(program, "template");
				int braceIndex, classIndex = 0;
				while (prevTemplateIndex > 0)
                {
					braceIndex = IndexOfPairedSymbol(program, program.IndexOf('<', prevTemplateIndex), '<', '>');
					classIndex = IndexOfKeyWord(program, "class", prevTemplateIndex);
					if (classIndex > 0 && classIndex < braceIndex)
					{
						program = program.Remove(prevTemplateIndex, braceIndex - prevTemplateIndex + 1);
						//program = program.Remove(classIndex, 5);
						//program = program.Insert(classIndex, "typename");
					}
					prevTemplateIndex = IndexOfKeyWord(program, "template", prevTemplateIndex + 1);
				}
			//	Debug.WriteLine(program);
				return program;
			}
			private void SplitIntoObjects(ref string text, int startIndex = 0, string announcedIn = "")
			{
				string[] types = { "enum", "class", "struct" };
				(int index, string type) curObject = FirstIndexOfWords(text, types, startIndex);

				while (curObject.index != -1)
                {
					int semicolonIndex = text.IndexOf(';', curObject.index);
					int braceIndex = text.IndexOf('{', curObject.index);
					int braceEndIndex;
					if (semicolonIndex > 0 && semicolonIndex < braceIndex)
                    {
						text = text.Remove(curObject.index, semicolonIndex - curObject.index + 1);
                    }
					else
                    {
						braceEndIndex = IndexOfPairedSymbol(text, braceIndex, '{', '}');
						string objectCode = text.Substring(curObject.index, braceEndIndex - curObject.index + 1);
						text = text.Remove(curObject.index, braceEndIndex - curObject.index + 1);
						string name = GetObjectName(objectCode, curObject.type);
						objects.Add((objectCode, announcedIn, curObject.type));
					}
					curObject = FirstIndexOfWords(text, types, startIndex);
				}
			}
			private bool IsAccessModifier(string str, int startIndex)
			{
				if (str.IndexOf('{', startIndex) > 0 && str.IndexOf(':', startIndex) > str.IndexOf('{', startIndex))
					return false;
				else if (str.IndexOf(':', startIndex) > 0 && str.IndexOf(':', startIndex) > str.IndexOf(';', startIndex))
					return false;
				else
					return true;
			}
			private string GetObjectName(string str, string type)
			{
				bool isFirstSymbol = false;
				int firstNameSymbol = 0, length = 0;

				for (int i = type.Length; i < str.Length; i++)
				{
					if ((str[i] == '{' || str[i] == '\n' || str[i] == ':' || str[i] == ' ') && isFirstSymbol)
						return str.Substring(firstNameSymbol, length).Trim();
					else if (str[i] == ' ')
						firstNameSymbol = i + 1;
					else
					{
						isFirstSymbol = true;
						length++;
					}
				}
				return str.Trim();
			}
			private string GetDadInfo(ref string str, string className)
			{
				int startIndex = str.IndexOf(className) + className.Length;
				string result = str.Substring(str.IndexOf(className) + className.Length, str.IndexOf('{') - startIndex - 1);
				str = str.Remove(startIndex, str.IndexOf('{') - startIndex);
				str = str.Insert(startIndex, "\n");
				return result.Trim();
			}
			private void GetField(string str, string accessMod, List<Field> classFields)
			{
				bool isStatic = false;
				int staticIndex = IndexOfKeyWord(str, "static");
				if (staticIndex > 0)
				{
					str = str.Remove(staticIndex, 6);
					isStatic = true;
				}

				string subdata = "", typeName = "";

				if (str.IndexOf("<") != -1)
				{
					int braceCount = 0;

					for (int i = str.IndexOf("<"); i < str.Length; i++)
					{
						if (str[i] == ',')
						{
							str = str.Remove(i, 1);
							str = str.Insert(i, "|");
						}
						else if (str[i] == '[')
						{
							str = str.Remove(i, 1);
							str = str.Insert(i, "└");
						}
						else if (str[i] == ']')
						{
							str = str.Remove(i, 1);
							str = str.Insert(i, "┘");
						}
						if (str[i] == '<')
							braceCount++;
						if (str[i] == '>')
							braceCount--;
						if (braceCount == 0)
							break;
					}
				}
				str = str.Trim();

				// удаление присваиваний типа (int a = ...) и тд
				while (str.IndexOf("=") != -1)
				{
					int startIndex = str.IndexOf("=");
					int braceIndex = str.IndexOf('{', startIndex);
					int parenthesisIndex = str.IndexOf('(', startIndex);
					int endIndex = 0;

					if ((braceIndex > 0 && parenthesisIndex > 0 && braceIndex < parenthesisIndex && braceIndex < str.IndexOf(',', startIndex)) ||
							 (braceIndex > 0 && parenthesisIndex < 0 && braceIndex < str.IndexOf(',', startIndex)))
					{
						endIndex = str.IndexOf(',', IndexOfPairedSymbol(str, braceIndex, '{', '}') + 1);
					}
					else if ((parenthesisIndex > 0 && braceIndex > 0 && parenthesisIndex < braceIndex && parenthesisIndex < str.IndexOf(',', startIndex)) ||
							 (parenthesisIndex > 0 && braceIndex < 0 && parenthesisIndex < str.IndexOf(',', startIndex)))
					{
						endIndex = str.IndexOf(',', IndexOfPairedSymbol(str, parenthesisIndex, '(', ')') + 1);
					}
					else
						endIndex = str.IndexOf(',', startIndex);

					if (endIndex < 0)
						endIndex = str.Length;
					str = str.Remove(startIndex, endIndex - startIndex);
				}

				str = str.Replace('\t', ' ');
				str = str.Replace('\n', ' ');

				// удаление лишних пробелов
				while (str.IndexOf("  ") != -1)
					str = str.Replace("  ", " ");

				while (str.IndexOf('[') != -1)
				{
					int startIndex = str.IndexOf('[');
					subdata += str.Substring(startIndex, str.IndexOf(']') - startIndex + 1);
					str = str.Remove(startIndex, str.IndexOf(']') - startIndex + 1);
				}

				string[] fields = str.Split(',');

				for (int i = 0; i < fields.Length; i++)
					fields[i] = fields[i].Trim();

				string[] firstField = fields[0].Split(' ');

				ChangeView(firstField);
				ChangeView(fields);

				if (firstField.Length < 2)
					return;

				for (int i = 0; i < firstField.Length - 2; i++)
					typeName += firstField[i] + " ";
				typeName += firstField[firstField.Length - 2];

				while (typeName.IndexOf(" &") != -1)
					typeName = typeName.Replace(" &", "&");
				while (typeName.IndexOf(" *") != -1)
					typeName = typeName.Replace(" *", "*");
				while (typeName.IndexOf("  ") != -1)
					typeName = typeName.Replace("  ", " ");

				if (subdata != "")
					typeName += subdata;

				classFields.Add(new Field(accessMod, isStatic, typeName, firstField[firstField.Length - 1]));
				for (int i = 1; i < fields.Length; i++)
					classFields.Add(new Field(accessMod, isStatic, typeName, fields[i]));

				void ChangeView(string[] s)
				{
					for (int i = 0; i < s.Length; i++)
					{
						s[i] = s[i].Replace("└", "[");
						s[i] = s[i].Replace("┘", "]");
						s[i] = s[i].Replace("|", ", ");
					}
				}
			}
			private void GetMethod(string str, string accessMod, List<Method> methods, string realisation = "")
			{
				// замена ',' в <>, для предсказуемой работы сплита в дальнейшем
				for (int k = 0; k < str.Length; k++)
				{
					if (str[k] == '<')
					{
						int arrowCount = 0;
						for (int i = str.IndexOf("<"); i < str.Length; i++)
						{
							if (str[i] == ',')
							{
								str = str.Remove(i, 1);
								str = str.Insert(i, "|");
							}
							if (str[i] == '<')
								arrowCount++;
							if (str[i] == '>')
								arrowCount--;
							if (arrowCount == 0)
							{
								k = i;
								break;
							}
						}
					}
				}

				str = str.Replace('\t', ' ');
				str = str.Replace('\n', ' ');
				while (str.IndexOf("  ") != -1)
					str = str.Replace("  ", " ");

				string[] arguments = null;

				bool isAbstract = false;

				int firstSymbol = str.IndexOf("(");
				int braceCount = 0;

				for (int i = firstSymbol; i < str.Length; i++)
				{
					if (str[i] == '(')
						braceCount++;

					if (str[i] == ')')
						braceCount--;

					if (braceCount == 0)
					{
						int length = i - firstSymbol - 1;
						arguments = str.Substring(firstSymbol + 1, length).Split(',');
						str = str.Remove(firstSymbol, length + 2);
						break;
					}
				}
				str = str.Trim();

				if (str.IndexOf("= 0") != -1)
				{
					str = str.Replace("= 0", "");
					isAbstract = true;
				}

				List<Field> args = new List<Field>();

				foreach (string a in arguments)
					GetField(a, "", args);

				List<string> words = str.Split(' ').ToList();
				for (int i = 0; i < words.Count; i++)
					words[i] = words[i].Trim();

				words.Remove("virtual");
				words.Remove("override");
				words.Remove("constexpr");
				words.Remove("noexcept");

				if (words.IndexOf("delete") != -1 || words.IndexOf("default") != -1 || words.IndexOf("operator") != -1 || words.Count < 2)
					return;

				foreach (var a in words)
					if (a.Length > 0 && a[0] == '~')
						return;

				if (words[words.Count - 1] == "const")
					words.RemoveAt(words.Count - 1);

				bool isStatic = false;
				if (words.Remove("static"))
					isStatic = true;

				for (int i = 0; i < words.Count; i++)
				{
					words[i] = words[i].Replace(" ", "");
					words[i] = words[i].Replace("└", "[");
					words[i] = words[i].Replace("┘", "]");
					words[i] = words[i].Replace("|", ",");
				}

				string name = words[words.Count - 1];
				string returnType = "";

				for (int i = 0; i < words.Count - 2; i++)
					returnType += words[i] + " ";
				returnType += words[words.Count - 2];

				while (returnType.IndexOf(" &") != -1)
					returnType = returnType.Replace(" &", "&");
				while (returnType.IndexOf(" *") != -1)
					returnType = returnType.Replace(" *", "*");
				while (returnType.IndexOf("  ") != -1)
					returnType = returnType.Replace("  ", " ");

				methods.Add(new Method(accessMod, returnType.Trim(), name.Trim(), args, realisation, isStatic, isAbstract));
			}
			private void GetFieldsAndMethods(string str, List<Field> classFields, List<Method> classMethods, string accessMod)
			{
				bool isField = true;
				int startIndex = str.IndexOf(':') + 1;

				while (str.IndexOf(';') > 0 || str.IndexOf('(') > 0)
				{
					int semicolonIndex = str.IndexOf(';');
					int parenthesisIndex = str.IndexOf('(');

					if ((semicolonIndex < parenthesisIndex && semicolonIndex > 0) || (semicolonIndex > 0 && parenthesisIndex < 0))
					{
						isField = true;
					}
					else if ((parenthesisIndex < semicolonIndex && parenthesisIndex > 0) || (parenthesisIndex > 0 && semicolonIndex < 0))
					{
						int firstBraceIndex = str.IndexOf("{", parenthesisIndex);
						int firstSemicolonIndex = str.IndexOf(";", parenthesisIndex);

						if (firstBraceIndex < firstSemicolonIndex && firstBraceIndex > 0 || firstSemicolonIndex < 0)
						{
							isField = false;
						}
						else
						{
							if (str.IndexOf('=') < parenthesisIndex && str.IndexOf('=') > 0 && (str.IndexOf("operator") == -1 || str.IndexOf("operator") > parenthesisIndex))
							{
								isField = true;
							}
							else if (str.IndexOf('(') > str.IndexOf('<') && str.IndexOf('<') > 0)
							{
								isField = true;
								parenthesisIndex = str.IndexOf('(', IndexOfPairedSymbol(str, str.IndexOf('<'), '<', '>'));
								if (parenthesisIndex > 0 && parenthesisIndex < str.IndexOf(';'))
									isField = false;
							}
							else
							{
								isField = false;
							}
						}
					}

					if (isField)
					{
						string field = str.Substring(startIndex, semicolonIndex - startIndex);
						str = str.Remove(startIndex, semicolonIndex - startIndex + 1);
						GetField(field, accessMod, classFields);
					}
					else
					{
						int angularIndex = str.IndexOf('<', startIndex);
						if (angularIndex > 0 && angularIndex < str.IndexOf('('))
						{
							int templateIndex = IndexOfPairedSymbol(str, angularIndex, '<', '>');
							if (templateIndex > 0 && templateIndex < str.IndexOf('('))
								parenthesisIndex = templateIndex;
						}
						string method, realisation;
						int braceIndex;
						int methodEndIndex = IndexOfPairedSymbol(str, parenthesisIndex, '(', ')') + 1;
						
						method = str.Substring(startIndex, methodEndIndex - startIndex);
						
						braceIndex = str.IndexOf("{", methodEndIndex);
						int firstColonIndex = str.IndexOf(":", methodEndIndex);
						while (firstColonIndex > 0 && firstColonIndex < braceIndex)
						{

							int lastColonIndex = IndexOfPairedSymbol(str, firstColonIndex, '(', ')');
							str = str.Remove(firstColonIndex, lastColonIndex - firstColonIndex);
							braceIndex = str.IndexOf("{", methodEndIndex);
							firstColonIndex = str.IndexOf(":", methodEndIndex);
						}

						semicolonIndex = str.IndexOf(';', methodEndIndex);

						if ((semicolonIndex > 0 && braceIndex > 0 && semicolonIndex < braceIndex) || braceIndex < 0)
						{
							str = str.Remove(startIndex, semicolonIndex + 1 - startIndex);
							GetMethod(method, accessMod, classMethods);
						}
						else
						{
							int lastBraceIndex = IndexOfPairedSymbol(str, braceIndex, '{', '}');
							realisation = str.Substring(braceIndex, lastBraceIndex - braceIndex + 1);
							str = str.Remove(startIndex, lastBraceIndex - startIndex + 1);
							GetMethod(method, accessMod, classMethods, realisation);
						}
					}
				}
			}

			private ClassInfo GetClassInfo(string str, string announcedIn, string type)
			{
				List<Method> classMethods = new List<Method>();
				List<Field> classFields = new List<Field>();

				string name = GetObjectName(str, type).Trim();
				string father = GetDadInfo(ref str, name);

				SplitIntoObjects(ref str, type.Length, name);

				string[] accessMods = { "private", "protected", "public" };
				string mod = "";

				while (IndexOfKeyWord(str, accessMods[0]) != -1 || IndexOfKeyWord(str, accessMods[1]) != -1 || IndexOfKeyWord(str, accessMods[2]) != -1)
				{
					int firstSymbol = str.Length - 1;
					int[] firstAccessModifier = { IndexOfKeyWord(str, "private"), IndexOfKeyWord(str, "protected"), IndexOfKeyWord(str, "public") };

					for (int i = 0; i < 3; i++)
					{
						if (firstAccessModifier[i] < firstSymbol && firstAccessModifier[i] > 0 && IsAccessModifier(str, firstAccessModifier[i]))
						{
							mod = accessMods[i];
							firstSymbol = firstAccessModifier[i];
						}
					}

					int lastSymbol = str.Length - 1;
					int[] lastAccessModifier = { IndexOfKeyWord(str, "private", firstSymbol + 1), IndexOfKeyWord(str, "protected", firstSymbol + 1), IndexOfKeyWord(str, "public", firstSymbol + 1) };

					for (int i = 0; i < 3; i++)
					{
						if (lastAccessModifier[i] < lastSymbol && lastAccessModifier[i] > 0 && IsAccessModifier(str, lastAccessModifier[i]))
							lastSymbol = lastAccessModifier[i];
					}

					GetFieldsAndMethods(str.Substring(firstSymbol, lastSymbol - firstSymbol), classFields, classMethods, mod);
					str = str.Remove(firstSymbol, lastSymbol - firstSymbol);
				}

				GetFieldsAndMethods(str.Substring(str.IndexOf('{') + 1, str.Length - 2 - str.IndexOf('{')), classFields, classMethods, (type == "class" ? "private" : "public"));

				return new ClassInfo(name, father, announcedIn, classFields, classMethods);
			}
			private EnumInfo GetEnumInfo(string str, string announcedIn)
			{
				int classIndex = IndexOfKeyWord(str, "class");
				if (classIndex > 0)
					str = str.Remove(classIndex, 5);

				string name = GetObjectName(str, "enum").Trim();
				
				str = str.Substring(str.IndexOf('{') + 1, str.Length - 2 - str.IndexOf('{'));

				int firstIndex;
				while ((firstIndex = str.IndexOf('=')) != -1)
				{
					int commaIndex = str.IndexOf(',', firstIndex);
					int lastIndex = (commaIndex > 0 ? commaIndex : str.Length - 1);
					str = str.Remove(firstIndex, lastIndex - firstIndex);
				}

				string[] enumerations = str.Split(',');
				for (int i = 0; i < enumerations.Length; i++)
					enumerations[i] = " " + enumerations[i].Trim();

				return new EnumInfo(name, announcedIn, enumerations);
			}
			public List<ObjectInfo> GetObjectsInfo()
			{
				int start;
				for (int i = 0; i < programs.Count; i++)
				{
					start = objectsInfo.Count;
					SplitIntoObjects(ref programs.ToArray()[i]);

					for (; start < objects.Count; start++)
					{
						switch (objects[start].type)
						{
							case "enum":
								objectsInfo.Add(GetEnumInfo(objects[start].realisation, objects[start].announcedIn));
								break;
							case "union": break;
							default:
								objectsInfo.Add(GetClassInfo(objects[start].realisation, objects[start].announcedIn, objects[start].type));
								break;
						}
					}
				}
				return objectsInfo;
			}
		}
	}
}
