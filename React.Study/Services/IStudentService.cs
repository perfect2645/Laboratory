using React.Study.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace React.Study.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
        Task<StudentDto?> GetStudentByIdAsync(int id);
        Task<StudentDto> CreateStudentAsync(StudentDto studentDto);
        Task<bool> UpdateStudentAsync(StudentDto studentDto);
        Task<bool> DeleteStudentAsync(int id);
        bool StudentExists(int id);
    }
}
