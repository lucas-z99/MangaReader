using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFUI
{

     internal class BracketsParser
     {
          static Stack<(int, int, int)> buffer = new Stack<(int, int, int)>();

          static List<int> cosunt;
          static int last_bracket_l;


          bool parse(string input, List<(char, char)> bracket_list, out List<(int, int, int)> bracketType_indexLeft_indexRight)
          {
               buffer.Clear();
               bracketType_indexLeft_indexRight = new List<(int, int, int)>();

               for (int i = 0; i < input.Length; i++)
               {
                    int left = bracket_list.FindIndex(x => x.Item1 == input[i]); // left bracket
                    if (left != -1)
                    {
                         var new_pair = (left, i, -1); // index of right bracket
                         buffer.Push(new_pair);
                         continue;
                    }

                    int right = bracket_list.FindIndex(x => x.Item2 == input[i]); // left bracket
                    if (right != -1)
                    {
                         if (buffer.Count == 0)
                              return false; // unbalanced R, eg ())

                         var pair = buffer.Pop();
                         if (pair.Item1 != right)
                              return false; // mis-match, eg (]

                         pair.Item3 = i;
                         bracketType_indexLeft_indexRight.Add(pair); // good
                    }
               }

               if (buffer.Count > 0)
                    return false; // unbalanced L, eg (()

               return true;
          }





          void example()
          {
               var list = new List<(char, char)>();// fill in with you brackets
               list.Add(('(', ')'));// comiket, parody
               list.Add(('[', ']'));// artist, other
               list.Add(('（', '）'));// full-width
          }







          void fun(string folder_name)

          {
               //(C999)[BETEMIUS (バシウス)] あなたのヤミ鎮守府 (艦隊これくしょん -艦これ-) [中国翻訳]
               string pattern = "";



               bool found_left = false;
               for (int i = 0; i < folder_name.Length; i++)
               {
                    var ch = folder_name[i];

                    if (found_left == false)
                    {
                         if (ch == bracket.Item1)
                         {
                              found_left = true;
                         }
                         else if (ch == bracket_sq.Item1)//([
                         {
                              return;//error
                         }
                    }
                    else
                    {
                         if (ch == bracket.Item2)
                         {

                         }

                         if (ch == bracket_sq.Item1 || ch == bracket_sq.Item2)
                    }


               }



               //for (int b = 0; b < braket_list.Count; b++)
               //{
               //     if (braket_list[b].Item1 == folder_name[i])
               //     {

               //     }
               //     else if (braket_list[b].Item2 == folder_name[i])
               //     {

               //     }
               //}






          }





     }


}




}
