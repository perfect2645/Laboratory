using System.ComponentModel.DataAnnotations;

namespace React.Study.Dto
{
    public class CreateStudentDto
    {
        public required StudentAttributes Attributes { get; set; }
    }

    public class StudentDto
    {
        public int Id { get; set; }
        public required StudentAttributes Attributes { get; set; }
    }

    public class StudentAttributes
    {
        [Required(ErrorMessage = "姓名不能为空")]
        public required string Name { get; set; }
        public required string Gender { get; set; }
        public required int Age { get; set; }
        public string? Address { get; set; }
    }
}
