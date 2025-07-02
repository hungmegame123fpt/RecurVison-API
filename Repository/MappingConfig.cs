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
using BusinessObject.DTO.VirtualInterview;
using BusinessObject.DTO.InterviewQuestion;
using BusinessObject.DTO.CvVersion;

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
            CreateMap<VirtualInterview, VirtualInterviewDto>().ReverseMap();
            CreateMap<VirtualInterview, CreateVirtualInterviewDto>().ReverseMap();
            CreateMap<VirtualInterview, UpdateInterviewStatusDto>().ReverseMap();
            CreateMap<InterviewQuestion, InterviewQuestionDto>().ReverseMap();
            CreateMap<InterviewQuestion, CreateInterviewQuestionDto>().ReverseMap();
            CreateMap<InterviewQuestion, UpdateQuestionScoreDto>().ReverseMap();
            CreateMap<InterviewQuestion, UpdateAnswerDto>().ReverseMap();
            CreateMap<CvVersion, CvVersionDTO>().ReverseMap();
        }
    }
}
