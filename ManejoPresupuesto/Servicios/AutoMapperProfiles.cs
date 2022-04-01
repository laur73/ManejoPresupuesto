using AutoMapper;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //De que tipo de dato a que tipo de dato vamos a mapear
            //Para este caso mapeamos desde CuentaViewModel a CuentaCreacionViewModel
            CreateMap<CuentaViewModel, CuentaCreacionViewModel>();
        }
    }
}
