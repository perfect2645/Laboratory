using Db.React.Study.Configurations.Data;
using Db.React.Study.Entities;
using Microsoft.EntityFrameworkCore;
using React.Study.Dto;
using Utils.Ioc;

namespace React.Study.Services
{
    [Register(Lifetime = Lifetime.Transient, ServiceType = typeof(IStudentService))]
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            var students = await _context.Students.ToListAsync();

            return students.Select(s => new StudentDto
            {
                Id = s.Id,
                Attributes = new StudentAttributes
                {
                    Name = s.Name,
                    Gender = s.Gender,
                    Age = s.Age,
                    Address = s.Address
                }
            });
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return null;

            return new StudentDto
            {
                Id = student.Id,
                Attributes = new StudentAttributes
                {
                    Name = student.Name,
                    Gender = student.Gender,
                    Age = student.Age,
                    Address = student.Address
                }
            };
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto studentDto)
        {
            var student = new Student
            {
                Name = studentDto.Attributes.Name,
                Gender = studentDto.Attributes.Gender,
                Age = studentDto.Attributes.Age,
                Address = studentDto.Attributes.Address
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return new StudentDto
            {
                Id = student.Id,
                Attributes = new StudentAttributes
                {
                    Name = student.Name,
                    Gender = student.Gender,
                    Age = student.Age,
                    Address = student.Address
                }
            };
        }

        public async Task<bool> UpdateStudentAsync(StudentDto studentDto)
        {
            var existingStudent = await _context.Students.FindAsync(studentDto.Id);
            if (existingStudent == null)
                return false;

            existingStudent.Name = studentDto.Attributes.Name;
            existingStudent.Gender = studentDto.Attributes.Gender;
            existingStudent.Age = studentDto.Attributes.Age;
            existingStudent.Address = studentDto.Attributes.Address;

            _context.Entry(existingStudent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(studentDto.Id))
                    return false;
                throw;
            }
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
