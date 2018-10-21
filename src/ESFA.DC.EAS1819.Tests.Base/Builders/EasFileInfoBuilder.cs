namespace ESFA.DC.EAS1819.Tests.Base.Builders
{
    using System;
    using ESFA.DC.EAS1819.Model;

    public class EasFileInfoBuilder
    {
        public EasFileInfoBuilder()
        {
            FileName = "";
            FilePath = @"";
        }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public static implicit operator EasFileInfo(EasFileInfoBuilder instance)
        {
            return instance.Build();
        }

        public EasFileInfo Build()
        {
            return new EasFileInfo
            {
                FileName = this.FileName,
                UKPRN = "10033670",
                DateTime = DateTime.UtcNow,
                FilePreparationDate = DateTime.UtcNow.AddHours(-2),
                FilePath = this.FilePath
            };
        }

        public EasFileInfoBuilder WithFileName(string filename)
        {
            FileName = filename;
            return this;
        }

        public EasFileInfoBuilder WithFilePath(string filePath)
        {
            FilePath = filePath;
            return this;
        }
    }
}
