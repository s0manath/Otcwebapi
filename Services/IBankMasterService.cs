using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface IBankMasterService
    {
        public Task <IEnumerable<Bankmaster>> GetBankMasterDetails(StringIdRequest request);
        public Task<IEnumerable<Bankmaster>> GetBankMasterList();
        public Task<string> UpsertBankMasterDeatils(Bankmaster bank);
    }
}
