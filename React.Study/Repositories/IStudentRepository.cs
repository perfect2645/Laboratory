using Db.React.Study.Entities;

namespace React.Study.Repositories
{
    public interface IStudentRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetStudentByPropertiesAsync(string name, int age, string gender, string? address);
        Task<IEnumerable<Student>?> GetAllAsync();
        Task CreateAsync(Student student);
        Task DeleteAsync(int id);
        Task UpdateAsync(Student student);
    }
}
