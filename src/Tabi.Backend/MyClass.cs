using System;
using System.ComponentModel.DataAnnotations;

namespace Tabi.DataObjects
{
    public class MyClass
    {
        [Key]
        public string Hello { get; set; }

        [Key]
        public string Eee { get; set; }
    }
}
