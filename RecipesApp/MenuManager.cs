﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Library;
using System.Linq;

namespace RecipesApp
{
    public class MenuManager
    {
        private delegate void PlayScript();
        private static int Menu(string header, int menuWidth, PlayScript script, params string[] options)
        {
            Console.Clear();
            int chosen = 0;
            bool scriptisPlayed = false;
            while (true)
            {
                Console.SetCursorPosition(0, 0);
                DrawHeader(header, menuWidth);
                ConsoleShef.ChangeMood("regular");

                foreach (var option in options)
                {
                    if (option == options[chosen])
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(TableBuilder.AlignCentre(option, menuWidth));
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write(TableBuilder.AlignCentre(option, menuWidth));
                    }
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                }
                if (!scriptisPlayed)
                {
                    script();
                    scriptisPlayed = true;
                }

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Enter:
                        Console.Clear();
                        return chosen;
                    case ConsoleKey.UpArrow:
                        if (chosen == 0) chosen = options.Length - 1;
                        else chosen--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (chosen == options.Length - 1) chosen = 0;
                        else chosen++;
                        break;
                    case ConsoleKey.Escape: return -1;
                }
            }
        }
        public static void DrawHeader(string header,int width)
        {
            header = header.ToUpper();          
            Console.WriteLine("".PadLeft(width,'='));
            Console.WriteLine("|" + TableBuilder.AlignCentre(header, width - 2) + "|");
            Console.WriteLine("".PadLeft(width, '='));
        }
        public static void MainMenu()
        {
            while (true)
            {
                var chosenOption = Menu("main menu", 37, MainMenuIntro, new string[] { "Search Recipes", "Add Recipe", "Delete Recipe", "Edit Receipe", "Turn Hints Off", "Exit" });
                switch (chosenOption)
                {
                    case 0:
                        ViewRecipe();
                        break;
                    case 1:
                        AddRecipe();
                        break;
                    case 2:
                        DeleteRecipe();
                        break;
                    case 3:
                        EditRecipe();
                        break;
                    case 4:
                        ConsoleShef.Disactivate();
                        break;
                    case 5:
                    case -1:
                        return;
                }
            }
        }

        private static void EditRecipe()
        {
            Console.Clear();
            var chosenRecipe = SearchRecipes("Edit Recipe");
            if (chosenRecipe == null) return;

            Console.Clear();
            int option = Menu("Edit Recipe", 73, BlankScript, "Change Name", "Change duration", "Change Calories Quantity",
                "Change Ingredients List");
            switch (option)
            {
                case -1: return;
                //case 0:
            }
        }

        private static Recipe SearchRecipes(string header = "Search Recipes")
        {
            Console.Clear();
            DrawHeader(header,73);
            Console.WriteLine("Please, enter name of recipe or name of ingredient:");
            var filter = Console.ReadLine().Trim().ToLower();
            Console.Write("Searching");
            for (int i = 0; i < 3; i++)
            {
                System.Threading.Thread.Sleep(500);
                Console.Write(".");
            }
            Console.WriteLine();
            var recipesList = Recipe.Recipes.Where(r => r.Name.ToLower().Contains(filter)
            || r.Ingredients.Select(i => i.Name.ToLower()).Contains(filter)).ToList();
            
            if(recipesList.Count == 0)
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine("    No matches found!");
                Console.WriteLine("-------------------------");
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
                return null;
            }
            recipesList.OrderBy(e => e.Name).OrderBy(e => e.Proteins + e.Fats + e.Carbonohydrates);
            var recipesArray = new string[recipesList.Count];
            for (int i = 0; i < recipesList.Count; i++)
            {
                recipesArray[i] = recipesList[i].ToString();
            }
            int chosenRecipe = Menu("Search results", 73, BlankScript, recipesArray);
            if (chosenRecipe == -1) return null;
            var recipe = recipesList[chosenRecipe];
            return recipe;
            
        }

        private static void ViewRecipe()
        {
            Console.Clear();
            SearchRecipes().ShowInfo();
            Console.WriteLine("-------------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        private static void AddRecipe()
        {
            Console.Clear();
            DrawHeader("add recipe", 73);
            Console.WriteLine("Please, enter your recipie.");
            Console.WriteLine("If you don't know some parameters just left fields empty and press ENTER.\n");
            Console.WriteLine("Enter name of your recipe: ");
            var name = Console.ReadLine().Trim();
            Console.WriteLine("Enter time of coocking(in minutes):");
            var duration = (int)GetNumb();
           
            Console.WriteLine("Now, Please, fill in ingredients list.");
            Console.WriteLine("To proceed press ENTER, or press ESC to go back to the Main menu");
            var key = Console.ReadKey().Key;
            while (key != ConsoleKey.Enter && key != ConsoleKey.Escape)
            {
                key = Console.ReadKey().Key;
            }
            if (key == ConsoleKey.Escape) return;

            List<Ingredient> ingredients = new List<Ingredient>(5);
            List<double> quantities = new List<double>(5);

            DrawHeader("Filling ingredients list", 73);
            for (int i = 1;; i++)
            {           
                Console.WriteLine("To stop filling list, enter \'q\' in the Name Field");
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine($"Enter ingredient {i}:");
                Console.WriteLine("Enter Name: ");
                var iName = Console.ReadLine().Trim();
                if (iName == "q") break;
                string unit;

                Console.WriteLine("Please, choose ingredient unit(enter digit): ");
                DrawHeader("Available units", 37);
                for (int j = 0; j < Ingredient._units.Length; j++)
                {
                    Console.WriteLine($" {j + 1}) {Ingredient._units[j].PadLeft(20)}");
                }
                var answer = Console.ReadLine();
                int chosenUnit;
                while (!int.TryParse(answer,out chosenUnit) || (chosenUnit < 1 || chosenUnit > Ingredient._units.Length))
                {
                    Console.WriteLine("wrong option! Try again");
                    answer = Console.ReadLine();
                }
                unit = Ingredient._units[chosenUnit - 1];

                Console.WriteLine("Enter Quantity :");
                quantities.Add(GetNumb());
                ingredients.Add(new Ingredient(iName, unit));
            }
            Console.WriteLine("===========================================================================");
            Console.WriteLine("Enter Receipt Proteins: ");
            var proteins = GetNumb();
            Console.WriteLine("Enter Receipt Fats: ");
            var fats = GetNumb();
            Console.WriteLine("Enter Receipt Carbonohydrates: ");
            var carbonohydrates = GetNumb();

            new Recipe(name, duration, proteins, fats, carbonohydrates, ingredients, quantities);
            Console.WriteLine("Recipe was successfully added!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        private static void DeleteRecipe()
        {
            var chosenRecipe = SearchRecipes("Delete Recipe");
            if (chosenRecipe == null) return;
            Console.WriteLine("Enter CONFIRM to delete recipe");
            if (Console.ReadLine().ToLower() == "confirm") chosenRecipe.Delete();
        }
        private static double GetNumb()
        {
            double number;
            var str = Console.ReadLine().Trim();
            while((!double.TryParse(str,out number) || number < 0) && !string.IsNullOrEmpty(str))
            {
                Console.WriteLine("Wrong input! Must be a positive double!");
                str = Console.ReadLine().Trim();
            }
            if (string.IsNullOrEmpty(str)) return default(double);
            return number;
        }
        // ConsoleShef Scenarios
        private static void BlankScript()
        {

        }       
        private static void MainMenuIntro()
        {
            ConsoleShef.ChangeMood("regular");
            ConsoleShef.Say("Hello!");
            ConsoleShef.Say("this is main menu");
        }
    }
}
