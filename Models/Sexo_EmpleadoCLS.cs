using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;

namespace EjercicioMVC.Models
{
    public class Sexo_EmpleadoCLS
    {
     
        [Display(Name = "Sexo")]
        [Required]
        public int ID_SEXO { get; set; }


        public string SEXO { get; set; }
        public Nullable<byte> HABILITADO { get; set; }
    }
}