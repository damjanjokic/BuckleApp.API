using BuckleApp.Core.IRepositories;
using BuckleApp.Core.IUnitOfWork;
using BuckleApp.Data.Data;
using BuckleApp.Data.Repositories;

namespace BuckleApp.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _applicationContext;

    private IAppointmentRepository _appointmentRepository;
    private IOrganisationRepository _organisationRepository;
    private IUserRepository _userRepository;
    
    public UnitOfWork(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }
    
    public IAppointmentRepository Appointment 
    {
        get
        {
            _appointmentRepository = new AppointmentRepository(_applicationContext);
            return _appointmentRepository;
        } 
    }

    public IOrganisationRepository Organisation
    {
        get
        {
            _organisationRepository = new OrganisationRepository(_applicationContext);
            return _organisationRepository;
        }
    }
    
    public IUserRepository User
    {
        get
        {
            _userRepository = new UserRepository(_applicationContext);
            return _userRepository;
        }
    }

    public async Task SaveAsync() => await _applicationContext.SaveChangesAsync();
}