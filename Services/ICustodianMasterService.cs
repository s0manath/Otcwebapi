using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface ICustodianMasterService
    {
        public Task<IEnumerable<CustodianMaster>> GetCustodianMasterList(CustodianMasterRequest request);
        public Task<CustodianMaster>  GetCustodianMasterDetails(StringIdRequest request);
        public Task<string> UpsertCustodianmaster(CustodianMaster request);
    }
}
