using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPFUI.Resources
{
     internal class AsyncTEST
     {

          internal class Bacon { }
          internal class Coffee { }
          internal class Egg { }
          internal class Juice { }
          internal class Toast { }


          public static void TEST()
          {
               CancelTest();
          }

          public static void TEST2()
          {
               //        <Image x:Name="gif_test" gif:AnimationBehavior.SourceUri="Resources/bg.jpg" Stretch="UniformToFill" HorizontalAlignment="Left" VerticalAlignment="Top" Canvas.Left="-540" Canvas.Top="-360" Height="595" Width="1102" Margin="344,176,0,0"/>

               //copyFUN();
               isCancel = true;
          }

          static async Task Wait_Or_Not_Wait()
          {
               Console.WriteLine(">>START");

               Task.Delay(2000);
               Console.WriteLine("1) this will not wait");

               await Task.Delay(2000);
               Console.WriteLine("2) this will wait");

               await Task.Delay(2000);
               Console.WriteLine("2) this will wait");

               Console.WriteLine(">>END");
          }

          static async Task StartTogether()
          {
               //start together, but finish differently
               Console.WriteLine(">>START");
               Get_A_String(1, false, 9999);
               Get_A_String(2, true, 1500);
               Get_A_String(3, true, 2001);
               Get_A_String(4, false, 9999);
               Get_A_String(5, false, 9999);
               Get_A_String(6, true, 2000);
               Console.WriteLine(">>END");
          }

          static async Task<string> Get_A_String(int index, bool wait, int t)
          {
               if (wait)
                    await Task.Delay(t);
               else
                    Task.Delay(t);
               return "THIS IS A CALL " + index + " t=" + t;
          }

          static async Task CancelTest()
          {
               //start together, handle when each one finish
               Console.WriteLine(">>START");


               for (int i = 0; i < 1000; i++)
               {
                    await Task.Delay(100);
                    Console.WriteLine(i + " complete");


                    if (isCancel)
                    {
                         Console.WriteLine(i + ">>> load cancel");
                         break;
                    }
               }

               Console.WriteLine(">>END");
          }

          static bool isCancel = false;


          static void copyFUN()
          {
               var taskList = new[] {
                    Task.Delay(3000).ContinueWith(_ => 3),
                    Task.Delay(1000).ContinueWith(_ => 1),
                    Task.Delay(2000).ContinueWith(_ => 2),
                    Task.Delay(5000).ContinueWith(_ => 5),
                    Task.Delay(4000).ContinueWith(_ => 4),
               };

               Console.WriteLine(">>1");

               foreach (var t in taskList)
               {
                    Console.WriteLine(">>2");
                    t.ContinueWith(task =>
                    {
                         if (task.Status == TaskStatus.RanToCompletion)
                              Console.WriteLine(task.Result);

                    });
               }

               Console.WriteLine(">>999");

          }




          // ------------------------------------- coffee test -------------------------------------

          public static void CoffeeTEST()
          {
               Console.WriteLine(">>> START");
               MakeBreakFast();
               Console.WriteLine(">>> I'm not awaiting!!!");
          }

          public static async void MakeBreakFast()
          {
               Coffee cup = PourCoffee();
               Console.WriteLine("coffee is ready");

               var eggsTask = FryEggsAsync(2);
               var baconTask = FryBaconAsync(3);
               var toastTask = MakeToastWithButterAndJamAsync(2);

               var breakfastTasks = new List<Task> { eggsTask, baconTask, toastTask };
               while (breakfastTasks.Count > 0)
               {
                    Task finishedTask = await Task.WhenAny(breakfastTasks);
                    if (finishedTask == eggsTask)
                    {
                         Console.WriteLine("eggs are ready");
                    }
                    else if (finishedTask == baconTask)
                    {
                         Console.WriteLine("bacon is ready");
                    }
                    else if (finishedTask == toastTask)
                    {
                         Console.WriteLine("toast is ready");
                    }
                    breakfastTasks.Remove(finishedTask);
               }

               Juice oj = PourOJ();
               Console.WriteLine("oj is ready");
               Console.WriteLine("Breakfast is ready!");
          }

          static async Task<Toast> MakeToastWithButterAndJamAsync(int number)
          {
               var toast = await ToastBreadAsync(number);
               ApplyButter(toast);
               ApplyJam(toast);

               return toast;
          }

          static Juice PourOJ()
          {
               Console.WriteLine("Pouring orange juice");
               return new Juice();
          }

          static void ApplyJam(Toast toast) =>
             Console.WriteLine("Putting jam on the toast");

          static void ApplyButter(Toast toast) =>
             Console.WriteLine("Putting butter on the toast");

          static async Task<Toast> ToastBreadAsync(int slices)
          {
               for (int slice = 0; slice < slices; slice++)
               {
                    Console.WriteLine("Putting a slice of bread in the toaster");
               }
               Console.WriteLine("Start toasting...");
               await Task.Delay(3000);
               Console.WriteLine("Remove toast from toaster");

               return new Toast();
          }

          static async Task<Bacon> FryBaconAsync(int slices)
          {
               Console.WriteLine($"putting {slices} slices of bacon in the pan");
               Console.WriteLine("cooking first side of bacon...");
               await Task.Delay(3000);
               for (int slice = 0; slice < slices; slice++)
               {
                    Console.WriteLine("flipping a slice of bacon");
               }
               Console.WriteLine("cooking the second side of bacon...");
               await Task.Delay(3000);
               Console.WriteLine("Put bacon on plate");

               return new Bacon();
          }

          static async Task<Egg> FryEggsAsync(int howMany)
          {
               Console.WriteLine("Warming the egg pan...");
               await Task.Delay(3000);
               Console.WriteLine($"cracking {howMany} eggs");
               Console.WriteLine("cooking the eggs ...");
               await Task.Delay(3000);
               Console.WriteLine("Put eggs on plate");

               return new Egg();
          }

          static Coffee PourCoffee()
          {
               Console.WriteLine("Pouring coffee");
               return new Coffee();
          }


     }
}
