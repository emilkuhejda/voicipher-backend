namespace Voicipher.Domain.OutputModels.MetaData
{
    public record AdministratorTokenOutputModel
    {
        public AdministratorTokenOutputModel(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}
