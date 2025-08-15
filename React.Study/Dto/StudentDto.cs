namespace React.Study.Dto
{
    public class StudentDto
    {
        public int Id { get; set; }
        public required StudentAttributes Attributes { get; set; }
    }

    // 包含学生属性的类
    public class StudentAttributes
    {
        public required string Name { get; set; }
        public required string Gender { get; set; }
        public required int Age { get; set; }
        public string? Address { get; set; }
    }
}
