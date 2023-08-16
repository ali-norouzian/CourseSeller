namespace CourseSeller.Core.Generators
{
    public static class CodeGenerators
    {
        // Generate unique code with length of 32
        public static string Generate32ByteUniqueCode()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        // Generate unique code with length of 64
        public static string Generate64ByteUniqueCode()
        {
            var uniqueCode1 = Guid.NewGuid().ToString();
            var uniqueCode2 = Guid.NewGuid().ToString();

            var result = $"{uniqueCode1}{uniqueCode2}".Replace("-", "");

            return result;
        }
    }
}