using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EjercicioMVC.Models
{
    public class EmpleadCLS
    {
        public int IDEMPLEADO { get; set; }


        [Display(Name = "Nombre")]
        [Required]
        [StringLength(80, ErrorMessage = "El Nombre no puede superar  mas de 80 caracteres")]
        public string NOMBRE { get; set; }

        [Required]
        [StringLength(80, ErrorMessage = "El Apellido no puede superar  mas de 80 caracteres")]
        [Display(Name = "Apellido")]
        public string APELLIDO { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Contrato")]
        public Nullable<System.DateTime> FECHACONTRATO { get; set; }

        [Required]
        [Display(Name = "Sueldo")]
        public Nullable<decimal> SUELDO { get; set; }


        [Display(Name = "Tipo de Usuario")]
        public int IDTIPOUSUARIO { get; set; }

        [Display(Name = "Tipo de Contrato")]
        public int IDTIPOCONTRATO { get; set; }

        [Display(Name = "Sexo")]
        [Required]
        public int IDSEXO { get; set; }


        public Nullable<int> HABILITADO { get; set; }
        public Nullable<int> TIENEUSUARIO { get; set; }

        [Display(Name = "Usuario Tipo")]
        public string TIPOUSUARIO { get; set; }

        public virtual Sexo_Empleado Sexo_Empleado { get; set; }
    }
}