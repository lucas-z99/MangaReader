using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;


namespace WPFUI
{
     internal class Parser
     {

          public static readonly string idConnecter = " #| ";
          //(C999)[BETEMIUS (バシウス)] あなたのヤミ鎮守府 (艦隊これくしょん -艦これ-) [中国翻訳]
          static string pattern = @"\(([^)]*)\)|\[(.*?)\]|([^\[\]()]+)";
          static string patternArtistGroup = @"\(([^)]*)\)";
          static AppState appState { get => Admin.appState; }


          public static bool CheckError(string text)
          {
               // check all brackets are closed
               int b = 0;
               int s = 0;

               for (int i = 0; i < text.Length; i++)
               {
                    if (text[i] == '(')
                         b++;
                    else if (text[i] == ')')
                         b--;
                    else if (text[i] == '[')
                         s++;
                    else if (text[i] == ']')
                         s--;

                    if (b < 0 || s < 0)
                         return true;
               }

               return false;
          }

          public static MangaEntry Parse(string folder)
          {
               if (CheckError(folder))
               {
                    Console.WriteLine("Unbalanced found!");
                    return null;
               }

               // parse
               var matches = Regex.Matches(folder, pattern);
               int appearOrder = 0;

               var manga = new MangaEntry();
               manga.folder = folder;

               foreach (Match match in matches)
                    for (int i = 1; i <= match.Groups.Count; i++)
                         if (string.IsNullOrWhiteSpace(match.Groups[i].Value))
                              continue;
                         else if (match.Groups[i].Success)
                         {
                              // 1 = () 
                              // 2 = []
                              // 3 = not in brackets
                              var t = match.Groups[i].Value.Trim();

                              if (i == 1)
                              {
                                   if (appearOrder == 0)
                                   {
                                        manga.comiket = t;
                                   }
                                   else
                                   {
                                        manga.parody = t;
                                   }
                              }
                              else if (i == 2)
                              {
                                   // artist always appear before title
                                   if (manga.artist == null && manga.title == null)
                                   {
                                        var ag = ParseArtistGroup(t);
                                        manga.artist = ag.Item1;
                                        manga.group = ag.Item2;
                                   }
                                   else
                                   {
                                        if (manga.tags == null)
                                             manga.tags = new List<string>();
                                        manga.tags.Add(t);
                                   }
                              }
                              else if (i == 3)
                              {
                                   manga.title = t;
                              }

                              appearOrder++;
                              break;
                         }

               if (manga.title == null)
                    return null;

               return manga;
          }


          public static (string, string) ParseArtistGroup(string artistGroup)
          {
               var match = Regex.Match(artistGroup, patternArtistGroup);
               if (match.Success)
               {
                    var a = match.Groups[1].Value.Trim();
                    var g = Regex.Replace(artistGroup, patternArtistGroup, "").Trim();

                    return (a, g);
               }
               else
               {
                    return (artistGroup, null);
               }
          }

          public static string GetCurID()
          {
               if (!Directory.Exists(appState.dir))
                    return "";

               var folder_name = new DirectoryInfo(appState.dir).Name;

               var manga = Parse(folder_name);
               if (manga == null)
                    return null;

               return manga.GetIdString();
          }

          public static string RemoveComiket(string origin)
          {
               //string pattern = @"^\s*\([^)]*\)\s*";
               //return Regex.Replace(origin.Trim(), pattern, "").Trim();


               string pattern = @"^\s*\([^)]*\)\s*";
               string result = Regex.Replace(origin, pattern, "").Trim();
               return result;
          }



     }

     public class MangaEntry
     {

          public string folder; //(C999)[BETEMIUS (バシウス)] あなたのヤミ鎮守府 (艦隊これくしょん -艦これ-) [中国翻訳]
          public string comiket; // C999
          public string artist; // バシウス
          public string group; // BETEMIUS
          public string title; // あなたのヤミ鎮守府
          public string parody; // 艦隊これくしょん -艦これ-
          public List<string> tags; //[中国翻訳], [DL版], [Digital],［カラー版, [AI Generated], [完結]

          public string GetIdString()
          {
               return artist + Parser.idConnecter + title;
          }

          public override string ToString()
          {
               string text = "folder = " + folder + "\n";
               text += "comiket = " + comiket + "\n";
               text += "artist = " + artist + "\n";
               text += "artistGroup = " + group + "\n";
               text += "title = " + title + "\n";
               text += "parody = " + parody + "\n";

               if (tags != null)
               {
                    text += "tag = ";
                    foreach (string tag in tags)
                         text += tag + ", ";
               }

               return text;
          }

     }


     //[Serializable]
     //public class IDString
     //{
     //     public string id { get; private set; }

     //     public IDString(string _id)
     //     {
     //          id = _id;
     //     }

     //     // overload ==
     //     public static bool operator ==(IDString a, IDString b) => a.id == b.id;
     //     public static bool operator !=(IDString a, IDString b) => a.id != b.id;
     //     public override int GetHashCode() => id.GetHashCode();
     //     public override bool Equals(object obj)
     //     {
     //          if (obj == null || this.GetType() != obj.GetType())
     //               return false;

     //          return id == ((IDString)obj).id;
     //     }

     //     public override string ToString()
     //     {
     //          return id;
     //     }
     //}


}
