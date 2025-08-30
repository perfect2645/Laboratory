using Db.React.Study.Configurations.Data;
using Db.React.Study.Entities;
using Microsoft.EntityFrameworkCore;
using React.Study.Dto;
using React.Study.Services;
using Utils.Ioc;

namespace React.Study.Repositories
{
    [Register(Lifetime = Lifetime.Transient, ServiceType = typeof(IStudentRepository))]
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _dbContext;

        public StudentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Students.AnyAsync(x => x.Id == id);
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _dbContext.Students.FindAsync(id);
        }

        public async Task<Student?> GetStudentByPropertiesAsync(string name, int age, string gender, string? address)
        {
            return await _dbContext.Students.FirstOrDefaultAsync(x =>
            !string.IsNullOrWhiteSpace(name) &&
            !string.IsNullOrWhiteSpace(x.Name) &&
            x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
            x.Age == age &&
            x.Gender == gender &&
            !string.IsNullOrWhiteSpace(address) &&
            !string.IsNullOrWhiteSpace(x.Address) &&
            x.Address.Equals(address, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Student>?> GetAllAsync()
        {
            return await _dbContext.Students.ToListAsync();
        }

        public async Task CreateAsync(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));

            await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public async Task UpdateAsync(Student student)
        {
            _dbContext.Update(student);
            await Task.CompletedTask;
        }
    }
}
