namespace ESFA.DC.EAS.Tests.Base.Builders
{
    using System;
    using ESFA.DC.EAS.Model;

    public class EasFileInfoBuilder
    {
        public EasFileInfoBuilder()
        {
            FileName = string.Empty;
            FilePath = string.Empty;
            UkPrn = "10033670";
            JobId = 100;
            ReturnPeriod = 12;
        }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string UkPrn { get; set; }

        public long JobId { get; set; }

        public int ReturnPeriod { get; set; }

        public static implicit operator EasFileInfo(EasFileInfoBuilder instance)
        {
            return instance.Build();
        }

        public EasFileInfo Build()
        {
            return new EasFileInfo
            {
                FileName = this.FileName,
                DateTime = DateTime.UtcNow,
                FilePreparationDate = DateTime.UtcNow.AddHours(-2),
                FilePath = this.FilePath,
                UKPRN = this.UkPrn,
                JobId = this.JobId,
                ReturnPeriod = this.ReturnPeriod
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

        public EasFileInfoBuilder WithUkPrn(string ukPrn)
        {
            UkPrn = ukPrn;
            return this;
        }

        public EasFileInfoBuilder WithJobId(long jobId)
        {
            JobId = jobId;
            return this;
        }

        public EasFileInfoBuilder WithReturnPeriod(int returnPeriod)
        {
            ReturnPeriod = returnPeriod;
            return this;
        }
    }
}
