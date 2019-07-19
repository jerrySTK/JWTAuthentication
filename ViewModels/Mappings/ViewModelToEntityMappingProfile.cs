using AutoMapper;
using NG_Core_Auth.Models.Entities;

namespace NG_Core_Auth.ViewModels.Mappings
{
    public class ViewModelToEntityMappingProfile:Profile
    {
        public ViewModelToEntityMappingProfile()
        {
            CreateMap<RegistrationViewModel, AppUser>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));
        }
    }
}