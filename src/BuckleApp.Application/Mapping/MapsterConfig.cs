using BuckleApp.API.Requests.Auth;
using BuckleApp.Application.Dtos;
using BuckleApp.Application.Features.Appointments.GetAppointmentById;
using BuckleApp.Application.Features.Appointments.GetUpcomingForUserByDate;
using BuckleApp.Application.Features.Authentication.RegisterUser;
using BuckleApp.Application.Features.Users.GetUsersByOrgAndRole;
using BuckleApp.Core.Entities;
using Mapster;

namespace BuckleApp.Application.Mapping;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        #region Auth

        TypeAdapterConfig<RegisterUserRequest, User>.NewConfig()
            .Map(dest => dest.UserName, src => src.Email);

        #endregion
        
        #region User

        TypeAdapterConfig<User, GetUsersByOrgAndRoleResponse>.NewConfig()
            .Map(dest => dest.Name, src => $"{src.FirstName} {src.LastName}");

        // CreateMap<User, GetUsersByOrgAndRoleResponse>()
        //     .ForMember(x => x.Name, opt => 
        //         opt.MapFrom(y => $"{y.FirstName} {y.LastName}"));
        
        #endregion
        
        #region Appointment

        // CreateMap<Appointment, GetAppointmentInfoDto>()
        //     .ForMember(x => x.Date, opt =>
        //         opt.MapFrom(y => y.StartTime.ToString("dd.MM.yyyy")))
        //     .ForMember(x => x.Day, opt =>
        //         opt.MapFrom(y => y.StartTime.Date.ToString("dddd")))
        //     .ForMember(x => x.Time, opt =>
        //         opt.MapFrom(a => String.Concat(a.StartTime.ToString("HH:mm"), " - ", a.EndTime.ToString("HH:mm"))))
        //     .ForMember(x => x.UserCanceled, opt =>
        //         opt.MapFrom(a =>
        //             a.UserAppointments.FirstOrDefault().UserCanceled.FirstName + " " +
        //             a.UserAppointments.FirstOrDefault().UserCanceled.LastName))
        //     .ForMember(x => x.Attendee, opt =>
        //         opt.MapFrom(y =>
        //             y.UserAppointments.FirstOrDefault().User.FirstName + " " +
        //             y.UserAppointments.FirstOrDefault().User.FirstName))
        //     .ForMember(x => x.Type, opt =>
        //         opt.MapFrom(a => (int)a.Type));
        
        TypeAdapterConfig<Appointment, GetAppointmentInfoDto>.NewConfig()
            .Map(dest => dest.Date, src => src.StartTime.ToString("dd.MM.yyyy"))
            .Map(dest => dest.Day, src => src.StartTime.ToString("dddd"))
            .Map(dest => dest.Time, src => $"{src.StartTime:HH:mm} - {src.EndTime:HH:mm}")
            .Map(dest => dest.UserCanceled, src => 
                src.UserAppointments.FirstOrDefault() != null && src.UserAppointments.FirstOrDefault().UserCanceled != null
                    ? src.UserAppointments.FirstOrDefault().UserCanceled.FirstName + " " + src.UserAppointments.FirstOrDefault().UserCanceled.LastName
                    : string.Empty)
            .Map(dest => dest.Attendee, src => 
                src.UserAppointments.FirstOrDefault() != null && src.UserAppointments.FirstOrDefault().User != null
                    ? src.UserAppointments.FirstOrDefault().User.FirstName + " " + src.UserAppointments.FirstOrDefault().User.LastName
                    : string.Empty)
            .Map(dest => dest.Type, src => (int)src.Type);

        TypeAdapterConfig<Appointment, UpcomingAppointmentDto>.NewConfig()
            .Map(dest => dest.OrganiserPhone, src => src.User.PhoneNumber)
            .Map(dest => dest.AttendeePhone, src => src.UserAppointments.Select(a => a.User.PhoneNumber));


        //GetById
        // CreateMap<Appointment, GetAppointmentByIdResponse>()
        //     .ForMember(x => x.Date, opt =>
        //         opt.MapFrom(y => y.StartTime.ToString("dd.MM.yyyy")))
        //     .ForMember(x => x.StartTime, opt =>
        //         opt.MapFrom(a => a.StartTime.ToString("HH:mm")))
        //     .ForMember(x => x.EndTime, opt =>
        //         opt.MapFrom(a => a.EndTime.ToString("HH:mm")))
        //     .ForMember(x => x.Attendee, opt =>
        //         opt.MapFrom(y => 
        //             y.UserAppointments.FirstOrDefault().User.FirstName + " " + y.UserAppointments.FirstOrDefault().User.FirstName));
        
        TypeAdapterConfig<Appointment, GetAppointmentByIdResponse>.NewConfig()
            .Map(dest => dest.Date, src => src.StartTime.ToString("dd.MM.yyyy"))
            .Map(dest => dest.StartTime, src => src.StartTime.ToString("HH:mm"))
            .Map(dest => dest.EndTime, src => src.EndTime.ToString("HH:mm"))
            .Map(dest => dest.Attendee, src =>
                src.UserAppointments.FirstOrDefault() != null && src.UserAppointments.FirstOrDefault().User != null
                    ? src.UserAppointments.FirstOrDefault().User.FirstName + " " + src.UserAppointments.FirstOrDefault().User.LastName
                    : string.Empty);


        #endregion

    }
}