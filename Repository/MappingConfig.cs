using AutoMapper;
using BusinessObject.DTO.User;
using BusinessObject.DTO;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO.SubscriptionPlan;
using BusinessObject.DTO.UserSubscription;

namespace Repository
{
    public class MappingConfig : Profile
    {
        public MappingConfig() {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UpdateUserDto, User>().ReverseMap()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<SubscriptionPlan, SubscriptionPlanDto>().ReverseMap();
            CreateMap<CreateSubscriptionPlanDto, SubscriptionPlan>().ReverseMap();
            CreateMap<UpdateSubscriptionPlanDto, SubscriptionPlan>().ReverseMap();
            CreateMap<UserSubscription, UserSubscriptionDto>().ReverseMap();
            CreateMap<UserSubscription, CreateUserSubscriptionDto>().ReverseMap();
        }
    }
}
