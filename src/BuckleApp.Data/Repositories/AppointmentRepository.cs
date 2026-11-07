using BuckleApp.Core.Entities;
using BuckleApp.Core.IRepositories;
using BuckleApp.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace BuckleApp.Data.Repositories;

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ApplicationContext context) : base(context)
    {
    }

    public async Task<Appointment> GetById(Guid id, bool includeUserAppointmnet = false)
    {
        if (includeUserAppointmnet)
        {
           return await Context.Appointments
                .Include(x => x.UserAppointments)
                .SingleOrDefaultAsync(x => x.Id == id);
        }
        else
        {
           return await GetAsync(id);
        }
    }
        

    public async Task<List<Appointment>> GetAllByOrganiserId(Guid userId) =>
        await Context.Appointments
            .Include(x => x.UserAppointments)
            .Where(x => x.UserId == userId)
            .ToListAsync();

    public async Task<List<Appointment>> GetNotCanceledByOrganiserId(Guid userId) =>
        await Context.Appointments
            .Include(x => x.UserAppointments)
            .Where(x => x.UserAppointments.Any(y => y.UserCanceled != null) && x.UserId == userId)
            .ToListAsync();

    public async Task<List<Appointment>> GetAllByOrganiserIdAndDates(Guid userId, List<DateTime> dates) =>
        await Context.Appointments
            .Include(x => x.User)
            .Include(x => x.UserAppointments)
            .ThenInclude(x => x.User)
            .Where(x => x.UserId == userId && dates.Any(y=> y.Date == x.StartTime.Date))
            .OrderBy(x => x.StartTime)
            .ToListAsync();

    public async Task<List<Appointment>> GetAllByAttendeeId(Guid userId) =>
        await Context.Appointments
            .Include(x => x.UserAppointments)
            .Where(x => x.UserAppointments.Any(a => a.UserId == userId))
            .ToListAsync();

    public async Task<List<Appointment>> GetNotCanceledByAttendeeId(Guid userId) => 
        await Context.Appointments
            .Include(x => x.UserAppointments)
            .Where(x => x.UserAppointments.Any(a => a.UserId == userId && a.UserCanceled != null))
            .ToListAsync();

    public async Task<List<Appointment>> GetUpcomingForUserByDate(Guid userId, DateTime date) =>
        await Context.Appointments
            .Include(x => x.UserAppointments)
            .Where(x => x.StartTime.Date == date.Date &&
                        x.StartTime >= DateTime.Now &&
                        x.UserAppointments.Any(a => a.UserId == userId))
            .OrderBy(x => x.StartTime)
            .ToListAsync();

    public async Task<List<Appointment>> GetCanceledForUser(Guid userId) =>
        await Context.Appointments
            .Include(x => x.UserAppointments)
            .Where(x => x.UserAppointments.Any(a => a.UserCanceled != null 
                                                    && a.UserId == userId))
            .ToListAsync();

    public async Task<List<Appointment>> GetCompletedForUser(Guid userId) =>
        await Context.Appointments
            .Include(x => x.UserAppointments)
            .Where(x =>
                x.UserAppointments.Any(a => a.UserId == userId)
                && x.EndTime < DateTime.Now)
            .ToListAsync();

    public async Task<Appointment> OverlappingAppointmentAsync(DateTime startTime, DateTime endTime, Guid userId) => 
        await Context.Appointments
            .SingleOrDefaultAsync(a => a.UserId == userId && 
                           ((startTime >= a.StartTime && startTime < a.EndTime) || 
                            (endTime > a.StartTime && endTime <= a.EndTime) || 
                            (startTime <= a.StartTime && endTime >= a.EndTime)));


        public async Task CreateAppointments (List<Appointment> appointments) =>
        await AddRange(appointments);
        
    
}