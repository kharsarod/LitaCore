using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Gameplay.Script.ScriptAttributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ScriptAttribute : Attribute
    {
        public string Name { get; set; }

        public ScriptAttribute(string name)
        {
            Name = name;
        }
    }
}
