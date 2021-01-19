using Library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RecipesApp
{
    public class Ingredient
    {          
        public string Name { get; set; }        
        public string Unit { get; set; }

        [NonSerialized]
        public readonly static string[] _units = new string[]
        {
            "table spoon",
            "tea spoon",
            "handful",
            "liter",
            "milliliter",
            "gram",
            "kilo",
            "piece",
            "not mentioned"
        };

        public Ingredient() { }
        public Ingredient(string name, string unit)
        {
            Name = name;
            Unit = unit;
        }
        public string[] ToDataArray()
        {
            return new string[] { Name, Unit, ""};
        }
    }
}
